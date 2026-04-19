namespace CompanyStructureApi.DTOs.Projects;

public class ProjectResponse
{
    public string DivisionCode { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string ProjectCode { get; set; } = null!;
    public string? LeaderEmail { get; set; }
    public string? LeaderFullName { get; set; }
}