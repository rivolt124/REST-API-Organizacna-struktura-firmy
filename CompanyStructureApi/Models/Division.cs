using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyStructureApi.Models;

public class Division
{
    [Key]
    public int division_id { get; set; }

    [Required]
    public int company_id { get; set; }

    public int? division_leader_id { get; set; }

    [Required, MaxLength(255)]
    public string division_name { get; set; } = null!;

    [Required, MaxLength(20)]
    public string division_code { get; set; } = null!;

    // Navigation properties
    [ForeignKey(nameof(company_id))]
    public Company? Company { get; set; }

    [ForeignKey(nameof(division_leader_id))]
    public Employee? DivisionLeader { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
