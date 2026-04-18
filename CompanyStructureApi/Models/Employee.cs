using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyStructureApi.Models;

public class Employee
{
    [Key]
    public int employee_id { get; set; }

    [Required]
    public int company_id { get; set; }

    [MaxLength(10)]
    public string? title { get; set; }

    [Required, MaxLength(20)]
    public string firstName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string lastName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string phone { get; set; } = null!;

    [Required, MaxLength(255)]
    public string email { get; set; } = null!;

    // Navigation properties
    [ForeignKey(nameof(company_id))]
    public Company? Company { get; set; }
}
