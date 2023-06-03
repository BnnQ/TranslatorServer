using System.Text;
using System.Text.Json;
using TranslatorServer.Models.Dto;
using TranslatorServer.Services.Abstractions;

namespace TranslatorServer.Services;

public class AzureTranslator : ITranslator
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions serializerOptions;
    public AzureTranslator(IHttpClientFactory httpClientFactory, JsonSerializerOptions serializerOptions)
    {
        httpClient = httpClientFactory.CreateClient(nameof(AzureTranslator));
        this.serializerOptions = serializerOptions;
    }
    
    /// <exception cref="InvalidOperationException">If Azure Translator API did not return success status code. See details in message.</exception>
    public async Task<string> TranslateAsync(string fromLanguage, string toLanguage, string text)
    {
        var route = $"/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";
        var body = new object[] { new { Text = text } };
        var requestBody = JsonSerializer.Serialize(body);
        
        var response = await httpClient.PostAsync(route, new StringContent(requestBody, Encoding.UTF8, "application/json"));
        
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Azure Translator API did not return a success status code. ({await response.Content.ReadAsStringAsync()})");
        }
        
        var responseData = await response.Content.ReadAsStringAsync();
        var translations = JsonSerializer.Deserialize<List<TranslationResponseDto>>(responseData, serializerOptions)!;
        return translations.First().Translations.First().Text;
    }
    
}