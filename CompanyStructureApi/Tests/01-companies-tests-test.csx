using Newtonsoft.Json.Linq;
using System.Linq;

tp.Test("GET-01: GetAll returns 200 with non-empty array containing test company", () =>
{
    Equal(200, (int)tp.Responses["GetAll"].StatusCode);
    var arr = JArray.Parse(tp.Responses["GetAll"].GetBody());
    True(arr.Count > 0);
    True(arr.Any(c => (string)c["companyCode"] == "CORP-TEST-AUTO"));
});

tp.Test("GET-02: GetById returns 200 with correct company", () =>
{
    Equal(200, (int)tp.Responses["GetById"].StatusCode);
    dynamic body = tp.Responses["GetById"].GetBodyAsExpando();
    Equal("CORP-TEST-AUTO", (string)body.companyCode);
    Equal("TestCorp s.r.o.", (string)body.companyName);
    NotNull((object)body.directorFullName);
    Equal("Ing. Test Director", (string)body.directorFullName);
    var obj = JObject.Parse(tp.Responses["GetById"].GetBody());
    Equal(1, obj["divisions"].Count());
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
    Equal("CORP-TEST-AUTO", (string)arr[0]["companyCode"]);
});

tp.Test("SEARCH-02: SearchByName returns at least 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByName"].GetBody());
    True(arr.Count >= 1);
    True(arr.Any(c => ((string)c["companyName"]).Contains("TestCorp")));
});

tp.Test("SEARCH-03: SearchHasDirector includes test company", () =>
{
    Equal(200, (int)tp.Responses["SearchHasDirector"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchHasDirector"].GetBody());
    True(arr.Any(c => (string)c["companyCode"] == "CORP-TEST-AUTO"));
});

tp.Test("SEARCH-04: SearchNoDirector does not include test company (has director)", () =>
{
    Equal(200, (int)tp.Responses["SearchNoDirector"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoDirector"].GetBody());
    True(arr.All(c => (string)c["companyCode"] != "CORP-TEST-AUTO"));
});

tp.Test("SEARCH-05: SearchNoMatch returns empty array", () =>
{
    Equal(200, (int)tp.Responses["SearchNoMatch"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoMatch"].GetBody());
    Equal(0, arr.Count);
});

tp.Test("POST-01: PostBadRequest returns 400 (director on create)", () =>
{
    Equal(400, (int)tp.Responses["PostBadRequest"].StatusCode);
});

tp.Test("POST-02: PostConflict returns 409 (duplicate code)", () =>
{
    Equal(409, (int)tp.Responses["PostConflict"].StatusCode);
});

tp.Test("PUT-01: PutUpdateName returns 200 with updated name", () =>
{
    Equal(200, (int)tp.Responses["PutUpdateName"].StatusCode);
    dynamic body = tp.Responses["PutUpdateName"].GetBodyAsExpando();
    Equal("TestCorp a.s.", (string)body.companyName);
    Null(body.directorEmail);
});

tp.Test("PUT-02: PutAssignDirector returns 200 with directorEmail populated", () =>
{
    Equal(200, (int)tp.Responses["PutAssignDirector"].StatusCode);
    dynamic body = tp.Responses["PutAssignDirector"].GetBodyAsExpando();
    NotNull((object)body.directorEmail);
    Equal("test.director.auto@testcorp.com", (string)body.directorEmail);
    NotNull((object)body.directorFullName);
    Equal("Ing. Test Director", (string)body.directorFullName);
});

tp.Test("PUT-03: PutRemoveDirector returns 200 with null directorEmail", () =>
{
    Equal(200, (int)tp.Responses["PutRemoveDirector"].StatusCode);
    dynamic body = tp.Responses["PutRemoveDirector"].GetBodyAsExpando();
    Null(body.directorEmail);
});

tp.Test("PUT-04: PutRestore returns 200 with original state", () =>
{
    Equal(200, (int)tp.Responses["PutRestore"].StatusCode);
    dynamic body = tp.Responses["PutRestore"].GetBodyAsExpando();
    Equal("TestCorp s.r.o.", (string)body.companyName);
    NotNull((object)body.directorEmail);
});

tp.Test("PUT-05: PutConflict returns 409 (code taken)", () =>
{
    Equal(409, (int)tp.Responses["PutConflict"].StatusCode);
});

tp.Test("PUT-06: PutBadDirector returns 400 (director from different company)", () =>
{
    Equal(400, (int)tp.Responses["PutBadDirector"].StatusCode);
});

tp.Test("PUT-07: PutNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["PutNotFound"].StatusCode);
});

tp.Test("POST-03: PostMissingFields returns 400", () =>
{
    Equal(400, (int)tp.Responses["PostMissingFields"].StatusCode);
});

tp.Test("DELETE-01: DeleteNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["DeleteNotFound"].StatusCode);
});

tp.Test("DELETE-02: DeleteCompanyConflict returns 409 (company still has employees)", () =>
{
    Equal(409, (int)tp.Responses["DeleteCompanyConflict"].StatusCode);
});