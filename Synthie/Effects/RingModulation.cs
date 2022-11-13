using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synthie.Effects
{
    public class RingModulation : Effect
    {
        private double phase;
        private double freq;
        private double amp;

        public RingModulation(double samplerate)
        {
            phase = 0;
            freq = MainForm.rm_dlg.Frequency;
            amp = MainForm.rm_dlg.Amplitude;
            sample_rate = samplerate;
        }

        public override double Apply(double data)
        {
            double multiplier = 1;
            if(data < 0)
            {
                multiplier = -1;
                data = Math.Abs(data);
            }

            double max_val = Math.Abs(amp * Math.Sin(phase * 2 * Math.PI));

            data = Math.Min(max_val, data) * multiplier;
            return data;
        }

        public override void UpdateTime()
        {
            phase += freq / sample_rate;
        }
    }
}
