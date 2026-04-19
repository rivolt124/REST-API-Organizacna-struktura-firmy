namespace CompanyStructureApi.DTOs.Employees;

public class EmployeeCreateRequest
{
    [Required]
    public string CompanyCode { get; set; } = null!;

    [MaxLength(10)]
    public string? Title { get; set; }

    [Required, MaxLength(20)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string LastName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Email { get; set; } = null!;
}