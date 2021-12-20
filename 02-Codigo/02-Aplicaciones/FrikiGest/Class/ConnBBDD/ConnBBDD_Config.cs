using Models.Tables;
using System.Collections.Generic;
using System.Linq;

namespace ContentGest.Class.ConnBBDD
{
    /// <summary>
    /// Clase interfaz con la tabla TBL_CONFIG
    /// </summary>
    public class ConnBBDD_Config: ConnBBDD_Base
    {
        //--------------------------------------------------------------------
        #region Variables
        private int _Form_Width;
        private int _Form_Height;
        private bool _Form_Maximized;
        private List<TBL_CONFIG> _listConfig;
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region propiedades
        public int Form_Widh { get => _Form_Width; }
        public int Form_Height { get => _Form_Height; }
        public bool Form_Maximized { get => _Form_Maximized; }
        public List<TBL_CONFIG> ListConfig { get => _listConfig; }
        #endregion  
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Constructores y destructores de la clase"
        public ConnBBDD_Config()
        {
            //Valores por defecto - 1) Configuración visual del formulario
            const int IDConfig_visual_form = 1;
            _Form_Maximized = false;
            _Form_Width = 1170;
            _Form_Height = 675;

            if (bConnOKtoBBDD)
            {
                _listConfig = objConection.Search<TBL_CONFIG>(typeof(TBL_CONFIG)).OrderByDescending(i => i.ID).ToList();

                TBL_CONFIG objConfig = _listConfig.Where(i => i.ID == IDConfig_visual_form).FirstOrDefault();

                if (objConfig != null)
                {
                    _Form_Maximized = objConfig.PANTALLA_COMPLETA;
                    _Form_Width = objConfig.WIDTH;
                    _Form_Height = objConfig.HEIGHT;
                }
            }
            else
            {
                _listConfig = new List<TBL_CONFIG>();
            }
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones varios
        /// <summary>
        /// Actualiza un item de la configuración de la aplicación
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="iHeight"></param>
        /// <param name="bFullScreen"></param>
        public bool Update_Config(int ID, int iWidth, int iHeight, bool bFullScreen)
        {
            //Declaración
            bool bResult = false;

            //Código
            TBL_CONFIG objConfigData = ListConfig.Where(i => i.ID == ID).SingleOrDefault();

            if (objConfigData != null)
            {
                TBL_CONFIG objConfigUpd = TBL_CONFIG.Create(objConfigData.DEFINICION, iHeight, iWidth, bFullScreen, ID);
                if (objConection.Update<TBL_CONFIG>(typeof(TBL_CONFIG), objConfigUpd, objConfigUpd.ID))
                {
                    bResult = true;
                }
            }

            //Resultado
            return bResult;
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
