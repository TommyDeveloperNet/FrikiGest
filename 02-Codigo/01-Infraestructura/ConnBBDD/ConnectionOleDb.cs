using System;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Utils;

namespace ConnBBDD
{
    /// <summary>
    /// Clase que gestiona la conexión a base de datos así como las operaciones de select / insert / udpate y delete para una base de datos de tipo ACCESS
    /// </summary>
    public class ConnectionOleDb : IDisposable
    {
        //--------------------------------------------------------------------
        #region variables y constantes (privadas)
        bool disposed = false;
        OleDbConnection objConnection;
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region constructores y destructores conexion BBDD
        public ConnectionOleDb(OleDbConnection objConnection)
        {
            this.objConnection = objConnection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //Objetos a liberar / eliminar
            }

            disposed = true;
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones (PUBLIC)
        /// <summary>
        /// Obtiene todos los elementos (registros) de una tabla o vista de la BBDD (ACCESS)
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="sTable_View_Name">Nombre de la tabla / vista sobre la que obtener los registros</param>
        /// <param name="sqlCommand">OPCIONAL - Sentencia SQL a ejecutar</param>
        /// <returns></returns>
        public List<T> Select<T>(string sTable_View_Name, string sqlCommand = "")
        {
            //Declaración
            List<T> lResult;

            string sCommand = "Select * from {0}";
            OleDbCommand objComando = new OleDbCommand();
            OleDbDataAdapter objAdaptador = null;
            DataSet miDataSet = new DataSet();

            //Código
            objComando.CommandText = string.Format(sCommand, sTable_View_Name);
            objComando.Connection = objConnection;

            if (!string.IsNullOrWhiteSpace(sqlCommand))
            {
                objComando.CommandText = sqlCommand;
            }

            //-Ejecutamos sentencia SQL
            try
            {
                objAdaptador = new OleDbDataAdapter(objComando.CommandText, objComando.Connection);
            }
            catch (Exception ex)
            {
                //ERROR!!!!!
                Log.SetLog(ex, false);
                lResult = new List<T>();
                return lResult;
            }

            //-Pasamos valores a un DataSet
            try
            {
                objAdaptador.Fill(miDataSet, sTable_View_Name);
            }
            catch (Exception ex)
            {
                //ERROR!!!!!
                Log.SetLog(ex, false);
                lResult = new List<T>();
                return lResult;
            }

            //Resultado
            lResult = Misc.FunctionsDATA.ConvertDataTAble<T>(miDataSet.Tables[sTable_View_Name]);
            return lResult;
        }

