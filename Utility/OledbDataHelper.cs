using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using Cocon90.Lib.Dal.Tools;
using Cocon90.Lib.Dal.Rule;

namespace Cocon90.Lib.Dal.Utility
{
    /// <summary>
    /// Oledb数据库操作实例
    /// </summary>
    public class OledbDataHelper : DataHelper
    {
        #region DataHelper 成员

        public override System.Data.Common.DbConnection createConnection()
        {
            OleDbConnection conn = new OleDbConnection(base.ConnectionString);
            conn.Open();
            return conn;
        }
        public override DbCommand createCommond(string tsql, CommandType commandType, params  Parameter[] paramKeyAndValue)
        {
            OleDbConnection conn = (OleDbConnection)createConnection();
            OleDbCommand cmd = new OleDbCommand(tsql.Replace('@', this.ParamChar), conn);
            cmd.CommandText = tsql.Replace('@', this.ParamChar);
            cmd.CommandType = commandType;
            foreach (var param in paramKeyAndValue)
            {
                cmd.Parameters.Add(createParameter(param.Name, param.Value));
            }
            return cmd;

        }
        public override DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
        {
            OleDbDataAdapter dap = new OleDbDataAdapter((OleDbCommand)selectCmd);
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
            OleDbParameter para = new OleDbParameter(keyStr, objValue);
            return para;
        }
        #endregion

        public override char ParamChar
        {
            get { return '@'; }
        }
    }

}
