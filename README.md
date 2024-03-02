# How to create a .NET8 WebAPI for sending messages to Azure ServiceBus

See the source code for this sample in this github repo: https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer

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

Creating a .NET 8 Web API using Visual Studio Code (VSCode) and the .NET CLI is a straightforward process

This guide assumes you have .NET 8 SDK, VSCode, and the C# extension for VSCode installed. If not, you'll need to install these first

**Step 1: Install .NET 8 SDK**

Ensure you have the .NET 8 SDK installed on your machine: https://dotnet.microsoft.com/es-es/download/dotnet/8.0

You can check your installed .NET versions by opening a terminal and running:

```
dotnet --list-sdks
```

If you don't have .NET 8 SDK installed, download and install it from the official .NET download page

**Step 2: Create a New Web API Project**

Open a terminal or command prompt

Navigate to the directory where you want to create your new project

Run the following command to create a new Web API project:

```
dotnet new webapi -n ServiceBusSenderApi
```

This command creates a new directory with the project name, sets up a basic Web API project structure, and restores any necessary packages

**Step 3: Open the Project in VSCode**

Once the project is created, you can open it in VSCode by navigating into the project directory and running:

```
code .
```

This command opens VSCode in the current directory, where . represents the current directory

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

We navigate to the application endpoint: http://localhost:5256/swagger/index.html

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/6cc58ace-4f47-4287-8d0a-c23418ce8ad2)

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/e39f973e-39a4-424a-a004-e72a29daca94)

After executing the above request we get this response

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/ff39b8ec-72db-4055-8ae6-80f8b1ed72f9)

We confirm in the Azure Service Bus we recevied the message

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/de2a5e26-f3df-410a-9d2c-da5510c3e30a)

We navigate to the subscription and see the received message

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/d0bd1047-3c77-4b8e-9296-d13e794d709a)

See also the custom message property we added to the message

![image](https://github.com/luiscoco/Azure_ServiceBus_with_dotNET8_WebAPI_producer/assets/32194879/8bcd324e-5406-4583-937d-aa0efd08a718)





