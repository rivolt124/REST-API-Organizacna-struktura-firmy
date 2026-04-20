using Newtonsoft.Json.Linq;
using System.Linq;

tp.Test("GET-01: GetAll returns 200 containing test department", () =>
{
    Equal(200, (int)tp.Responses["GetAll"].StatusCode);
    var arr = JArray.Parse(tp.Responses["GetAll"].GetBody());
    True(arr.Any(d => (string)d["departmentCode"] == "DEP-TEST-AUTO"));
});

tp.Test("GET-02: GetById returns 200 with correct department", () =>
{
    Equal(200, (int)tp.Responses["GetById"].StatusCode);
    dynamic body = tp.Responses["GetById"].GetBodyAsExpando();
    Equal("DEP-TEST-AUTO", (string)body.departmentCode);
    Equal("Test Department", (string)body.departmentName);
    NotNull((object)body.leaderEmail);
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
    Equal("DEP-TEST-AUTO", (string)arr[0]["departmentCode"]);
});

tp.Test("SEARCH-02: SearchByName returns at least 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByName"].GetBody());
    True(arr.Count >= 1);
});

tp.Test("SEARCH-03: SearchByCompanyCode returns exactly 1 test department", () =>
{
    Equal(200, (int)tp.Responses["SearchByCompanyCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByCompanyCode"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-04: SearchByDivisionCode returns exactly 1 test department", () =>
{
    Equal(200, (int)tp.Responses["SearchByDivisionCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByDivisionCode"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-05: SearchByProjectCode returns exactly 1 test department", () =>
{
    Equal(200, (int)tp.Responses["SearchByProjectCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByProjectCode"].GetBody());
    Equal(1, arr.Count);
    Equal("DEP-TEST-AUTO", (string)arr[0]["departmentCode"]);
});

tp.Test("SEARCH-06: SearchCombined returns exactly 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchCombined"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchCombined"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-07: SearchNoMatch returns empty array", () =>
{
    Equal(200, (int)tp.Responses["SearchNoMatch"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoMatch"].GetBody());
    Equal(0, arr.Count);
});

tp.Test("POST-01: PostConflict returns 409 (duplicate code in same project)", () =>
{
    Equal(409, (int)tp.Responses["PostConflict"].StatusCode);
});

tp.Test("POST-02: PostNotFoundProject returns 404 (wrong company/division/project combo)", () =>
{
    Equal(404, (int)tp.Responses["PostNotFoundProject"].StatusCode);
});

tp.Test("POST-03: PostBadLeader returns 404 (leader from different company)", () =>
{
    Equal(404, (int)tp.Responses["PostBadLeader"].StatusCode);
});

tp.Test("PUT-01: PutUpdateName returns 200 with updated name", () =>
{
    Equal(200, (int)tp.Responses["PutUpdateName"].StatusCode);
    dynamic body = tp.Responses["PutUpdateName"].GetBodyAsExpando();
    Equal("Updated Test Department", (string)body.departmentName);
    NotNull((object)body.leaderEmail);
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
    Equal("Test Department", (string)body.departmentName);
    NotNull((object)body.leaderEmail);
});

tp.Test("PUT-04: PutNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["PutNotFound"].StatusCode);
});