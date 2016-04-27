using System;
namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 允许传入一个对像进行赋值操作。
    /// </summary>
    public interface ISetModel
    {
        void SetListValue(System.Data.DataTable dataTable);
    }
}
