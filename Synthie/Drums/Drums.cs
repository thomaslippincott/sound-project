using AudioProcess;
using NAudio.Gui;
using Synthie.Effects;
using Synthie.WaveTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Synthie.Drums
{
    public class Drums : Instrument
    {
        private double duration;
        private double time;
        private SoundChunk sample;
        private int pos;
        private float[] wavetable;
        

        public Drums()
        {
            duration = 0.1;
            envelope = new AR(0.1, 0.1);
        }
        public override void SetNote(Note note)
        {
            string name = note.Pitch;
            if(name == "cymbal")
                sample = new SoundChunk(Properties.Resources.cymbal);
            if (name == "tom1")
                sample = new SoundChunk(Properties.Resources.tom1);
            if (name == "tom2")
                sample = new SoundChunk(Properties.Resources.tom2);
            if (name == "tom3")
                sample = new SoundChunk(Properties.Resources.tom3);
            if (name == "snare")
                sample = new SoundChunk(Properties.Resources.snare);
            if (name == "bass")
                sample = new SoundChunk(Properties.Resources.bass);
            duration = Math.Min(sample.Duration, note.Count);
        }

        public override void Start()
        {
            time = 0;
            pos = 0;

            wavetable = new float[sample.FrameCount];
            float[] temp;
            for (int i = 0; i < sample.FrameCount; i++)
            {
                temp = sample.ReadNextFrame();
                wavetable[i] = temp[0];
            }

            // Tell the AR object where it gets its samples from 
            // the sine wave object.
            envelope.Source = this;
            envelope.SampleRate = SampleRate;
            envelope.Duration = duration;
            envelope.Start();
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
    }
}
