using System;
using System.IO;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Data;
using Utils;

namespace ConnBBDD
{
    /// <summary>
    /// Clase que gestiona la conexión a base de datos así como las operaciones de select / insert / udpate y delete
    /// </summary>
    public class ConnectionBBDD : IDisposable
    {
        //--------------------------------------------------------------------
        #region Tipos enumerados
        public enum TypeConn { type_Access = 1, type_XML = 2, type_ODBC = 3 };
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region variables y constantes (privadas)
        bool disposed = false;

        private TypeConn tcConn;
        private string sCadConn;

        private List<string> lTables;

        private bool ConnectionToBBDDOK = false;
        private string sErr = "";
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Propiedades
        /// <summary>
        /// Captura con el error producido (código + descripción)
        /// </summary>
        public string connERROR { get => sErr.Trim(); }

        /// <summary>
        /// Objeto con la conexión a base de datos
        /// </summary>
        public object connectionBD { get; set; }

        /// <summary>
        /// Tipo de conexión a base de datos
        /// </summary>
        public TypeConn tipoConnectionBD { get => tcConn; }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region constructores y destructores conexion BBDD
        /// <summary>
        /// Inicializa el objeto de conexión a BBDD (Para bases de datos ACCESS)
        /// </summary>
        /// <param name="tcConn">Tipo de conexión</param>
        /// <param name="sCadConn">Cadena de conexión / PATH al archivo físico</param>
        public ConnectionBBDD(TypeConn tcConn, string sCadConn)
        {
            this.tcConn = tcConn;
            this.sCadConn = sCadConn.Trim();
            this.lTables = new List<string>();
        }

        /// <summary>
        /// Inicializa el objeto de conexión a BBDD (para bases de datos XML)
        /// </summary>
        /// <param name="tcConn"></param>
        /// <param name="lTables"></param>
        /// <param name="sPathXML"></param>
        public ConnectionBBDD(TypeConn tcConn, List<string> lTables, string sPathXML)
        {
            this.tcConn = tcConn;
            this.sCadConn = sPathXML.Trim();
            this.lTables = lTables;
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
                if (ConnectionToBBDDOK)
                {
                    //Hay una conexión a BBDD abierta. Cerramos
                    switch (tcConn)
                    {
                        case TypeConn.type_Access:
                            //ACCESS
                            connectionBD = null;
                            GC.Collect();
                            break;
                        //--------------------------------------------------------------------
                        case TypeConn.type_XML:
                            GC.Collect();
                            //XML
                            break;
                        //--------------------------------------------------------------------
                        case TypeConn.type_ODBC:
                            //ACCESS
                            connectionBD = null;
                            GC.Collect();
                            break;
                            //--------------------------------------------------------------------
                    }
                }
            }

            disposed = true;
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procecimientos y funciones varios (PUBLIC)
        /// <summary>
        /// Crea / Abre la conexión a base de datos
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            ConnectionToBBDDOK = false; //Por defecto
            sErr = "";

            switch (tcConn)
            {
                case TypeConn.type_Access:
                    //ACCESS
                    ConnectionToBBDDOK = setConnection_OleDbConnection();
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_XML:
                    //XML
                    ConnectionToBBDDOK = setConnection_XMLConnection();
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_ODBC:
                    //XML
                    ConnectionToBBDDOK = setConnection_ODBCConnection();
                    break;
                //--------------------------------------------------------------------
                default:
                    //ERROR
                    sErr = "No se ha definido el tipo de conexión a base de datos";
                    break;
                    //--------------------------------------------------------------------
            }

            return ConnectionToBBDDOK;
        }

        /// <summary>
        /// Devuelve el nombre de una tabla o vista asociada a un modelo
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public string GetTable_ViewName(Type tModelo)
        {
            //Declaración
            string sResult = "";

            //Código
            try
            {
                sResult = tModelo.Name.Trim();
            }
            catch (Exception ex)
            {
                Log.SetLog(ex, false);
                sResult = "";
            }

            //Resultado
            return sResult.Trim();
        }

        /// <summary>
        /// Obtiene todos los elementos (registros) de una tabla o vista de la BBDD
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="tModelo">Modelo de datos</param>
        /// <param name="sqlCommand">OPCIONAL - Sentencia SQL a ejecutar</param>
        /// <returns></returns>
        public List<T> Search<T>(Type tModelo, string sqlCommand = "")
        {
            //Declaración
            string sTableViewName = GetTable_ViewName(tModelo);
            List<T> lResult;

            //Código
            switch (tcConn)
            {
                case TypeConn.type_Access:
                    //ACCESS
                    using (ConnectionOleDb objConnectionOLEDB = new ConnectionOleDb((OleDbConnection)connectionBD))
                    {
                        lResult = objConnectionOLEDB.Select<T>(sTableViewName, sqlCommand);
                    }
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_XML:
                    //XML
                    using (ConnectionXML objConnectionXML = new ConnectionXML(lTables, sCadConn))
                    {
                        lResult = objConnectionXML.Select<T>(sTableViewName);
                    }
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_ODBC:
                    //ODBC
                    using (ConnectionODBC objConnectionOLEDBC = new ConnectionODBC((OdbcConnection)connectionBD))
                    {
                        lResult = objConnectionOLEDBC.Select<T>(sTableViewName, sqlCommand);
                    }
                    break;
                //--------------------------------------------------------------------
                default:
                    //ERROR
                    lResult = new List<T>();
                    break;
                    //--------------------------------------------------------------------
            }

            //Resultado
            return lResult;
        }

