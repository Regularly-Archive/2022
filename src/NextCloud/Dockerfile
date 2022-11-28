FROM nextcloud as base

COPY /docker-reference/krb5.conf /etc/krb5.conf
COPY /docker-reference/sources.list /etc/apt/sources.list
COPY /docker-reference/krb-container.keytab /etc/apache2/krb-container.keytab
COPY /docker-reference/certs/* /etc/certs/
COPY /docker-reference/crontab/* /usr/crontab/
COPY custom_entrypoint.sh /etc/

RUN chown www-data:www-data /etc/apache2/krb-container.keytab
RUN chmod 777 /etc/apache2/krb-container.keytab
RUN chmod +x /usr/crontab/renew.sh
    
RUN apt-get update; \
    apt-get install -y libldap2-dev; \
    apt-get install -y -t sid libapache2-mod-auth-kerb; \
    apt-get install -y -q krb5-user libpam-krb5; \
    apt-get install -y realmd dbus; \
    apt-get install -y smbclient curl heimdal-clients cron; 

RUN a2enmod ssl && service apache2 restart

# RUN kinit -kt /etc/apache2/krb-container.keytab HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM && klist

CMD ["sh", "/etc/custom_entrypoint.sh"]


