using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synthie
{
    internal class Tremolo : Envelope
    {
        private double depth;
        private double freq;

        public double Depth { get { return depth; } set { depth = value; } }
        public double Frequency { get { return freq; } set { freq = value; } }

        public Tremolo()
        {
            depth = 0.1;
            freq = 4.0;
        }

        public Tremolo(double _depth, double _freq)
        {
            depth = _depth;
            freq = _freq;
        }

        public override bool Generate()
        {
            frame = source.Frame();

            float amplitude = (float)(1.0f + depth * (float)Math.Sin(freq * 2 * Math.PI * time));

            for (int i = 0; i < frame.Length; i++)
                frame[i] *= (float)amplitude;

            time += samplePeriod;
            return time < duration;
        }

        public override void Start()
        {
            time = 0;
        }
    }
}
