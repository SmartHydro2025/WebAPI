# Use Microsoft SQL Server 2022 (Linux)
FROM mcr.microsoft.com/mssql/server:2022-latest

# Required environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express
ENV MSSQL_SA_PASSWORD=Password1!

# Copy initialization SQL script
COPY init-smarthydrodb.sql /app/init-smarthydrodb.sql

# Copy entrypoint script
COPY entrypoint.sh /app/entrypoint.sh

# Install SQL tools
USER root
RUN apt-get update && apt-get install -y mssql-tools unixodbc-dev && apt-get clean
USER mssql

# Healthcheck to verify SQL Server is up
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "SELECT 1" || exit 1

# Use entrypoint script to start SQL Server and initialize DB
ENTRYPOINT ["/app/entrypoint.sh"]

