namespace CompanyStructureApi.Models;

public class Company
{
    [Key]
    public int company_id { get; set; }

    public int? director_id { get; set; }

    [Required, MaxLength(255)]
    public string company_name { get; set; } = null!;

    [Required, MaxLength(20)]
    public string company_code { get; set; } = null!;

    // Navigation properties
    [ForeignKey(nameof(director_id))]
    public Employee? Director { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
    public ICollection<Division> Divisions { get; set; } = new List<Division>();
}
