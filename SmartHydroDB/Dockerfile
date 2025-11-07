FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express
ENV SA_PASSWORD=Password1!

COPY init-smarthydrodb.sql /init-smarthydrodb.sql
COPY entrypoint.sh /entrypoint.sh

HEALTHCHECK --interval=10s --timeout=5s --start-period=60s --retries=10 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "SELECT 1" || exit 1

ENTRYPOINT ["bash", "/entrypoint.sh"]
