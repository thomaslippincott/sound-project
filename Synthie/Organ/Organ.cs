using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Synthie.Organ
{
    public class Organ : Instrument
    {
        private double duration;
        private double time;
        private int[] drawbar_setting; // Array of nine integers indicating each drawbar's level from 0-8
        private int vibrato_setting; // Integer noting which vibrato setting (1 - third, 2 - half, or 3 - full) is to be used, or if it is off (0)
        private int leslie_setting; // Integer which leslie speaker setting (1 - chorale, 2 - tremolo) is to be used, or if it is off (0)
        private static double[] drawbars = new double[] { 0.5, 1.5, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 8.0 }; // Harmonic multipliers for each drawbar on a Hammond organ
        private static double[] vibrato_depth = new double[] { 3.0, 5.0, 10.0 }; // Vibrato frequency change multipliers for each vibrato setting (0 - third, 1 - half, or 2 - full)
        private static double vibrato_freq = 7.0; // Vibrato frequency on a Hammond organ: 7Hz
        private static double[] leslie_freqs = new double[] { 0.833, 6.677 }; // Frequencies of the tremolo made by a leslie speaker by setting (0 - chorale, 1 - tremolo)
        private static double leslie_depth = 0.1; // Depth of the amplitude change made by leslie speaker
        private double fundamental; // Fundamental frequency of the given note
        private string[] settings; // Array of strings containing different organ settings (0 - drawbar settings, 1 - leslie setting, 2 - vibrato setting)
        private List<AudioNode> tonewheels;
        private Envelope leslie_speaker;

        public Organ(Envelope env)
        {
            duration = 0.1;
            envelope = env;
            tonewheels = new List<AudioNode>();
        }

        private double DrawbarAmp(int setting)
        {
            if (setting == 8)
                return 1.0;
            else if (setting == 0)
                return 0.0;
            else
            {
                double dB = -3.0 * (8 - setting);
                return Math.Pow(10, (dB / 20.0));
            }
        }

        private int[] GetDrawbarSetting(string s)
        {
            int[] temp = new int[9];

            for (int i = 0; i < s.Length; i++)
                temp[i] = int.Parse(s[i].ToString());

            return temp;
        }

        public override bool Generate()
        {
            frame[0] = 0;
            frame[1] = 0;

            // Sum all harmonic waves
            foreach (AudioNode wheel in tonewheels)
            {
                wheel.Generate();
                frame[0] += wheel.Frame(0);
                frame[1] += wheel.Frame(1);
            }

            if (leslie_speaker != null)
            {
                leslie_speaker.Generate();
                frame = leslie_speaker.Frame();
            }

            envelope.Generate();                       //adjust the gain on base sample
            frame[0] = envelope.Frame(0);      //pull the adjusted sample
            frame[1] = envelope.Frame(1);

            time += samplePeriod; //normal time increment
            return time < duration;
        }

        public override void SetNote(Note note)
        {
            // Get note duration and frequency
            duration = note.Count * 60.0 / bpm;
            fundamental = Notes.NoteToFrequency(note.Pitch);

            // Get note organ settings
            settings = note.Setting.Split(',').ToArray();
            drawbar_setting = GetDrawbarSetting(settings[0]);
            leslie_setting = int.Parse(settings[1]);
            vibrato_setting = int.Parse(settings[2]);
        }

        public override void Start()
        {
            leslie_speaker = null;

            // Create list of sine waves to simulate tonewheels
            for (int i = 0; i < 9; i++)
            {
                // If drawbar setting is greater than 0 (i.e, on), create a wave for that harmonic
                if (drawbar_setting[i] > 0)
                {
                    double amp = DrawbarAmp(drawbar_setting[i]); // Amplitude of wave based on drawbar setting
                    double freq = fundamental * drawbars[i]; // Harmonic of current note frequency based on which drawbar is pulled
                    // If vibrato is on
                    if (vibrato_setting > 0)
                    {
                        double v_alpha = vibrato_depth[vibrato_setting - 1]; // Vibrato depth based on vibrato setting
                        // Use vibrato waves to create harmonics
                        tonewheels.Add(new VibratoWave(amp, freq, v_alpha, vibrato_freq));
                    }
                    else
                    {
                        // Otherwise, use normal sine waves
                        tonewheels.Add(new SineWave(amp, freq));
                    }
                }
            }

            // Initialize the created sine waves
            foreach (AudioNode wheel in tonewheels)
            {
                wheel.SampleRate = SampleRate;
                wheel.Start();
            }

            time = 0;

            // If leslie speaker setting is not off
            if (leslie_setting > 0)
            {
                // Create tremolo envelope to simulate lesie speaker
                leslie_speaker = new Tremolo(leslie_depth, leslie_freqs[leslie_setting - 1]);
                leslie_speaker.Source = this;
                leslie_speaker.SampleRate = SampleRate;
                leslie_speaker.Duration = duration;
                leslie_speaker.Start();
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
