using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Utils;

namespace ConnBBDD
{
    /// <summary>
    /// Clase que gestiona la conexión a base de datos así como las operaciones de select / insert / udpate y delete para una base de datos de tipo XML
    /// </summary>
    public class ConnectionXML : IDisposable
    {
        //--------------------------------------------------------------------
        #region variables y constantes (privadas)
        bool disposed = false;
        List<string> lTables;
        string sPathXML;
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region constructores y destructores conexion BBDD
        public ConnectionXML(List<string> lTables, string sPathXML)
        {
            this.lTables = lTables;
            this.sPathXML = sPathXML;
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
        /// <returns></returns>
        public List<T> Select<T>(string sTable_View_Name)
        {
            //Declaración
            List<T> lResult;

            //Código
            //1.- Validamos
            if (!lTables.Contains(sTable_View_Name))
            {
                lResult = new List<T>();
            }
            else
            {
                string sXMLRootAttribute = "{0}Items";
                sXMLRootAttribute = string.Format(sXMLRootAttribute, sTable_View_Name);
                XmlSerializer objSerializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(sXMLRootAttribute));

                string sPathXMLSelect = @"{0}{1}.xml";
                sPathXMLSelect = string.Format(sPathXMLSelect, sPathXML, sTable_View_Name);

                try
                {
                    using (StreamReader objReader = new StreamReader(sPathXMLSelect))
                    {
                        lResult = (List<T>)objSerializer.Deserialize(objReader);
                        objReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.SetLog(ex, false);
                    lResult = new List<T>();
                }
            }

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
            return false;
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
            return false;
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
            return false;
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
