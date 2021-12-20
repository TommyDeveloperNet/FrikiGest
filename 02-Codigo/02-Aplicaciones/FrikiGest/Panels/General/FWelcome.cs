using Fathers.Forms;
using ContentGest.Class.General;
using System;

namespace FrikiGest.Panels.General
{
    public partial class FWelcome : IniForm
    {
        public FWelcome()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------------------
        #region Eventos del formulario
        private void FWelcome_Load(object sender, EventArgs e)
        {
            circularProgressBar1.Value = 0;
            this.Opacity = 0;

            lTitle.Text = GetInfoApp();
            lUserName.Text = Environment.UserName.ToUpperInvariant();
            this.Text = "¡Bienvenido! " + lTitle.Text;

            timer1.Start();
        }

        private void FWelcome_Shown(object sender, EventArgs e)
        {
            this.Icon = new System.Drawing.Icon(@"Resources/Ico_collection.ico");
        }
        #endregion
        //--------------------------------------------------------------------


        //--------------------------------------------------------------------
        #region Eventos Timer
        /// <summary>
        /// Temporizador para mostrar de forma gradual el form de bienvenida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            circularProgressBar1.Value += 1;
            circularProgressBar1.Text = circularProgressBar1.Value.ToString();

            if (this.Opacity < 1)
            {
                this.Opacity += 0.05;
            }

            if (circularProgressBar1.Value == 100)
            {
                timer1.Stop();
                timer2.Start();
            }
        }

        /// <summary>
        /// Temporizador para ocultar el form de forma gradual y lanzar la ventana principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.01;
            if (this.Opacity == 0)
            {
                timer2.Stop();
                FPrincipal formPrincipal = new FPrincipal();
                this.Hide();
                formPrincipal.Show();
                formPrincipal.FormClosed += this.CloseApp;
            }
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones varios (PROTECTED)
        protected string GetInfoApp()
        {
            return Constant_config_App.InfoApp;
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones varios (PRIVATE)
        /// <summary>
        /// Cierra la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApp(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
