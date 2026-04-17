# REST API – Company Organizational Structure

A REST API for managing a company's hierarchical organizational structure and employee records, built with **.NET / C#** and **Microsoft SQL Server**. Developed as part of a technical assignment.

---

## 📋 Features

- **4-level organizational hierarchy**: Company → Division → Project → Department
- Each node has a **name**, **code**, and an assigned **head** (director / division lead / project lead / department head)
- Full **CRUD** for organizational nodes and employees
- Employee records include: title, first name, last name, phone, and email
- **Input validation** with meaningful error responses for missing or invalid data
- Interactive API docs via **Scalar**
- Endpoint testing via **TeaPie**

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET |
| Database | Microsoft SQL Server (Express) |
| API Docs | Scalar |
| API Testing | TeaPie |

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/rivolt124/REST-API-Organiza-n-trukt-ra-firmy.git
   cd REST-API-Organiza-n-trukt-ra-firmy
   ```

2. **Configure the database connection**  
   Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CompanyDB;Trusted_Connection=True;"
   }
   ```

3. **Apply migrations and run**
   ```bash
   dotnet ef database update
   dotnet run
   ```

4. **Explore the API**  
   Open your browser and navigate to `/scalar` to view and try all available endpoints.

---

## 📁 API Overview

| Resource | Endpoints |
|---|---|
| Companies | `GET/POST/PUT/DELETE /api/companies` |
| Divisions | `GET/POST/PUT/DELETE /api/divisions` |
| Projects | `GET`
