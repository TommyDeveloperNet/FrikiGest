using System;
using System.Windows.Forms;

namespace Fathers.Forms
{
    public partial class PrincipalForm: IniForm
    {
        //----------------------------------------------------------------------
        #region Propiedades a establecer en el hijo del form
        public string CultureAPP { get; set; }
        public string NameAPP { get; set; }
        public string IconAPP { get; set; }

        public Label LEstado { get; set; }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region PROTECTED - Eventos del formulario a invocar en los hijos
        protected void PrincipalForm_Load(object sender, System.EventArgs e)
        {
            OnLoad();

            this.Text = GetInfoApp();

            InitializeForm();

            InitializeToolTip();

            LEstado.Text = "";
            if (DebugModeApp)
            {
                LEstado.Text = "DEBUG MODE!";
            }
        }

        protected void PrincipalForm_Show(object sender, EventArgs e)
        {
            if ((((IniForm)sender).Visible == true) && (!string.IsNullOrWhiteSpace(IconAPP)))
            {
                this.Icon = new System.Drawing.Icon(IconAPP);
            }
        }

        protected void PrincipalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseAppWithoutConfirm == false)
            {
                if (!ConfirmCloseApp())
                {
                    e.Cancel = true;
                }
            }

            GC.Collect();
        }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Variables y constantes al form
        /// <summary>
        /// Indica si se puede cerrar la aplicacíon con confirmacion (FALSE) o con confirmación (TRUE)
        /// </summary>
        protected bool CloseAppWithoutConfirm = false;
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Procedimientos y funciones varios (PROTECTED)
        /// <summary>
        /// Devuelve el identificador único del control (que se corresponde con el name) - (SOBREESCRIBIR EN EL HIJO)
        /// </summary>
        /// <returns></returns>
        protected virtual string GetInfoApp()
        {
            return "";
        }

        /// <summary>
        /// Inicializa el formulario, estado de los controles, clases y demás para su funcionamiento - (SOBREESCRIBIR EN EL HIJO)
        /// </summary>
        protected virtual void InitializeForm()
        {

        }

        /// <summary>
        /// Inicializa y establece los tooltips en los controles del form - (SOBREESCRIBIR EN EL HIJO)
        /// </summary>
        protected virtual void InitializeToolTip()
        {

        }

        /// <summary>
        /// Indica si se ha cerrar la aplicación o no - (SOBREESCRIBIR EN EL HIJO)
        /// </summary>
        /// <returns></returns>
        protected virtual bool ConfirmCloseApp()
        {
            return true;
        }
        #endregion
        //----------------------------------------------------------------------
    }
}
