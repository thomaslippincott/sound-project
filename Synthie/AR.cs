namespace Synthie
{
    public class AR : Envelope
    {
        private double _attack;
        private double _release;

        public double Attack { get { return _attack; } set { _attack = value; } }
        public double Release { get { return _release; } set { _release = value; } }

        public AR()
        {
            _attack = 0.05;
            _release = 0.05;
        }

        public AR(double attack, double release)
        {
            _attack = attack;
            _release = release;
        }

        public override bool Generate()
        {
            double ramp = 1;
            frame = source.Frame();

            if (time < _attack)
                ramp = time / _attack;
            else if (time > duration - _release)
                ramp = (duration - time) / _release;

            for (int i = 0; i < frame.Length; i++)
                frame[i] *= (float)ramp;

            time += samplePeriod;
            return time < duration;
        }

        public override void Start()
        {
            time = 0;
        }
    }
}
