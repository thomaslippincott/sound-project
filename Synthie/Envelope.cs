namespace Synthie
{
    public abstract class Envelope : AudioNode
    {
        protected double time;
        protected double duration;
        protected Instrument source;
        public double Duration { get => duration; set => duration = value; }
        public Instrument Source { get => source; set => source = value; }
    }
}
