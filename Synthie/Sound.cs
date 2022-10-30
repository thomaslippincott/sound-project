using NAudio.Wave;
using System;
using System.IO;


public enum SoundFileTypes
{ UNSUPPORTED = 0, WAV = 1, MP3 = 2 }

public abstract class Sound : WaveProvider32
{
    protected string filename;
    protected WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
    protected string lastFile = "temp.wav";
    protected SoundFileTypes lastFileType = SoundFileTypes.UNSUPPORTED;
    protected WaveOutEvent outputPlayDevice;
    protected bool loop = false;

    public static float RangeBound(double y)
    {
        if (y < -1)
            y = -1;
        if (y > 1)
            y = 1;
        return (float)y;
    }
    public int Channels
    {
        get
        {
            if (format != null)
            {
                return format.Channels;
            }
            else
                return 0;
        }
    }

    public abstract float Duration { get; }


    public int FrameCount
    {
        get { return (int)(Duration * format.SampleRate); }
    }

    public int SampleRate
    {
        get
        {
            if (format != null)
            {
                return format.SampleRate;
            }
            else
                return 0;
        }
    }

    public abstract float[] GetFrame(int index);

    public abstract bool isSupportedForOpen(SoundFileTypes type);

    public abstract bool isSupportedForSave(SoundFileTypes type);
    public abstract override int Read(float[] buffer, int offset, int sampleCount);


    /// <summary>
    /// Move the read (and write if applicable), to the given byte position.
    /// </summary>
    /// <returns>Teh byte position.</returns>
    public abstract void Seek(int i);


    /// <summary>
    /// Gets the next sample frame and auto advances. Work with both streaming and 
    /// fully loaded sound files.
    /// </summary>
    /// <returns>Gets the next sample frame, or null if nothing left.</returns>
    public abstract float[] ReadNextFrame();


    /// <summary>
    /// Write the next frame of a streamed output file.
    /// </summary>
    /// <param name="frame">The next frame to save</param>
    public abstract void WriteNextFrame(float[] frame);

    /// <summary>
    /// Write the next frame of a streamed output file.
    /// </summary>
    /// <param name="frame">The value the next frame is to save</param>
    public abstract void WriteNextFrame(float frame);


    #region Open, Close

    /// <summary>
    /// Release data from program
    /// </summary>
    public virtual void Close()
    {
        format = null;

        if (outputPlayDevice != null)
        {
            outputPlayDevice.Dispose();
            outputPlayDevice = null;
        }
    }

    /// <summary>
    /// Closes the current sound, and opens a sound file if possible
    /// </summary>
    /// <param name="path">path to a sound file</param>
    /// <returns>true if opened successfully</returns>
    public abstract bool Open(string path);

    /// <summary>
    /// Closes the current sound, and opens a sound file.
    /// Currently supports IEEE format (most WAVs and MP3s). Other formats may
    /// complete, but the value may be incorrect.
    /// </summary>
    /// <param name="path">path to a sound file</param>
    /// <returns>true if opened successfully</returns>
    public abstract bool Open(UnmanagedMemoryStream resourceStream);

    /// <summary>
    /// Save the file to the last saved location
    /// </summary>
    /// <returns>true if saved successfully</returns>
    public virtual bool Save()
    {
        if (lastFileType != SoundFileTypes.UNSUPPORTED)
        {
            return Save(lastFile, lastFileType);
        }

        return false;
    }

    /// <summary>
    /// Save the sound file at the given path with a given type.
    /// </summary>
    /// <param name="path">location to save</param>
    /// <param name="type">file format enum  value</param>
    /// <returns></returns>
    public bool Save(string path, SoundFileTypes type)
    {
        if (!isSupportedForSave(type))
        {
            return false;
        }
        else
        {
            lastFile = path;
            lastFileType = type;
            return SaveAs(path, type);
        }
    }

    protected abstract bool SaveAs(string path, SoundFileTypes type);

