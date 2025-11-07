FROM mcr.microsoft.com/mssql/server:2022-latest

USER root
RUN apt-get update && \
    apt-get install -y curl gnupg2 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

USER mssql

ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express
ENV SA_PASSWORD=Password1!

COPY init-smarthydrodb.sql /init-smarthydrodb.sql
COPY entrypoint.sh /entrypoint.sh

HEALTHCHECK --interval=10s --timeout=5s --start-period=120s --retries=30 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "SELECT 1" || exit 1

ENTRYPOINT ["bash", "/entrypoint.sh"]