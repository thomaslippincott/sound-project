using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synthie.Effects
{
    public partial class RingModInputs : Form
    {
        public RingModInputs()
        {
            InitializeComponent();
        }

        public double Frequency
        {
            get { return Convert.ToDouble(txtfreq.Text); }
            set { txtfreq.Text = value.ToString(); }
        }

        public double Amplitude
        {
            get { return Convert.ToDouble(txtamp.Text); }
            set { txtamp.Text = value.ToString(); }
        }
    }
}
