using AudioProcess;
using System;
using System.Windows.Forms;

namespace Synthie
{
    public class SineWave : AudioNode
    {
        private double freq;
        private double amp;
        private double phase;

        public double Frequency { get => freq; set => freq = value; }

        public SineWave()
        {
            phase = 0;
            amp = 0.1;
            freq = 440;
        }

        public override void Start()
        {
            phase = 0;
        }

        public override bool Generate()
        {
            frame[0] = amp * Math.Sin(phase * 2 * Math.PI);
            frame[1] = frame[0];

            phase += freq * samplePeriod;

            return true;
        }

        public void MakeSine(Sound sound)
        {

            if (sound == null)
            {
                MessageBox.Show("Need a sound loaded first", "Generation Error");
                return;
            }

            sound.Seek(0);

            //pull needed sound file encoding parameters
            int sampleRate = sound.SampleRate;
            float duration = sound.Duration - 1.0f / sampleRate;

            //setup progress bar
            ProgressBar progress = new ProgressBar();
            progress.Runworker();

            //make the sine wave
            for (double time = 0.0; time < duration; time += 1.0 / sampleRate)
            {
                //make the value at this frame
                float val = (float)(amp * Math.Sin(time * 2 * Math.PI * freq));

                sound.WriteNextFrame(val);

                progress.UpdateProgress(time / duration);
            }
        }
    }
}
