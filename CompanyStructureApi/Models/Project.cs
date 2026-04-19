namespace CompanyStructureApi.Models;

public class Project
{
    [Key]
    public int project_id { get; set; }

    [Required]
    public int division_id { get; set; }

    public int? project_leader_id { get; set; }

    [Required, MaxLength(255)]
    public string project_name { get; set; } = null!;

    [Required, MaxLength(20)]
    public string project_code { get; set; } = null!;

    // Navigation properties
    [ForeignKey(nameof(division_id))]
    public Division? Division { get; set; }

    [ForeignKey(nameof(project_leader_id))]
    public Employee? ProjectLeader { get; set; }

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
