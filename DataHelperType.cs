using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal
{
    /// <summary>
    /// 数据库辅助类的类型
    /// </summary>
    public enum DataHelperType
    {
        FirebirdDataHelper = 10,
        MySqlDataHelper = 20,
        OdbcDataHelper = 30,
        OledbDataHelper = 40,
        OracleDataHelper = 50,
        PostgreSqlDataHelper = 60,
        SqlCeDataHelper = 70,
        SQLDataHelper = 80,
        SQLiteDataHelper = 90
    }
}
