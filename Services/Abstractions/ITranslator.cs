namespace TranslatorServer.Services.Abstractions;

public interface ITranslator
{
    public Task<string> TranslateAsync(string fromLanguage, string toLanguage, string text);
}