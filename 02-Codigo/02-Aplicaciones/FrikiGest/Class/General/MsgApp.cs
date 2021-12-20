using System.Reflection;
using System.Resources;

namespace ContentGest.Class.General
{
    class MsgApp
    {
        //----------------------------------------------------------------------
        #region Propiedades
        #region Confirmacion
        public static string MSG_ConfirmCloseApp { get => GetMSGResources("MSG_ConfirmCloseApp"); }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region ToolTip
        public static string MSGToolTip_CloseApp { get => GetMSGResources("MSGToolTip_CloseApp"); }
        public static string MSGToolTip_Maximize { get => GetMSGResources("MSGToolTip_Maximize"); }
        public static string MSGToolTip_Minimize { get => GetMSGResources("MSGToolTip_Minimize"); }
        public static string MSGToolTip_Restore { get => GetMSGResources("MSGToolTip_Restore"); }
        public static string MSGToolTip_MenuMusic { get => GetMSGResources("MSGToolTip_MenuMusic"); }
        public static string MSGToolTip_MenuCine { get => GetMSGResources("MSGToolTip_MenuCine"); }
        public static string MSGToolTip_MenuLibros { get => GetMSGResources("MSGToolTip_MenuLibros"); }
        public static string MSGToolTip_MenuConfig { get => GetMSGResources("MSGToolTip_MenuConfig"); }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Validación
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Error
        #endregion
        //----------------------------------------------------------------------

        #region Otros
        #endregion
        //----------------------------------------------------------------------
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Procedimientos y funciones varios
        private static string GetMSGResources(string sKey)
        {
            ResourceManager rm = new ResourceManager(Constant_Resources.ResourceMsg, Assembly.GetExecutingAssembly());
            return rm.GetString(sKey);
        }
        #endregion
        //----------------------------------------------------------------------
    }
}
