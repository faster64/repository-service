using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using static System.Guid;

namespace KiotVietTimeSheet.Utilities
{
    public static class ConvertHelper
    {
        public static DataSet ToDataSet<T>(IList<T> list)
        {
            var elementType = typeof(T);
            var ds = new DataSet();
            var t = new DataTable { TableName = elementType.Name };
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            var dtc = new DataColumn("GUID", typeof(string));
            t.Columns.Add(dtc);
            t.PrimaryKey = new[] { dtc };
            foreach (var propInfo in elementType.GetProperties())
            {
                var colType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                t.Columns.Add(propInfo.Name, colType);
            }

            //go through each property on T and add each value to the table
            foreach (var item in list)
            {
                var row = t.NewRow();
                foreach (var propInfo in elementType.GetProperties())
                {

                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;

                }
                row["GUID"] = NewGuid().ToString();
                t.Rows.Add(row);
            }
            ds.AcceptChanges();
            return ds;
        }
        public static Guid ToGuid(object val, Guid defValue)
        {
            var ret = default(Guid);
            try
            {
                ret = new Guid(val.ToString());
            }
            catch
            {
                // ignored
            }

            return ret;
        }

        public static Guid ToGuid(object val)
        {
            return ToGuid(val.ToString(), Empty);
        }

        public static Guid ToGuid(SqlGuid val)
        {
            return val.IsNull ? Empty : val.Value;
        }

        public static byte ToByte(object obj)
        {
            byte retVal;

            try
            {
                retVal = Convert.ToByte(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static byte ToByte(object obj, byte defaultValue)
        {
            byte retVal;

            try
            {
                if (obj == null) return defaultValue;
                retVal = Convert.ToByte(obj);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        public static int ToInt32(object obj)
        {
            int retVal;

            try
            {
                retVal = Convert.ToInt32(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static int ToInt32(object obj, int defaultValue)
        {
            int retVal;
            try
            {
                if (obj == null)
                    return defaultValue;
                retVal = Convert.ToInt32(obj);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        public static long ToInt64(object obj)
        {
            long retVal;

            try
            {
                retVal = Convert.ToInt64(obj);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static string ToString(object obj)
        {
            string retVal;
            try
            {
                retVal = Convert.ToString(obj);
            }
            catch
            {
                retVal = String.Empty;
            }

            return retVal;
        }

        public static string ToString(object obj, string def)
        {
            string retVal;

            try
            {
                retVal = Convert.ToString(obj);
            }
            catch
            {
                retVal = def;
            }

            return retVal;
        }
        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, DateTime.Now);
        }
        public static DateTime ToDateTime(double obj)
        {
            DateTime retVal;
            try
            {
                retVal = DateTime.FromOADate(obj);
            }
            catch
            {
                retVal = default(DateTime);
            }
            return retVal;
        }
        public static DateTime ToDateTime(object obj, string format)
        {
            DateTime retVal;

            try
            {
                retVal = DateTime.ParseExact(obj.ToString(), format, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                retVal = default(DateTime);
            }

            return retVal;
        }

        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            DateTime retVal;
            try
            {
                retVal = Convert.ToDateTime(obj);
            }
            catch
            {
                retVal = defaultValue;
            }
            if (retVal >= (DateTime)SqlDateTime.MaxValue) return (DateTime)SqlDateTime.MaxValue;
            return retVal <= (DateTime)SqlDateTime.MinValue ? ((DateTime)SqlDateTime.MinValue).AddYears(5) : retVal;
        }

        public static bool ToBoolean(object obj)
        {
            bool retVal;

            try
            {
                retVal = Convert.ToBoolean(obj);
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        public static double ToDouble(object obj)
        {
            double retVal;

            try
            {
                retVal = Convert.ToDouble(obj, CultureInfo.InvariantCulture);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }
        public static double ToDouble(object obj, double defaultValue)
        {
            double retVal;

            try
            {
                retVal = Convert.ToDouble(obj.ToString().Trim(), CultureInfo.InvariantCulture);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }
        public static float ToSingle(object obj)
        {
            float retVal;

            try
            {
                retVal = Convert.ToSingle(obj, CultureInfo.InvariantCulture);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static decimal ToDecimal(object val, decimal defValue)
        {
            try
            {
                decimal dec;
                if (Decimal.TryParse(val.ToString(), out dec))
                    return Convert.ToDecimal(val, CultureInfo.InvariantCulture);

                return defValue;
            }
            catch { return defValue; }
        }

        public static decimal ToDecimal(object val)
        {
            return ToDecimal(val, 0);
        }

        public static decimal ToDecimal(SqlDecimal val)
        {
            return val.IsNull ? decimal.Zero : val.Value;
        }

        public static long ToJavaScriptMilliseconds(DateTime dt)
        {
            var datetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            return ((dt.ToUniversalTime().Ticks - datetimeMinTimeTicks) / 10000);
        }
        public static decimal CurrencyToDecimal(object val)
        {
            var strval = ToString(val);
            strval = strval.Replace("$", string.Empty);
            strval = strval.Replace(",", string.Empty);
            return ToDecimal(strval.Trim());
        }

        /// <summary>
        /// Format to $0.00
        /// </summary>
        /// <param name="val">Value to format</param>
        /// <returns></returns>
        public static string ToCurrency(object val)
        {
            return $"{val:C}";
        }
        /// <summary>
        /// Format to 0.00
        /// </summary>
        /// <param name="val">Value to format</param>
        /// <returns></returns>
        public static string ToNumberic(object val)
        {
            var result = string.Format(new CultureInfo("en-US"), "{0:#,##0}", val);
            return string.IsNullOrEmpty(result) ? "0" : result;
        }
    }
}
