using Cocon90.Lib.Dal.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Tools
{
    /// <summary>
    /// 事务执行者对像，但是需要传入一个新的代理DataHelper来构造。
    /// </summary>
    public class TransactionExcutor : IDisposable
    {
        #region 构造函数
        /// <summary>
        /// 在某个DataHelper上，构造一个事务执行者,并开始事务。
        /// </summary>
        public TransactionExcutor(IDataHelper dataHelper)
        {
            this.ProxyDataHelper = dataHelper;
            this.Connection = dataHelper.createConnection();
            this.Transaction = this.Connection.BeginTransaction();
        }
        /// <summary>
        /// 在某个DataHelper上，构造一个事务执行者,并开始事务。可以传入隔离级别（事务锁定行为）
        /// </summary>
        public TransactionExcutor(IDataHelper dataHelper, System.Data.IsolationLevel isolationLevel)
        {
            this.ProxyDataHelper = dataHelper;
            this.Connection = dataHelper.createConnection();
            this.Transaction = this.Connection.BeginTransaction(isolationLevel);
        }
        #endregion

        #region 常用属性
        /// <summary>
        /// 获取 事务执行者构造时的代理DataHelper对像
        /// </summary>
        public IDataHelper ProxyDataHelper { get; private set; }
        /// <summary>
        /// 获取 事务执行者对应的连接对像
        /// </summary>
        public DbConnection Connection { get; private set; }
        /// <summary>
        /// 获取 事务执行者对应的隔离级别（事务锁定行为）
        /// </summary>
        public System.Data.IsolationLevel IsolationLevel { get { return this.Transaction.IsolationLevel; } }
        /// <summary>
        /// 获取 当前事物执行者对应的事物对像。
        /// </summary>
        public DbTransaction Transaction { get; private set; }
        #endregion

        #region 规定方法
        /// <summary>
        /// 执行Transaction对像的事务回滚操作，可以传入是否执行完成后，释放TransactionExcutor的连接对象
        /// </summary>
        public virtual void Rollback(bool isDisposeConnectionOnExcuteFinish = false)
        {
            this.Transaction.Rollback();
            if (isDisposeConnectionOnExcuteFinish) { this.Dispose(); }
        }
        /// <summary>
        /// 执行Transaction对像的事务提交操作，可以传入是否执行完成后，释放TransactionExcutor的连接对象
        /// </summary>
        public virtual void Commit(bool isDisposeConnectionOnExcuteFinish = false)
        {
            this.Transaction.Commit();
            if (isDisposeConnectionOnExcuteFinish) { this.Dispose(); }
        }
        /// <summary>
        /// 释放此对象占用的资源。
        /// </summary>
        public virtual void Dispose()
        {
            try//可能对像已经释放。
            {
                if (Connection != null && Connection.State != ConnectionState.Closed) { Connection.Close(); Connection.Dispose(); }
            }
            catch { }
        }
        /// <summary>
        /// 返回一个DbConnection对像
        /// </summary>
        public virtual DbConnection getConnection()
        {
            return this.Connection;
        }
        /// <summary>
        /// 创建一个DbCommand对像
        /// </summary>
        public virtual DbCommand createCommond(string tsqlParamed, System.Data.CommandType commandType, params Parameter[] paramKeyAndValue)
        {
            var cmd = this.Connection.CreateCommand();
            cmd.CommandText = tsqlParamed.Replace('@', ProxyDataHelper.ParamChar);
            cmd.CommandType = commandType;
            if (paramKeyAndValue != null)
            {
                var paramsList = paramKeyAndValue.ToList().ConvertAll(p => createParameter(p.Name, p.Value));
                cmd.Parameters.AddRange(paramsList.ToArray());
            }
            cmd.Transaction = this.Transaction;
            return cmd;

        }
        /// <summary>
        /// 创建一个DbDataAdapter对像
        /// </summary>
        public virtual DbDataAdapter createDataAdapter(DbCommand selectCmd)
        {
            var adp = ProxyDataHelper.createDataAdapter(selectCmd);
            return adp;
        }
        /// <summary>
        /// 创建一个DbParameter对像
        /// </summary>
        public virtual DbParameter createParameter(string key, object value)
        {
            return ProxyDataHelper.createParameter(key, value);
        }
        #endregion

        #region 常用执行方法体
        /// <summary>
        /// 执行ExecuteNonQuery命令（Insert、Delete、Update） 返回受影响行数。
        /// </summary>
        public virtual int execNoQuery(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            int temp = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return temp;
        }
        /// <summary>
        /// 执行存储过程 返回受影响行数。
        /// </summary>
        public virtual int execNoQueryProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            int temp = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return temp;
        }
        /// <summary>
        /// 执行ExecuteScalar命令（Select） 返回执行结果表的第一行第一列值
        /// </summary>
        public virtual object getScalar(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            object temp = cmd.ExecuteScalar();
            cmd.Dispose();
            return temp;
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回第一行第一列
        /// </summary>
        public virtual object getScalarProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            object temp = cmd.ExecuteScalar();
            cmd.Dispose();
            return temp;
        }
        /// <summary>
        /// 对Bool类型进行转换，boolString为True的语句是："on","1","true","yes" 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static bool toBool(string boolString)
        {
            boolString = boolString + "";
            if (boolString.ToLower() == "on" || boolString.ToLower() == "1" || boolString.ToLower() == "true" || boolString.ToLower() == "yes") return true;
            else return false;
        }
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回bool类型的第一行第一列，如果结果为空，则返回"false"。为True的语句是："on","1","true","yes" 
        /// </summary>
        public virtual bool getBoolean(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return toBool(getScalar(tsqlParamed, paramKeyAndValue) + "");
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回bool类型的第一行第一列，如果结果为空，则返回"false"。为True的语句是："on","1","true","yes" 
        /// </summary>
        public virtual bool getBooleanProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            return toBool(getScalarProc(storedProcedureName, paramKeyAndValue) + "");
        }
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回结果DataSet
        /// </summary>
        public virtual System.Data.DataSet getDataSet(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {

            DataSet ds = new DataSet();
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            var dap = createDataAdapter(cmd);
            dap.Fill(ds);
            cmd.Dispose();
            dap.Dispose();
            return ds;
        }
        /// <summary>
        /// 执行存储过程的查询操作 返回结果DataSet
        /// </summary>
        public virtual System.Data.DataSet getDataSetProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DataSet ds = new DataSet();
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            var dap = createDataAdapter(cmd);
            dap.Fill(ds);
            cmd.Dispose();
            dap.Dispose();
            return ds;
        }
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回DateTime类型的第一行第一列，如果结果为空，则返回"1970-1-1"。
        /// </summary>
        public virtual DateTime getDateTime(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            DateTime temp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }
        /// <summary>
        /// 执行存储过程的查询操作 返回DateTime类型的第一行第一列，如果结果为空，则返回"1970-1-1"。
        /// </summary>
        public virtual DateTime getDateTimeProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DateTime temp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回float类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        public virtual float getFloat(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            float temp = -1;
            float.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回float类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        public virtual float getFloatProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            float temp = -1;
            float.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }
        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回int类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        public virtual int getNumber(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            int temp = -1;
            int.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回int类型的第一行第一列，如果结果为空，则返回"-1"。
        /// </summary>
        public virtual int getNumberProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            int temp = -1;
            int.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回String类型的第一行第一列，如果结果为空，则返回""。
        /// </summary>
        public virtual string getString(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return getScalar(tsqlParamed, paramKeyAndValue) + "";
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回String类型的第一行第一列，如果结果为空，则返回""。
        /// </summary>
        public virtual string getStringProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            return getScalarProc(storedProcedureName, paramKeyAndValue) + "";
        }

        /// <summary>
        /// 执行tsql(可以是带参的)查询操作 返回结果表，如果结果为空，则返回一个空的表
        /// </summary>
        public virtual System.Data.DataTable getTable(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            DataTable dt = new DataTable();
            var ds = getDataSet(tsqlParamed, paramKeyAndValue);
            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;

        }
        /// <summary>
        /// 执行存储过程的查询操作 返回结果表，如果结果为空，则返回一个空的表
        /// </summary>
        public virtual System.Data.DataTable getTableProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DataTable dt = new DataTable();
            var ds = getDataSetProc(storedProcedureName, paramKeyAndValue);
            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }
        #endregion


    }
}
