using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class AnalysisMedia : BaseEntity
{
    public Guid AnalysisId { get; set; }
    public Analysis Analysis { get; set; } = null!;

    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MediaType { get; set; } = "Video"; // Video, Audio, Image
    
    public string? Description { get; set; }
    public int? OffsetMs { get; set; }
    public int? DurationMs { get; set; }
    public int SortOrder { get; set; } = 0;
    public string? Thumbnail { get; set; }
    public bool IsUploaded { get; set; } = false;
}
