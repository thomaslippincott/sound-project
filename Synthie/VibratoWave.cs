using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie
{
    public class VibratoWave: AudioNode
    {

        private double freq;
        private double amp;
        private double radians1;
        private double radians2;
        private double alpha;
        private double vib_freq;

        public double Frequency { get => freq; set => freq = value; }
        public double Amplitude { get => amp; set => amp = value; }
        public double Alpha { get => alpha; set => alpha = value; }
        public double VibratoFrequency { get => vib_freq; set => vib_freq = value; }

        public VibratoWave()
        {
            radians1 = 0.0;
            radians2 = 0.0;
            amp = 0.1;
            freq = 440.0;
            alpha = 10.0;
            vib_freq = 5.0;
        }

        public VibratoWave(double _amp, double _freq, double _alpha, double _vib_freq)
        {
            radians1 = 0.0;
            radians2 = 0.0;
            amp = _amp;
            freq = _freq;
            alpha = _alpha;
            vib_freq = _vib_freq;
        }

        public override void Start()
        {
            radians1 = 0.0;
            radians2 = 0.0;
        }

        public override bool Generate()
        {
            float sample = (float)(amp * Math.Sin(radians1));

            frame[0] = sample;
            frame[1] = sample;

            // Frequency differential at this time
            double diff = alpha * Math.Sin(radians2);

            // Increment the phases
            radians1 += (2 * Math.PI * (freq + diff)) / sampleRate;
            radians2 += (2 * Math.PI * vib_freq) / sampleRate;

            return true;
        }
    }
}
