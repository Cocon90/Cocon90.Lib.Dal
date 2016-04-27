1、Sqlite说明：
Lib目录下的x64表示在64位系统上运行时，需要使用的System.Data.SQLite.dll。
Lib目录下的x86表示在32位系统上运行时，需要使用的System.Data.SQLite.dll。
Lib目录下的Auto表示不管在32位还是64位系统上运行时，都支持的System.Data.SQLite.dll。

区别在于：
X64和X86上的System.Data.SQLite.dll在使用的时候，需要用户计算机上安装VC++2010运行库。
而Auto下的System.Data.SQLite.dll在使用的时候，用户电脑上不需要安装VC++2010运行库，也不用区分X86或者X64，但是在使用它的时候，可能存在个问题就是：如果运行在FTP虚拟目录中，而又无法给目录设置权限的话，就会使得Auto下的这System.Data.SQLite.dll在使用时，无法自动释放出对应所需dll来（因为有些情况下计算机没有提供创建目录的权限），这时候，可以使用X86或X64去解决。




2、MySql说明：
当使用MySql数据库时，请将MySql.Data.dll置于应用程序目录下。


3、Firebird说明：
当使用Firebird数据库时，请将FirebirdSql.Data.FirebirdClient.dll和官方下载的Server包（大约10个dll）置于应用程序目录下。

4、SqlServerCe说明：
当使用SqlServerCe数据库时，请将System.Data.SqlServerCe.dll置于应用程序目录下。然后安装 SQL Server Compact 4.0 Sp1（最新版本。）

5、PostgetSql说明：
当使用PostgetSql数据时，请将Npgsql.2.2.5目录下的Mono.Security.dll和Npgsql.dll置于程序目录下。
或直接执行：Install-Package Npgsql -Version 2.2.5

6、Oracle说明：
当使用Oracle数据库时，请将odp.net.managed.121.1.2包中的Oracle.ManagedDataAccess.dll复制到程序程序目录下。