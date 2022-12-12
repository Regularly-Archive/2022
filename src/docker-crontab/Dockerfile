FROM php:8.1-apache-bullseye as final 
COPY /etc/apt/sources.list /etc/apt/
COPY entrypoint.sh /usr/docker/entrypoint.sh
COPY echo.sh /usr/docker/echo.sh
COPY /config/nginx.conf /usr/docker/nginx.conf
RUN chmod +x /usr/docker/echo.sh
RUN apt-get update; \
    apt-get install -y cron vim gettext-base; \
    export EDITOR="/usr/bin/vim"; \
    rm -rf /var/lib/apt/lists/*; \
    apt-get clean; 
CMD ["sh", "/usr/docker/entrypoint.sh"]

