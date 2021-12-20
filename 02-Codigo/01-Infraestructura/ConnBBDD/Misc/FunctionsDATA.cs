using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Utils;

namespace ConnBBDD.Misc
{
    /// <summary>
    /// Clase que implementa métodos y funciones a todo el proyecto de conexion a BBDD
    /// </summary>
    public class FunctionsDATA
    {
        //--------------------------------------------------------------------
        #region Variables y propiedades públicas
        public const string cKeyAttribute = "KeyAttribute";
        #endregion  
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones (PUBLIC)
        /// <summary>
        /// Convierte los elementos de un dadtatable en una lista de entidades
        /// </summary>
        /// <param name="dt">Datatable</param>
        /// <returns></returns>
        public static List<T> ConvertDataTAble<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// Obtiene el valor de un row concreto del datatable y lo convierte en un objeto de entidades para añadir a la lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns></returns>
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        object objValue;

                        if (dr[column.ColumnName] != null)
                        {
                            objValue = dr[column.ColumnName];
                        }
                        else
                        {
                            objValue = "";
                        }

                        try
                        {
                            pro.SetValue(obj, objValue, null);
                        }
                        catch (Exception ex)
                        {
                            Log.SetLog(ex, false);
                            pro.SetValue(obj, "", null);
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        /// <summary>
        /// Formatea un valor concreto según sitipo para devolverlo para concatenar en una cadena de texto (para construir una sentencia INSERT, SELECT o UPDATE
        /// </summary>
        /// <param name="sTypeValue">Tipo de valor (string, int, etc...)</param>
        /// <param name="objValue">Valor a formatear</param>
        /// <returns></returns>
        public static object FormatValue(string sTypeValue, object objValue)
        {
            //Declaración
            object sResult;

            const string cString = "String";
            const string cInt32 = "Int32";
            const string cBoolean = "Boolean";
            const string cDouble = "Double";
            const string cDateTime = "DateTime";

            //Código
            switch (sTypeValue)
            {
                case cInt32:
                    sResult = (Int32)objValue;
                    break;
                case cDouble:
                    sResult = (double)objValue;
                    break;
                case cDateTime:
                    sResult = (DateTime)objValue;
                    break;
                case cString:
                    sResult = (string)objValue.ToString().Trim();
                    break;
                case cBoolean:
                    sResult = (Boolean)objValue;
                    break;
                default:
                    sResult = (string)"";
                    break;
            }

            //Resultado
            return sResult;
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
