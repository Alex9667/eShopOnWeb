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

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    public OrderService(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        Guard.Against.Null(basket, nameof(basket));
        Guard.Against.EmptyBasketOnCheckout(basket.Items);

        var Ids = basket.Items.Select(item => item.CatalogItemId).ToArray();
        var outIds = Ids.Select(Id => new {Id = Id }).ToJson();
        //var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());
        //var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);

        var _messagingService = new MessagingService();
        await _messagingService.SendMessage(outIds, "get_catalog","catalogRequestQueue");

        var _messageRevicer = new MessagingServiceRecive();
        var response =  _messageRevicer.ReceiveMessage("catalog", "catalogResponseQueue");
        var ResponseItems = JsonSerializer.Deserialize<CatalogItemOrdered[]>(await response);

        var items = basket.Items.Select(basketItem =>
        {

            //var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
            //var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
            
            var itemOrdered = ResponseItems.First(c => c.CatalogItemId == basketItem.CatalogItemId);
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);

            return orderItem;
            
        }).ToList();


        await _messagingService.SendMessage(outIds, "inventory", "inventoryRequestQueue");
        var inventoryAnswer = _messageRevicer.ReceiveMessage("catalog", "inventoryRequestQueue");


        var order = new Order(basket.BuyerId, shippingAddress, items);

        await _orderRepository.AddAsync(order);
    }
}
