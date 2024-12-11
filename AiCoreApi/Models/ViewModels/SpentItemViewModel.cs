namespace AiCoreApi.Models.ViewModels;

public class SpentItemViewModel
{
    public int LoginId { get; set; }
    public string? Login { get; set; }
    public string? LoginType { get; set; } = "Password";
    public int TokensOutgoing { get; set; } = 0;
    public int TokensIncoming { get; set; } = 0;
    public decimal Cost { get; set; } = 0;
    public List<decimal> CostDayByDay = new();
}
