FROM nextcloud as base

ADD /config/etc/krb5.conf /etc/
ADD /config/etc/apt/sources.list /etc/apt/
ADD /config/etc/krb-container.keytab /etc/apache2/
RUN a2enmod ldap
RUN a2enmod authnz_ldap
# RUN apt-get update; \
    # apt-get install -y libldap2; \
    # apt-get install -y -t sid libapache2-mod-auth-kerb; \
    # apt-get install -y libapache2-mod-auth-gssapi; \
    # apt-get install -y lighttpd-mod-authn-ldap; \
    # apt-get install -y mod_ldap
    # apt-get install -y mod-authnz-ldap; \