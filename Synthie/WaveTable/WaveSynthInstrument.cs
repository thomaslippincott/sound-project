using System.Collections.Generic;
using System.Xml;

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
        private List<Glissando> glissandos;

        public WaveSynthInstrument(WaveSample sample)
        {
            sound = sample.sample;
            startFrame = sample.startFrame;
            loopEndFrame = sample.loopEndFrame;
            loopStartFrame = sample.loopStartFrame;
            endFrame = sample.endFrame;
            sourceFreq = sample.sourceFreq;
            glissandos = new List<Glissando>();
        }

        public override void SetNote(Note note)
        {
            duration = note.Count * 60.0 / bpm;
            targetFreq = Notes.NoteToFrequency(note.Pitch);

            XmlNode xml = note.Node;

            foreach (XmlNode node in xml.ChildNodes)
            {
                if (node.Name == "glissando")
                {
                    glissandos.Add(new Glissando(node));
                }
            }

            glissandos.Sort((x, y) =>
            {
                if (x.start == y.start)
                    return 0;
                else if (x.start < y.start)
                    return 1;
                else
                    return -1;
            });
        }

        private void Glissando()
        {
            if (glissandos.Count > 0)
            {
                int nextindex = glissandos.Count - 1;

                Glissando glissando = glissandos[nextindex];

                double gStartTime = glissando.start * 60.0 / bpm;
                if (time < gStartTime)
                    return;

                double gCountTime = glissando.count * 60.0 / bpm;
                double gElapsTime = time - gStartTime;
                double newFreq;

                if (gElapsTime >= gCountTime)
                {
                    glissandos.RemoveAt(nextindex);
                    newFreq = targetFreq = glissando.newFreq;
                }
                else
                {
                    double gElapsedRatio = gElapsTime / gCountTime;
                    newFreq = gElapsedRatio * glissando.newFreq
                          + (1 - gElapsedRatio) * targetFreq;
                }

                speed = newFreq / sourceFreq;
            }
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

            Glissando();

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
