mv /etc/samba/smb.conf /etc/samba/smb.conf.orig
samba-tool domain provision --domain=DOMAIN --use-rfc2307 --realm=FGMS.DEV.COM --adminpass=Stelect2013
cp /var/lib/samba/private/krb5.conf /etc/ 

service smbd stop
service nmbd stop
service samba-ad-dc start
service samba-ad-dc restart
service samba-ad-dc status

samba-tool domain level show