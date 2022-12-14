FROM nextcloud:24

COPY /docker-reference/crontab/* /usr/crontab/
COPY /docker-reference/workflow/* /etc/workflow/
COPY ./docker-reference/sources.list /etc/apt/sources.list
COPY ./docker-reference/krb5.conf /etc/krb5.conf
COPY ./docker-reference/krb-container.keytab /etc/apache2/krb-container.keytab
COPY ./docker-reference/pdfstamp.sh /etc/pdfstamp.sh
COPY custom_entrypoint.sh /etc/custom_entrypoint.sh


 
RUN apt-get update; \
    apt-get install -y libldap2-dev smbclient curl heimdal-clients cron realmd dbus; \
    apt-get install -y -t sid libapache2-mod-auth-kerb; \
    apt-get install -y -q krb5-user libpam-krb5 wget gettext-base pdftk-java vim ghostscript; \
    a2enmod ssl && service apache2 restart; 
    # rm -rf /var/lib/apt/lists/*; \
    # apt-get clean;

CMD ["/etc/custom_entrypoint.sh"]


