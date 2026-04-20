// SETUP PHASE 2 - Validates and stores division/project/dept IDs
using Newtonsoft.Json.Linq;

tp.Test("SETUP: AssignDirector returned 200 with directorEmail", () =>
{
    Equal(200, (int)tp.Responses["AssignDirector"].StatusCode);
    dynamic body = tp.Responses["AssignDirector"].GetBodyAsExpando();
    NotNull((object)body.directorEmail);
});

tp.Test("SETUP: CreateDivision returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateDivision"].StatusCode);
    dynamic body = tp.Responses["CreateDivision"].GetBodyAsExpando();
    Equal("DIV-TEST-AUTO", (string)body.divisionCode);
    tp.SetVariable("testDivisionId", ((int)body.divisionId).ToString());
});

tp.Test("SETUP: CreateProject returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateProject"].StatusCode);
    dynamic body = tp.Responses["CreateProject"].GetBodyAsExpando();
    Equal("PRJ-TEST-AUTO", (string)body.projectCode);
    tp.SetVariable("testProjectId", ((int)body.projectId).ToString());
});

tp.Test("SETUP: CreateDepartment returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateDepartment"].StatusCode);
    dynamic body = tp.Responses["CreateDepartment"].GetBodyAsExpando();
    Equal("DEP-TEST-AUTO", (string)body.departmentCode);
    tp.SetVariable("testDepartmentId", ((int)body.departmentId).ToString());
});
