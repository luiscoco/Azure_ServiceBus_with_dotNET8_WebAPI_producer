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
