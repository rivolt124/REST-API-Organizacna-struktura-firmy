namespace CompanyStructureApi.Models;

public class Department
{
    [Key]
    public int department_id { get; set; }

    [Required]
    public int project_id { get; set; }

    public int? department_leader_id { get; set; }

    [Required, MaxLength(255)]
    public string department_name { get; set; } = null!;

    [Required, MaxLength(20)]
    public string department_code { get; set; } = null!;

    // Navigation properties
    [ForeignKey(nameof(project_id))]
    public Project? Project { get; set; }

    [ForeignKey(nameof(department_leader_id))]
    public Employee? DepartmentLeader { get; set; }
}
