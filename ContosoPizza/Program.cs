using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ContosoPizza.Data;
using ContosoPizza.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Collections.Immutable.ImmutableArray;
using static System.Collections.Immutable.ImmutableDictionary;
using Azure.Identity;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        ConfigureApp(app);

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        var serviceBusConnectionString = "Endpoint=sb://pizzawebsite.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iAwgsPPfs2iiBre+NKOyvVbm0JKHRKene+ASbDuTmQo=";
ServiceBusClient client = new(serviceBusConnectionString);
ServiceBusSender sender = client.CreateSender(builder.Configuration["ServiceBusTopicName"]);


        // The processor that reads and processes messages from the subscription
        ServiceBusProcessor processor = client.CreateProcessor(builder.Configuration["ServiceBusTopicName"], builder.Configuration["ServiceBusSubscriptionName"]);

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
        await client.DisposeAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container
        services.AddRazorPages();
        services.AddDbContext<PizzaContext>(options =>
        options.UseSqlite("Data Source=C:\\home\\site\\wwwroot\\ContosoPizza.db"));
        services.AddScoped<PizzaService>();
        services.AddSingleton<MessageService>();
    }

    private static void ConfigureApp(WebApplication app)
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
        app.Run();
    }
}