        /// <summary>
        /// Inserta un elemento (registro) en una tabla de la BBDD
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="tModelo">Modelo de datos</param>
        /// <param name="objDatosAdd">Información a insertar</param>
        /// <returns></returns>
        public bool Insert<T>(Type tModelo, T objDatosAdd)
        {
            //Declaración
            bool bResult = false;
            string sTableViewName = GetTable_ViewName(tModelo);

            //Código
            switch (tcConn)
            {
                case TypeConn.type_Access:
                    //ACCESS
                    using (ConnectionOleDb objConnectionOLEDB = new ConnectionOleDb((OleDbConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDB.Insert<T>(sTableViewName, objDatosAdd);
                    }
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_ODBC:
                    //ODBC
                    using (ConnectionODBC objConnectionOLEDBC = new ConnectionODBC((OdbcConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDBC.Insert<T>(sTableViewName, objDatosAdd);
                    }
                    break;
                //--------------------------------------------------------------------
                default:
                    //ERROR
                    bResult = false;
                    break;
                    //--------------------------------------------------------------------
            }

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Actualiza un elemento (registro) en una tabla de la BBDD. No se trata de una actualización masiva sino de una actualización mediante campo ID
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="tModelo">Modelo de datos</param>
        /// <param name="objDatosUpdate">Información a editar</param>
        /// <param name="IDUpdate">Valor ID del registro a actualizar</param>
        /// <param name="sFieldID">Nombre del campo ID (Por defecto ID) de la tabla a actualizar</param>
        /// <returns></returns>
        public bool Update<T>(Type tModelo, T objDatosUpdate, int IDUpdate, string sFieldID = "ID")
        {
            //Declaración
            bool bResult = false;
            string sTableViewName = GetTable_ViewName(tModelo);

            //Código
            switch (tcConn)
            {
                case TypeConn.type_Access:
                    //ACCESS
                    using (ConnectionOleDb objConnectionOLEDB = new ConnectionOleDb((OleDbConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDB.Update<T>(sTableViewName, objDatosUpdate, IDUpdate, sFieldID);
                    }
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_ODBC:
                    //ODBC
                    using (ConnectionODBC objConnectionOLEDBC = new ConnectionODBC((OdbcConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDBC.Update<T>(sTableViewName, objDatosUpdate, IDUpdate, sFieldID);
                    }
                    break;
                //--------------------------------------------------------------------
                default:
                    //ERROR
                    bResult = false;
                    break;
                    //--------------------------------------------------------------------
            }

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Elimina un elemento (registro) en una tabla de la BBDD. No se trata de un borrado masivo sino de un borrado mediante campo ID
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="tModelo">Modelo de datos</param>
        /// <param name="IDDelete">Valor ID del registro a eliminar</param>
        /// <param name="sFieldID">Nombre del campo ID (Por defecto ID) de la tabla a actualizar (eliminar registro)</param>
        /// <returns></returns>
        public bool Delete<T>(Type tModelo, int IDDelete, string sFieldID = "ID")
        {
            //Declaración
            bool bResult = false;
            string sTableViewName = GetTable_ViewName(tModelo);

            //Código
            switch (tcConn)
            {
                case TypeConn.type_Access:
                    //ACCESS
                    using (ConnectionOleDb objConnectionOLEDB = new ConnectionOleDb((OleDbConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDB.Delete<T>(sTableViewName, IDDelete, sFieldID);
                    }
                    break;
                //--------------------------------------------------------------------
                case TypeConn.type_ODBC:
                    //ODBC
                    using (ConnectionODBC objConnectionOLEDBC = new ConnectionODBC((OdbcConnection)connectionBD))
                    {
                        bResult = objConnectionOLEDBC.Delete<T>(sTableViewName, IDDelete, sFieldID);
                    }
                    break;
                //--------------------------------------------------------------------                
                default:
                    //ERROR
                    bResult = false;
                    break;
                    //--------------------------------------------------------------------
            }

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Pasa el conteniddo de una lista de objetos de tipo T (Tipo entidad) a un DataSet
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="list">Listade objetos de tipo T, contenedora de datos</param>
        /// <returns></returns>
        public DataSet ListToDataSet<T>(List<T> list)
        {
            //Declaración
            Type elementType = typeof(T);
            DataSet dsResult = new DataSet();
            DataTable dtTable = new DataTable();

            //Código
            dtTable.TableName = GetTable_ViewName(elementType);

            dsResult.Tables.Add(dtTable);

            //Añadimos columnas a la tabla mediante un bucle ForEach
            foreach (var propInfo in elementType.GetProperties())
            {
                Type Coltype = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                dtTable.Columns.Add(propInfo.Name, Coltype);
            }

            //Añadimos valores (registro) a la tabla mediante un bucle ForEach
            foreach (T item in list)
            {
                DataRow drRow = dtTable.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    drRow[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                dtTable.Rows.Add(drRow);
            }

            //Resultado
            return dsResult;
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones varios (PRIVATE)
        /// <summary>
        /// Crea / Establece la conexión a base de datos - Access DataBase
        /// </summary>
        /// <returns></returns>
        private bool setConnection_OleDbConnection()
        {
            //Declaración
            bool bResult = false;
            string sPathFileAccess = sCadConn.Trim();

            string sMDB = ".mdb";
            string sACCDB = ".accdb";

            string sCadConnACCDB = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = '{0}'; Persist Security Info = False;";
            string sCadConnMDB = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};"; ;

            //Código
            //1.- Comprobamos si el fichero existe físicamente
            if (!File.Exists(sPathFileAccess))
            {
                sErr = string.Format("No existe o no se ha encontrado el fichero de base de datos ACCESS - {0}", sPathFileAccess);
                return bResult;
            }

            //2.- Determinamos tipo de access / cadena de conexión a implementar
            string dbConnString = "";
            if (Path.GetExtension(sPathFileAccess.ToLower()) == sACCDB)
            {
                dbConnString = sCadConnACCDB;
            }
            else if (Path.GetExtension(sPathFileAccess.ToLower()) == sMDB)
            {
                dbConnString = sCadConnMDB;
            }
            else
            {
                sErr = string.Format("No se ha reconocido la extensión para el fichero de datos - {0}", sPathFileAccess);
                return bResult;
            }

            //3.- Conectamos
            dbConnString = string.Format(dbConnString, sPathFileAccess);

            try
            {
                connectionBD = new OleDbConnection(dbConnString);
                bResult = true;
            }
            catch (Exception ex)
            {
                bResult = false;
                Log.SetLog(ex, true);
                sErr = string.Format("No se ha podido establecer la conexión con la BBDD ({0} - Error: {1})", sCadConn, ex.Message);
                return bResult;
            }

            //Resultado
            return bResult;
        }

        private bool setConnection_ODBCConnection()
        {
            //Declaración
            bool bResult = false;

            //Código
            //1.- Conectamos
            string dbConnString = "DSN={0}";
            dbConnString = string.Format(dbConnString, sCadConn);

            try
            {
                connectionBD = new OdbcConnection(dbConnString);
                bResult = true;
            }
            catch (Exception ex)
            {
                Log.SetLog(ex, true);
                sErr = string.Format("No se ha podido establecer la conexión con la BBDD ({0} - Error: {1})", sCadConn, ex.Message);
                return bResult;
            }

            //Resultado
            return bResult;
        }

        /// <summary>
        /// Crea / Establece la conexión a base de datos - XML DataBase
        /// </summary>
        /// <returns></returns>
        private bool setConnection_XMLConnection()
        {
            //Declaración
            bool bResult = false;

            //Código
            //1.- Validamos datos (tablas y ubicación física de los ficheors xml)
            if ((lTables == null) || (lTables.Count == 0))
            {
                sErr = "No se ha indicado un origen de datos XML válido";
                return bResult;
            }

            if (!Directory.Exists(sCadConn))
            {
                sErr = string.Format("No existe o no se ha encontrado el directorio {0}.", sCadConn);
                return bResult;
            }

            string sPathXML = "{0}/{1}{2}";
            string sFileXML;
            const string sXMLExtension = ".xml";

            foreach (string sXML in lTables)
            {
                if (!File.Exists(string.Format(sPathXML, sCadConn, sXML, sXMLExtension)))
                {
                    sErr = string.Format("No se ha encontrado el archivo {0} dentro del directorio {1}", sXML, sCadConn);
                    return bResult;
                }
                else
                {
                    sFileXML = (sCadConn + sXML + sXMLExtension).ToString().ToLower();
                    if (Path.GetExtension(sFileXML) != sXMLExtension)
                    {
                        sErr = string.Format("El archivo {0} no es un fichero .xml válido", sXML);
                        return bResult;
                    }
                }
            }

            //Resultado
            bResult = true;
            return bResult;
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
