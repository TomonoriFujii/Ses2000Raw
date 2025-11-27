using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ses2000Raw
{
    
    public partial class UsageAddContactForm : Form
    {

        //AnalysisForm AnalysisForm;
        public bool DoNotShowAgain => checkBox1.Checked;
        public UsageAddContactForm()
        {
            InitializeComponent();
            if(checkBox1.Checked)
            {


            }

        }
        


    }
}
