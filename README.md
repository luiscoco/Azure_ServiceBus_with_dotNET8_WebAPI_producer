# How to create a .NET8 WebAPI for sending messages to Azure ServiceBus

## 1. Create Azure ServiceBus (Topic)

We first log in to Azure Portal and search for Azure Service Bus 

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/c1083a36-37ed-41cd-b338-05b79338d256)

We create a new Azure Service Bus 

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/c55dfe80-c170-4a11-abd5-64fba5d3d038)

We input the required data: Subscription, ResourceGroup, Namespace, location and pricing tier

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/ceb2546c-a073-41c7-8ec5-0f29e59766fb)

We verify the new Azure Service Bus

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/d5d306e4-cea0-4898-a9e5-9b0ebb6d9eca)

We get the connection string

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/d540d906-ce3b-4d5d-b984-563a1895654b)

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/8c077842-6b05-46e2-a03e-de04f8bd1dcf)

This is the connection string:

```
Endpoint=sb://myservicebus1974.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c/7ve5kw9QuPqM8YSUWQvNTrjM+y5hkmp+ASbE85qY4=
```

We have to create a new topic

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/4042c8cc-f5f3-4e0e-9dfc-139722d6297d)

## 2. Create a .NET8 WebAPI with VSCode

## 3. Load project dependencies

We run this command to add the Azure Service Bus library

```
dotnet add package Azure.Messaging.ServiceBus
```

We also have to add the Swagger and OpenAPI libraries to access the API Docs

This is the csproj file including the project dependencies

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/2990d2e5-48bb-4239-b708-5b934664d5a5)

## 4. Create the project structure

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_consumer/assets/32194879/d6c0249d-f9a1-4019-aaca-4d07849ae963)

## 5. Create the Controller

```csharp
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace ServiceBusSenderApi.Controllers
{
    public class MessageDto
    {
        public string? Body { get; set; }
        public string? Priority { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private static string connectionString = "Endpoint=sb://myservicebus1974.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c/7ve5kw9QuPqM8YSUWQvNTrjM+y5hkmp+ASbE85qY4=";
        private static string topicName = "mytopic";
        private static ServiceBusClient client;
        private static ServiceBusSender sender;

        static ServiceBusController()
        {
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            var message = new ServiceBusMessage(messageDto.Body)
            {
                ApplicationProperties =
                {
                    ["priority"] = messageDto.Priority
                }
            };
            await sender.SendMessageAsync(message);
            return Ok($"Sent message: {messageDto.Body}, Priority: {messageDto.Priority}");
        }
    }
}
```

## 6. Modify the application middleware(program.cs)

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServiceBusSenderApi", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceBusSenderApi v1");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 7. Run and Test the application

We execute this command to run the application

```
dotnet run
```

