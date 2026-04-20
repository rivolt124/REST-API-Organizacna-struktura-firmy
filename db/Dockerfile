FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

COPY database-init.sql /database-init.sql
COPY docker-entrypoint.sh /docker-entrypoint.sh

# Strip CRLF in case the script was saved on Windows
RUN sed -i 's/\r$//' /docker-entrypoint.sh && chmod +x /docker-entrypoint.sh

USER mssql

ENTRYPOINT ["/docker-entrypoint.sh"]