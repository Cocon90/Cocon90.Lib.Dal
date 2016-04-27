using Cocon90.Lib.Dal.Error;
using Cocon90.Lib.Dal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cocon90.Lib.Dal
{
    /// <summary>
    /// DataHelper工厂，它会根据配置文件自动生成一个新的DataHelper实例。
    /// </summary>
    public class DataHelperFactory
    {
        /// <summary>
        /// <para>取得数据库连接实例 配置文件中的connectionStrings中：</para>
        /// <para>连接语句中请用${app}代表应用程序所在目录（应用程序时，是所在目录，如:d:\proj\bin；WEB网站时，是网站目录,如:d:\myweb。），</para>
        /// <para>使用${Environment.SpecialFolder枚举名}表示对应的特殊目录，注意，区分大小写，结尾统一没有‘\\’（如${System}表示：C:\Windows\system32），</para>
        /// <para>MSSQL请添加 name="ConnectionString" providerName="Cocon90.Lib.Dal.Utility.SQLDataHelper"  connectionString="server=.;database=Cocon90_OA;uid=sa;pwd=123456;"</para>
        /// <para>SQLite请添加 name="ConnectionString" providerName="Cocon90.Lib.Dal.Utility.SQLiteDataHelper" connectionString="Data Source=${app}\data\cpms.db;"</para>
        /// <para>MySQL请添加 name="ConnectionString"  providerName="Cocon90.Lib.Dal.Utility.MySqlDataHelper" connectionString="Server=localhost;Port=3306;Database=data;User=root;Password=123456;"</para>
        /// <para>PostgreSql请添加 name="ConnectionString"  providerName="Cocon90.Lib.Dal.Utility.PostgreSqlDataHelper" connectionString="Server=127.0.0.1;Port=5432;Database=mydb;User Id=postgres;Password=123456;"</para>
        /// <para>其它数据库也是一样，providerName为Utility下的类全名，连接语句为：connectionString</para>
        /// <para>需要注意的是，如果给连接字符串的Name指定了其它值，则会影响其它类如ModelHelper的调用。</para>
        /// <para>如果连接字符串加密，请将密文写在： key="IsConnectionStringEncry" value="1" 到AppSettings节点下 </para>
        /// </summary>
        /// <returns>返回IDataHelper数据库基本操作接口</returns>
        public static IDataHelper CreateInstence(string connectionStringName = "ConnectionString")
        {
            string classFullNamespace = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            if (!classFullNamespace.Contains(".")) throw new InstenceException("ConnectionString的ProviderName不正确，必须包含完整的类名路径");
            var result = (IDataHelper)Activator.CreateInstance(Type.GetType(classFullNamespace));
            var connString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString + "";
            connString = HandlerConnectionString(connString);
            result.ConnectionString = connString;
            return result;
        }

        /// <summary>
        /// <para>连接语句中请用${app}代表应用程序所在目录（应用程序时，是所在目录，如:d:\proj\bin；WEB网站时，是网站目录,如:d:\myweb。），</para>
        /// <para>使用${Environment.SpecialFolder枚举名}表示对应的特殊目录，注意，区分大小写，结尾统一没有‘\\’（如${System}表示：C:\Windows\system32），</para>
        /// <para>MSSQL请使用 providerName="Cocon90.Lib.Dal.Utility.SQLDataHelper"  connectionString="server=.;database=Cocon90_OA;uid=sa;pwd=123456;"</para>
        /// <para>SQLite请使用 providerName="Cocon90.Lib.Dal.Utility.SQLiteDataHelper" connectionString="Data Source=${app}data\cpms.db;"</para>
        /// <para>MySQL请使用 providerName="Cocon90.Lib.Dal.Utility.MySqlDataHelper" connectionString="Server=localhost;Port=3306;Database=data;User=root;Password=123456"</para>
        /// <para>PostgreSql请添加 name="ConnectionString"  providerName="Cocon90.Lib.Dal.Utility.PostgreSqlDataHelper" connectionString="Server=127.0.0.1;Port=5432;Database=mydb;User Id=postgres;Password=123456;"</para>
        /// <para>其它数据库也是一样，providerName为Utility下的类全名，连接语句为：connectionString</para>
        /// <para>需要注意的是，如果给连接字符串的Name指定了其它值，则会影响其它类如ModelHelper的调用。</para>
        /// <para>如果连接字符串加密，请将密文写在： key="IsConnectionStringEncry" value="1" 到AppSettings节点下 </para>
        /// </summary>
        /// <returns>返回IDataHelper数据库基本操作接口</returns>
        public static IDataHelper CreateInstence(string providerName, string connectionString)
        {
            string classFullNamespace = providerName;
            if (!classFullNamespace.Contains(".")) throw new InstenceException("ConnectionString的ProviderName不正确，必须包含完整的类名路径");
            var result = (IDataHelper)Activator.CreateInstance(Type.GetType(classFullNamespace));
            var connString = connectionString + "";
            connString = HandlerConnectionString(connString);
            result.ConnectionString = connString;
            return result;
        }
        /// <summary>
        /// <para>连接语句中请用${app}代表应用程序所在目录（应用程序时，是所在目录，如:d:\proj\bin；WEB网站时，是网站目录,如:d:\myweb。），</para>
        /// <para>使用${Environment.SpecialFolder枚举名}表示对应的特殊目录，注意，区分大小写，结尾统一没有‘\\’（如${System}表示：C:\Windows\system32），</para>
        /// <para>MSSQL请使用 providerName="Cocon90.Lib.Dal.Utility.SQLDataHelper"  connectionString="server=.;database=Cocon90_OA;uid=sa;pwd=123456;"</para>
        /// <para>SQLite请使用 providerName="Cocon90.Lib.Dal.Utility.SQLiteDataHelper" connectionString="Data Source=${app}\\data\\cpms.db;"</para>
        /// <para>MySQL请使用 providerName="Cocon90.Lib.Dal.Utility.MySqlDataHelper" connectionString="Server=localhost;Port=3306;Database=data;User=root;Password=123456"</para>
        /// <para>PostgreSql请添加 name="ConnectionString"  providerName="Cocon90.Lib.Dal.Utility.PostgreSqlDataHelper" connectionString="Server=127.0.0.1;Port=5432;Database=mydb;User Id=postgres;Password=123456;"</para>
        /// <para>其它数据库也是一样，providerName为Utility下的类全名，连接语句为：connectionString</para>
        /// <para>需要注意的是，如果给连接字符串的Name指定了其它值，则会影响其它类如ModelHelper的调用。</para>
        /// <para>如果连接字符串加密，请将密文写在： key="IsConnectionStringEncry" value="1" 到AppSettings节点下 </para>
        /// </summary>
        /// <returns>返回IDataHelper数据库基本操作接口</returns>
        public static IDataHelper CreateInstence(DataHelperType dataHelperType, string connectionString)
        {
            return CreateInstence(string.Format("Cocon90.Lib.Dal.Utility.{0}", dataHelperType.ToString()), connectionString);
        }

        /// <summary>
        /// 取得加工后的连接语句
        /// </summary>
        /// <returns></returns>
        private static string HandlerConnectionString(string connString)
        {
            var appSetting = (System.Configuration.ConfigurationManager.AppSettings["IsConnectionStringEncry"] + "").Trim().ToLower();
            if (appSetting == "1" || appSetting == "true" || appSetting == "on")
            {
                connString = des(connString, "chinapsu");
            }
            if (connString.ToLower().Contains("${app}"))
            {
                var dir = System.AppDomain.CurrentDomain.BaseDirectory;
                dir = dir.TrimEnd('\\');
                connString = connString.ToLower().Replace("${app}", dir);
            }
            foreach (var item in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                if (connString.Contains("${" + item + "}"))
                {
                    Environment.SpecialFolder folder = Environment.SpecialFolder.CommonApplicationData;
                    if (Enum.TryParse<Environment.SpecialFolder>(item, out folder))
                    {
                        var dir = System.Environment.GetFolderPath(folder);
                        connString = connString.Replace("${" + item + "}", dir.TrimEnd('\\'));
                    }
                }
            }
            return connString;
        }

        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey8Letter">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        private static string des(string decryptString, string decryptKey8Letter)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey8Letter);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }
}
