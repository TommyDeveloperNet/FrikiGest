using System;
using ConnBBDD;

namespace ContentGest.Class.ConnBBDD
{
    /// <summary>
    /// Clase de la que heredan los objetos de conexión a usar en la aplicación
    /// </summary>
    public class ConnBBDD_Base : IDisposable
    {
        //----------------------------------------------------------------------
        #region Estructuras y tipos enumerados
        /// <summary>
        /// Indica si se va a trabajar / cargar con el contenido de una tabla o de una vista
        /// </summary>
        public enum TypeLoadInfo { type_Table = 0, type_View = 1 };
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Propiedades
        public string TypeConnection { get; set; } = "";
        #endregion
        //----------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region variables y constantes (privadas)
        bool disposed = false;
        const string modeConnACCESS = "ACCESS";
        const string modeConnODBC = "ODBC";
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region variables y constantes (protected)
        /// <summary>
        /// Objeto de conexión a la base de datos
        /// </summary>
        protected ConnectionBBDD objConection;

        /// <summary>
        /// Indica si la conexión a BBDD se ha realizado ok (TRUE) o no (FALSE)
        /// </summary>
        protected bool bConnOKtoBBDD = false;
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region constructores y destructores conexion BBDD
        public ConnBBDD_Base()
        {
            //Declaración
            string ConnectionMode = General.Constant_config_App.ConnectionMode;
            string sBBDD = General.Constant_config_App.BBDDAPPCollector;
            ConnectionBBDD.TypeConn iTypeConn;

            //Código
            switch (ConnectionMode)
            {
                case modeConnACCESS:
                    sBBDD = AppDomain.CurrentDomain.BaseDirectory + @"Data\" + sBBDD;
                    iTypeConn = ConnectionBBDD.TypeConn.type_Access;
                    break;
                case modeConnODBC:
                    iTypeConn = ConnectionBBDD.TypeConn.type_ODBC;
                    break;
                default:
                    sBBDD = AppDomain.CurrentDomain.BaseDirectory + @"Data\" + sBBDD;
                    iTypeConn = ConnectionBBDD.TypeConn.type_Access;
                    break;
            }

            objConection = new ConnectionBBDD(iTypeConn, sBBDD);
            bConnOKtoBBDD = objConection.Open();

            TypeConnection = "";
            if (bConnOKtoBBDD)
            {
                switch (objConection.tipoConnectionBD)
                {
                    case ConnectionBBDD.TypeConn.type_Access:
                        TypeConnection = modeConnACCESS;
                        break;
                    case ConnectionBBDD.TypeConn.type_XML:
                        TypeConnection = "XML";
                        break;
                    case ConnectionBBDD.TypeConn.type_ODBC:
                        TypeConnection = modeConnODBC;
                        break;
                }
            }
            else
            {
                TypeConnection = "ERROR! Conexión no establecida con ninguna BBDD";
            }
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
                objConection.Dispose();
            }

            disposed = true;
        }
        #endregion  
        //--------------------------------------------------------------------
    }
}
