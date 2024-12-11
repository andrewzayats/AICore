namespace AiCoreApi.Models.ViewModels;

public class ResourcePriceViewModel
{
    public string ResourceName { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal PriceHour { get; set; } = 0;
}

public class ResourcePriceLocationViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}