    #endregion Open, Close

    #region Playback functions

    /// <summary>
    /// Plays the raw samples to the speakers
    /// </summary>
    public virtual void Play(bool loop = false)
    {
        this.loop = loop;
        Seek(0);

        //if output is not running
        if (outputPlayDevice == null)
        {
            //make the output varaiable
            outputPlayDevice = new WaveOutEvent();

            //give it a method to call when done (for memory release)
            outputPlayDevice.PlaybackStopped += OnPlaybackStopped;

            this.SetWaveFormat(SampleRate, Channels);
            //initalize the output and play
            outputPlayDevice.Init(this);
            outputPlayDevice.Play();
        }
        else
        {
            //if paused, restart
            if (outputPlayDevice.PlaybackState == PlaybackState.Paused)
            {
                outputPlayDevice.Play();
            }
        }
    }

    /// <summary>
    /// Pause the sound playback
    /// </summary>
    public virtual void Pause()
    {
        if (outputPlayDevice != null)
            outputPlayDevice.Pause();
    }

    /// <summary>
    /// Stop the playback and release memeory
    /// </summary>
    public virtual void Stop()
    {
        if (outputPlayDevice != null)
            outputPlayDevice.Stop();
    }

    /// <summary>
    /// Sound cleanup
    /// </summary>
    /// <param name="sender">the object the trigger the event</param>
    /// <param name="args">details about the event</param>
    protected virtual void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
        if (outputPlayDevice != null)
            outputPlayDevice.Dispose();
        outputPlayDevice = null;
    }
    #endregion Playback functions

    #region Conversion Helper Functions

    /// <summary>
    /// Converts a raw byte sound data into a raw float sound data.
    /// Warning: the float data will only convert cleanly if the loaded sound bytes are in float format.
    /// If they are not, coversion will complete, and conversion back is possible, but min and max value may be inaccurate.
    /// This can be checked with the WaveFormat. IEEE will work properly.
    /// </summary>
    /// <param name="input">raw byte sound data</param>
    /// <returns>a raw float sound data</returns>
    protected float[] ByteToFloat(byte[] input)
    {
        var floatArray2 = new float[input.Length / 4];
        Buffer.BlockCopy(input, 0, floatArray2, 0, input.Length);
        return floatArray2;
    }

    /// <summary>
    /// Converts a raw byte sound data into a raw short sound data.
    /// Warning: the short data will only convert cleanly if the loaded sound bytes are in short format.
    /// If they are not, coversion will complete, and conversion back is possible, but min and max value may be inaccurate.
    /// This can be checked with the WaveFormat. PCM16 will work properly.
    /// </summary>
    /// <param name="input">raw byte sound data</param>
    /// <returns>a raw short sound data</returns>
    protected short[] ByteToShort(byte[] input)
    {
        short[] sdata = new short[(int)Math.Ceiling(input.Length / 2.0)];
        Buffer.BlockCopy(input, 0, sdata, 0, input.Length);
        return sdata;
    }

    /// <summary>
    /// Converts a raw float sound data into a raw byte sound data.
    /// </summary>
    /// <param name="input">raw float sound data</param>
    /// <returns>a raw byte sound data</returns>
    protected byte[] FloatToByte(float[] input)
    {
        var byteArray = new byte[input.Length * 4];
        Buffer.BlockCopy(input, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    /// <summary>
    /// Converts a raw short sound data into a raw byte sound data.
    /// </summary>
    /// <param name="input">raw short sound data</param>
    /// <returns>a raw byte sound data</returns>
    protected byte[] ShortToByte(short[] input)
    {
        byte[] result = new byte[input.Length];

        for (int i = 0; i < input.Length / 2; i++)
        {
            byte[] temp = BitConverter.GetBytes(input[i]);
            result[i * 2 + 0] = temp[0];
            result[i * 2 + 1] = temp[1];
        }
        return result;
    }

    #endregion Conversion Helper Functions
}