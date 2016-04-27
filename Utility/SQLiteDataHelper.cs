using Cocon90.Lib.Dal.Rule;
using Cocon90.Lib.Dal.Tools;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// SQLite数据操作实例
    /// </summary>
    public class SQLiteDataHelper : DataHelper
    {
        public override System.Data.Common.DbConnection createConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override System.Data.Common.DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params  Parameter[] paramKeyAndValue)
        {
            SQLiteConnection conn = (SQLiteConnection)createConnection();
            SQLiteCommand cmd = new SQLiteCommand(tsqlParamed.Replace('@', this.ParamChar), conn);
            cmd.CommandType = commandType;
            foreach (var param in paramKeyAndValue)
            {
                cmd.Parameters.Add(createParameter(param.Name, param.Value));
            }
            return cmd;
        }
        public override System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            SQLiteDataAdapter dap = new SQLiteDataAdapter((SQLiteCommand)selectCmd);
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
            SQLiteParameter param = new SQLiteParameter(keyStr, objValue);
            return param;
        }

        public override char ParamChar
        {
            get { return '@'; }
        }
    }
}
