using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoPizza.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Log a trace message at the Information level
            _logger.LogInformation("Visited the Privacy page.");

            // You can also log messages at other levels, e.g., Warning, Error, etc.
            // _logger.LogWarning("This is a warning message on the Privacy page.");
            // _logger.LogError("This is an error message on the Privacy page.");
        }
    }
}
