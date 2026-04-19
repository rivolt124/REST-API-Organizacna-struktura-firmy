namespace CompanyStructureApi.DTOs.Departments;

public class DepartmentResponse
{
    public int DepartmentId { get; set; }
    public string ProjectCode { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public string DepartmentCode { get; set; } = null!;
    public string? LeaderEmail { get; set; }
    public string? LeaderFullName { get; set; }
}