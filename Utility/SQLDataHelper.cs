using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Cocon90.Lib.Dal.Tools;
using Cocon90.Lib.Dal.Rule;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// Ms SQLServer数据库操作实例
    /// </summary>
    public class SQLDataHelper : DataHelper
    {
        #region DataHelper 成员
        public override System.Data.Common.DbConnection createConnection()
        {
            SqlConnection conn = new SqlConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override DbCommand createCommond(string tsql, CommandType commandType, params  Parameter[] paramKeyAndValue)
        {
            SqlConnection conn = (SqlConnection)createConnection();
            SqlCommand cmd = new SqlCommand(tsql.Replace('@', this.ParamChar), conn);
            cmd.CommandText = tsql.Replace('@', this.ParamChar);
            cmd.CommandType = commandType;
            if (paramKeyAndValue != null)
            {
                foreach (var param in paramKeyAndValue)
                {
                    cmd.Parameters.Add(createParameter(param.Name, param.Value));
                }
            }
            return cmd;
        }
        public override DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            SqlDataAdapter dap = new SqlDataAdapter((SqlCommand)selectCmd);
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
            SqlParameter para = new SqlParameter(keyStr, objValue);
            return para;
        }
        #endregion

        public override char ParamChar
        {
            get { return '@'; }
        }
    }
}
