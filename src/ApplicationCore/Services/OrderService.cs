﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services.Messaging;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IOptions<RabbitMqSettings> _rabbitMqSettings;

    public OrderService(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer,
        IOptions<RabbitMqSettings> rabbitMqSettings)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
        _rabbitMqSettings = rabbitMqSettings;
    }

    public async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        Guard.Against.Null(basket, nameof(basket));
        Guard.Against.EmptyBasketOnCheckout(basket.Items);

        var Ids = basket.Items.Select(item => item.CatalogItemId).ToArray();
        var outIds = Ids.Select(Id => new {Id = Id }).ToJson();

        var _messagingService = new MessagingService(_rabbitMqSettings);
        await _messagingService.SendMessage(outIds, "get_catalog","catalogRequestQueue");

        var _messageRevicer = new MessagingServiceRecive(_rabbitMqSettings);
        var response =  _messageRevicer.ReceiveMessage("catalog", "catalogResponseQueue");
        var ResponseItems = JsonSerializer.Deserialize<CatalogItemOrdered[]>(await response);

        var items = basket.Items.Select(basketItem =>
        {
            
            var itemOrdered = ResponseItems.First(c => c.CatalogItemId == basketItem.CatalogItemId);
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);

            return orderItem;
            
        }).ToList();


        await _messagingService.SendMessage(outIds, "inventory", "inventoryRequestQueue");
        var inventoryAnswer = _messageRevicer.ReceiveMessage("inventory_amount", "inventoryResponseQueue");

        bool hasEnoughStock = true;

        List<InventoryModel> inventoryUpdateMessage = new List<InventoryModel>();

        if (inventoryAnswer != null)
        {
            List<InventoryModel> inventoryAmounts = new(JsonSerializer.Deserialize<InventoryModel[]>(await inventoryAnswer));
            var basketItems = basket.Items;

            foreach (var item in basketItems)
            {
                inventoryUpdateMessage.Add(new InventoryModel(item.CatalogItemId, item.Quantity, 0));

                foreach (var inventoryModel in inventoryAmounts)
                {
                    if(inventoryModel.CatalogItemId == item.CatalogItemId)
                    {
                        if(inventoryModel.Units < item.Quantity)
                        {
                            hasEnoughStock = false;
                        }
                    }
                }
            }
        }

        if (hasEnoughStock)
        {
            var order = new Order(basket.BuyerId, shippingAddress, items);

            await _orderRepository.AddAsync(order);

            await _messagingService.SendMessage(JsonSerializer.Serialize(inventoryUpdateMessage), "inventory_update", "inventoryUpdateQueue");
        }
    }
}

public class InventoryModel
{
    public int CatalogItemId { get; set; }
    public int Units { get; set; }
    public int ReservedUnits { get; set; }

    public InventoryModel(int catalogItemId, int units, int reservedUnits)
    {
        CatalogItemId = catalogItemId;
        Units = units;
        ReservedUnits = reservedUnits;
    }
}
