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