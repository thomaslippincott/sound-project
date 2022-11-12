using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie
{
    internal class ADSR : Envelope
    {
        private double _attack;
        private double _decay;
        private double _sustain;
        private double _release;

        public double Attack { get => _attack; set => _attack = value; }
        public double Decay { get => _decay; set => _decay = value; }
        public double Sustain { get => _sustain; set => _sustain = value; }
        public double Release { get => _release; set => _release = value; }

        public ADSR()
        {
            _attack = 0.05;
            _decay = 0.05;
            _sustain = 1.0;
            _release = 0.05;
        }

        public ADSR(double attack, double decay, double sustain, double release)
        {
            _attack = attack;
            _decay = decay;
            _sustain = sustain;
            _release = release;
        }

        public override bool Generate()
        {
            double ramp;
            frame = source.Frame();

            if (time <= _attack)
                ramp = time / _attack;
            else if (time > _attack && time <= (_attack + _decay))
                ramp = (((_sustain - 1.0f) / _decay) * (time - _attack)) + 1.0f;
            else if (time >= duration - _release)
                ramp = ((duration - time) * _sustain) / _release;
            else
                ramp = _sustain;

            for (int i = 0; i < frame.Length; i++)
                frame[i] *= (float)ramp;

            time += samplePeriod;
            return time < duration;
        }

        public override void Start()
        {
            time = 0;
        }
    }
}
