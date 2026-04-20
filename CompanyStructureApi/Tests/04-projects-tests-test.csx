using Newtonsoft.Json.Linq;
using System.Linq;

tp.Test("GET-01: GetAll returns 200 containing test project", () =>
{
    Equal(200, (int)tp.Responses["GetAll"].StatusCode);
    var arr = JArray.Parse(tp.Responses["GetAll"].GetBody());
    True(arr.Any(p => (string)p["projectCode"] == "PRJ-TEST-AUTO"));
});

tp.Test("GET-02: GetById returns 200 with correct project", () =>
{
    Equal(200, (int)tp.Responses["GetById"].StatusCode);
    dynamic body = tp.Responses["GetById"].GetBodyAsExpando();
    Equal("PRJ-TEST-AUTO", (string)body.projectCode);
    Equal("Test Project", (string)body.projectName);
    NotNull((object)body.leaderEmail);
    NotNull((object)body.leaderFullName);
    Equal("Test ProjLeader", (string)body.leaderFullName);
    var obj = JObject.Parse(tp.Responses["GetById"].GetBody());
    Equal(1, obj["departments"].Count());
});

tp.Test("GET-03: GetByIdNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["GetByIdNotFound"].StatusCode);
});

tp.Test("SEARCH-01: SearchByCode returns exactly 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByCode"].GetBody());
    Equal(1, arr.Count);
    Equal("PRJ-TEST-AUTO", (string)arr[0]["projectCode"]);
});

tp.Test("SEARCH-02: SearchByName returns at least 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByName"].GetBody());
    True(arr.Count >= 1);
});

tp.Test("SEARCH-03: SearchByCompanyCode returns exactly 1 test project", () =>
{
    Equal(200, (int)tp.Responses["SearchByCompanyCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByCompanyCode"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-04: SearchByDivisionCode returns exactly 1 test project", () =>
{
    Equal(200, (int)tp.Responses["SearchByDivisionCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByDivisionCode"].GetBody());
    Equal(1, arr.Count);
    Equal("PRJ-TEST-AUTO", (string)arr[0]["projectCode"]);
});

tp.Test("SEARCH-05: SearchCombined returns 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchCombined"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchCombined"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-06: SearchNoMatch returns empty array", () =>
{
    Equal(200, (int)tp.Responses["SearchNoMatch"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoMatch"].GetBody());
    Equal(0, arr.Count);
});

tp.Test("POST-01: PostConflict returns 409 (duplicate code in same division)", () =>
{
    Equal(409, (int)tp.Responses["PostConflict"].StatusCode);
});

tp.Test("POST-02: PostNotFoundDivision returns 404 (division not in given company)", () =>
{
    Equal(404, (int)tp.Responses["PostNotFoundDivision"].StatusCode);
});

tp.Test("POST-03: PostBadLeader returns 404 (leader from different company)", () =>
{
    Equal(404, (int)tp.Responses["PostBadLeader"].StatusCode);
});

tp.Test("PUT-01: PutUpdateName returns 200 with updated name", () =>
{
    Equal(200, (int)tp.Responses["PutUpdateName"].StatusCode);
    dynamic body = tp.Responses["PutUpdateName"].GetBodyAsExpando();
    Equal("Updated Test Project", (string)body.projectName);
    NotNull((object)body.leaderEmail);
    NotNull((object)body.leaderFullName);
    Equal("Test ProjLeader", (string)body.leaderFullName);
});

tp.Test("PUT-02: PutRemoveLeader returns 200 with null leaderEmail", () =>
{
    Equal(200, (int)tp.Responses["PutRemoveLeader"].StatusCode);
    dynamic body = tp.Responses["PutRemoveLeader"].GetBodyAsExpando();
    Null(body.leaderEmail);
});

tp.Test("PUT-03: PutRestore returns 200 with original state", () =>
{
    Equal(200, (int)tp.Responses["PutRestore"].StatusCode);
    dynamic body = tp.Responses["PutRestore"].GetBodyAsExpando();
    Equal("Test Project", (string)body.projectName);
    NotNull((object)body.leaderEmail);
});

tp.Test("PUT-04: PutNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["PutNotFound"].StatusCode);
});

tp.Test("POST-04: PostMissingFields returns 400", () =>
{
    Equal(400, (int)tp.Responses["PostMissingFields"].StatusCode);
});

tp.Test("POST-05: CreateTempProject returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateTempProject"].StatusCode);
});

tp.Test("PUT-05: PutConflict returns 409 (code already used in same division)", () =>
{
    Equal(409, (int)tp.Responses["PutConflict"].StatusCode);
});

tp.Test("CLEANUP: DeleteTempProject returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteTempProject"].StatusCode);
});

tp.Test("PUT-06: PutBadLeader returns 404 (leader from different company)", () =>
{
    Equal(404, (int)tp.Responses["PutBadLeader"].StatusCode);
});

tp.Test("DELETE-01: DeleteNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["DeleteNotFound"].StatusCode);
});

tp.Test("SCOPE-01: CreateTempDivision2 returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateTempDivision2"].StatusCode);
});

tp.Test("SCOPE-02: PostSameCodeDifferentDivision returns 201 (code is scoped per division)", () =>
{
    Equal(201, (int)tp.Responses["PostSameCodeDifferentDivision"].StatusCode);
    dynamic body = tp.Responses["PostSameCodeDifferentDivision"].GetBodyAsExpando();
    Equal("PRJ-TEST-AUTO", (string)body.projectCode);
    Equal("DIV-TEMP-SCOPETEST", (string)body.divisionCode);
});

tp.Test("CLEANUP: DeleteScopeTestProject returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteScopeTestProject"].StatusCode);
});

tp.Test("CLEANUP: DeleteTempDivision2 returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteTempDivision2"].StatusCode);
});