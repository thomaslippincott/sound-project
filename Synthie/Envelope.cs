namespace Synthie
{
    public abstract class Envelope : AudioNode
    {
        protected double time;
        protected double duration;
        protected AudioNode source;
        public double Duration { get => duration; set => duration = value; }
        public AudioNode Source { get => source; set => source = value; }
    }
}
