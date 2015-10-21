using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kaliber3D.Modules
{
    public partial class ConfigPane : UserControl
    {
        public ConfigPane()
        {
            InitializeComponent();
        }
        public string CBOuterValue
        {
            get
            {
                return CPOuter.Text;
            }
            set
            {
                CPOuter.Text = value;
            }
        }
        public string CBInnerValue
        {
            get
            {
                return CPInner.Text;
            }
            set
            {
                CPInner.Text = value;
            }
        } 
    }
}
