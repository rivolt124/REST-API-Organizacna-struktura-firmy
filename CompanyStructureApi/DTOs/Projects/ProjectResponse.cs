namespace CompanyStructureApi.DTOs.Projects;

public class ProjectResponse
{
    public int ProjectId { get; set; }
    public string DivisionCode { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string ProjectCode { get; set; } = null!;
    public string? LeaderEmail { get; set; }
    public string? LeaderFullName { get; set; }
    public List<DepartmentResponse> Departments { get; set; } = [];
}