using AudioProcess;
using System;
using System.Windows.Forms;

namespace Synthie
{
    public class SineWave : AudioNode
    {
        private double freq;
        private double amp;
        private double phase;

        public double Frequency { get => freq; set => freq = value; }
        public double Amplitude { get => amp; set => amp = value; }

        public SineWave()
        {
            phase = 0;
            amp = 0.1;
            freq = 440;
        }

        public SineWave(double Phase, double Amp, double Freq)
        {
            phase = Phase;
            amp = Amp;
            freq = Freq;
        }

        public SineWave(double _amp, double _freq)
        {
            phase = 0;
            amp = _amp;
            freq = _freq;
        }

        public override void Start()
        {
            phase = 0;
        }

        public override bool Generate()
        {
            frame[0] = (float)(amp * Math.Sin(phase * 2 * Math.PI));
            frame[1] = frame[0];

            phase += freq * samplePeriod;

            return true;
        }
    }
}
