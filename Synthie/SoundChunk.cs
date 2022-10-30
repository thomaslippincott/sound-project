using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AudioProcess
{

    public class SoundChunk : Sound
    {

        protected int lastReadSampleIndex = 0;
        protected int lastWriteSampleIndex = 0;
        private float[] cachedSamples;
        /// <summary>
        /// Constructor for a default, 10 samples of silence with a 44100 sample rate and mono sound.
        /// </summary>
        public SoundChunk()
        {
            cachedSamples = new float[10];
        }

        /// <summary>
        /// Constructor for a sound with a the given sample rate and channels.
        /// Create 0.5s of silence by defalt
        /// </summary>
        /// <param name="sampleRate">sample rate</param>
        /// <param name="channels">channels</param>
        /// <param name="duration">duration in seconds (defaults to 0.5)</param>
        public SoundChunk(int sampleRate, int channels, float duration = 0.5f)
        {
            format = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            cachedSamples = new float[(int)(sampleRate * duration * channels)];
        }

        /// <summary>
        /// Constructor for a a Sound object loaded from a file
        /// </summary>
        /// <param name="path">path to file</param>
        public SoundChunk(string path)
        {
            Open(path);
        }


        /// <summary>
        /// Constructor for a a Sound object loaded from a file
        /// </summary>
        /// <param name="resourceStream">embedded resource stream</param>
        public SoundChunk(UnmanagedMemoryStream resourceStream)
        {
            Open(resourceStream);
        }

        override public float Duration { get => (float)cachedSamples.Length / (format.SampleRate * format.Channels); }

        public void clearTo(float duration)
        {
            cachedSamples = new float[(int)(SampleRate * duration * Channels)];
        }

        public void SetFrame(float[] val, int index)
        {
            for (int i = 0; i < Channels; i++)
                cachedSamples[index * Channels + i] = val[i];
        }


        public void SetFrame(float val, int index)
        {
            for (int i = 0; i < Channels; i++)
                cachedSamples[index * Channels + i] = val;
        }

        public void SetFrame(float val, int index, int channel)
        {
            cachedSamples[index * Channels + channel] = val;
        }

        override public float[] GetFrame(int index)
        {
            float[] temp = new float[Channels];
            for (int i = 0; i < Channels; i++)
                temp[i] = cachedSamples[index * Channels + i];
            return temp;
        }

        override public bool isSupportedForOpen(SoundFileTypes index)
        {
            try
            {
                SoundFileTypes temp = (SoundFileTypes)index;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        override public bool isSupportedForSave(SoundFileTypes index)
        {
            try
            {
                SoundFileTypes temp = (SoundFileTypes)index;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #region File open/close operations


        /// <summary>
        /// Release data from program
        /// </summary>
        override public void Close()
        {
            base.Close();
            lastReadSampleIndex = 0;
            lastWriteSampleIndex = 0;
        }

        override public bool Open(string path)
        {
            Close();
            lastReadSampleIndex = 0;
            lastWriteSampleIndex = 0;
            return OpenFullCopy(path);
        }

        override public bool Open(UnmanagedMemoryStream resourceStream)
        {
            Close();
            lastReadSampleIndex = 0;
            lastWriteSampleIndex = 0;
            return OpenFullCopy(resourceStream);
        }

        /// <summary>
        /// Save the file to the last saved location
        /// </summary>
        /// <returns>true if saved successfully</returns>
        override public bool Save()
        {
            return Save(lastFile, lastFileType);
        }

        /// <summary>
        /// Save the file to the given saved location, with the given format
        /// </summary>
        /// <param name="path">path to save at</param>
        /// <param name="type">the desired sound format </param>
        /// <returns></returns>
        override protected bool SaveAs(string path, SoundFileTypes type)
        {
            if (cachedSamples == null)
            {
                MessageBox.Show("No sound samples available", "Saving Error");
                return false;
            }
            try
            {
                switch (type)
                {
                    case SoundFileTypes.WAV:  //WAVE
                        WaveFormat f = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, format.Channels);
                        using (WaveFileWriter writer = new WaveFileWriter(path, f))
                        {
                            writer.WriteSamples(cachedSamples, 0, cachedSamples.Length);
                        }
                        break;

                    case SoundFileTypes.MP3: //MP3
                                             //confirm codec exists
                        var mediaType = MediaFoundationEncoder.SelectMediaType(
                            AudioSubtypes.MFAudioFormat_MP3,
                            new WaveFormat(format.SampleRate, format.Channels),
                            format.SampleRate);

                        if (mediaType != null) //mp3 encoding supported
                        {
                            using (MediaFoundationEncoder enc = new MediaFoundationEncoder(mediaType))
                            {
                                //converts back to bytes, and put in provider for pulling samples when writing
                                byte[] tt = FloatToByte(cachedSamples);
                                IWaveProvider provider = new RawSourceWaveStream(
                                   new MemoryStream(tt), format);

                                //use the Microsoft media foundation API to save the file.
                                MediaFoundationApi.Startup();
                                enc.Encode(path, provider);
                                MediaFoundationApi.Shutdown();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Your computer does not support encoding mp3. Download a codec", "Saving Error");
                        }
                        break;
                }

                //save file location for ease of later saving
                lastFile = path;
                lastFileType = type;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Saving Error");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opens a sound file and load in the raw 
        /// data for later editing.
        /// Currently supports IEEE format (most WAVs and MP3s). Other formats may 
        /// complete, but the value may be incorrect.
        /// </summary>
        /// <param name="path">path to a sound file</param>
        /// <returns>true if opened successfully</returns>
        private bool OpenFullCopy(string path)
        {
            try
            {
                //open file

                AudioFileReader audioFile = new AudioFileReader(path);

                int bytePerSampe = audioFile.WaveFormat.BitsPerSample / 8;

                //save format
                format = audioFile.WaveFormat;
                cachedSamples = new float[audioFile.Length / bytePerSampe];
                audioFile.Read(cachedSamples, 0, cachedSamples.Length);

                filename = path;

                audioFile.Close();

                if (format.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    MessageBox.Show("Sound file not a float format. Values may be incorrect", "Loading problem");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading Error");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opens a resource sound file
        /// </summary>
        /// <param name="resourceStream">the sound resource</param>
        /// <returns>true if successful</returns>
        private bool OpenFullCopy(UnmanagedMemoryStream resourceStream)
        {
            //helper function for fast opening
            try
            {
                //open file
                WaveFileReader readerStream = new WaveFileReader(resourceStream);
                ISampleProvider provider = readerStream.ToSampleProvider();

                int bytePerSampe = readerStream.WaveFormat.BitsPerSample / 8;

                //save format
                format = provider.WaveFormat;
                cachedSamples = new float[readerStream.Length / bytePerSampe];
                provider.Read(cachedSamples, 0, cachedSamples.Length);

                readerStream.Close();

                if (format.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    MessageBox.Show("Sound file not a float format. Values may be incorrect", "Loading problem");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Loading Error");
                return false;
            }
            return true;
        }
        #endregion File open/close operations

        #region Read write operations
        override public void Seek(int i)
        {
            lastReadSampleIndex = i;
            lastWriteSampleIndex = i;
        }

        public void SeekRead(int i)
        {
            lastReadSampleIndex = i;
        }

        public void SeekWrite(int i)
        {
            lastWriteSampleIndex = i;
        }

        override public float[] ReadNextFrame()
        {
            //sanity check
            if (lastReadSampleIndex / Channels >= FrameCount)
                return null;

            float[] result = new float[format.Channels];
            Array.Copy(cachedSamples, lastReadSampleIndex, result, 0, format.Channels);
            lastReadSampleIndex += format.Channels;
            return result;
        }

        override public void WriteNextFrame(float[] frame)
        {
            for (int i = 0; i < frame.Length; i++)
                cachedSamples[lastWriteSampleIndex + i] = frame[i];

            lastWriteSampleIndex += format.Channels;
        }


        override public void WriteNextFrame(float frame)
        {
            for (int i = 0; i < Channels; i++)
                cachedSamples[lastWriteSampleIndex + i] = frame;

            lastWriteSampleIndex += format.Channels;
        }


        #endregion

        #region Playback functions
        override public void Play(bool loop = false)
        {
            base.Play(loop);

        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int count = 0;
            for (int n = 0; n < sampleCount && lastReadSampleIndex / Channels < FrameCount; n++)
            {
                buffer[n + offset] = cachedSamples[lastReadSampleIndex];
                lastReadSampleIndex++;

                count++;

                //restart if looping
                if (lastReadSampleIndex / Channels >= FrameCount && loop)
                    lastReadSampleIndex = 0;
            }
            return count;
        }

        override public void Stop()
        {
            base.Stop();
            lastReadSampleIndex = 0;
        }

        override protected void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            base.OnPlaybackStopped(sender, args);
            lastReadSampleIndex = 0;
        }
        #endregion Playback functions

    }
}
