using Cocon90.Lib.Dal.Rule;
using Cocon90.Lib.Dal.Tools;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// MySql DataHelper
    /// </summary>
    public class MySqlDataHelper : DataHelper
    {
        public override System.Data.Common.DbConnection createConnection()
        {
            MySqlConnection conn = new MySqlConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override System.Data.Common.DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params Parameter[] paramKeyAndValue)
        {
            MySqlConnection conn = (MySqlConnection)createConnection();
            MySqlCommand cmd = new MySqlCommand(tsqlParamed.Replace('@', this.ParamChar), conn);
            cmd.CommandType = commandType;
            foreach (var param in paramKeyAndValue)
            {
                cmd.Parameters.Add(createParameter(param.Name, param.Value));
            }
            return cmd;
        }

        public override System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            MySqlDataAdapter dap = new MySqlDataAdapter((MySqlCommand)selectCmd);
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
            MySqlParameter param = new MySqlParameter(keyStr, objValue);
            return param;
        }


        public override char ParamChar
        {
            get { return '?'; }
        }
    }
}
