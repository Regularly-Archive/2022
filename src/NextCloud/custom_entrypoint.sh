# 生成 crontab 配置
cronfile='/usr/crontab/cron.conf'
renewTicket='0 0 */1 * * root /usr/crontab/renew.sh'
echo "$renewTicket" | tee -a $cronfile
# 激活 crontab
crontab $cronfile
/etc/init.d/cron restart
# 启动 Apache
apache2-foreground