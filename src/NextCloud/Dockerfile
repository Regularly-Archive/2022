FROM nextcloud

COPY /docker-reference/crontab/* /usr/crontab/
COPY ./docker-reference/sources.list /etc/apt/sources.list
COPY ./docker-reference/krb5.conf /etc/krb5.conf
COPY ./docker-reference/krb-container.keytab /etc/apache2/krb-container.keytab
COPY custom_entrypoint.sh /etc/custom_entrypoint.sh
    
RUN apt-get update; \
    apt-get install -y libldap2-dev smbclient curl heimdal-clients cron realmd dbus; \
    apt-get install -y -t sid libapache2-mod-auth-kerb; \
    apt-get install -y -q krb5-user libpam-krb5 wget; 

RUN a2enmod ssl && service apache2 restart

CMD ["/etc/custom_entrypoint.sh"]


