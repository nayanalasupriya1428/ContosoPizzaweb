using Azure.Messaging.ServiceBus;
using ContosoPizza.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
public class ServiceBusSenderService
{
    private readonly ServiceBusSender _sender;
    public List<SentMessages> SentMessages { get; } = new List<SentMessages>();

    public ServiceBusSenderService(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public async Task SendMessageAsync(string messageContent)
    {
        ServiceBusMessage message = new ServiceBusMessage(messageContent);
        await _sender.SendMessageAsync(message);
        SentMessages.Add(new SentMessages { MessageContent = messageContent, SentTime = DateTime.Now });
    }
}
