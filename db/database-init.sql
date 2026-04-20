IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CompanyStructuredb')
    CREATE DATABASE CompanyStructuredb;
GO
USE CompanyStructuredb;
GO

CREATE TABLE [Companies] (
  [company_id] INT IDENTITY(1,1) PRIMARY KEY,
  [director_id] INT,
  [company_name] NVARCHAR(255) NOT NULL,
  [company_code] NVARCHAR(20) NOT NULL,
  CONSTRAINT UQ_Company_Code UNIQUE ([company_code])
)
GO

CREATE TABLE [Employees] (
  [employee_id] INT IDENTITY(1,1) PRIMARY KEY,
  [company_id] INT NOT NULL,
  [title] NVARCHAR(10),
  [firstName] NVARCHAR(20) NOT NULL,
  [lastName] NVARCHAR(20) NOT NULL,
  [phone] NVARCHAR(50) NOT NULL,
  [email] NVARCHAR(255) NOT NULL,
  CONSTRAINT UQ_Employee_Email UNIQUE ([email])
)
GO

CREATE TABLE [Divisions] (
  [division_id] INT IDENTITY(1,1) PRIMARY KEY,
  [company_id] INT NOT NULL,
  [division_leader_id] INT,
  [division_name] NVARCHAR(255) NOT NULL,
  [division_code] NVARCHAR(20) NOT NULL,
  CONSTRAINT UQ_Division_Code_Per_Company UNIQUE ([division_code], [company_id])
)
GO

CREATE TABLE [Projects] (
  [project_id] INT IDENTITY(1,1) PRIMARY KEY,
  [division_id] INT NOT NULL,
  [project_leader_id] INT,
  [project_name] NVARCHAR(255) NOT NULL,
  [project_code] NVARCHAR(20) NOT NULL,
  CONSTRAINT UQ_Project_Code_Per_Division UNIQUE ([project_code], [division_id])
)
GO

CREATE TABLE [Departments] (
  [department_id] INT IDENTITY(1,1) PRIMARY KEY,
  [project_id] INT NOT NULL,
  [department_leader_id] INT,
  [department_name] NVARCHAR(255) NOT NULL,
  [department_code] NVARCHAR(20) NOT NULL,
  CONSTRAINT UQ_Department_Code_Per_Project UNIQUE ([department_code], [project_id])
)
GO

-- FK: Companies <-> Employees (circular)
ALTER TABLE [Companies] ADD CONSTRAINT FK_Company_Director FOREIGN KEY ([director_id]) REFERENCES [Employees]([employee_id])
GO
ALTER TABLE [Employees] ADD CONSTRAINT FK_Employee_Company FOREIGN KEY ([company_id]) REFERENCES [Companies]([company_id])
GO

-- FK: Divisions
ALTER TABLE [Divisions] ADD CONSTRAINT FK_Division_Company FOREIGN KEY ([company_id]) REFERENCES [Companies]([company_id])
GO
ALTER TABLE [Divisions] ADD CONSTRAINT FK_Division_Leader FOREIGN KEY ([division_leader_id]) REFERENCES [Employees]([employee_id])
GO

-- FK: Projects
ALTER TABLE [Projects] ADD CONSTRAINT FK_Project_Division FOREIGN KEY ([division_id]) REFERENCES [Divisions]([division_id])
GO
ALTER TABLE [Projects] ADD CONSTRAINT FK_Project_Leader FOREIGN KEY ([project_leader_id]) REFERENCES [Employees]([employee_id])
GO

-- FK: Departments
ALTER TABLE [Departments] ADD CONSTRAINT FK_Department_Project FOREIGN KEY ([project_id]) REFERENCES [Projects]([project_id])
GO
ALTER TABLE [Departments] ADD CONSTRAINT FK_Department_Leader FOREIGN KEY ([department_leader_id]) REFERENCES [Employees]([employee_id])
GO

