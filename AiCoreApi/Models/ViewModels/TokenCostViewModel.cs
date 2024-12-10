namespace AiCoreApi.Models.ViewModels;

public class TokenCostViewModel
{
    public int TokenCostId { get; set; } = 0;
    public string ModelName { get; set; } = string.Empty;
    public string ModelTitle { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public decimal Outgoing { get; set; } = 0;
    public decimal Incoming { get; set; } = 0; 
}