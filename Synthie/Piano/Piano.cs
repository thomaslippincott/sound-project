using NAudio.SoundFont;
using Synthie.WaveTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioProcess;
using System.Security.Policy;
using Synthie.Properties;
using System.Resources;

namespace Synthie.Piano
{
    public class Piano : Instrument
    {

        private double duration;
        private double time;
        private SoundChunk sound;
        private string sample;
        private float[] wavetable;
        private int pos;

        public Piano(Envelope env)
        {
            duration = 0.1;
            envelope = env;
            pos = 0;
        }

        public override bool Generate()
        {

            frame[0] = wavetable[pos];
            frame[1] = wavetable[pos];

            pos++;

            if (pos >= wavetable.Length)
                pos = 0;

            envelope.Generate();                       //adjust the gain on base sample
            frame[0] = envelope.Frame(0);      //pull the adjusted sample
            frame[1] = envelope.Frame(1);

            time += samplePeriod; //normal time increment
            return time < duration;
        }

        public override void SetNote(Note note)
        {
            duration = note.Count * 60.0 / bpm;
            sample = "../../res/PianoSamples/" + note.Pitch + ".wav";
        }

        public override void Start()
        {
            sound = new SoundChunk(sample);
            time = 0;
            pos = 0;

            wavetable = new float[sound.FrameCount];
            float[] temp;
            for (int i = 0; i < sound.FrameCount; i++)
            {
                temp = sound.ReadNextFrame();
                wavetable[i] = temp[0];
            }

            // Tell the AR object where it gets its samples from 
            // the sine wave object.
            envelope.Source = this;
            envelope.SampleRate = SampleRate;
            envelope.Duration = duration;
            envelope.Start();
        }
    }
}
