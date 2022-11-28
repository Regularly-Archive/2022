echo [$(date)] 'request a new ticket for HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM' > /proc/1/fd/1
kinit -kt /etc/apache2/krb-container.keytab HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM