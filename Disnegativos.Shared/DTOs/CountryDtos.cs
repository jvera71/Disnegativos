namespace Disnegativos.Shared.DTOs;

public record CountryDto(
    Guid Id,
    string IsoCode,
    string NameKey,
    string NationalityKey,
    string LanguageCode,
    int SortOrder
);

public class CountryEditDto
{
    public Guid Id { get; set; }
    public string IsoCode { get; set; } = string.Empty;
    public string NameKey { get; set; } = string.Empty;
    public string NationalityKey { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
