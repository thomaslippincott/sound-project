namespace Synthie
{
    public class ToneInstrument : Instrument
    {
        private double duration;
        private double time;
        private double frequency;
        private SineWave sinewave;

        public ToneInstrument(Envelope env)
        {
            duration = 0.1;
            time = 0.0;
            frequency = 440.0;
            envelope = env;
        }

        public override bool Generate()
        {
            sinewave.Generate();         //make the base sample
            frame = sinewave.Frame();
            frame[0] *= 2;
            frame[1] *= 2;
            envelope.Generate();                       //adjust the gain on base sample
            frame[0] = envelope.Frame(0);      //pull the adjusted sample
            frame[1] = envelope.Frame(1);

            time += samplePeriod; //normal time increment
            return time < duration;
        }

        public override void Start()
        {
            sinewave = new SineWave(0.5, frequency)
            {
                SampleRate = SampleRate
            };
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
            frequency = Notes.NoteToFrequency(note.Pitch);
        }
    }
}
