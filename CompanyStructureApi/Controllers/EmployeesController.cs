namespace CompanyStructureApi.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly CompanyStructureDbContext _db;

    public EmployeesController(CompanyStructureDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? email,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? title,
        [FromQuery] bool? hasTitle,
        [FromQuery] string? companyCode
    )
    {
        var query = _db.Employees
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(e => e.email == email);
        if (!string.IsNullOrWhiteSpace(firstName))
            query = query.Where(e => e.firstName.Contains(firstName));
        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(e => e.lastName.Contains(lastName));
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(e => e.title != null && e.title.Contains(title));
        if (hasTitle.HasValue)
        {
            if (hasTitle.Value)
                query = query.Where(e => e.title != null);
            else
                query = query.Where(e => e.title == null);
        }
        if (!string.IsNullOrWhiteSpace(companyCode))
            query = query.Where(e => e.Company != null && e.Company.company_code == companyCode);

        var employees = await query
            .Select(e => new EmployeeResponse
            {
                EmployeeId = e.employee_id,
                CompanyCode = e.Company!.company_code,
                Title = e.title,
                FirstName = e.firstName,
                LastName = e.lastName,
                Phone = e.phone,
                Email = e.email
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return await Search(null, null, null, null, null, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var employee = await _db.Employees
            .Where(e => e.employee_id == id)
            .Select(e => new EmployeeResponse
            {
                EmployeeId = e.employee_id,
                CompanyCode = e.Company!.company_code,
                Title = e.title,
                FirstName = e.firstName,
                LastName = e.lastName,
                Phone = e.phone,
                Email = e.email
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound("Employee not found.");
        
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeCreateRequest request)
    {
        if (await _db.Employees.AnyAsync(e => e.email == request.Email))
            return Conflict($"Employee already exists.");

        var company = await _db.Companies
            .FirstOrDefaultAsync(c => c.company_code == request.CompanyCode);
        if (company == null)
            return NotFound("Company not found.");

        var employee = new Employee
        {
            company_id = company.company_id,
            title = request.Title,
            firstName = request.FirstName,
            lastName = request.LastName,
            phone = request.Phone,
            email = request.Email
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = employee.employee_id }, new EmployeeResponse
        {
            EmployeeId = employee.employee_id,
            CompanyCode = company.company_code,
            Title = employee.title,
            FirstName = employee.firstName,
            LastName = employee.lastName,
            Phone = employee.phone,
            Email = employee.email
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeUpdateRequest request)
    {
        var employee = await _db.Employees
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.employee_id == id);

        if (employee == null)
            return NotFound($"Employee not found.");

        if (request.Email != employee.email && await _db.Employees.AnyAsync(e => e.email == request.Email))
            return Conflict($"Employee already exists.");

        employee.title = request.Title;
        employee.firstName = request.FirstName;
        employee.lastName = request.LastName;
        employee.phone = request.Phone;
        employee.email = request.Email;

        await _db.SaveChangesAsync();
        return Ok(new EmployeeResponse
        {
            EmployeeId = employee.employee_id,
            CompanyCode = employee.Company!.company_code,
            Title = employee.title,
            FirstName = employee.firstName,
            LastName = employee.lastName,
            Phone = employee.phone,
            Email = employee.email
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.employee_id == id);

        if (employee == null)
            return NotFound($"Employee not found.");

        _db.Employees.Remove(employee);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Employee is still referenced by other records.");
        }
        return NoContent();
    }
}