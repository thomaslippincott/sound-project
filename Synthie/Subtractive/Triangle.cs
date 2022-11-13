using AudioProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Subtractive
{
    public class Triangle : Subtractive
    {
        public Triangle()
        {
            sample = new SoundChunk("../../res/SubtractiveSamples/triangle.wav");
            envelope = new AR(0.005, 0.005);
        }
    }
}
