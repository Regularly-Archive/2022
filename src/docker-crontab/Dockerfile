FROM php:8.1-apache-bullseye

COPY /etc/apt/sources.list /etc/apt/
COPY entrypoint.sh /usr/docker/entrypoint.sh
COPY echo.sh /usr/docker/echo.sh
RUN chmod +x /usr/docker/echo.sh

ADD /etc/apt/sources.list /etc/apt/
RUN apt-get update && apt-get install -y cron rsyslog

RUN rm -f /etc/rsyslog.d/listen.conf

# Create the pseudo log file to point to stdout
RUN ln -sf /proc/1/fd/1 /var/log/test.log && rsyslogd 

CMD ["sh", "/usr/docker/entrypoint.sh"]