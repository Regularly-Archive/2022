echo [$(date)] 'request a new ticket for HTTP/'$NEXTCLOUD_SERVER_NAME'@'$DOMAIN_SERVER_NAME > /proc/1/fd/1
kinit -kt /etc/apache2/krb-container.keytab HTTP/$NEXTCLOUD_SERVER_NAME@$DOMAIN_SERVER_NAME