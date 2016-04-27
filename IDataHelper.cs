using Cocon90.Lib.Dal.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal
{
    /// <summary>
    /// 表示ADO操作接口
    /// </summary>
    public interface IDataHelper
    {

        /// <summary>
        /// 获取 当前DataHelper相应的参数前坠
        /// </summary>
        char ParamChar { get; }
        /// <summary>
        /// 取得或设置 当前实例对应的连接字符串
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// 创建一个事务。
        /// </summary>
        /// <returns></returns>
        Tools.TransactionExcutor beginTransaction();
        /// <summary>
        /// 传入一个隔离级别，并创建一个事务。
        /// </summary>
        Tools.TransactionExcutor beginTransaction(System.Data.IsolationLevel isolationLevel);
        /// <summary>
        /// 建立一个事务域对像，与createTransactionScope()方法的区别是，主程序在调用此方法后，只能通过disposeTransactionScopeObject方法来提交事务，优点是调用程序无需引入System.Transactions.dll即可使用事务。
        /// </summary>
        /// <returns></returns>
        object createTransactionScopeObject();
        /// <summary>
        /// 建立一个事务域对像，与createTransactionScope()方法的区别是，主程序在调用此方法后，只能通过disposeTransactionScopeObject方法来提交事务，优点是调用程序无需引入System.Transactions.dll即可使用事务。
        /// </summary>
        /// <param name="transactionScopeOption"></param>
        /// <returns></returns>
        object createTransactionScopeObject(Rule.TransactionScopeOption transactionScopeOption);
        /// <summary>
        /// 建立一个事务域对像，与createTransactionScope()方法的区别是，主程序在调用此方法后，只能通过disposeTransactionScopeObject方法来提交事务，优点是调用程序无需引入System.Transactions.dll即可使用事务。
        /// </summary>
        /// <param name="transactionScopeOption"></param>
        /// <param name="transactionOptions"></param>
        /// <returns></returns>
        object createTransactionScopeObject(Rule.TransactionScopeOption transactionScopeOption, Rule.TransactionOptions transactionOptions);
        /// <summary>
        /// 创建一个事务域，如需要手动提交事务，请调用其Complate方法。
        /// </summary>
        /// <returns></returns>
        System.Transactions.TransactionScope createTransactionScope();
        /// <summary>
        /// 创建一个事务域，传入事务域的模式，如需要提交事务，请调用其Complate方法。
        /// </summary>
        /// <param name="transactionScopeOption"></param>
        /// <returns></returns>
        System.Transactions.TransactionScope createTransactionScope(System.Transactions.TransactionScopeOption transactionScopeOption);
        /// <summary>
        /// 创建一个事务域，传入事务域的模式和设置，如需要提交事务，请调用其Complate方法。
        /// </summary>
        /// <param name="transactionScopeOption"></param>
        /// <param name="transactionOptions"></param>
        /// <returns></returns>
        System.Transactions.TransactionScope createTransactionScope(System.Transactions.TransactionScopeOption transactionScopeOption, System.Transactions.TransactionOptions transactionOptions);
        /// <summary>
        /// 释放事务域，传入要提交并释放的事务域对像(与disposeTransactionScope方法完全相同，仅仅是参数不同)，和是否提交事务，最后释放事务域资源。返回操作是否成功完成。
        /// </summary>
        /// <param name="transactionScopeObject"></param>
        /// <param name="isCommit"></param>
        /// <returns></returns>
        bool disposeTransactionScopeObject(object transactionScopeObject, bool isCommit);
        /// <summary>
        /// 释放事务域，传入要提交并释放的事务域对像(与disposeTransactionScopeObject方法完全相同，仅仅是参数不同)，和是否提交事务，最后释放事务域资源。返回操作是否成功完成。
        /// </summary>
        /// <returns></returns>
        bool disposeTransactionScope(System.Transactions.TransactionScope transactionScope, bool isCommit);
        /// <summary>
        /// 创建一个DbCommand实例，注意DBCommand被创建后，对应的Connection将保持打开，所以请在使用完DBCommand后，及时关闭它的Connection。
        /// </summary>
        DbCommand createCommond(string tsqlParamed, CommandType commandType, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 创建一个DbParameter实例
        /// </summary>
        DbParameter createParameter(string key, object value);
        /// <summary>
        /// 创建一个DbParameter数组实例.
        /// </summary>
        DbParameter[] createParamters(params object[][] keyValueArr);
        /// <summary>
        /// 创建一个DbParameter数组实例.
        /// </summary>
        DbParameter[] createParamters(Dictionary<string, object> keyValueDic);
        /// <summary>
        /// 创建一个DataAdapter实例
        /// </summary>
        DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd);
        /// <summary>
        /// 创建一个DbConnection实例，并打开连接。
        /// </summary>
        System.Data.Common.DbConnection createConnection();
        /// <summary>
        /// 批量使用事务执行Sql
        /// </summary>
        /// <param name="sqlBatch"></param>
        /// <param name="isCommit"></param>
        /// <param name="allowThrowException"></param>
        /// <returns></returns>
        int execBatch(IEnumerable<SqlBatch> sqlBatch, bool isCommit, bool allowThrowException = true);
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回第一行第一列
        /// </summary>
        object getScalar(string tsqlParamed, params   Parameter[] paramKeyAndValue);
        /// <summary>
        ///  执行存储过程的查询操作 返回第一行第一列
        /// </summary>
        object getScalarProc(string storedProcedureName, params Parameter[] parameterValues);


        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回String类型的第一行第一列，如果结果为空，则返回""。
        /// </summary>
        string getString(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        ///  执行存储过程的查询操作 返回String类型的第一行第一列，如果结果为空，则返回""。
        /// </summary>
        string getStringProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回int类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        int getNumber(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        ///  执行存储过程的查询操作 返回int类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        int getNumberProc(string storedProcedureName, params Parameter[] parameterValues);
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回long类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        long getLong(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回long类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        long getLongProc(string storedProcedureName, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回float类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        float getFloat(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        ///  执行存储过程的查询操作 返回float类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        float getFloatProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回DateTime类型的第一行第一列，如果结果为空，则返回"1970-1-1"。
        /// </summary>
        DateTime getDateTime(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回DateTime类型的第一行第一列，如果结果为空，则返回"1970-1-1"。
        /// </summary>
        DateTime getDateTimeProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回bool类型的第一行第一列，如果结果为空，则返回"false"。为True的语句是："on","1","true","yes" 
        /// </summary>
        bool getBoolean(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        ///  执行存储过程的查询操作 返回bool类型的第一行第一列，如果结果为空，则返回"false"。为True的语句是："on","1","true","yes" 
        /// </summary>
        bool getBooleanProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        /// 执行tsql(可以是带参的)更新操作 返回受影响行数，执行失败返回-1
        /// </summary>
        int execNoQuery(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的更新操作 返回受影响行数，执行失败返回-1
        /// </summary>
        int execNoQueryProc(string storedProcedureName, params Parameter[] parameterValues);


        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回结果DataSet
        /// </summary>
        DataSet getDataSet(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果DataSet
        /// </summary>
        DataSet getDataSetProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回结果表，如果结果为空，则返回一个空的表
        /// </summary>
        DataTable getTable(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果表，如果结果为空，则返回一个空的表
        /// </summary>
        DataTable getTableProc(string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        ///  执行tsql(可以是带参的)操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合，isDistinct表示是否结果是否去重复 
        /// </summary>
        List<string> getListString(bool isDistinct, string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合。isDistinct表示是否结果是否去重复 
        /// </summary>
        List<string> getListStringProc(bool isDistinct, string storedProcedureName, params Parameter[] parameterValues);
        /// <summary>
        ///  执行tsql(可以是带参的)操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合（不去重复）
        /// </summary>
        List<string> getListString(string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合（不去重复）
        /// </summary>
        List<string> getListStringProc(string storedProcedureName, params Parameter[] parameterValues);
        /// <summary>
        ///  执行tsql(可以是带参的)操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合，isDistinct表示是否结果是否去重复 ， convertErrorDefaultValue表示当转换类型出错时使用的默认值。
        /// </summary>
        List<long> getListLong(bool isDistinct, long convertErrorDefaultValue, string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合，isDistinct表示是否结果是否去重复 ， convertErrorDefaultValue表示当转换类型出错时使用的默认值。
        /// </summary>
        List<long> getListLongProc(bool isDistinct, long convertErrorDefaultValue, string storedProcedureName, params Parameter[] parameterValues);

        /// <summary>
        ///  执行tsql(可以是带参的)操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合，(不去重复) 参数：convertErrorDefaultValue表示当转换类型出错时使用的默认值。
        /// </summary>
        List<long> getListLong(long convertErrorDefaultValue, string tsqlParamed, params Parameter[] paramKeyAndValue);
        /// <summary>
        /// 执行存储过程的查询操作 返回结果列的第一列，如果结果为空，则返回一个空泛型集合，(不去重复) 参数：convertErrorDefaultValue表示当转换类型出错时使用的默认值。
        /// </summary>
        List<long> getListLongProc(long convertErrorDefaultValue, string storedProcedureName, params Parameter[] parameterValues);
        /// <summary>
        /// 获取 当前DataHelper相应的SqlHelper对像
        /// </summary>
        /// <returns></returns>
        Tools.SqlHelper getSqlHelper();
    }
}
