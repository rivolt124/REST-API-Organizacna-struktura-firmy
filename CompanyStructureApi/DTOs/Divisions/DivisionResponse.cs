namespace CompanyStructureApi.DTOs.Divisions;

public class DivisionResponse
{
    public string CompanyCode { get; set; } = null!;
    public string DivisionName { get; set; } = null!;
    public string DivisionCode { get; set; } = null!;
    public string? LeaderEmail { get; set; }
    public string? LeaderFullName { get; set; }
}