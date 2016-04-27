//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Cocon90.Lib.Dal.Tools;
//using Oracle.ManagedDataAccess.Client;
//using Cocon90.Lib.Dal.Rule;
//namespace Cocon90.Lib.Dal.Utility
//{
//    /// <summary>
//    /// Oracle DataHelper
//    /// </summary>
//    public class OracleDataHelper : DataHelper
//    {
//        public override System.Data.Common.DbConnection createConnection()
//        {
//            OracleConnection conn = new OracleConnection(base.ConnectionString);
//            conn.Open();
//            return conn;
//        }
//        public override System.Data.Common.DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params  Parameter[] paramKeyAndValue)
//        {
//            OracleConnection conn = (OracleConnection)createConnection();
//            OracleCommand cmd = new OracleCommand(tsqlParamed.Replace('@', this.ParamChar), conn);
//            cmd.CommandType = commandType;
//            foreach (var param in paramKeyAndValue)
//            {
//                cmd.Parameters.Add(createParameter(param.Name, param.Value));
//            }
//            return cmd;
//        }
//        public override System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd)
//        {
//            OracleDataAdapter dap = new OracleDataAdapter((OracleCommand)selectCmd);
//            return dap;
//        }
//        public override System.Data.Common.DbParameter createParameter(string key, object value)
//        {
//            object objValue = value;
//            if (value == null)
//            {
//                objValue = DBNull.Value;
//            }
//            var keyStr = key.Replace('@', this.ParamChar);
//            if (!keyStr.Contains(this.ParamChar)) { keyStr = this.ParamChar + keyStr.Trim(); }
//            OracleParameter param = new OracleParameter(keyStr, objValue);
//            return param;
//        }

//        public override char ParamChar
//        {
//            get { return ':'; }
//        }
//    }
//}
