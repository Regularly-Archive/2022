#!/bin/sh
set -e

echo 'initializing nextcloud and starting apache server...'
/entrypoint.sh apache2-foreground

echo "creating renew ticket cron job..."
export cronfile='/usr/crontab/cron.conf'
renewTicket='*/10 * * * * /usr/crontab/renew.sh'
echo "$renewTicket" | tee -a $cronfile
crontab $cronfile
/etc/init.d/cron reload
/etc/init.d/cron restart