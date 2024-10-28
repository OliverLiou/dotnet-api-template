using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
// using NPOI.XSSF.UserModel; //XSSF 用來產生Excel 2007檔案（.xlsx）
// using NPOI.SS.UserModel;
// using NPOI.HSSF.UserModel;
using System.Reflection;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace Quickly_PriceQuotationApi
{
    public class PublicMethod
    {
        public PublicMethod()
        {

        }

        // public static bool IsHook(ICell cell)
        // {
        //     try
        //     {

        //         string value = cell.CellType == CellType.Formula ? cell.StringCellValue : cell.ToString().Trim();

        //         if (value == "V" | value == "v")
        //             return true;
        //         else
        //             return false;
        //     }
        //     catch (Exception)
        //     {
        //         return false;
        //     }
        // }

        // //取得欄位值
        // public static T? getCellValue<T>(ICell cell)
        // {
        //     try
        //     {
        //         if (cell.CellType == CellType.Blank)
        //             return default;

        //         object obj = null;
        //         if (cell.CellType == CellType.String)
        //             obj = cell.StringCellValue.Trim();
        //         else if (cell.CellType == CellType.Numeric)
        //         {
        //             if (DateUtil.IsCellInternalDateFormatted(cell) || DateUtil.IsCellDateFormatted(cell))
        //                 obj = cell.DateCellValue!;
        //             else
        //                 obj = cell.NumericCellValue;
        //         }
        //         else if (cell.CellType == CellType.Boolean)
        //             obj = cell.BooleanCellValue;


        //         return (T?)Convert.ChangeType(obj, typeof(T));
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        /// <summary>
        /// 判斷及拼接where
        /// </summary>
        /// <param name="querySearch">搜尋字</param>
        /// <param name="piInfos">物件屬性</param>
        /// <param name="outResult">DB物件</param>
        /// <returns></returns>
        public static IQueryable<T> setWhereStr<T>(string querySearch, PropertyInfo[] piInfos, IQueryable<T> outResult)
        {
            var queryList = new List<string>();
            DateTime dateTime = DateTime.Now;
            string format = "yyyyMMdd";

            foreach (var pi in piInfos)
            {
                if (DateTime.TryParseExact(querySearch, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    if (Type.Equals(pi.PropertyType, typeof(DateTime?)))
                    {
                        queryList.Add($"({pi.Name} != null || {pi.Name}.Value.Date >= @0 && {pi.Name}.Value.Date <= @0)");
                    }
                }

                if (Type.Equals(pi.PropertyType, typeof(int)) && querySearch.All(char.IsNumber))
                {
                    queryList.Add($"{pi.Name}.ToString().Contains(\"{querySearch}\")");
                }
                if (Type.Equals(pi.PropertyType, typeof(long)) && querySearch.All(char.IsNumber))
                {
                    queryList.Add($"{pi.Name}.ToString().Contains(\"{querySearch}\")");
                }

                Boolean parsedValue;
                if (Boolean.TryParse(querySearch, out parsedValue))
                {
                    if (Type.Equals(pi.PropertyType, typeof(bool)))
                    {
                        queryList.Add($"{pi.Name} == (\"{querySearch}\")");
                    }
                }

                if (Type.Equals(pi.PropertyType, typeof(string)))
                {
                    queryList.Add($"{pi.Name}.Contains(\"{querySearch}\")");
                }
            }
            //若有日期則擺上對應時間
            outResult = outResult.Where(String.Join(" || ", queryList.ToArray()), dateTime);
            return outResult;
        }

    }
}