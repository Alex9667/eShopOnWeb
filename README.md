eShopOnWeb semesterprojekt
==========================
Dette projekt er udarbejdet ud fra [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb) projektet som er et eksempel på en webshop. Vi har splittet denne op i microservices via et stranglerpattern, implementeret asynkron messaging i systemet via RabbitMQ og sat en CI/CD pipeline op for projektet. Der er også implementeret ELK stack for logging og fejlovervågning af systemet

- [eShopOnWeb semesterprojekt](#eshoponweb-semesterprojekt)
- [1. Afgrænsning af services](#1-afgrænsning-af-services)
- [2. Strangler pattern](#2-strangler-pattern)
- [3. Lagerstyrignssystem](#3-lagerstyrignssystem)
- [4. API Gateway](#4-api-gateway)
- [5. Message bus](#5-message-bus)
- [6. Dataseparation](#6-dataseparation)
- [7. Fejlhåndtering og logging](#7-fejlhåndtering-og-logging)
- [8. CI/CD](#8-cicd)
- [Testing?](#testing)


# 1. Afgrænsning af services
For at kunne dele monolitten op skal vi først identificere hvilke services der skal isoleres. Vi har identificeret følgende services i monolitten som skal splittes ud i deres egne services:
- Catalog
- Order
- Basket
- Identity

Derudover skal der også implementeres et lagerstyringssystem som sin egen microservice. <br>
Dette viser vores bounded contexts: <br>
<img src="Documentation\Bounded contexts.png" width="550"/>

# 2. Strangler pattern
For at 'strangle' monolitten begyndte vi med at isolere en service ad gangen. Vi startede med at isolere koden først da dette havde en lavere kompleksitet en at separere dataet først

# 3. Lagerstyrignssystem
Lagerstyrignssystemet er implementeret som sin egen microservice, som snakker sammen med monolitten, og kan sende lagerstatus på produkter, samt fjerne mængden der er på lager.
<img src="Documentation\inventoryDoku.png" height="500" width="400"/>

# 4. API Gateway
Der er implementeret en API gateway i projektet de de enkelte microsevices fungerer som deres egne API'er og klient applikationer alle skal kalde disse API'er fra den samme IP. <br>
Dette er en basis implementering af en API Gateway hvor Gateway'en router alle API-kald til den rigtige API.
<img src="https://ocelot.readthedocs.io/en/latest/_images/OcelotBasic.jpg" width="550"/><br>
Vi har brugt [Ocelots](https://www.nuget.org/packages/Ocelot) NuGet pakke som er en open source pakke lavet til brug i microservice arkitekturer. Implementeringen er en simpel ASP.NET Core WebHost projekt hvor Ocelots middleware håndterer Gateway funktionerne. Selve routingen defineres i konfigurationsfilen [ocelot.json](eShopOnWebStrangler\APIGateway\APIGateway\ocelot.json) hvor de enkelte endpoint mappes mellem API'et og Gateway'en

# 5. Message bus
Vi har brugt [RabbitMQ](https://www.nuget.org/packages/rabbitmq.client/) til implementeringen af vores Messagebus. RabbitMQ er en messagebroker der håndterer asynkron kommunikation mellem services. RabbitMQ tilbyder en fleksibel og simpel måde at implementere messaging i et system og det er ligeledes en teknologi vi alle er bekendt med. <br>
![](<Documentation\Message channels.png>)

# 6. Dataseparation
Som en del af opdelingen af monolitten skal dataet også deles op for at opnå ordentlig adskillelse.

# 7. Fejlhåndtering og logging
Der er implementeret ELK stack i løsningen for at vi kan overvåge fejl og logging i vores system. <br>
ELK stack består af Elasticsearch, Logstash og Kibana der tilsammen giver god mulighed for at overvåge hele systemet

# 8. CI/CD
I dette system har vi implementeret en CI/CD pipeline via github actions. I [workflow](.github\workflows\dotnetcore.yml) filen bygger vi vores solution og kører tests på projektet. 

# Start med Docker
For at køre systemet i containere skal kommandoen ``docker compose up -d --build`` i et CLI fra roden af projektet

