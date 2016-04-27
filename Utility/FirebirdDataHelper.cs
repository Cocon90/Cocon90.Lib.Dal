using Cocon90.Lib.Dal.Rule;
using Cocon90.Lib.Dal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// Firebird数据操作实例
    /// </summary>
    public class FirebirdDataHelper : DataHelper
    {
        private System.Reflection.Assembly Assembly
        {
            get
            {
                var path = (AppDomain.CurrentDomain.BaseDirectory + "").TrimEnd('\\');
                var webPath = path + "\\bin\\FirebirdSql.Data.FirebirdClient.dll";
                var appPath = path + "\\FirebirdSql.Data.FirebirdClient.dll";
                var ass = System.Reflection.Assembly.LoadFile(File.Exists(appPath) ? appPath : webPath);
                return ass;
            }
        }
        public override System.Data.Common.DbConnection createConnection()
        {
            var ass = this.Assembly;
            var type = ass.GetType("FirebirdSql.Data.FirebirdClient.FbConnection");
            var cons = type.GetConstructor(new Type[] { typeof(string) });
            var obj = cons.Invoke(new object[] { base.ConnectionString });
            obj.GetType().GetMethod("Open").Invoke(obj, null);
            return (System.Data.Common.DbConnection)obj;
            //FbConnection conn = new FbConnection(base.GetConnectionString);
            //conn.Open();
            //return conn;
        }
        public override System.Data.Common.DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params Parameter[] paramKeyAndValue)
        {
            var ass = this.Assembly;
            var conn = createConnection();
            var type = ass.GetType("FirebirdSql.Data.FirebirdClient.FbCommand");
            var cons = type.GetConstructor(new Type[] { typeof(string), conn.GetType() });
            var cmd = cons.Invoke(new object[] { tsqlParamed.Replace('@', this.ParamChar), conn });
            cmd.GetType().GetProperty("CommandType").SetValue(cmd, commandType, null);
            List<System.Data.Common.DbParameter> dbParamKeyAndValue = new List<System.Data.Common.DbParameter>();
            if (paramKeyAndValue != null)
            {
                foreach (var dp in paramKeyAndValue)
                {
                    dbParamKeyAndValue.Add(createParameter(dp.Name, dp.Value));
                }
            }
            foreach (var param in dbParamKeyAndValue)
            {
                var props = cmd.GetType().GetProperties();
                object paramList = null;
                foreach (var item in props)
                {
                    if (item.Name == "Parameters")
                    {
                        paramList = item.GetValue(cmd, null);
                        break;
                    }
                }

                if (paramList != null)
                {
                    paramList.GetType().GetMethod("Add", new Type[] { param.GetType() }).Invoke(paramList, new object[] { param });
                }
            }
            return (System.Data.Common.DbCommand)cmd;
            //FbConnection conn = (FbConnection)createConnection();
            //FbCommand cmd = new FbCommand(tsqlParamed.Replace('@', sqlHelper.ParamChar), conn);
            //cmd.CommandType = commandType;
            //foreach (var param in paramKeyAndValue)
            //{
            //    cmd.Parameters.Add(param);
            //}
            //return cmd;
        }
        public override System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            var ass = this.Assembly;
            var conn = createConnection();
            var type = ass.GetType("FirebirdSql.Data.FirebirdClient.FbDataAdapter");
            var cons = type.GetConstructor(new Type[] { selectCmd.GetType() });
            var adp = cons.Invoke(new object[] { selectCmd });
            return (System.Data.Common.DbDataAdapter)adp;
            //FbDataAdapter dap = new FbDataAdapter((FbCommand)selectCmd);
            //return dap;
        }
        public override System.Data.Common.DbParameter createParameter(string key, object value)
        {
            object objValue = value;
            if (value == null)
            {
                objValue = DBNull.Value;
            }
            var ass = this.Assembly;
            var conn = createConnection();
            var type = ass.GetType("FirebirdSql.Data.FirebirdClient.FbParameter");
            var cons = type.GetConstructor(new Type[] { typeof(string), typeof(object) });
            var keyStr = key.Replace('@', this.ParamChar);
            if (!keyStr.Contains(this.ParamChar)) { keyStr = this.ParamChar + keyStr.Trim(); }
            var param = cons.Invoke(new object[] { keyStr, objValue });
            return (System.Data.Common.DbParameter)param;
        }


        public override char ParamChar
        {
            get { return '@'; }
        }
    }
}
