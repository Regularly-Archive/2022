ldap://192.168.6.30:389


CN=administrator,OU=OPS,OU=4GMS-Users,DC=fgms,DC=dev,DC=com
Stelect2013

CN=Configuration,DC=fgms,DC=dev,DC=com，这是默认的 DN

------------------------------------------------------------------------
OU=OPS,OU=4GMS-Users,DC=fgms,DC=dev,DC=com
OU=Admin,OU=4GMS-Users,DC=fgms,DC=dev,DC=com
------------------------------------------------------------------------

修改 config.php 增加 'check_data_directory_permissions' => false,

容器起來后等待初始化完成

需要明确组和用户是哪些

启用审计日志，应用 -> Auditing/Logging
修改配置 config/config.php：

  'log.condition' => [
    'apps' => ['admin_audit'],
  ],

1、日志中的用户名无法使用 DisplayName，用户/专家/内部用户名，配置为：displayname
2、单点登录需要输入用户名、密码

在应用/应用捆绑包内找到  SSO & SAML 下载并启用

如果下载慢，可以在 config/config.php 中添加下面的配置，v1 或者 v2 都可以

  'appstoreenabled' => true,
  'appstoreurl' => 'https://www.orcy.net/ncapps/v1/',

生成 keytab 文件

ktpass -out krb-container.keytab -mapUser krb-container@fgms.dev.com +rndPass -mapOp set +DumpSalt -crypto AES256-SHA1 -ptype KRB5_NT_PRINCIPAL -princ HTTP/nextcloud.fgms.dev.com@fgms.dev.com

ktpass ﻿-out krb-container.keytab -mapUser krb-container@fgms.dev.com /pass Stelect2013 -ptype KRB5_NT_PRINCIPAL -princ HTTP/nextcloud.fgms.dev.com@fgms.dev.com

在 config.php 中增加可信任的域名 trusted_domains

nextcloud.fgms.dev.com

同时修改 hosts 文件，即可通过域名来访问

火狐打开 about:config 配置以下参数

network.negotiate-auth.trusted-uris .fgms.dev.com

 network.auth.use-sspi false

Edge/Chrome  edge://policy 配置以下参数

修改注册表，HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge
AuthNegotiateDelegateAllowlist，维护值：.fgms.dev.com
BasicAuthOverHttpEnabled: true
AuthSchemes basic,digest,ntlm,negotiate

验证 keytab 文件
kinit -kt /etc/apache2/krb-container.keytab HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM

生成 keytab 文件
ktpass -out krb-container.keytab -mapUser KRB-CONTAINER@FGMS.DEV.COM /pass Stelect2013 -ptype KRB5_NT_PRINCIPAL -princ HTTP/nextcloud.fgms.dev.com@FGMS.DEV.COM -crypto ALL

https://learn.microsoft.com/zh-cn/troubleshoot/developer/webapps/iis/www-authentication-authorization/kerberos-double-hop-authentication-edge-chromium#step-2-install-the-microsoft-edge-administrative-templates

下载 https://www.microsoft.com/download/details.aspx?id=57576 
安装后从 C:\Program Files (x86)\Microsoft Group Policy\Windows 10 October 2018 Update (1809) v2 复制 PolicyDefinitions 到域服务器的 

edge://net-internals/#hsts