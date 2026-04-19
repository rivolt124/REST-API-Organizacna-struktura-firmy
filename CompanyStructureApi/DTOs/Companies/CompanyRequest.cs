namespace CompanyStructureApi.DTOs.Companies;

public class CompanyRequest
{
    [Required, MaxLength(255)]
    public string CompanyName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string CompanyCode { get; set; } = null!;

    public string? DirectorEmail { get; set; }
}