#!/bin/bash
# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1
do
  echo "SQL Server not ready yet..."
  sleep 5
done

# Create database if it doesn't exist
echo "Creating SmartHydroDB if not exists..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "IF DB_ID('SmartHydroDB') IS NULL CREATE DATABASE SmartHydroDB"

# Run init script if exists
if [ -f /init-smarthydrodb.sql ]; then
  echo "Running init-smarthydrodb.sql..."
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SmartHydroDB -i /init-smarthydrodb.sql
fi

# Bring SQL Server to foreground
wait
