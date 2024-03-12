using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ContosoPizza.Data;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

// Directly set Service Bus configuration
string serviceBusConnectionString = "Endpoint=sb://pizzawebsite.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=XhvPw6PvciFm3DmEMnTCZaDIayfdoSB9Q+ASbDx5avQ=";
string serviceBusTopicName = "mytopic";
string serviceBusSubscriptionName = "b1";

builder.Services.AddSingleton<ServiceBusClient>(_ => new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddSingleton<ServiceBusSender>(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(serviceBusTopicName);
});
builder.Services.AddSingleton<ServiceBusSenderService>();
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = "InstrumentationKey=950afa3b-f60b-4c7f-b6cb-497ece055e4c;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/";
});

// Configure other services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureApp(app);

// The processor that reads and processes messages from the subscription
ServiceBusProcessor processor = app.Services.GetRequiredService<ServiceBusClient>()
    .CreateProcessor(serviceBusTopicName, serviceBusSubscriptionName);

// Handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body} from subscription");

    // Complete the message. Messages are deleted from the subscription.
    await args.CompleteMessageAsync(args.Message);

    // Store the received message
    var messageService = app.Services.GetRequiredService<MessageService>();
    messageService.AddReceivedMessage(body);
}

// Handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

// Start processing messages
processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;
await processor.StartProcessingAsync();

app.Run();

// Stop processing messages and clean up resources
await processor.StopProcessingAsync();
await processor.DisposeAsync();
await app.Services.GetRequiredService<ServiceBusClient>().DisposeAsync();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add services to the container
    services.AddRazorPages();
    services.AddDbContext<PizzaContext>(options =>
        options.UseSqlite("Data Source=C:\\home\\site\\wwwroot\\ContosoPizza.db"));
    services.AddScoped<PizzaService>();
    services.AddSingleton<MessageService>();
}

void ConfigureApp(WebApplication app)
{
    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages();
}
