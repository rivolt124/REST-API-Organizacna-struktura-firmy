namespace CompanyStructureApi.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly CompanyStructureDbContext _db;

    public CompaniesController(CompanyStructureDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? code,
        [FromQuery] bool? hasDirector
    )
    {
        var query = _db.Companies
            .Include(c => c.Director)
            .Include(c => c.Divisions).ThenInclude(d => d.DivisionLeader)
            .Include(c => c.Divisions).ThenInclude(d => d.Projects).ThenInclude(p => p.ProjectLeader)
            .Include(c => c.Divisions).ThenInclude(d => d.Projects).ThenInclude(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.company_name.Contains(name));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(c => c.company_code.Contains(code));
        if (hasDirector.HasValue)
        {
            if (hasDirector.Value)
                query = query.Where(c => c.director_id != null);
            else
                query = query.Where(c => c.director_id == null);
        }
        var companies = await query.ToListAsync();
        
        return Ok(companies.Select(ResponseMapper.ToCompanyResponse));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await Search(null, null, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var company = await _db.Companies
            .Include(c => c.Director)
            .Include(c => c.Divisions).ThenInclude(d => d.DivisionLeader)
            .Include(c => c.Divisions).ThenInclude(d => d.Projects).ThenInclude(p => p.ProjectLeader)
            .Include(c => c.Divisions).ThenInclude(d => d.Projects).ThenInclude(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .FirstOrDefaultAsync(c => c.company_id == id);

        if (company == null)
            return NotFound($"Company not found.");

        return Ok(ResponseMapper.ToCompanyResponse(company));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CompanyRequest request)
    {
        if (request.DirectorId != null)
            return BadRequest("A director cannot be assigned when creating a company.");

        if (await _db.Companies.AnyAsync(c => c.company_code == request.CompanyCode))
            return Conflict($"Company already exists.");

        var company = new Company
        {
            company_name = request.CompanyName,
            company_code = request.CompanyCode,
            director_id = null
        };

        _db.Companies.Add(company);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = company.company_id }, new CompanyResponse
        {
            CompanyId = company.company_id,
            CompanyName = company.company_name,
            CompanyCode = company.company_code,
            DirectorEmail = null,
            DirectorFullName = null,
            Divisions = []
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CompanyRequest request)
    {
        var company = await _db.Companies
            .FirstOrDefaultAsync(c => c.company_id == id);

        if (company == null)
            return NotFound($"Company not found.");

        if (request.CompanyCode != company.company_code && await _db.Companies.AnyAsync(c => c.company_code == request.CompanyCode))
            return Conflict($"Company already exists.");

        Employee? director = null;
        if (request.DirectorId != null)
        {
            director = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.DirectorId && e.company_id == company.company_id);
            if (director == null)
                return BadRequest($"Employee not found in this company.");
        }

        company.company_name = request.CompanyName;
        company.company_code = request.CompanyCode;
        company.director_id = director?.employee_id;

        await _db.SaveChangesAsync();
        return Ok(new CompanyResponse
        {
            CompanyId = company.company_id,
            CompanyName = company.company_name,
            CompanyCode = company.company_code,
            DirectorEmail = director?.email,
            DirectorFullName = director != null
                ? $"{director.title} {director.firstName} {director.lastName}".Trim()
                : null,
            Divisions = []
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _db.Companies
            .FirstOrDefaultAsync(c => c.company_id == id);

        if (company == null)
            return NotFound($"Company not found.");

        _db.Companies.Remove(company);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Company is still referenced by other records.");
        }
        return NoContent();
    }
}
