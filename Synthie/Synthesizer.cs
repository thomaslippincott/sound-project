using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Synthie
{
    public class Synthesizer
    {
        private SoundStream soundOut;
        private SoundStream soundIn;
        string tempFilePath = "output.wav";

        private SongScore song;
        private int channels = 2;
        private int sampleRate = 44100;

        public Synthesizer()
        {
            song = new SongScore();
            song.Channels = channels;
            song.SampleRate = sampleRate;
        }

        /// <summary>
        /// Helper function to insure sound smaples are within range.
        /// </summary>
        /// <param name="frame">sound sample</param>
        /// <returns>clamped ( [-1, 1] ) sound sample</returns>
        private float[] ClampFrame(double[] frame)
        {
            float[] audio = new float[frame.Length];
            for (int i = 0; i < channels; i++)
                audio[i] = (float)Math.Min(1.0, Math.Max(-1.0, frame[i]));

            return audio;
        }

        /// <summary>
        /// Generate sound samples for the given music score. 
        /// </summary>
        public void Generate()
        {
            soundOut = new SoundStream(tempFilePath, 'w');
            double[] frame = new double[2];

            //reinitialize sampler
            song.Start();

            //keep asking for samples, until otherwise indicated
            while (song.Generate(frame))
            {
                soundOut.WriteNextFrame(ClampFrame(frame));
            }

            soundOut.Close();
        }

        public void OpenScore(string filename)
        {
            song.OpenScore(filename);
        }

        /// <summary>
        /// Generates a 5 second 1000HZ tone.
        /// </summary>
        public void OnGenerate1000hztone()
        {
            if (soundOut == null)
                soundOut = new SoundStream(tempFilePath, 'w', 44100, 1);

            double freq = 1000;
            double duration = 5;
            double frameDuration = 1.0 / soundOut.SampleRate;

            float val;
            for (double time = 0.0; time < duration; time += frameDuration)
            {
                val = (float)(Math.Sin(time * 2 * Math.PI * freq));
                soundOut.WriteNextFrame(val);
            }
        }

        public void OnPaint(Graphics g)
        {
            //TODO if desired
        }

        #region play and save functions

        //clean up temporary files
        ~Synthesizer()
        {
            if (File.Exists(tempFilePath))
            {
                // If file found, delete it    
                File.Delete(tempFilePath);
            }
        }

        public void Play()
        {
            //sound output file is still open, close to release the lock
            if (soundOut != null)
            {
                soundOut.Close();
                soundOut = null;
            }

            //if a new sound stream is needed for playback, make it
            if (soundIn == null)
            {
                soundIn = new SoundStream(tempFilePath);
            }

            //sanity chack for a file
            if (soundIn != null)
                soundIn.Play();
            else
                MessageBox.Show("No sound has yet been generated. Please generate a sound first.", "No Sound");
        }

        /// <summary>
        /// Saves the generated file. The output will be a wav
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            //safety check for overwrite
            if (File.Exists(fileName))
            {
                DialogResult result = MessageBox.Show("The current file exists. Do you want to overwrite?", "Overwrite?");
                if (result == DialogResult.OK)
                {
                    File.Delete(fileName);
                    File.Copy(tempFilePath, fileName);
                }
            }
            else
            {
                //new file, simply copy over
                File.Copy(tempFilePath, fileName);
            }
        }

        public void Stop()
        {
            if (soundIn != null)
            {
                soundIn.Stop();
                soundIn.Close();
                soundIn = null;
            }
        }
        #endregion
    }
}
