namespace CompanyStructureApi.DTOs.Projects;

public class ProjectCreateRequest
{
    [Required]
    public string CompanyCode { get; set; } = null!;

    [Required]
    public string DivisionCode { get; set; } = null!;

    [Required, MaxLength(255)]
    public string ProjectName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string ProjectCode { get; set; } = null!;

    public int? LeaderId { get; set; }
}