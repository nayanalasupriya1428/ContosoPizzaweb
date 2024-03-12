using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.ApplicationInsights;

namespace ContosoPizza.Pages
{
    public class PizzaListModel : PageModel
    {
        private readonly PizzaService _service;
        private readonly TelemetryClient _telemetryClient;
        public IList<Pizza> PizzaList { get; set; } = default!;
        [BindProperty]
        public Pizza NewPizza { get; set; } = default!;

        public PizzaListModel(PizzaService service, TelemetryClient telemetryClient)
        {
            _service = service;
            _telemetryClient = telemetryClient;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || NewPizza == null)
            {
                return Page();
            }

            _service.AddPizza(NewPizza);
            _telemetryClient.TrackTrace($"New pizza added: {NewPizza.Name}");

            return RedirectToAction("Get");
        }

        public IActionResult OnPostDelete(int id)
        {
            _service.DeletePizza(id);
            _telemetryClient.TrackTrace($"Pizza deleted: ID {id}");

            return RedirectToAction("Get");
        }

        public void OnGet()
        {
            PizzaList = _service.GetPizzas();
        }
    }
}
