namespace CompanyStructureApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly CompanyStructureDbContext _db;

    public ProjectsController(CompanyStructureDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? code,
        [FromQuery] string? divisionCode
    )
    {
        var query = _db.Projects
            .Include(p => p.Division)
            .Include(p => p.ProjectLeader)
            .Include(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.project_name.Contains(name));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(p => p.project_code.Contains(code));
        if (!string.IsNullOrWhiteSpace(divisionCode))
            query = query.Where(p => p.Division != null && p.Division.division_code == divisionCode);

        var projects = await query.ToListAsync();
        return Ok(projects.Select(p => ResponseMapper.ToProjectResponse(p, p.Division!.division_code)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await Search(null, null, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var project = await _db.Projects
            .Include(p => p.Division)
            .Include(p => p.ProjectLeader)
            .Include(p => p.Departments).ThenInclude(dep => dep.DepartmentLeader)
            .FirstOrDefaultAsync(p => p.project_id == id);

        if (project == null)
            return NotFound($"Project not found.");

        return Ok(ResponseMapper.ToProjectResponse(project, project.Division!.division_code));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectCreateRequest request)
    {
        var division = await _db.Divisions
            .Include(d => d.Company)
            .FirstOrDefaultAsync(d => d.division_code == request.DivisionCode);
        if (division == null)
            return NotFound($"Division not found.");

        if (await _db.Projects.AnyAsync(p =>
                p.division_id == division.division_id &&
                p.project_code == request.ProjectCode))
            return Conflict($"Project already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == division.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        var project = new Project
        {
            division_id = division.division_id,
            project_name = request.ProjectName,
            project_code = request.ProjectCode,
            project_leader_id = leader?.employee_id
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = project.project_id },
            new ProjectResponse
            {
                ProjectId = project.project_id,
                DivisionCode = division.division_code,
                ProjectName = project.project_name,
                ProjectCode = project.project_code,
                LeaderEmail = leader?.email,
                LeaderFullName = leader != null
                    ? $"{leader.title} {leader.firstName} {leader.lastName}".Trim()
                    : null,
                Departments = []
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateRequest request)
    {
        var project = await _db.Projects
            .Include(p => p.Division)
            .FirstOrDefaultAsync(p => p.project_id == id);

        if (project == null)
            return NotFound($"Project not found.");

        if (request.ProjectCode != project.project_code &&
            await _db.Projects.AnyAsync(p =>
                p.division_id == project.division_id &&
                p.project_code == request.ProjectCode))
            return Conflict($"Project already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == project.Division!.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        project.project_name = request.ProjectName;
        project.project_code = request.ProjectCode;
        project.project_leader_id = leader?.employee_id;

        await _db.SaveChangesAsync();
        return Ok(new ProjectResponse
        {
            ProjectId = project.project_id,
            DivisionCode = project.Division!.division_code,
            ProjectName = project.project_name,
            ProjectCode = project.project_code,
            LeaderEmail = leader?.email,
            LeaderFullName = leader != null
                ? $"{leader.title} {leader.firstName} {leader.lastName}".Trim()
                : null,
            Departments = []
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.project_id == id);

        if (project == null)
            return NotFound($"Project not found.");

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
