using System.Text.Json;
using TranslatorServer.Services;
using TranslatorServer.Services.Abstractions;

namespace TranslatorServer.Utils.Extensions;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
// Add services to the container.

        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient(nameof(AzureTranslator), httpClientBuilder =>
        {
            const string TranslatorEndpointPath = "Azure:Translator:Endpoint";
            var translatorEndpoint = builder.Configuration[TranslatorEndpointPath] ??
                                     throw new InvalidOperationException(
                                         $"'{TranslatorEndpointPath}' configuration value is not provided.");

            httpClientBuilder.BaseAddress = new Uri(translatorEndpoint);

            const string TranslatorSubscriptionKeyPath = "Azure:Translator:SubscriptionKey";
            var translatorSubscriptionKey = builder.Configuration[TranslatorSubscriptionKeyPath] ??
                                            throw new InvalidOperationException(
                                                $"'{TranslatorSubscriptionKeyPath}' configuration value is not provided.");
            
            httpClientBuilder.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", translatorSubscriptionKey);

            const string TranslatorRegionPath = "Azure:Translator:Region";
            var translatorRegion = builder.Configuration[TranslatorRegionPath] ??
                                   throw new InvalidOperationException($"'{TranslatorRegionPath}' configuration value is not provided.");

            httpClientBuilder.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", translatorRegion);
        });

        builder.Services.AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        builder.Services.AddTransient<ITranslator, AzureTranslator>();

        return builder;
    }

    public static void Configure(this WebApplication app)
    {
// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        
        app.UseAuthorization();

        app.UseCors(policyBuilder =>
        {
            policyBuilder.WithMethods("POST")
                .AllowAnyOrigin().AllowAnyHeader();
            //.WithOrigins("http://localhost:3000");
        });
        
        app.MapControllers();
    }
}