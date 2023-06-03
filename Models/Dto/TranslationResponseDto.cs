// ReSharper disable CollectionNeverUpdated.Global
#nullable disable
namespace TranslatorServer.Models.Dto;

public class TranslationResponseDto
{
    public List<Translation> Translations { get; set; }
}

public class Translation
{
    public string Text { get; set; }
}