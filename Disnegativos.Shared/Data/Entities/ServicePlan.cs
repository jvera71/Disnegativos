using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Entities;

public class ServicePlan : BaseEntity
{
    public int SecondsBeforeClip { get; set; } = 5;
    public int SecondsAfterClip { get; set; } = 5;
    public int MaxTemplates { get; set; } = 10;
    public int MaxPanels { get; set; } = 20;
    public int MaxBlocks { get; set; } = 50;
    public int MaxButtons { get; set; } = 200;
    public int MaxAnalyses { get; set; } = 100;
    public bool AllowFieldView { get; set; } = true;
    public bool AllowOrgChart { get; set; } = false;
    public bool DeviceOnly { get; set; } = false;
    public string? BucketName { get; set; }
    public bool AllowMediaUtils { get; set; } = false;
    public bool AllowDrawings { get; set; } = false;
    public bool AllowConcatVideo { get; set; } = false;
    public bool AllowMultiVideo { get; set; } = false;
}
