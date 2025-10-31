using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DarkForm;
using static DarkForm.NativeMethods;


namespace DarkForm
{
    public partial class BaseForm : Form
    {

        #region For DarkForm
        private FormWindowState m_oldWindowState;
        private int m_iResizeCnt = 3;
        private bool m_bSizing = false;
        private WINDOWPOS m_firstResizePos;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            int width =
                this.lblIcon.Margin.Left +
                this.lblIcon.Width +
                this.lblTitle.Margin.Left +
                this.btnMinForm.Width +
                this.btnMaxForm.Width +
                this.btnCloseForm.Width;
            this.MinimumSize = new Size(width + 18, 31);

            this.lblTitle.Text = this.Text;
        }

        #region Event
        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // DarkForm
            this.subClassChildren(this.Controls);
            this.m_oldWindowState = FormWindowState.Normal;

            this.BackColor = VS2019ColorTable.BACKCOLOR;
            this.ForeColor = VS2019ColorTable.FORECOLOR;
        }
        /// <summary>
        /// Form Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        /// <summary>
        /// Form Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        #endregion


        #region DarkForm Method
        /// <summary>
        /// subClassChildren
        /// </summary>
        /// <param name="ctls"></param>
        private void subClassChildren(Control.ControlCollection ctls)
        {
            foreach (Control ctl in ctls)
            {
                var rc = this.RectangleToClient(this.RectangleToScreen(ctl.DisplayRectangle));
                if (rc.Left < NativeMethods.EDGE || rc.Right > this.ClientSize.Width - NativeMethods.EDGE ||
                    rc.Top < NativeMethods.EDGE || rc.Bottom > this.ClientSize.Height - NativeMethods.EDGE)
                {
                    new MouseFilter(this, ctl);
                }
                this.subClassChildren(ctl.Controls);
            }
        }
        #endregion


        #region TitleBar Event
        /// <summary>
        /// タイトルバーアイコンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblIcon_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 「最小化」ボタンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinForm_Click(object sender, EventArgs e)
        {
            this.m_oldWindowState = this.WindowState;
            this.WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// 「最大化」ボタンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMaxForm_Click(object sender, EventArgs e)
        {
            // 元に戻す場合
            if (this.WindowState == FormWindowState.Minimized ||
                this.WindowState == FormWindowState.Maximized)
            {

                if (this.m_oldWindowState == FormWindowState.Maximized)
                {
                    this.m_oldWindowState = this.WindowState;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.m_oldWindowState = this.WindowState;
                    this.Size = this.RestoreBounds.Size;
                    this.WindowState = FormWindowState.Normal;
                }

                // 最大化の場合
            }
            else
            {
                this.m_oldWindowState = this.WindowState;
                this.WindowState = FormWindowState.Maximized;
            }
        }
        /// <summary>
        /// 「×」ボタンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// TitleBarのマウスダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // メニューバーのボタンの場合は抜ける
                int width = this.btnMinForm.Width + this.btnMaxForm.Width + this.btnCloseForm.Width;
                if (this.titleBar.Width - width <= e.X)
                {
                    return;
                }
                else if (this.titleBar.Margin.Left + this.lblIcon.Margin.Left <= e.X &&
                         e.X <= this.titleBar.Margin.Left + this.lblIcon.Margin.Left + this.lblIcon.Width &&
                         1 < e.Clicks)
                {
                    this.btnCloseForm_Click(null, null);
                    return;
                }

                // クリックの場合
                if (0 < e.Clicks && e.Clicks <= 1)
                {
                    //マウスのキャプチャを解除
                    ReleaseCapture();
                    //タイトルバーでマウスの左ボタンが押されたことにする
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HT_CAPTION, IntPtr.Zero);
                    // ダブルクリックした場合
                }
                else if (1 < e.Clicks)
                {
                    //マウスのキャプチャを解除
                    ReleaseCapture();
                    //タイトルバーでマウスの左ボタンが押されたことにする
                    SendMessage(this.Handle, WM_NCLBUTTONDBLCLK, (IntPtr)HT_CAPTION, IntPtr.Zero);
                }
            }
            else
            {
                // コンテキストメニュー表示
                var id = TrackPopupMenuEx(
                            GetSystemMenu(this.Handle, false),
                            TPM_LEFTALIGN | TPM_RETURNCMD,
                            Cursor.Position.X,
                            Cursor.Position.Y,
                            this.Handle,
                            IntPtr.Zero);

                SendMessage(this.Handle, WM_SYSCOMMAND, id, 0);
            }
        }
        /// <summary>
        /// Form Resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Resize(object sender, EventArgs e)
        {
            int width =
                this.lblIcon.Margin.Left +
                this.lblIcon.Width +
                this.lblTitle.Margin.Left +
                this.btnMinForm.Width +
                this.btnMaxForm.Width +
                this.btnCloseForm.Width;

            this.lblTitle.Size = new Size(Math.Max(0, this.titleBar.Width - width - 1), this.titleBar.Height);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            // 最大化するとメニューがウィンドウの外にはみでないようにする
            if (this.WindowState == FormWindowState.Maximized)
            {
                //this.Padding = new Padding(8, 8, 8, 8);
                this.Padding = new Padding(10, 10, 10, 10);
                this.btnMaxForm.Image = imageList1.Images[0];
                this.btnMaxForm.ToolTipText = "元に戻す";
            }
            else
            {
                // 枠線付けるために1px間を空ける
                this.Padding = new Padding(1, 1, 1, 1);
                this.btnMaxForm.Image = imageList1.Images[1];
                this.btnMaxForm.ToolTipText = "最大化";
            }
        }
        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine(m.ToString());
            //Debug.WriteLine(deBugCnt + ":" + m.ToString());
            //deBugCnt++;

            switch (m.Msg)
            {
                case WM_ENTERSIZEMOVE:
                    this.m_bSizing = this.WindowState == FormWindowState.Maximized ? true : false;
                    this.m_iResizeCnt = 3;
                    base.WndProc(ref m);
                    break;

                case WM_EXITSIZEMOVE:
                    this.m_bSizing = false;
                    base.WndProc(ref m);
                    break;

                // サイズ変更時
                case WM_WINDOWPOSCHANGING:
                    WINDOWPOS wp = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));

                    // ウィンドウを最大化してドラッグで元に戻すと
                    // 3回リサイズ(ちらつく)するため、その対応
                    if (wp.x != 0 && wp.y != 0 && wp.width != 0 && wp.height != 0)
                    {
                        if (this.m_bSizing)
                        {
                            if (this.m_iResizeCnt == 3)
                            {
                                this.m_firstResizePos = wp;
                            }
                            else if (0 < m_iResizeCnt)
                            {
                                wp.x = this.m_firstResizePos.x;
                                wp.y = this.m_firstResizePos.y;
                                wp.width = this.m_firstResizePos.width;
                                wp.height = this.m_firstResizePos.height;
                            }
                            this.m_iResizeCnt--;
                        }
                    }
                    Marshal.StructureToPtr(wp, m.LParam, true);
                    base.WndProc(ref m);
                    break;

                // ここの WindowState は変更前の状態(タイトルバードラッグ時は0xf012)
                case WM_SYSCOMMAND:

                    // タイトルバードラッグ時
                    if (m.WParam.ToInt32() == 0xf012)
                    {
                        ;
                        // 最大化
                    }
                    else if (m.WParam.ToInt32() == SC_MAXIMIZE)
                    {
                        this.btnMaxForm_Click(null, EventArgs.Empty);
                        break;
                        // 元に戻す
                    }
                    else if (m.WParam.ToInt32() == SC_RESTORE)
                    {
                        this.btnMaxForm_Click(null, EventArgs.Empty);
                        break;
                        // 最小化
                    }
                    else if (m.WParam.ToInt32() == SC_MINIMIZE)
                    {
                        this.btnMinForm_Click(null, EventArgs.Empty);
                        break;
                    }
                    base.WndProc(ref m);
                    break;

                case WM_ACTIVATE:
                    MARGINS margins = new MARGINS();
                    margins.topHeight = 1;
                    margins.bottomHeight = 0;
                    margins.leftWidth = 0;
                    margins.rightWidth = 0;

                    DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    m.Result = IntPtr.Zero;
                    base.WndProc(ref m);
                    break;

                // クライアント領域計算
                case WM_NCCALCSIZE:
                    if (m.WParam != IntPtr.Zero)
                    {
                        NCCALCSIZE_PARAMS nc = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
                        // 非クライアント領域を全て削除する
                        nc.rcNewWindow.Top -= SystemInformation.CaptionHeight + SystemInformation.FrameBorderSize.Height + GetPaddingBorder();
                        nc.rcNewWindow.Bottom += SystemInformation.FrameBorderSize.Height + GetPaddingBorder();
                        nc.rcNewWindow.Left -= SystemInformation.FrameBorderSize.Width + GetPaddingBorder();
                        nc.rcNewWindow.Right += SystemInformation.FrameBorderSize.Width + GetPaddingBorder();
                        nc.rcOldWindow = nc.rcNewWindow;

                        Marshal.StructureToPtr(nc, m.LParam, false);
                        m.Result = (IntPtr)WVR_VALIDRECTS;
                        base.WndProc(ref m);

                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;

                // タイトルバーでダブルクリック
                case NativeMethods.WM_NCLBUTTONDBLCLK:
                    this.btnMaxForm_Click(null, EventArgs.Empty);
                    break;

                // マウス操作
                case NativeMethods.WM_NCHITTEST:
                    // ドラッグ
                    //Debug.WriteLine(true + "\n");
                    //int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                    //int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                    System.Drawing.Point pos = new System.Drawing.Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                    System.Drawing.Point pt = this.PointToClient(pos);
                    Size clientSize = this.ClientSize;

                    // 右下
                    if (pt.X >= clientSize.Width - NativeMethods.EDGE && pt.Y >= clientSize.Height - NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTBOTTOMRIGHT);
                        return;
                    }
                    // 左下
                    if (pt.X <= NativeMethods.EDGE && pt.Y >= clientSize.Height - NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTBOTTOMLEFT);
                        return;
                    }
                    // 左上
                    if (pt.X <= NativeMethods.EDGE && pt.Y <= NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTTOPLEFT);
                        return;
                    }
                    // 右上
                    if (pt.X >= clientSize.Width - NativeMethods.EDGE && pt.Y <= NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTTOPRIGHT);
                        return;
                    }
                    // 上
                    if (pt.Y <= NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTTOP);
                        return;
                    }
                    // 下
                    if (pt.Y >= clientSize.Height - NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTBOTTOM);
                        return;
                    }
                    // 左
                    if (pt.X <= NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTLEFT);
                        return;
                    }
                    // 右
                    if (pt.X >= clientSize.Width - NativeMethods.EDGE && clientSize.Height >= NativeMethods.EDGE)
                    {
                        m.Result = (IntPtr)(HTRIGHT);
                        return;
                    }
                    break;

                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }
        /// <summary>
        /// CreateParams
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var par = base.CreateParams;
                return par;
            }
        }
        /// <summary>
        /// OnPaintイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen p = new Pen(Color.FromArgb(255, 1, 122, 204));
            e.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);

        }
        #endregion


    }
}
