#!/bin/bash
# Wait until SQL Server is ready
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1; do
  echo "Waiting for SQL Server to start..."
  sleep 5
done

echo "SQL Server is up. Creating database if missing..."

# Create database if it doesn't exist
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "IF DB_ID('SmartHydroDB') IS NULL CREATE DATABASE SmartHydroDB"

# Run initialization script
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -d SmartHydroDB -i /app/init-smarthydrodb.sql

echo "Initialization complete."
