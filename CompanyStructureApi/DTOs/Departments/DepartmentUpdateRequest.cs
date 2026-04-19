namespace CompanyStructureApi.DTOs.Departments;

public class DepartmentUpdateRequest
{
    [Required, MaxLength(255)]
    public string DepartmentName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DepartmentCode { get; set; } = null!;

    public string? LeaderEmail { get; set; }
}