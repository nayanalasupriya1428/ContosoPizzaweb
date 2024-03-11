

using ContosoPizza.Pages;

public class MessageService
{
    private readonly List<MessageModel> _sentMessages = new();
    private readonly List<MessageModel> _receivedMessages = new();

    public void AddSentMessage(string message)
    {
        _sentMessages.Add(new MessageModel { Content = message, Timestamp = DateTime.Now });
    }

    public void AddReceivedMessage(string message)
    {
        _receivedMessages.Add(new MessageModel { Content = message, Timestamp = DateTime.Now });
    }

    public IEnumerable<MessageModel> GetSentMessages() => _sentMessages;
    public IEnumerable<MessageModel> GetReceivedMessages() => _receivedMessages;
}