-- Check if an employee belongs to a specific company
CREATE FUNCTION dbo.fn_IsEmployeeOfCompany (
    @employee_id INT,
    @company_id  INT
) RETURNS BIT AS
BEGIN
    RETURN (
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM Employees
            WHERE employee_id = @employee_id
              AND company_id  = @company_id
        ) THEN 1 ELSE 0 END
    )
END
GO

-- Check if an employee belongs to the company that owns a given division
CREATE FUNCTION dbo.fn_IsEmployeeOfDivisionCompany (
    @employee_id INT,
    @division_id INT
) RETURNS BIT AS
BEGIN
    RETURN (
        SELECT CASE WHEN EXISTS (
            SELECT 1
            FROM   Employees e
            JOIN   Divisions d ON d.company_id = e.company_id
            WHERE  e.employee_id = @employee_id
              AND  d.division_id = @division_id
        ) THEN 1 ELSE 0 END
    )
END
GO

-- Check if an employee belongs to the company that owns a given project
CREATE FUNCTION dbo.fn_IsEmployeeOfProjectCompany (
    @employee_id INT,
    @project_id  INT
) RETURNS BIT AS
BEGIN
    RETURN (
        SELECT CASE WHEN EXISTS (
            SELECT 1
            FROM   Employees e
            JOIN   Divisions d  ON d.company_id  = e.company_id
            JOIN   Projects  p  ON p.division_id = d.division_id
            WHERE  e.employee_id = @employee_id
              AND  p.project_id  = @project_id
        ) THEN 1 ELSE 0 END
    )
END
GO

-- Director must be an employee of that company
ALTER TABLE Companies
ADD CONSTRAINT CK_Director_BelongsToCompany
CHECK (
    director_id IS NULL
    OR dbo.fn_IsEmployeeOfCompany(director_id, company_id) = 1
);
GO

-- Division leader must be an employee of the division's company
ALTER TABLE Divisions
ADD CONSTRAINT CK_DivisionLeader_BelongsToCompany
CHECK (
    division_leader_id IS NULL
    OR dbo.fn_IsEmployeeOfCompany(division_leader_id, company_id) = 1
);
GO

-- Project leader must be an employee of the project's company (via division)
ALTER TABLE Projects
ADD CONSTRAINT CK_ProjectLeader_BelongsToCompany
CHECK (
    project_leader_id IS NULL
    OR dbo.fn_IsEmployeeOfDivisionCompany(project_leader_id, division_id) = 1
);
GO

-- Department leader must be an employee of the department's company (via project → division)
ALTER TABLE Departments
ADD CONSTRAINT CK_DepartmentLeader_BelongsToCompany
CHECK (
    department_leader_id IS NULL
    OR dbo.fn_IsEmployeeOfProjectCompany(department_leader_id, project_id) = 1
);
GO

