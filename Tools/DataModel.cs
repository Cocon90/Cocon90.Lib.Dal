using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocon90.Lib.Dal.Tools
{
    /// <summary>
    /// DataTable与List之间的转换
    /// </summary>
    public class DataModel<T> where T : new()
    {
        /// <summary>
        /// DataTable转实体集合
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>泛类列表</returns>
        public virtual List<T> GetList(DataTable table)
        {
            List<T> result;
            if (table == null)
            {
                result = new List<T>();
            }
            else
            {
                List<T> list = new List<T>();
                Type typeFromHandle = typeof(T);
                PropertyInfo[] properties = typeFromHandle.GetProperties();
                foreach (DataRow dataRow in table.Rows)
                {
                    T t = (T)((object)Activator.CreateInstance(typeFromHandle));
                    PropertyInfo[] array = properties;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyInfo propertyInfo = array[i];
                        if (dataRow.Table.Columns.Contains(propertyInfo.Name))
                        {
                            if (propertyInfo.CanWrite)
                            {
                                if (dataRow[propertyInfo.Name] is DBNull)
                                {
                                    propertyInfo.SetValue(t, null, null);
                                }
                                else
                                {
                                    var val = dataRow[propertyInfo.Name];
                                    try { propertyInfo.SetValue(t, val, null); }
                                    catch
                                    {
                                        if (val is double && (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?)))
                                        {
                                            if (val == null) { propertyInfo.SetValue(t, null, null); }
                                            else { propertyInfo.SetValue(t, decimal.Parse(val + ""), null); }
                                        }
                                        if (val is string && (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)))
                                        {
                                            if (val == null) { propertyInfo.SetValue(t, null, null); }
                                            else { propertyInfo.SetValue(t, DateTime.Parse(val + ""), null); }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                    list.Add(t);
                }
                result = list;
            }
            return result;
        }
        /// <summary>
        /// 实体集合转DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(IList<T> list)
        {
            //检查实体集合不能为空
            if (list == null || list.Count < 1)
            {
                return null;
            }

            //取出第一个实体的所有Propertie
            Type entityType = list[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            var dt = new DataTable();
            foreach (PropertyInfo t in entityProperties)
            {
                object value = t.GetValue(list[0], null);
                if (value == null) { dt.Columns.Add(t.Name, typeof(string)); }
                else { dt.Columns.Add(t.Name, value.GetType()); }
            }
            //将所有entity添加到DataTable中
            foreach (object entity in list)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    continue;
                }
                var entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
    }
}
