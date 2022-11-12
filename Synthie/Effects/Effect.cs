using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Effects
{
    public abstract class Effect
    {
        public float time = 0;
        public float timeperframe = 0;
        public double wet_percentage = 0.5;
        public double sample_rate = 0;
        public int channels = 0;

        public abstract double Apply(double data);

        public abstract void UpdateTime();
    }
}
