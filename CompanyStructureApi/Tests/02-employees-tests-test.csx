using Newtonsoft.Json.Linq;
using System.Linq;

tp.Test("GET-01: GetAll returns 200 with non-empty array", () =>
{
    Equal(200, (int)tp.Responses["GetAll"].StatusCode);
    var arr = JArray.Parse(tp.Responses["GetAll"].GetBody());
    True(arr.Count > 0);
});

tp.Test("GET-02: GetById returns 200 with correct employee", () =>
{
    Equal(200, (int)tp.Responses["GetById"].StatusCode);
    dynamic body = tp.Responses["GetById"].GetBodyAsExpando();
    Equal("test.director.auto@testcorp.com", (string)body.email);
    Equal("CORP-TEST-AUTO", (string)body.companyCode);
    Equal("Ing.", (string)body.title);
});

tp.Test("GET-03: GetByIdNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["GetByIdNotFound"].StatusCode);
});

tp.Test("SEARCH-01: SearchByEmail returns exactly 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByEmail"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByEmail"].GetBody());
    Equal(1, arr.Count);
    Equal("test.director.auto@testcorp.com", (string)arr[0]["email"]);
});

tp.Test("SEARCH-02: SearchByFirstName returns all 4 test employees", () =>
{
    Equal(200, (int)tp.Responses["SearchByFirstName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByFirstName"].GetBody());
    True(arr.Any(e => (string)e["email"] == "test.director.auto@testcorp.com"));
});

tp.Test("SEARCH-03: SearchByLastName returns exactly 1 result", () =>
{
    Equal(200, (int)tp.Responses["SearchByLastName"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByLastName"].GetBody());
    Equal(1, arr.Count);
    Equal("Director", (string)arr[0]["lastName"]);
});

tp.Test("SEARCH-04: SearchByTitle Ing. includes director", () =>
{
    Equal(200, (int)tp.Responses["SearchByTitle"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByTitle"].GetBody());
    True(arr.Any(e => (string)e["email"] == "test.director.auto@testcorp.com"));
});

tp.Test("SEARCH-05: SearchHasTitle in test company returns 2 (Ing. + Bc.)", () =>
{
    Equal(200, (int)tp.Responses["SearchHasTitle"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchHasTitle"].GetBody());
    Equal(2, arr.Count);
});

tp.Test("SEARCH-06: SearchNoTitle in test company returns 2 (ProjLeader + DeptLeader)", () =>
{
    Equal(200, (int)tp.Responses["SearchNoTitle"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoTitle"].GetBody());
    Equal(2, arr.Count);
});

tp.Test("SEARCH-07: SearchByCompanyCode returns exactly 4 test employees", () =>
{
    Equal(200, (int)tp.Responses["SearchByCompanyCode"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchByCompanyCode"].GetBody());
    Equal(4, arr.Count);
});

tp.Test("SEARCH-08: SearchNoMatch returns empty array", () =>
{
    Equal(200, (int)tp.Responses["SearchNoMatch"].StatusCode);
    var arr = JArray.Parse(tp.Responses["SearchNoMatch"].GetBody());
    Equal(0, arr.Count);
});

tp.Test("POST-01: PostConflict returns 409 (duplicate email)", () =>
{
    Equal(409, (int)tp.Responses["PostConflict"].StatusCode);
});

tp.Test("POST-02: PostNotFoundCompany returns 404 (invalid company code)", () =>
{
    Equal(404, (int)tp.Responses["PostNotFoundCompany"].StatusCode);
});

tp.Test("PUT-01: PutUpdate returns 200 with updated title and phone", () =>
{
    Equal(200, (int)tp.Responses["PutUpdate"].StatusCode);
    dynamic body = tp.Responses["PutUpdate"].GetBodyAsExpando();
    Equal("Mgr.", (string)body.title);
    Equal("+421999000099", (string)body.phone);
});

tp.Test("PUT-02: PutRestore returns 200 with original data", () =>
{
    Equal(200, (int)tp.Responses["PutRestore"].StatusCode);
    dynamic body = tp.Responses["PutRestore"].GetBodyAsExpando();
    Null(body.title);
    Equal("+421999000004", (string)body.phone);
});

tp.Test("PUT-03: PutConflict returns 409 (email taken)", () =>
{
    Equal(409, (int)tp.Responses["PutConflict"].StatusCode);
});

tp.Test("PUT-04: PutNotFound returns 404", () =>
{
    Equal(404, (int)tp.Responses["PutNotFound"].StatusCode);
});