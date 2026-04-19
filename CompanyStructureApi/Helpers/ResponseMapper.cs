namespace CompanyStructureApi.Helpers;

public static class ResponseMapper
{
    public static DepartmentResponse ToDepartmentResponse(Department dep, string projectCode) => new()
    {
        DepartmentId = dep.department_id,
        ProjectCode = projectCode,
        DepartmentName = dep.department_name,
        DepartmentCode = dep.department_code,
        LeaderEmail = dep.DepartmentLeader?.email,
        LeaderFullName = dep.DepartmentLeader != null
            ? $"{dep.DepartmentLeader.title} {dep.DepartmentLeader.firstName} {dep.DepartmentLeader.lastName}".Trim()
            : null
    };

    public static ProjectResponse ToProjectResponse(Project p, string divisionCode) => new()
    {
        ProjectId = p.project_id,
        DivisionCode = divisionCode,
        ProjectName = p.project_name,
        ProjectCode = p.project_code,
        LeaderEmail = p.ProjectLeader?.email,
        LeaderFullName = p.ProjectLeader != null
            ? $"{p.ProjectLeader.title} {p.ProjectLeader.firstName} {p.ProjectLeader.lastName}".Trim()
            : null,
        Departments = p.Departments.Select(dep => ToDepartmentResponse(dep, p.project_code)).ToList()
    };

    public static DivisionResponse ToDivisionResponse(Division d, string companyCode) => new()
    {
        DivisionId = d.division_id,
        CompanyCode = companyCode,
        DivisionName = d.division_name,
        DivisionCode = d.division_code,
        LeaderEmail = d.DivisionLeader?.email,
        LeaderFullName = d.DivisionLeader != null
            ? $"{d.DivisionLeader.title} {d.DivisionLeader.firstName} {d.DivisionLeader.lastName}".Trim()
            : null,
        Projects = d.Projects.Select(p => ToProjectResponse(p, d.division_code)).ToList()
    };

    public static CompanyResponse ToCompanyResponse(Company c) => new()
    {
        CompanyId = c.company_id,
        CompanyName = c.company_name,
        CompanyCode = c.company_code,
        DirectorEmail = c.Director?.email,
        DirectorFullName = c.Director != null
            ? $"{c.Director.title} {c.Director.firstName} {c.Director.lastName}".Trim()
            : null,
        Divisions = c.Divisions.Select(d => ToDivisionResponse(d, c.company_code)).ToList()
    };
}