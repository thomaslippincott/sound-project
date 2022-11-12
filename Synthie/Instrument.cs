namespace Synthie
{
    public abstract class Instrument : AudioNode
    {
        protected Envelope envelope;
        public abstract void SetNote(Note note);

        public abstract void SetEffect(ref Effects.Effect eff);
    }
}
