using Microsoft.AspNetCore.Mvc.RazorPages;

public class SentMessagesModel : PageModel
{
    private readonly MessageService _messageService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SentMessagesModel(MessageService messageService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _messageService = messageService;
    }

    public IEnumerable<ContosoPizza.Pages.MessageModel> Messages { get; private set; }

    public void OnGet()
    {
        Messages = _messageService.GetSentMessages();
    }
}

public class ReceivedMessagesModel : PageModel
{
    private readonly MessageService _messageService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ReceivedMessagesModel(MessageService messageService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _messageService = messageService;
    }

    public IEnumerable<ContosoPizza.Pages.MessageModel> Messages { get; private set; }

    public void OnGet()
    {
        Messages = _messageService.GetReceivedMessages();
    }
}
