using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ContosoPizza.Services;
using Microsoft.ApplicationInsights;

namespace ContosoPizza.Pages;

public class SendMessageModel : PageModel
{
    private readonly ServiceBusSenderService _serviceBusSenderService;
    private readonly TelemetryClient _telemetryClient;

    public SendMessageModel(ServiceBusSenderService serviceBusSenderService, TelemetryClient telemetryClient)
    {
        _serviceBusSenderService = serviceBusSenderService;
        _telemetryClient = telemetryClient;
    }

    [BindProperty]
    public string Message { get; set; }

    public List<SentMessages> SentMessages => _serviceBusSenderService.SentMessages;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrEmpty(Message))
        {
            await _serviceBusSenderService.SendMessageAsync(Message);
            TempData["SuccessMessage"] = "Message sent successfully!";
            
            // Log a custom trace message
            _telemetryClient.TrackTrace($"Message sent: {Message}");
        }
        else
        {
            ModelState.AddModelError("", "Message content cannot be empty.");
        }

        return Page();
    }
}
