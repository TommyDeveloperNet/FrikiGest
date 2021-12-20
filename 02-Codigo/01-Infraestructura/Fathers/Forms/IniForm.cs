using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Drawing;

namespace Fathers.Forms
{
    public partial class IniForm : Form
    {
        public IniForm()
        {
            InitializeCulture();
        }

        //----------------------------------------------------------------------
        #region Propiedades
        public bool DebugModeApp { get; set; }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Procedimientos y funciones
        protected void OnLoad()
        {
            DebugModeApp = false;
#if DEBUG
            DebugModeApp = true;
#endif
        }

        /// <summary>
        /// Inicializa / establece el idioma de la aplicación
        /// </summary>
        protected void InitializeCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        }
        #endregion
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        #region Procedimientos y funciones - PROTECTED
        protected void OpenForm<MyForm>(ref Panel panel, ref Button button, bool dock = true) where MyForm : IniForm, new()
        {
            IniForm form = panel.Controls.OfType<MyForm>().FirstOrDefault();
            if (form == null)
            {
                form = new MyForm();
                form.TopLevel = false;
                if (dock)
                {
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.Dock = DockStyle.Fill;
                }
                else
                {
                    form.Width = panel.Width - 50;
                    form.Height = panel.Height - 50;
                    form.StartPosition = FormStartPosition.CenterParent;
                }
                panel.Controls.Add(form);
                panel.Tag = form;
                form.Show();
                form.BringToFront();
                form.FormClosed += new FormClosedEventHandler(CloseForm);
            }
            else
            {
                form.BringToFront();
            }

            button.BackColor = Color.FromArgb(12, 61, 92);
        }

        protected virtual void CloseForm(object sender, FormClosedEventArgs e)
        {

        }
        #endregion
        //----------------------------------------------------------------------
    }
}
