using AudioProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Subtractive
{
    public class Sawtooth : Subtractive
    {

        public Sawtooth()
        {
            sample = new SoundChunk("../../res/SubtractiveSamples/sawtooth.wav");
            envelope = new AR(0.005, 0.005);
        }
    }
}
