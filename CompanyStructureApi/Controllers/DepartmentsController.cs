namespace CompanyStructureApi.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly CompanyStructureDbContext _db;

    public DepartmentsController(CompanyStructureDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? code,
        [FromQuery] string? projectCode
    )
    {
        var query = _db.Departments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(d => d.department_name.Contains(name));
        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(d => d.department_code.Contains(code));
        if (!string.IsNullOrWhiteSpace(projectCode))
            query = query.Where(d => d.Project != null && d.Project.project_code == projectCode);

        var departments = await query
            .Select(d => new DepartmentResponse
            {
                DepartmentId = d.department_id,
                ProjectCode = d.Project!.project_code,
                DepartmentName = d.department_name,
                DepartmentCode = d.department_code,
                LeaderEmail = d.DepartmentLeader != null ? d.DepartmentLeader.email : null,
                LeaderFullName = d.DepartmentLeader != null
                    ? $"{d.DepartmentLeader.title} {d.DepartmentLeader.firstName} {d.DepartmentLeader.lastName}".Trim()
                    : null
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await Search(null, null, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var department = await _db.Departments
            .Where(d => d.department_id == id)
            .Select(d => new DepartmentResponse
            {
                DepartmentId = d.department_id,
                ProjectCode = d.Project!.project_code,
                DepartmentName = d.department_name,
                DepartmentCode = d.department_code,
                LeaderEmail = d.DepartmentLeader != null ? d.DepartmentLeader.email : null,
                LeaderFullName = d.DepartmentLeader != null
                    ? $"{d.DepartmentLeader.title} {d.DepartmentLeader.firstName} {d.DepartmentLeader.lastName}".Trim()
                    : null
            })
            .FirstOrDefaultAsync();

        if (department == null)
            return NotFound($"Department not found.");

        return Ok(department);
    }


    [HttpPost]
    public async Task<IActionResult> Create(DepartmentCreateRequest request)
    {
        var project = await _db.Projects
            .Include(p => p.Division)
            .FirstOrDefaultAsync(p => p.project_code == request.ProjectCode);
        if (project == null)
            return NotFound($"Project not found.");

        if (await _db.Departments.AnyAsync(d =>
                d.project_id == project.project_id &&
                d.department_code == request.DepartmentCode))
            return Conflict($"Department already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == project.Division!.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        var department = new Department
        {
            project_id = project.project_id,
            department_name = request.DepartmentName,
            department_code = request.DepartmentCode,
            department_leader_id = leader?.employee_id
        };

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = department.department_id },
            new DepartmentResponse
            {
                DepartmentId = department.department_id,
                ProjectCode = project.project_code,
                DepartmentName = department.department_name,
                DepartmentCode = department.department_code,
                LeaderEmail = leader?.email,
                LeaderFullName = leader != null
                    ? $"{leader.title} {leader.firstName} {leader.lastName}".Trim()
                    : null
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DepartmentUpdateRequest request)
    {
        var department = await _db.Departments
            .Include(d => d.Project).ThenInclude(p => p!.Division)
            .FirstOrDefaultAsync(d => d.department_id == id);

        if (department == null)
            return NotFound($"Department not found.");

        if (request.DepartmentCode != department.department_code &&
            await _db.Departments.AnyAsync(d =>
                d.project_id == department.project_id &&
                d.department_code == request.DepartmentCode))
            return Conflict($"Department already exists.");

        Employee? leader = null;
        if (request.LeaderId != null)
        {
            leader = await _db.Employees.FirstOrDefaultAsync(
                e => e.employee_id == request.LeaderId && e.company_id == department.Project!.Division!.company_id);
            if (leader == null)
                return NotFound($"Employee not found.");
        }

        department.department_name = request.DepartmentName;
        department.department_code = request.DepartmentCode;
        department.department_leader_id = leader?.employee_id;

        await _db.SaveChangesAsync();
        return Ok(new DepartmentResponse
        {
            DepartmentId = department.department_id,
            ProjectCode = department.Project!.project_code,
            DepartmentName = department.department_name,
            DepartmentCode = department.department_code,
            LeaderEmail = department.DepartmentLeader?.email,
            LeaderFullName = department.DepartmentLeader != null
                ? $"{department.DepartmentLeader.title} {department.DepartmentLeader.firstName} {department.DepartmentLeader.lastName}".Trim()
                : null
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _db.Departments
            .FirstOrDefaultAsync(d => d.department_id == id);

        if (department == null)
            return NotFound($"Department not found.");

        _db.Departments.Remove(department);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
