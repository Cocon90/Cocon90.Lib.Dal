using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 分页后的实体传递中间实体
    /// </summary>
    [DataContract]
    [Serializable]
    public class PagedModel
    {
        /// <summary>
        /// 分页后的表
        /// </summary>
        [DataMember]
        public System.Data.DataTable Table { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        [DataMember]
        public int Total { get; set; }
    }
    /// <summary>
    ///  分页后的实体传递中间实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    [Serializable]
    public class PagedModel<T>
    {
        /// <summary>
        /// 分页后的List
        /// </summary>
        [DataMember]
        public List<T> List { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        [DataMember]
        public int Total { get; set; }
    }
   
    /// <summary>
    ///  分页后的实体传递中间实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    [Serializable]
    public class PagedList
    {
        /// <summary>
        /// 分页后的List
        /// </summary>
        [DataMember]
        public IList List { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        [DataMember]
        public int Total { get; set; }
    }
}
