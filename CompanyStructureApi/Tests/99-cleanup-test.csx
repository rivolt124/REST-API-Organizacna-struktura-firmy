using Newtonsoft.Json.Linq;

tp.Test("CLEANUP-01: DeleteDepartment returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteDepartment"].StatusCode);
});

tp.Test("CLEANUP-02: DeleteProject returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteProject"].StatusCode);
});

tp.Test("CLEANUP-03: DeleteDivision returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteDivision"].StatusCode);
});

tp.Test("CLEANUP-04: RemoveDirector returned 200", () =>
{
    Equal(200, (int)tp.Responses["RemoveDirector"].StatusCode);
});

tp.Test("CLEANUP-05: DeleteDeptLeader returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteDeptLeader"].StatusCode);
});

tp.Test("CLEANUP-06: DeleteProjLeader returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteProjLeader"].StatusCode);
});

tp.Test("CLEANUP-07: DeleteDivLeader returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteDivLeader"].StatusCode);
});

tp.Test("CLEANUP-08: DeleteDirector returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteDirector"].StatusCode);
});

tp.Test("CLEANUP-09: DeleteOtherEmployee returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteOtherEmployee"].StatusCode);
});

tp.Test("CLEANUP-10: DeleteMainCompany returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteMainCompany"].StatusCode);
});

tp.Test("CLEANUP-11: DeleteOtherCompany returned 204", () =>
{
    Equal(204, (int)tp.Responses["DeleteOtherCompany"].StatusCode);
});

tp.Test("VERIFY-01: Main company is gone - 404", () =>
{
    Equal(404, (int)tp.Responses["VerifyMainCompanyGone"].StatusCode);
});

tp.Test("VERIFY-02: No employees remain for CORP-TEST-AUTO", () =>
{
    Equal(200, (int)tp.Responses["VerifyEmployeesGone"].StatusCode);
    var arr = JArray.Parse(tp.Responses["VerifyEmployeesGone"].GetBody());
    Equal(0, arr.Count);
});