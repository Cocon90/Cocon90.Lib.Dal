using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using Cocon90.Lib.Dal.Tools;
using Cocon90.Lib.Dal.Rule;
namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// Odbc数据操作实例
    /// </summary>
    public class OdbcDataHelper : DataHelper
    {
        public override System.Data.Common.DbConnection createConnection()
        {
            OdbcConnection conn = new OdbcConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override System.Data.Common.DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params  Parameter[] paramKeyAndValue)
        {
            OdbcConnection conn = (OdbcConnection)createConnection();
            OdbcCommand cmd = new OdbcCommand(tsqlParamed.Replace('@', this.ParamChar), conn);
            cmd.CommandType = commandType;
            foreach (var param in paramKeyAndValue)
            {
                cmd.Parameters.Add(createParameter(param.Name, param.Value));
            }
            return cmd;
        }
        public override System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            OdbcDataAdapter dap = new OdbcDataAdapter((OdbcCommand)selectCmd);
            return dap;
        }
        public override System.Data.Common.DbParameter createParameter(string key, object value)
        {
            object objValue = value;
            if (value == null)
            {
                objValue = DBNull.Value;
            }
            var keyStr = key.Replace('@', this.ParamChar);
            if (!keyStr.Contains(this.ParamChar)) { keyStr = this.ParamChar + keyStr.Trim(); }
            OdbcParameter param = new OdbcParameter(keyStr, objValue);
            return param;
        }

        public override char ParamChar
        {
            get { return '?'; }
        }
    }
}
