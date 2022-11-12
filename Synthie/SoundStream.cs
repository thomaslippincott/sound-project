using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Forms;


class SoundStream : Sound
{
    private int bytesPerFrame = 0;
    private WaveFileReader readerStream;
    private WaveFileWriter writerStream;
    public SoundStream(string filePath, char read = 'r', int sampleRate = 44100, int channels = 2)
    {
        Open(filePath, read, sampleRate, channels);
    }

    override public float Duration
    {
        get
        {
            if (readerStream != null)
                return (float)readerStream.TotalTime.TotalSeconds;
            else
                return (float)writerStream.TotalTime.TotalSeconds;
        }
    }

    /// <summary>
    /// Plays the raw samples to the speakers
    /// </summary>
    override public void Play(bool loop = false)
    {
        //not in read mode
        if (readerStream == null)
            return;

        Seek(0);

        base.Play(loop);

        //if output is not running
        if (outputPlayDevice == null)
        {
            //give it a method to call when done (for memory release)
            outputPlayDevice.PlaybackStopped += OnPlaybackStopped;
        }
    }

    /// <summary>
    /// Release data from program
    /// </summary>
    override public void Close()
    {
        base.Close();

        bytesPerFrame = 0;

        if (writerStream != null)
        {
            writerStream.Close();
            writerStream.Dispose();
            writerStream = null;
        }

        if (readerStream != null)
        {
            readerStream.Dispose();
            readerStream.Close();
            readerStream = null;
        }
    }

    override public float[] GetFrame(int index)
    {
        long oldPos = readerStream.Position;
        readerStream.Position = index * 4;
        float[] temp = readerStream.ReadNextSampleFrame();
        readerStream.Position = oldPos;
        return temp;
    }

    override public bool isSupportedForOpen(SoundFileTypes index)
    {
        return index == SoundFileTypes.WAV;
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
    override public bool Open(string path)
    {
        Close();
        return OpenFileStream(path);
    }

    public bool Open(string path, char read = 'r', int sampleRate = 44100, int channels = 2)
    {
        Close();
        if (read == 'r')
            return OpenFileStream(path);
        else if (read == 'w')
            return OpenWriteStream(path, sampleRate, channels);
        else
            MessageBox.Show("Unknown open mode");
        return false;
    }


    override public bool Open(UnmanagedMemoryStream resourceStream)
    {
        Close();
        return OpenFileStream(resourceStream);
    }
    /// <summary>
    /// Open a file to stream read. Currently only works properly (and tests) with Wave files.
    /// </summary>
    /// <param name="path">file name to save as</param>
    private bool OpenFileStream(string path)
    {
        try
        {
            //open file
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            readerStream = new WaveFileReader(stream);

            //save format
            format = readerStream.WaveFormat;
            bytesPerFrame = format.BitsPerSample / 8 * format.Channels;
            filename = path;

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
    /// Open a file to stream read. Currently only works properly (and tests) with Wave files.
    /// </summary>
    /// <param name="resourceStream">resoruce stream</param>
    private bool OpenFileStream(UnmanagedMemoryStream resourceStream)
    {
        try
        {
            //open file
            readerStream = new WaveFileReader(resourceStream);

            //save format
            format = readerStream.WaveFormat;
            bytesPerFrame = format.BitsPerSample / 8 * format.Channels;

            if (format.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                MessageBox.Show("Sound file not a float format. Values may be incorrect", "Loading problem");
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Loading Resource Error");
            return false;
        }
        return true;
    }


    /// <summary>
    /// Create a file to stream output.
    /// While there is a file type, only WAV is currently implemented.
    /// </summary>
    /// <param name="path">file name to save as</param>
    /// <param name="sampleRate"> defaults to 44100</param>
    /// <param name="channels">number of channels</param>
    /// <param name="type">The file type to output</param>
    private bool OpenWriteStream(string path, int sampleRate = 44100, int channels = 2)
    {
        Close();
        format = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        try
        {
            Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            writerStream = new WaveFileWriter(stream, format);
        }
        catch (Exception e)
        {
            MessageBox.Show("Could not open to Write: " + e.Message);
            return false;
        }
        return true;
    }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        int count = 0;
        while (count < sampleCount && readerStream.Position < readerStream.Length)
        {
            float[] frame = readerStream.ReadNextSampleFrame();

            //copy over frame
            for (int i = 0; i < frame.Length; i++)
            {
                buffer[count + offset + i] = frame[i];
            }
            count += frame.Length;

            //restart if looping
            if (readerStream.Position >= readerStream.Length && loop)
                Seek(0);
        }
        return count;
    }

    //override public float[] ReadNextFrame()
    //{
    //    //sanity check
    //    if (readerStream.Position >= readerStream.Length)
    //        return null;

    //    byte[] temp = new byte[bytesPerFrame];
    //    readerStream.Read(temp, 0, bytesPerFrame);
    //    return ByteToFloat(temp);
    //}

    override public float[] ReadNextFrame()
    {
        //sanity check
        if (readerStream.Position >= readerStream.Length)
            return null;
        
        byte[] temp = new byte[bytesPerFrame];
        readerStream.Read(temp, 0, bytesPerFrame);

        if (readerStream.WaveFormat.Encoding == WaveFormatEncoding.Pcm && readerStream.WaveFormat.BitsPerSample == 16)
        {
            short[] temp2 = ByteToShort(temp);
            float[] temp3 = new float[Channels];

            for (int i = 0; i < Channels; i++)
            {
                temp3[i] = (float)temp2[i] / short.MaxValue;
            }
            return temp3;
        }

        else
            return ByteToFloat(temp);

    }

    override public void Seek(int i)
    {
        if (readerStream != null)
            readerStream.Seek(i, SeekOrigin.Begin);
    }

    new public void Stop()
    {
        base.Stop();

        readerStream.Position = 0;
    }


    override public void WriteNextFrame(float[] frame)
    {
        writerStream.WriteSamples(frame, 0, frame.Length);
    }

    override public void WriteNextFrame(float frame)
    {
        float[] temp = new float[Channels];
        for (int i = 0; i < Channels; i++)
            temp[i] = frame;

        writerStream.WriteSamples(temp, 0, temp.Length);
    }

    new protected void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
        base.OnPlaybackStopped(sender, args);
        readerStream.Position = 0;
    }

    /// <summary>
    /// Copy the file to the given saved location, with the given format
    /// </summary>
    /// <param name="path">path to save at</param>
    /// <param name="type">the desired sound format </param>
    /// <returns></returns>
    override protected bool SaveAs(string path, SoundFileTypes type)
    {
        try
        {
            switch (type)
            {
                case SoundFileTypes.WAV:  //WAVE
                    WaveFormat f = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, format.Channels);
                    using (WaveFileWriter writer = new WaveFileWriter(path, f))
                    {
                        readerStream.Seek(0, SeekOrigin.Begin);
                        readerStream.CopyTo(writer);
                        readerStream.Seek(0, SeekOrigin.Begin);
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
                            MediaFoundationApi.Startup();
                            enc.Encode(path, readerStream);
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


}
