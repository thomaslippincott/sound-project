/*
Done	55	Tutorial completed
Done	20	Duration Fix
Done	25	Attack/Release
----   ---
Total  100
 */

using System;
using System.Windows.Forms;

namespace Synthie
{
    public partial class MainForm : Form
    {
        Synthesizer synth = new Synthesizer();

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Helper function to handle file output and/or automatic playback when gneration is done
        /// </summary>
        private void OnPostGeneration()
        {
            if (fileOutputItem.Checked)
            {
                if (saveFileDlg.ShowDialog() == DialogResult.OK)
                {
                    synth.Save(saveFileDlg.FileName);
                }
                saveFileDlg.Dispose();
            }
            if (audioOutputItem.Checked)
            {
                synth.Play();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            synth.OnPaint(e.Graphics);
        }
        #region Menu Handlers
        private void audioOutputItem_Click(object sender, EventArgs e)
        {
            audioOutputItem.Checked = !audioOutputItem.Checked;
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileOutputItem_Click(object sender, EventArgs e)
        {
            fileOutputItem.Checked = !fileOutputItem.Checked;
        }

        private void hz1000Item_Click(object sender, EventArgs e)
        {
            synth.OnGenerate1000hztone();
            OnPostGeneration();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synth.Play();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synth.Stop();
        }
        #endregion

        private void songToolStripMenuItem_Click(object sender, EventArgs e)
        {
            synth.Generate();
            OnPostGeneration();
        }

        private void openScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                synth.OpenScore(openFileDlg.FileName);
            }
        }
    }
}
