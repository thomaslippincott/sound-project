namespace Synthie
{
    public abstract class Envelope : AudioNode
    {
        protected double time;
        protected double duration;
        protected ToneInstrument source;
        public double Duration { get => duration; set => duration = value; }
        public ToneInstrument Source { get => source; set => source = value; }
    }
}
