// SETUP PHASE 1 - Validates creation and stores IDs to environment
using Newtonsoft.Json.Linq;

tp.Test("SETUP: CreateMainCompany returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateMainCompany"].StatusCode);
    dynamic body = tp.Responses["CreateMainCompany"].GetBodyAsExpando();
    Equal("CORP-TEST-AUTO", (string)body.companyCode);
    tp.SetVariable("testCompanyId", ((int)body.companyId).ToString());
});

tp.Test("SETUP: CreateOtherCompany returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateOtherCompany"].StatusCode);
    dynamic body = tp.Responses["CreateOtherCompany"].GetBodyAsExpando();
    Equal("CORP-OTHER-AUTO", (string)body.companyCode);
    tp.SetVariable("testOtherCompanyId", ((int)body.companyId).ToString());
});

tp.Test("SETUP: CreateDirector returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateDirector"].StatusCode);
    dynamic body = tp.Responses["CreateDirector"].GetBodyAsExpando();
    Equal("CORP-TEST-AUTO", (string)body.companyCode);
    tp.SetVariable("testDirectorId", ((int)body.employeeId).ToString());
    tp.SetVariable("testDirectorEmail", (string)body.email);
});

tp.Test("SETUP: CreateDivLeader returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateDivLeader"].StatusCode);
    dynamic body = tp.Responses["CreateDivLeader"].GetBodyAsExpando();
    tp.SetVariable("testDivLeaderId", ((int)body.employeeId).ToString());
});

tp.Test("SETUP: CreateProjLeader returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateProjLeader"].StatusCode);
    dynamic body = tp.Responses["CreateProjLeader"].GetBodyAsExpando();
    tp.SetVariable("testProjLeaderId", ((int)body.employeeId).ToString());
});

tp.Test("SETUP: CreateDeptLeader returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateDeptLeader"].StatusCode);
    dynamic body = tp.Responses["CreateDeptLeader"].GetBodyAsExpando();
    tp.SetVariable("testDeptLeaderId", ((int)body.employeeId).ToString());
});

tp.Test("SETUP: CreateOtherEmployee returned 201", () =>
{
    Equal(201, (int)tp.Responses["CreateOtherEmployee"].StatusCode);
    dynamic body = tp.Responses["CreateOtherEmployee"].GetBodyAsExpando();
    tp.SetVariable("testOtherEmployeeId", ((int)body.employeeId).ToString());
});
