#!/bin/bash

# (Use a subshell with parentheses so it runs independently)
(
    echo "Waiting for SQL Server to start..."
    # Wait loop until SQL is ready to accept connections
    until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -C &> /dev/null
    do
        sleep 2
    done

    echo "Running initialization script..."
    # Run your actual init file
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /init-smarthydrodb.sql -C
    echo "Initialization complete."
) &


# This 'exec' line must be last. It replaces this bash script as PID 1.
exec /opt/mssql/bin/sqlservr "$@"