using Microsoft.AspNetCore.Mvc.RazorPages;
namespace ContosoPizza.Pages {

public class ReceivedMessagesModel : PageModel
{
    private readonly MessageService _messageService;

        public ReceivedMessagesModel(MessageService messageService)
        {
            _messageService = messageService;
    }

    public IEnumerable<ContosoPizza.Pages.MessageModel> Messages { get; private set; }

    public void OnGet()
    {
        Messages = _messageService.GetReceivedMessages();
    }
}
}