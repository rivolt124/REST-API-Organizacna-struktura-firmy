namespace CompanyStructureApi.DTOs.Employees;

public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public string CompanyCode { get; set; } = null!;
    public string? Title { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
}