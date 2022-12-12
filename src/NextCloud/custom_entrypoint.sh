#!/bin/sh
set -e

#分配权限
chown www-data:www-data /etc/apache2/krb-container.keytab
chmod 777 /etc/apache2/krb-container.keytab
chmod +x /usr/crontab/renew.sh

#初始化票据
echo [$(date)] 'Initializing Kerberos ticket...'
kinit -kt /etc/apache2/krb-container.keytab HTTP/$NEXTCLOUD_SERVER_NAME@$DOMAIN_SERVER_NAME

#生成刷新票据的定时任务
echo [$(date)] "Creating renew ticket cron job..."
export cronfile='/usr/crontab/cron.conf'
renewTicket='*/10 * * * * /usr/crontab/renew.sh'
echo "$renewTicket" | tee -a $cronfile
crontab $cronfile
/etc/init.d/cron reload
/etc/init.d/cron restart

echo [$(date)] 'Initializing NextCloud and starting Apache server...'
/entrypoint.sh apache2-foreground

#如果作为 Docker 容器的入口脚本，建议在这里增加以下脚本来支持参数传递
#其效果约等于 ENTRYPOINT CMD
#exec "$@"
