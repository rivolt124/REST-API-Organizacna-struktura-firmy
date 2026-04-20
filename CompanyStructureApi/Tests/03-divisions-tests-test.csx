using Newtonsoft.Json.Linq;
using System.Linq;

tp.Test("GET-01: GetAll returns 200 with non-empty array containing test division", () =>
{
    Equal(200, (int)tp.Responses["GetAll"].StatusCode);
    var arr = JArray.Parse(tp.Responses["GetAll"].GetBody());
    True(arr.Any(d => (string)d["divisionCode"] == "DIV-TEST-AUTO"));
});

tp.Test("GET-02: GetById returns 200 with correct division", () =>
{
    Equal(200, (int)tp.Responses["GetById"].StatusCode);
    dynamic body = tp.Responses["GetById"].GetBodyAsExpando();
    Equal("DIV-TEST-AUTO", (string)body.divisionCode);
    Equal("Test Division", (string)body.divisionName);
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
    Equal("DIV-TEST-AUTO", (string)arr[0]["divisionCode"]);
});

tp.Test("SEARCH-02: SearchByName returns at least 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByName"].GetBody());
    True(arr.Count >= 1);
});

tp.Test("SEARCH-03: SearchByCompanyCode returns exactly 1 test division", () =>
{
    Equal(200, (int)tp.Responses["SearchByCompanyCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByCompanyCode"].GetBody());
    Equal(1, arr.Count);
    Equal("CORP-TEST-AUTO", (string)arr[0]["companyCode"]);
});

tp.Test("SEARCH-04: SearchCombined returns 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchCombined"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchCombined"].GetBody());
    Equal(1, arr.Count);
});

tp.Test("SEARCH-05: SearchNoMatch returns empty array", () =>
{
    Equal(200, (int)tp.Responses["SearchNoMatch"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoMatch"].GetBody());
    Equal(0, arr.Count);
});

tp.Test("POST-01: PostConflict returns 409 (duplicate code in same company)", () =>
{
    Equal(409, (int)tp.Responses["PostConflict"].StatusCode);
});

tp.Test("POST-02: PostNotFoundCompany returns 404", () =>
{
    Equal(404, (int)tp.Responses["PostNotFoundCompany"].StatusCode);
});

tp.Test("POST-03: PostBadLeader returns 404 (leader from different company)", () =>
{
    Equal(404, (int)tp.Responses["PostBadLeader"].StatusCode);
});

tp.Test("PUT-01: PutUpdateName returns 200 with updated name", () =>
{
    Equal(200, (int)tp.Responses["PutUpdateName"].StatusCode);
    dynamic body = tp.Responses["PutUpdateName"].GetBodyAsExpando();
    Equal("Updated Test Division", (string)body.divisionName);
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
    Equal("Test Division", (string)body.divisionName);
    NotNull((object)body.leaderEmail);
});

tp.Test("PUT-04: PutNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["PutNotFound"].StatusCode);
});