        /// <summary>
        /// Inserta un registro en una tabla de ACCESS
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="sTable_View_Name">Nombre de la tabla en la cual realizar el insert</param>
        /// <param name="objDatosAdd">Información a insertar</param>
        /// <returns></returns>
        public bool Insert<T>(string sTable_View_Name, T objDatosAdd)
        {
            //Declaración
            bool bResult = false;
            string sbCommand = "";
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();

            Type typeOfDatosAdd = objDatosAdd.GetType();
            PropertyInfo[] propInfoDatosAdd = typeOfDatosAdd.GetProperties();

            string sField = "";
            object fieldValue;
            Type fieldValueType;

            bool bFieldValueToInsert = true;

            //Código
            sbCommand = "Insert into {0} ({1}) Values ({2})";

            List<Misc.ParametersWithValue> lstParamsInsert = new List<Misc.ParametersWithValue>();

            foreach (PropertyInfo prop in propInfoDatosAdd)
            {
                object[] lstAttrs = prop.GetCustomAttributes(true);
                bFieldValueToInsert = true;
                foreach (object attr in lstAttrs)
                {
                    if (attr.GetType().Name == Misc.FunctionsDATA.cKeyAttribute)
                    {
                        bFieldValueToInsert = false;
                        break;
                    }
                }

                if (bFieldValueToInsert)
                {
                    sField = prop.Name.Trim();
                    fieldValue = objDatosAdd.GetType().GetProperty(sField).GetValue(objDatosAdd, null);

                    //Columna...
                    if (sbColumns.Length > 0)
                    {
                        sbColumns.Append(", ");
                    }

                    sbColumns.Append("[" + sField + "]");

                    //Valor...
                    if (sbValues.Length > 0)
                    {
                        sbValues.Append(", ");
                    }

                    fieldValueType = fieldValue.GetType();

                    sbValues.Append("?");
                    lstParamsInsert.Add(new Misc.ParametersWithValue { sParam = "@" + sField, sValue = Misc.FunctionsDATA.FormatValue(fieldValueType.Name, fieldValue) });
                }
            }

            sbCommand = string.Format(sbCommand, sTable_View_Name, sbColumns.ToString(), sbValues.ToString());

            //Una vez construida la sentencia INSERT, añadimos...
            OleDbCommand objComando = new OleDbCommand();

            objComando.CommandType = CommandType.Text;
            objComando.Connection = objConnection;
            objComando.CommandText = sbCommand.Trim();

            foreach (Misc.ParametersWithValue Parameter in lstParamsInsert)
            {
                objComando.Parameters.AddWithValue(Parameter.sParam, Parameter.sValue);
            }

            try
            {
                objComando.Connection.Open();
                objComando.ExecuteNonQuery();
                {
                    objComando.Connection.Close();
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                Log.SetLog(ex, false);
                bResult = false;
            }

            objComando.Dispose();

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Actualiza un registro en una tabla de ACCESS
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="sTable_View_Name">Nombre de la tabla en la cual realizar el update</param>
        /// <param name="objDatosUpdate">Datos a actualizar</param>
        /// <param name="IDUpdate">Valor ID del registro a actualizar</param>
        /// <param name="sFieldID">Nombre del campo ID (Por defecto ID) de la tabla a actualizar</param>
        /// <returns></returns>
        public bool Update<T>(string sTable_View_Name, T objDatosUpdate, int IDUpdate, string sFieldID = "ID")
        {
            //Declaración
            bool bResult = false;
            string sbCommand = "";
            StringBuilder sbValues = new StringBuilder();

            //Código
            Type typeOfDatosUpd = objDatosUpdate.GetType();
            PropertyInfo[] propInfoDatosAdd = typeOfDatosUpd.GetProperties();
            sFieldID = "[" + sFieldID + "]";

            string sField = "";
            string sFieldParam = "";
            string sAppend = "{0} = {1}";
            object fieldValue;
            Type fieldValueType;

            bool bFieldValueToUpdate = true;

            sbCommand = "Update {0} SET {1} where {2} = {3}";

            List<Misc.ParametersWithValue> lstParamsUpdate = new List<Misc.ParametersWithValue>();

            foreach (PropertyInfo prop in propInfoDatosAdd)
            {
                object[] lstAttrs = prop.GetCustomAttributes(true);
                bFieldValueToUpdate = true;
                foreach (object attr in lstAttrs)
                {
                    if (attr.GetType().Name == Misc.FunctionsDATA.cKeyAttribute)
                    {
                        bFieldValueToUpdate = false;
                        break;
                    }
                }

                if (bFieldValueToUpdate)
                {
                    sField = prop.Name.Trim();
                    sFieldParam = "@" + sField;

                    fieldValue = objDatosUpdate.GetType().GetProperty(sField).GetValue(objDatosUpdate, null);

                    if (sbValues.Length > 0)
                    {
                        sbValues.Append(", ");
                    }

                    sbValues.Append(string.Format(sAppend, "[" + sField + "]", sFieldParam));

                    fieldValueType = fieldValue.GetType();

                    lstParamsUpdate.Add(new Misc.ParametersWithValue { sParam = sFieldParam, sValue = Misc.FunctionsDATA.FormatValue(fieldValueType.Name, fieldValue) });
                }
            }

            sbCommand = string.Format(sbCommand, sTable_View_Name, sbValues.ToString(), sFieldID, IDUpdate.ToString());

            //Una vez construida la sentencia UPDATE, actualizamos...
            OleDbCommand objComando = new OleDbCommand();

            objComando.CommandType = CommandType.Text;
            objComando.Connection = objConnection;
            objComando.CommandText = sbCommand.Trim();

            foreach (Misc.ParametersWithValue Parameter in lstParamsUpdate)
            {
                objComando.Parameters.AddWithValue(Parameter.sParam, Parameter.sValue);
            }

            try
            {
                objComando.Connection.Open();
                objComando.ExecuteNonQuery();
                {
                    objComando.Connection.Close();
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                Log.SetLog(ex, false);
                bResult = false;
            }

            objComando.Dispose();

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Elimina un registro en una tabla de ACCESS
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="sTable_View_Name">Nombre d ela tabla en la cual realizar el delete</param>
        /// <param name="IDDelete">Valor ID del registro a eliminar</param>
        /// <param name="sFieldID">NOmbre del campo ID (por defecto ID) de la tabla a actualizar (eliminar registro)</param>
        /// <returns></returns>
        public bool Delete<T>(string sTable_View_Name, int IDDelete, string sFieldID = "ID")
        {
            //Declaración
            bool bResult = false;
            string sbCommand = "";

            //Código
            sbCommand = "Delete * from {0} where {1} = {2}";
            sFieldID = "[" + sFieldID + "]";
            sbCommand = string.Format(sbCommand, sTable_View_Name, sFieldID, IDDelete.ToString());

            //Una vez construida la sentencia DELETE, eliminamos...
            OleDbCommand objComando = new OleDbCommand();

            objComando.CommandType = CommandType.Text;
            objComando.Connection = objConnection;
            objComando.CommandText = sbCommand.Trim();

            try
            {
                objComando.Connection.Open();
                objComando.ExecuteNonQuery();
                {
                    objComando.Connection.Close();
                }
                bResult = true;
            }
            catch (Exception ex)
            {
                Log.SetLog(ex, false);
                bResult = false;
            }

            objComando.Dispose();

            //Resultado
            return bResult;

        }
        #endregion
        //--------------------------------------------------------------------
    }
}
