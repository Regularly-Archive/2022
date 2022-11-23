FROM nextcloud as base

ADD /config/etc/krb5.conf /etc/
ADD /config/etc/apt/sources.list /etc/apt/
ADD /config/etc/krb-container.keytab /etc/apache2/
COPY /apache2/certs/* /etc/certs/
RUN chown www-data:www-data /etc/apache2/krb-container.keytab
RUN chmod 777 /etc/apache2/krb-container.keytab
    
RUN apt-get update; \
    apt-get install -y libldap2-dev; \
    apt-get install -y -t sid libapache2-mod-auth-kerb; \
    apt-get install -y -q krb5-user libpam-krb5; \
    apt-get install -y realmd dbus; \
    apt-get install -y smbclient curl heimdal-clients; 

RUN a2enmod ssl && service apache2 restart

RUN kinit -kt /etc/apache2/krb-container.keytab HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM
RUN klist 
