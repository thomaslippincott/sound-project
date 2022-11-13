using System;

namespace Synthie
{
    public class ToneInstrument : Instrument
    {
        private double duration;
        private double time;
        private SineWave sinewave = new SineWave();

        public double Frequency { get => sinewave.Frequency; set => sinewave.Frequency = value; }

        public ToneInstrument(Envelope env)
        {
            duration = 0.1;
            envelope = env;
        }

        public override void SetEffect(ref Effects.Effect eff)
        {
            effect = eff;
        }

        public override bool Generate()
        {
            sinewave.Generate();         //make the base sample
            frame = sinewave.Frame();

            if( effect != null)
            {
                double amp = sinewave.GetAmp();
                frame[0] = effect.Apply(frame[0]);
                frame[1] = effect.Apply(frame[1]);
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

        public override void SetNote(Note note)
        {
            duration = note.Count * 60.0 / bpm;
            Frequency = Notes.NoteToFrequency(note.Pitch);
        }
    }
}
