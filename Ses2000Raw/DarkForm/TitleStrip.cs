using System;
using System.Windows.Forms;
using static DarkForm.NativeMethods;

namespace DarkForm
{
    /// <summary>
    /// タイトルバー
    /// </summary>
    public class TitleStrip : ToolStrip {

        public event MouseEventHandler TitleBarMouseDown;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TitleStrip() {
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            // クリックでメニューを開く
            if (m.Msg == WM_MOUSEACTIVATE) {
                m.Result = new IntPtr(MA_ACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="mea"></param>
        protected override void OnMouseDown(MouseEventArgs mea) {
            base.OnMouseDown(mea);
            this.TitleBarMouseDown?.Invoke(this, mea);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}