-- Companies director must not hold any other leader role
CREATE TRIGGER trg_Companies_UniqueLeader
ON Companies
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT UPDATE(director_id) RETURN;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.director_id IS NOT NULL
          AND (
            EXISTS (SELECT 1 FROM Companies c
                WHERE c.director_id = i.director_id
                AND c.company_id <> i.company_id)
           OR EXISTS (SELECT 1 FROM Divisions
                WHERE division_leader_id = i.director_id)
           OR EXISTS (SELECT 1 FROM Projects
                WHERE project_leader_id = i.director_id)
           OR EXISTS (SELECT 1 FROM Departments
                WHERE department_leader_id = i.director_id)
          )
    )
    BEGIN
        RAISERROR('This employee already holds a leader role elsewhere.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

-- Divisions leader must not hold any other leader role
CREATE TRIGGER trg_Divisions_UniqueLeader
ON Divisions
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT UPDATE(division_leader_id) RETURN;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.division_leader_id IS NOT NULL
          AND (
              EXISTS (SELECT 1 FROM Companies
                WHERE director_id = i.division_leader_id)
           OR 
              EXISTS (SELECT 1 FROM Divisions d
                WHERE d.division_leader_id = i.division_leader_id
                AND d.division_id <> i.division_id)
           OR EXISTS (SELECT 1 FROM Projects
                WHERE project_leader_id = i.division_leader_id)
           OR EXISTS (SELECT 1 FROM Departments
                WHERE department_leader_id = i.division_leader_id)
          )
    )
    BEGIN
        RAISERROR('This employee already holds a leader role elsewhere.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

-- Projects leader must not hold any other leader role
CREATE TRIGGER trg_Projects_UniqueLeader
ON Projects
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT UPDATE(project_leader_id) RETURN;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.project_leader_id IS NOT NULL
          AND (
              EXISTS (SELECT 1 FROM Companies
                      WHERE director_id = i.project_leader_id)
           OR EXISTS (SELECT 1 FROM Divisions
                      WHERE division_leader_id = i.project_leader_id)
           OR
              EXISTS (SELECT 1 FROM Projects p
                      WHERE p.project_leader_id = i.project_leader_id
                        AND p.project_id <> i.project_id)
           OR EXISTS (SELECT 1 FROM Departments
                      WHERE department_leader_id = i.project_leader_id)
          )
    )
    BEGIN
        RAISERROR('This employee already holds a leader role elsewhere.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

-- Departments leader must not hold any other leader role
CREATE TRIGGER trg_Departments_UniqueLeader
ON Departments
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT UPDATE(department_leader_id) RETURN;

    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.department_leader_id IS NOT NULL
          AND (
              EXISTS (SELECT 1 FROM Companies
                      WHERE director_id = i.department_leader_id)
           OR EXISTS (SELECT 1 FROM Divisions
                      WHERE division_leader_id = i.department_leader_id)
           OR EXISTS (SELECT 1 FROM Projects
                      WHERE project_leader_id = i.department_leader_id)
           OR
              EXISTS (SELECT 1 FROM Departments d
                      WHERE d.department_leader_id = i.department_leader_id
                        AND d.department_id <> i.department_id)
          )
    )
    BEGIN
        RAISERROR('This employee already holds a leader role elsewhere.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO


-- Seed data
SET IDENTITY_INSERT [Companies] ON;
INSERT INTO [Companies] ([company_id], [director_id], [company_name], [company_code])
VALUES (1, NULL, 'Acme Corp', 'ACME');
SET IDENTITY_INSERT [Companies] OFF;
GO

SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([employee_id], [company_id], [title], [firstName], [lastName], [phone], [email])
VALUES
    (1, 1, 'Ing.', 'John',    'Smith',   '+421900000001', 'john.smith@acme.com'),
    (2, 1, 'Bc.',  'Jane',    'Doe',     '+421900000002', 'jane.doe@acme.com'),
    (3, 1, NULL,   'Bob',     'Johnson', '+421900000003', 'bob.johnson@acme.com'),
    (4, 1, NULL,   'Alice',   'Brown',   '+421900000004', 'alice.brown@acme.com'),
    (5, 1, NULL,   'Charlie', 'Wilson',  '+421900000005', 'charlie.wilson@acme.com');
SET IDENTITY_INSERT [Employees] OFF;
GO

UPDATE [Companies] SET [director_id] = 1 WHERE [company_id] = 1;
GO

SET IDENTITY_INSERT [Divisions] ON;
INSERT INTO [Divisions] ([division_id], [company_id], [division_leader_id], [division_name], [division_code])
VALUES (1, 1, 2, 'Engineering', 'ENG');
SET IDENTITY_INSERT [Divisions] OFF;
GO

SET IDENTITY_INSERT [Projects] ON;
INSERT INTO [Projects] ([project_id], [division_id], [project_leader_id], [project_name], [project_code])
VALUES (1, 1, 3, 'Alpha Project', 'ALPHA');
SET IDENTITY_INSERT [Projects] OFF;
GO

SET IDENTITY_INSERT [Departments] ON;
INSERT INTO [Departments] ([department_id], [project_id], [department_leader_id], [department_name], [department_code])
VALUES (1, 1, 4, 'Backend', 'BACKEND');
SET IDENTITY_INSERT [Departments] OFF;
GO