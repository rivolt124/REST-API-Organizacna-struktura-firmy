namespace CompanyStructureApi.Controllers;

[ApiController]
[Route("api/divisions")]
public class DivisionsController : ControllerBase
{
    private readonly CompanyStructureDbContext _db;

    public DivisionsController(CompanyStructureDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? code,
        [FromQuery] string? companyCode
    )
    {
        var query = _db.Divisions
            .Include(d => d.Company)
            .Include(d => d.DivisionLeader)
            .Include(d => d.Projects).ThenInclude(p => p.ProjectLeader)
            .Include(d => d.Projects).ThenInclude(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(d => d.division_name.Contains(name));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(d => d.division_code.Contains(code));
        if (!string.IsNullOrWhiteSpace(companyCode))
            query = query.Where(d => d.Company != null && d.Company.company_code == companyCode);

        var divisions = await query.ToListAsync();
        return Ok(divisions.Select(d => ResponseMapper.ToDivisionResponse(d, d.Company!.company_code)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await Search(null, null, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var division = await _db.Divisions
            .Include(d => d.Company)
            .Include(d => d.DivisionLeader)
            .Include(d => d.Projects).ThenInclude(p => p.ProjectLeader)
            .Include(d => d.Projects).ThenInclude(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .FirstOrDefaultAsync(d => d.division_id == id);

        if (division == null)
            return NotFound($"Division not found.");

        return Ok(ResponseMapper.ToDivisionResponse(division, division.Company!.company_code));
    }

    [HttpPost]
    public async Task<IActionResult> Create(DivisionCreateRequest request)
    {
        var company = await _db.Companies
            .FirstOrDefaultAsync(c => c.company_code == request.CompanyCode);
        if (company == null)
            return NotFound($"Company not found.");

        if (await _db.Divisions.AnyAsync(d =>
                d.company_id == company.company_id &&
                d.division_code == request.DivisionCode))
            return Conflict($"Division already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == company.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        var division = new Division
        {
            company_id = company.company_id,
            division_name = request.DivisionName,
            division_code = request.DivisionCode,
            division_leader_id = leader?.employee_id
        };

        _db.Divisions.Add(division);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = division.division_id },
            new DivisionResponse
            {
                DivisionId = division.division_id,
                CompanyCode = company.company_code,
                DivisionName = division.division_name,
                DivisionCode = division.division_code,
                LeaderEmail = leader?.email,
                LeaderFullName = leader != null
                    ? $"{leader.title} {leader.firstName} {leader.lastName}".Trim()
                    : null,
                Projects = []
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DivisionUpdateRequest request)
    {
        var division = await _db.Divisions
            .Include(d => d.Company)
            .FirstOrDefaultAsync(d => d.division_id == id);

        if (division == null)
            return NotFound($"Division not found.");

        if (request.DivisionCode != division.division_code &&
            await _db.Divisions.AnyAsync(d =>
                d.company_id == division.company_id &&
                d.division_code == request.DivisionCode))
            return Conflict($"Division already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == division.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        division.division_name = request.DivisionName;
        division.division_code = request.DivisionCode;
        division.division_leader_id = leader?.employee_id;

        await _db.SaveChangesAsync();
        return Ok(new DivisionResponse
        {
            DivisionId = division.division_id,
            CompanyCode = division.Company!.company_code,
            DivisionName = division.division_name,
            DivisionCode = division.division_code,
            LeaderEmail = leader?.email,
            LeaderFullName = leader != null
                ? $"{leader.title} {leader.firstName} {leader.lastName}".Trim()
                : null,
            Projects = []
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var division = await _db.Divisions
            .FirstOrDefaultAsync(d => d.division_id == id);

        if (division == null)
            return NotFound($"Division not found.");

        _db.Divisions.Remove(division);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Division is still referenced by other records.");
        }
        return NoContent();
    }
}
