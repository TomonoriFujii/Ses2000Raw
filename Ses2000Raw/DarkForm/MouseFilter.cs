using System;
using System.Drawing;
using System.Windows.Forms;

namespace DarkForm
{
    /// <summary>
    /// ウィンドウ枠を透明にするクラス
    /// </summary>
    class MouseFilter : NativeWindow {

        /// <summary>
        /// 
        /// </summary>
        private Form form;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="form"></param>
        /// <param name="child"></param>
        public MouseFilter(Form form, Control child) {
            this.form = form;
            this.AssignHandle(child.Handle);
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            if (m.Msg == NativeMethods.WM_NCHITTEST) {
                var pos = new Point(m.LParam.ToInt32());
                if (pos.X < this.form.Left + NativeMethods.EDGE ||
                    pos.Y < this.form.Top + NativeMethods.EDGE ||
                    pos.X > this.form.Right - NativeMethods.EDGE ||
                    pos.Y > this.form.Bottom - NativeMethods.EDGE) {
                    m.Result = new IntPtr(-1);
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}