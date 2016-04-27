using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 表示已经修改过的模块
    /// </summary>
    public class EditedEntry<T> where T : class ,Rule.IModel, new()
    {
        string connectionStringName = "";
        Tools.ModelHelper<T> model;
        /// <summary>
        /// Initializes a new instance of the <see cref="EditedEntry{T}"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public EditedEntry(string connectionStringName)
        {
            this.connectionStringName = connectionStringName;
            model = new Tools.ModelHelper<T>(connectionStringName);
        }
        /// <summary>
        /// 更改前的数据模型
        /// </summary>
        public T OldDataModel { get; set; }
        /// <summary>
        /// 更改后的数据模型
        /// </summary>
        public T NewDataModel { get; set; }
        /// <summary>
        /// 发生更改的属性和对应旧的值
        /// </summary>
        public Dictionary<string, object> ChangedPropertyOldValue { get; set; }
        /// <summary>
        /// 发生更改的属性和对应的新值
        /// </summary>
        public Dictionary<string, object> ChangedPropertyNewValue { get; set; }
        /// <summary>
        /// 向数据库中更新此变化，返回是否成功，可以添加自定义Where条件，无需where单词
        /// </summary>
        public bool UpdateModel()
        {
            var obj = (T)NewDataModel;
            var primeryKey = typeof(T).GetProperty(model.GetPrimeryKey());
            var primeryKeyValue = primeryKey.GetValue(OldDataModel, null);
            var newPrimeryKeyValue = primeryKey.GetValue(NewDataModel, null);
            primeryKey.SetValue(NewDataModel, null, null);//清空它的主键值
            bool isUpdateSuccess = model.EditByPrimeryKey(NewDataModel, primeryKeyValue);
            List<string> col = new List<string>();
            col.Add(primeryKey.Name);
            var sql = model.SqlHelper.getUpdateSqlParam(obj.TableName, col, String.Format("{0}=@OldPrimeryKey", primeryKey.Name));
            var db = DataHelperFactory.CreateInstence(this.connectionStringName);
            try
            {
                var execPrimery = db.getNumber(sql, db.createParameter("@" + primeryKey.Name, newPrimeryKeyValue), db.createParameter("@OldPrimeryKey", primeryKeyValue));
                if (execPrimery >= 0) primeryKey.SetValue(NewDataModel, newPrimeryKeyValue, null);//如果主键列可以被更新，且更新成功，则赋予它新的主键值
            }
            catch
            {   //还原它的主键
                primeryKey.SetValue(NewDataModel, primeryKeyValue, null);//清空它的主键值 
            }

            return isUpdateSuccess;
        }
    }
}
