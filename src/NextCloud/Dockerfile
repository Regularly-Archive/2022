FROM nextcloud as base

ADD /config/etc/krb5.conf /etc/

ADD /config/etc/resolv.conf /etc/
ADD /config/etc/apt/sources.list /etc/apt/
ADD /config/etc/krb-container.keytab /etc/apache2/
COPY /apache2/certs/* /etc/certs/
# RUN chmod 777 /etc/apache2/krb-container.keytab
RUN chown root /etc/apache2/krb-container.keytab
RUN chmod 640 /etc/apache2/krb-container.keytab

COPY clear.sh /etc/
    

RUN apt-get update; \
    apt-get install -y libldap2; \
    apt-get install -y -t sid libapache2-mod-auth-kerb; \
    # apt-get install -y --no-install-recommends libapache2-mod-auth-gssapi; \
    # apt-get install -y msktutil; \
    # apt-get install -y kinit; \
    apt-get install -y -q krb5-user libpam-krb5; 
    # apt-get install -y krb5-config; 
    # apt-get install -y krb5-clients; \
    # apt-get install -y lighttpd-mod-authn-ldap; \
    # apt-get install -y mod_ldap
    # apt-get install -y mod-authnz-ldap; \

# RUN apt-get install -y --no-install-recommends winbind smbclient libldap-common; 

RUN apt -y install realmd dbus;

# RUN a2enmod ldap
# RUN a2enmod authnz_ldap
RUN a2enmod ssl

# RUN  service apache2 restart

RUN apt-get -y install smbclient curl heimdal-clients

RUN echo 'Stelect2013' | kinit --password-file=STDIN -f KRB-CONTAINER@FGMS.DEV.COM
# RUN kinit -t -k /etc/apache2/krb-container.keytab
RUN klist 

# ADD run.sh /run.sh
# RUN chmod +x /run.sh
# RUN 