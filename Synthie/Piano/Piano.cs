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
        private ADSR dampen = new ADSR();

        public Piano()
        {
            duration = 0.1;
            pos = 0;
        }

        public override bool Generate()
        {

            frame[0] = wavetable[pos];
            frame[1] = wavetable[pos];

            pos++;

            // If at the end of the sample, loop the last 5000 frames
            if (pos >= wavetable.Length)
                pos = wavetable.Length - 5000;

            dampen.Generate();                       //adjust the gain on base sample
            frame[0] = dampen.Frame(0);      //pull the adjusted sample
            frame[1] = dampen.Frame(1);

            time += samplePeriod; //normal time increment
            return time < duration;
        }

        public override void SetNote(Note note)
        {
            sample = "../../res/PianoSamples/" + note.Pitch + "l.wav";
            sound = new SoundChunk(sample);
            duration = note.Count * 60.0 / bpm;

            int pedal_setting = int.Parse(note.Setting);

            // If pedal is on
            if (pedal_setting == 1)
            {
                // Play the sample for its full duration
                duration = sound.Duration;
                // And only add attack and release to the beginning and end for 5% of the duration
                // to allow for full decay without popping
                dampen.Attack = duration * 0.05;
                dampen.Decay = 0.0;
                dampen.Sustain = 1.0;
                dampen.Release = duration * 0.05;
            }
            else
            {
                // Increase duration slightly to mimic the way that the damper
                // takes a few miliseconds to silence the string after key release
                duration += 0.25;
                // Set envelope to attack for 5% of duration
                dampen.Attack = duration * 0.05;
                // Decay for 5% of duration
                dampen.Decay = duration * 0.05;
                // Sustain at 85% amplitude
                dampen.Sustain = 0.85;
                // And release for 0.25s to mimic damper on key release
                dampen.Release = 0.25;
            }
        }

        public override void Start()
        {
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
            dampen.Source = this;
            dampen.SampleRate = SampleRate;
            dampen.Duration = duration;
            dampen.Start();
        }
    }
}
