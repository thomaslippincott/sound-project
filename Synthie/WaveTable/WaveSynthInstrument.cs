namespace Synthie.WaveTable
{
    public class WaveSynthInstrument : Instrument
    {
        private double duration;
        private double time;
        private double framePosition;
        private int frameIndex;
        private float[] currFrame;
        private float[] nextFrame;

        private int startFrame;
        private int loopStartFrame;
        private int loopEndFrame;
        private int endFrame;

        private double sourceFreq;
        private double targetFreq;
        private double speed;

        private double sustainTime;
        private double sampleTime;

        private SoundStream sound;

        public WaveSynthInstrument(WaveSample sample)
        {
            sound = sample.sample;
            startFrame = sample.startFrame;
            loopEndFrame = sample.loopEndFrame;
            loopStartFrame = sample.loopStartFrame;
            endFrame = sample.endFrame;
            sourceFreq = sample.sourceFreq;
        }

        public override void SetNote(Note note)
        {
            duration = note.Count * 60.0 / bpm;
            targetFreq = Notes.NoteToFrequency(note.Pitch);
        }

        public override bool Generate()
        {
            while (frameIndex <= framePosition)
            {
                currFrame = nextFrame;
                nextFrame = sound.ReadNextFrame();
                frameIndex++;
            }

            if (nextFrame == null)
                return false;

            for (int i = 0; i < currFrame.Length; i++)
                frame[i] = currFrame[i];

            for (int c = 0; c < currFrame.Length; c++)
            {
                frame[c] = (framePosition - (int)framePosition) * nextFrame[c]
                    + (1 - (framePosition - (int)framePosition)) * currFrame[c];
            }

            if (framePosition > loopEndFrame && duration - time > sampleTime - sustainTime)
            {
                framePosition = frameIndex = loopStartFrame;
                sound.Seek(loopStartFrame);
            }
            else
                framePosition += speed;

            time += samplePeriod;
            return time < duration;
        }

        public override void Start()
        {
            speed = targetFreq / sourceFreq;

            sustainTime = loopStartFrame / SampleRate;
            sampleTime = sound.Duration;

            time = 0;
            framePosition = frameIndex = startFrame;
            sound.Seek(startFrame);

            nextFrame = sound.ReadNextFrame();
        }
    }
}
