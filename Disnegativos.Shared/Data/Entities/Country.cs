using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class Country : BaseEntity
{
    public string IsoCode { get; set; } = string.Empty;     // e.g. "ES"
    public string NameKey { get; set; } = string.Empty;     // i18n
    public string NationalityKey { get; set; } = string.Empty; // i18n
    public string LanguageCode { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
}
