namespace CompanyStructureApi.DTOs.Companies;

public class CompanyResponse
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public string? DirectorEmail { get; set; }
    public string? DirectorFullName { get; set; }
    public List<DivisionResponse> Divisions { get; set; } = [];
}