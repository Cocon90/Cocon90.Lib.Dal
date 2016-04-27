using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 包含实体集合的类  泛型T表示集合中元素的类型。
    /// </summary>
    /// <typeparam name="T">表示集合中元素的类型</typeparam>
    [DataContract]
    [Serializable]
    public class ModelData<T> : Cocon90.Lib.Dal.Rule.ISetModel where T : new()
    {
        List<T> lst = new List<T>();
        /// <summary>
        /// 获取或设置 数据类型集合。
        /// </summary>
        [DataMember]
        public List<T> List { get { return lst; } set { lst = value; } }
        /// <summary>
        /// 获取 数据集合中的第一个实体。如List为空，则返回NULL或者其数据类型默认值。
        /// </summary>
        public T TopOneModel
        {
            get
            {
                if (this.List == null || this.List.Count <= 0) { return default(T); }
                else return List.FirstOrDefault();
            }
            set { }
        }
        Guid guid = Guid.Empty;
        /// <summary>
        /// 获取 本ModelData对象的唯一标志
        /// </summary>
        [DataMember]
        public Guid ID { get { if (guid == Guid.Empty) { guid = Guid.NewGuid(); } return guid; } set { guid = value; } }
        /// <summary>
        /// 返回唯一编码
        /// </summary>
        /// <returns></returns>
        public virtual string ToString()
        {
            return this.ID.ToString();
        }
        /// <summary>
        /// 获取 集体中元素的数量。
        /// </summary>
        public int ListCount { get { if (this.List == null) { return 0; } else { return this.List.Count; } } }
        /// <summary>
        /// 分页获取List属性所表示的集合中的数据。传入请求页数，和总页数。
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedModel<T> GetPagedList(int pageNum, int pageSize)
        {
            PagedModel<T> pm = new PagedModel<T>();
            if (this.List == null) return pm;
            pm.Total = List.Count;
            if (pageNum <= 0 || pageSize <= 0) { pm.List = new List<T>(); }
            else
            {
                pm.List = List.Take(pageNum * pageSize).Skip((pageNum - 1) * pageSize).ToList();
            }
            return pm;
        }
        /// <summary>
        /// 获取泛型T的类型。
        /// </summary>
        public Type TType { get { return typeof(T); }  }
        private Cocon90.Lib.Dal.Tools.DataModel<T> dm = new Tools.DataModel<T>();
        /// <summary>
        /// 传入一个DataTable对像，将它想办法转为List集合，并付给当前对像的List属性。
        /// </summary>
        /// <param name="dataTable"></param>
        public void SetListValue(DataTable dataTable)
        {
            this.List = dm.GetList(dataTable);
        }

    }
}
