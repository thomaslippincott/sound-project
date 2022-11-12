using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Synthie.Additive
{
    public class Additive : Instrument
    {
        private double duration;
        private double time;
        private VibratoWave vibewave = new VibratoWave();
        private SineWave sinewave = new SineWave();
        private bool vibe;
        double attack = 0.5;
        double release = 0.1;

        public double Frequency { get => sinewave.Frequency; set => sinewave.Frequency = value; }

        public Additive()
        {
            duration = 0.1;
            envelope = new AR(attack, release);
            vibe = false;
            //sinewave.Alpha = 4.0;
            //sinewave.VibratoFrequency = 3.0; 
        }
        public override void SetNote(Note note)
        {
            duration = note.Count * 60.0 / bpm;
            Frequency = Notes.NoteToFrequency(note.Pitch);
            //if(note.Settings = "vibe")
            //{
                vibe = true;
            //}
            sinewave.Frequency = Frequency;
            vibewave.Frequency = Frequency;
        }

        public override bool Generate()
        {
            float val = 0;

            //Cross-fade sounds when switching to vibrato
            if (vibe && time >= attack - 0.1 && time <= attack + 0.1)
            {
                sinewave.Generate();         //Sound A
                frame[0] = sinewave.Frame(0) * (float)(attack / time);
                frame[1] = sinewave.Frame(1) * (float)(attack / time);

                for (float i = 2.0f; i * sinewave.Frequency <= Notes.NoteToFrequency("E7"); i++)
                {
                    val = (float)(0.04 / i * Math.Sin(time * 2 * Math.PI * sinewave.Frequency * i));
                    for (int c = 0; c < frame.Length; c++)
                    {
                        frame[c] += val * (float)(attack / time);
                    }
                }

                vibewave.Generate();         //Sound B
                frame[0] += vibewave.Frame(0) * (float)(time / attack);
                frame[1] += vibewave.Frame(1) * (float)(time / attack);

                for (float i = 2.0f; i * vibewave.Frequency <= Notes.NoteToFrequency("E7"); i++)
                {
                    val = (float)(0.04 / i * Math.Sin(time * 2 * Math.PI * vibewave.Frequency * i));
                    for (int c = 0; c < frame.Length; c++)
                    {
                        frame[c] += val * (float)(time / attack);
                    }
                }
                frame[0] /= 2;
                frame[1] /= 2;

            }
            // Play straight from vibrato
            else if (vibe && time > attack &&  duration > 0.25)
            {
                vibewave.Generate();         //Sound B
                frame = vibewave.Frame();

                for (float i = 2.0f; i * vibewave.Frequency <= Notes.NoteToFrequency("E7"); i++)
                {
                    val = (float)(0.04 / i * Math.Sin(time * 2 * Math.PI * vibewave.Frequency * i));
                    for (int c = 0; c < frame.Length; c++)
                    {
                        frame[c] += val;
                    }
                }
            }
            // Play straight from "normal" sound
            else
            {
                sinewave.Generate();         //Sound A
                frame = sinewave.Frame();

                for (float i = 2.0f; i * sinewave.Frequency <= Notes.NoteToFrequency("E7"); i++)
                {
                    val = (float)(0.04 / i * Math.Sin(time * 2 * Math.PI * sinewave.Frequency * i));
                    for (int c = 0; c < frame.Length; c++)
                    {
                        frame[c] += val;
                    }
                }
            }

            envelope.Generate();                       //adjust the gain on base sample
            frame[0] = envelope.Frame(0);      //pull the adjusted sample
            frame[1] = envelope.Frame(1);

            time += samplePeriod; //normal time increment

            return time < duration;
        }

        public override void Start()
        {
            sinewave.SampleRate = SampleRate;
            sinewave.Start();
            time = 0;

            // Tell the AR object where it gets its samples from 
            // the sine wave object.
            envelope.Source = this;
            envelope.SampleRate = SampleRate;
            envelope.Duration = duration;
            envelope.Start();
        }
    }
}
