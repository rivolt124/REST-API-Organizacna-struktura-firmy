namespace CompanyStructureApi.DTOs.Divisions;

public class DivisionUpdateRequest
{
    [Required, MaxLength(255)]
    public string DivisionName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DivisionCode { get; set; } = null!;

    public int? LeaderId { get; set; }
}