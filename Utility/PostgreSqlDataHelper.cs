using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;

using Cocon90.Lib.Dal.Tools;
using Npgsql;
using Cocon90.Lib.Dal.Rule;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// PostgreSql数据库操作实例
    /// </summary>
    public class PostgreSqlDataHelper : DataHelper
    {
        #region DataHelper 成员
        public override System.Data.Common.DbConnection createConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override DbCommand createCommond(string tsql, CommandType commandType, params  Parameter[] paramKeyAndValue)
        {
            NpgsqlConnection conn = (NpgsqlConnection)createConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(tsql.Replace('@', this.ParamChar), conn);
            cmd.CommandText = tsql.Replace('@', this.ParamChar);
            cmd.CommandType = commandType;
            if (paramKeyAndValue != null)
            {
                foreach (var para in paramKeyAndValue)
                {
                    cmd.Parameters.Add(createParameter(para.Name, para.Value));
                }
            }
            return cmd;
        }
        public override DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            NpgsqlDataAdapter dap = new NpgsqlDataAdapter((NpgsqlCommand)selectCmd);
            return dap;
        }
        public override DbParameter createParameter(string key, object value)
        {
            object objValue = value;
            if (value == null)
            {
                objValue = DBNull.Value;
            }
            var keyStr = key.Replace('@', this.ParamChar);
            if (!keyStr.Contains(this.ParamChar)) { keyStr = this.ParamChar + keyStr.Trim(); }
            NpgsqlParameter para = new NpgsqlParameter(keyStr, objValue);
            return para;
        }
        #endregion

        public override char ParamChar
        {
            get { return ':'; }
        }
    }
}
