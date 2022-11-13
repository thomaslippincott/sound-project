using AudioProcess;
using NAudio.Gui;
using Synthie.Effects;
using Synthie.WaveTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Subtractive
{
    public abstract class Subtractive : Instrument
    {

        public double duration;
        public double time;
        public SoundChunk sample;
        public double pos;
        public float[] wavetable;
        public double lowest_freq = 27.5;
        public SineWave sinewave = new SineWave();

        public double Frequency { get => sinewave.Frequency; set => sinewave.Frequency = value; }

        

        public override bool Generate()
        {
            frame[0] = wavetable[(int)pos] * (float)sinewave.Amplitude;
            frame[1] = wavetable[(int)pos] * (float)sinewave.Amplitude;
            pos += (Frequency/lowest_freq);

            if (pos >= wavetable.Length)
                pos -= wavetable.Length;

            envelope.Generate();                       //adjust the gain on base sample
            frame[0] = envelope.Frame(0);      //pull the adjusted sample
            frame[1] = envelope.Frame(1);

            time += samplePeriod; //normal time increment
            return time < duration;
        }

        public override void SetNote(Note note)
        {
            Frequency = Notes.NoteToFrequency(note.Pitch);
            duration = note.Count;

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

        
    }
}
