namespace CompanyStructureApi.DTOs.Projects;

public class ProjectUpdateRequest
{
    [Required, MaxLength(255)]
    public string ProjectName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string ProjectCode { get; set; } = null!;

    public int? LeaderId { get; set; }
}