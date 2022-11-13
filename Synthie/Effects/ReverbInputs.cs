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
    public partial class ReverbInputs : Form
    {
        public ReverbInputs()
        {
            InitializeComponent();
        }

        public double Delay
        {
            get { return Convert.ToDouble(txtDelay.Text); }
            set { txtDelay.Text = value.ToString(); }
        }

        public int Taps
        {
            get { return Convert.ToInt32(txtTaps.Text); }
            set { txtTaps.Text = value.ToString(); }
        }

        public double WetPercentage
        {
            get { return Convert.ToDouble(txtwetper.Text); }
            set { txtwetper.Text = value.ToString(); }
        }

        public bool Piano
        {
            get { return cbPiano.Checked; }
        }

        public bool Drums
        {
            get { return cbdrums.Checked; }
        }

        public bool Organ
        {
            get { return cborgan.Checked; }
        }

        public bool Additive
        {
            get { return cbadditive.Checked; }
        }
    }
}
