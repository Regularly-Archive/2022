FROM php:8.1-apache-bullseye

COPY /etc/apt/sources.list /etc/apt/
COPY entrypoint.sh /usr/docker/entrypoint.sh
COPY echo.sh /usr/docker/echo.sh
RUN chmod +x /usr/docker/echo.sh

ADD /etc/apt/sources.list /etc/apt/
RUN apt-get update && apt-get install -y cron vim && export EDITOR="/usr/bin/vim"  

CMD ["sh", "/usr/docker/entrypoint.sh"]

