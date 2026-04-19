namespace CompanyStructureApi.DTOs.Divisions;

public class DivisionCreateRequest
{
    [Required]
    public string CompanyCode { get; set; } = null!;

    [Required, MaxLength(255)]
    public string DivisionName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DivisionCode { get; set; } = null!;

    public string? LeaderEmail { get; set; }
}