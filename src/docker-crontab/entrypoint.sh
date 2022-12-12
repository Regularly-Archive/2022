env >> /etc/default/locale

cronfile='/usr/docker/cron.conf'
echoJob='*/3 * * * * /usr/docker/echo.sh'
echo "$echoJob" | tee -a $cronfile
crontab $cronfile
/etc/init.d/cron reload
/etc/init.d/cron restart
envsubst < /usr/docker/nginx.conf > /usr/docker/nginx-1.conf

apache2-foreground