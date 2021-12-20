using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Globalization;
using Fathers.Forms;
using ContentGest.Class.General;
using ContentGest.Class.ConnBBDD;

namespace FrikiGest.Panels.General
{
    public partial class FPrincipal : PrincipalForm
    {
        public FPrincipal()
        {
            InitializeComponent();
            InitializeComponentFPrincipal();
        }

        //--------------------------------------------------------------------
        #region Funcionalidades del formulario - Diseño visual
        private void InitializeComponentFPrincipal()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        //--------------------------------------------------------------------
        #region RESIZE
        private int tolerance = 12;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;

        /// <summary>
        /// Resize - Método para redimensionar / cambiar tamaño del formulario en tiempo de ejecución
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// Dibujar rectánculo / Excluir esquina del panel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);
            region.Exclude(sizeGripRectangle);
            this.pContenedor.Region = region;
            this.Invalidate();
        }

        /// <summary>
        /// Color y gripo de rectánculo inferior
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(244, 244, 244));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);
            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }

        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Botones
        int lx, ly;
        int sw, sh;

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;
            btnMaximize.Visible = false;
            btnRestore.Visible = true;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            btnMaximize.Visible = true;
            btnRestore.Visible = false;
            this.Size = new Size(sw, sh);
            this.Location = new Point(lx, ly);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //--------------------------------------------------------------------
        #endregion
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Eventos del formulario
        private void FPrincipal_Load(object sender, EventArgs e)
        {
            PrincipalForm_Load(sender, e);
        }

        private void FPrincipal_Shown(object sender, EventArgs e)
        {
            PrincipalForm_Show(sender, e);
        }

        private void FPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            PrincipalForm_FormClosing(sender, e);
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Timer
        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo(CultureAPP);
            lInfoSystem.Text = string.Format("{0} - Fecha y hora: {1}", NameAPP, localDate.ToString(culture));
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Botones del formulario
        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        //--------------------------------------------------------------------

        //--------------------------------------------------------------------
        #region Procedimientos y funciones varios (PROTECTED)

        /// <summary>
        /// Inicializa el formulario, estado de los controles, clases y demás para su funcionamiento
        /// </summary>
        protected override void InitializeForm()
        {
            //1.- Inicializamos labels y controles menores
            CultureAPP = Constant_config_App.CultureES;
            NameAPP = Constant_config_App.NameApp;
            IconAPP = @"Resources/Ico_collection.ico";
            LEstado = lEstado;
            lTitle.Text = GetInfoApp();
            lInfoSystem.Text = "";
            lUserName.Text = Environment.UserName.ToUpperInvariant();

            //2.- Estado del formulario (pantalla completa / ancho / alto)
            using (ConnBBDD_Config objConfig = new ConnBBDD_Config())
            {
                if (objConfig.Form_Maximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                    this.MaximizeBox = false;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Width = objConfig.Form_Widh;
                    this.Height = objConfig.Form_Height;
                }
            }

            //3.- Tooltip
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(btnClose, MsgApp.MSGToolTip_CloseApp);
            tooltip.SetToolTip(btnCloseApp, MsgApp.MSGToolTip_CloseApp);
            tooltip.SetToolTip(btnMaximize, MsgApp.MSGToolTip_Maximize);
            tooltip.SetToolTip(btnMinimize, MsgApp.MSGToolTip_Minimize);
            tooltip.SetToolTip(btnRestore, MsgApp.MSGToolTip_Restore);
            tooltip.SetToolTip(btnMusic, MsgApp.MSGToolTip_MenuMusic);
            tooltip.SetToolTip(btnCine, MsgApp.MSGToolTip_MenuCine);
            tooltip.SetToolTip(btnBooks, MsgApp.MSGToolTip_MenuLibros);
            tooltip.SetToolTip(btnConfig, MsgApp.MSGToolTip_MenuConfig);
        }

        protected override string GetInfoApp()
        {
            return Constant_config_App.InfoApp;
        }

        protected override bool ConfirmCloseApp()
        {
            if (MessageBox.Show(MsgApp.MSG_ConfirmCloseApp, Constant_config_App.NameApp, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void CloseForm(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["ContenedorMusic"] == null)
            {
                btnMusic.BackColor = Color.FromArgb(37, 54, 75);
            }

            if (Application.OpenForms["ContenedorConfig"] == null)
            {
                btnConfig.BackColor = Color.FromArgb(37, 54, 75);
            }
        }
        #endregion
        //--------------------------------------------------------------------
    }
}
