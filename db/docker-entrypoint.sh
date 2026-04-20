#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "Waiting for SQL Server to start..."
for i in $(seq 1 60); do
    if /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
        -Q "SELECT 1" -b -No 2>/dev/null; then

        # Only initialize if the database doesn't exist yet
        DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd \
            -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
            -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name='CompanyStructuredb'" \
            -h -1 -b -No 2>/dev/null | tr -d '[:space:]')

        if [ "$DB_EXISTS" = "0" ]; then
            echo "Initializing database..."
            /opt/mssql-tools18/bin/sqlcmd \
                -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
                -i /database-init.sql -b -No
            echo "Database initialization complete."
        else
            echo "Database already exists."
        fi
        break
    fi
    echo "Waiting for SQL Server... ($i/60)"
    sleep 1
done

# Keep SQL Server running in foreground
wait $SQL_PID