# …˙≥… crontab ≈‰÷√
cronfile='/usr/docker/cron.conf'
echoJob='*/3 * * * * /usr/docker/echo.sh'
echo "$echoJob" | tee -a $cronfile
# º§ªÓ crontab
crontab $cronfile
/etc/init.d/cron reload
/etc/init.d/cron restart
apache2-foreground