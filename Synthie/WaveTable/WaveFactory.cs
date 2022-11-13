namespace Synthie.WaveTable
{
    public abstract class WaveSample
    {
        public SoundStream sample;
        public int startFrame;
        public int loopStartFrame;
        public int loopEndFrame;
        public int endFrame;
        public double sourceFreq;
    }

    public class Bruh : WaveSample
    {
        public Bruh(int SampleRate)
        {
            sample = new SoundStream("res/WaveTable/Bruh-sound-effect.wav", 'r', SampleRate, 2);
            startFrame = 1546;
            loopStartFrame = 9175;
            loopEndFrame = 9482;
            endFrame = 19134;
            sourceFreq = 294.0;
        }
    }

    public class Tuba : WaveSample
    {
        public Tuba(int SampleRate)
        {
            sample = new SoundStream("res/WaveTable/tuba_A2_025_mezzo-piano_normal.wav", 'r', SampleRate, 2);
            startFrame = 1384;
            loopStartFrame = 5440;
            loopEndFrame = 6254;
            endFrame = 15599;
            sourceFreq = 108.0;
        }
    }

    public class Sax : WaveSample
    {
        public Sax(int SampleRate)
        {
            sample = new SoundStream("res/WaveTable/saxophone_A4_05_forte_normal.wav", 'r', SampleRate, 2);
            startFrame = 3136;
            loopStartFrame = 15453;
            loopEndFrame = 18956;
            endFrame = 36335;
            sourceFreq = 452.0;
        }
    }

    public class Trumpet : WaveSample
    {
        public Trumpet(int SampleRate)
        {
            sample = new SoundStream("res/WaveTable/trumpet_A4_05_forte_normal.wav", 'r', SampleRate, 2);
            startFrame = 12200;
            loopStartFrame = 21823;
            loopEndFrame = 22625;
            endFrame = 34830;
            sourceFreq = 452.0;
        }
    }

    public class Oboe : WaveSample
    {
        public Oboe(int SampleRate)
        {
            sample = new SoundStream("res/WaveTable/oboe_A4_05_forte_normal.wav", 'r', SampleRate, 2);
            startFrame = 5180;
            loopStartFrame = 11415;
            loopEndFrame = 17978;
            endFrame = 35159;
            sourceFreq = 452.0;
        }
    }

    public static class WaveFactory
    {
        public static int SampleRate { get; set; }
        public static WaveSample Bruh => new Bruh(SampleRate);
        public static WaveSample Tuba => new Tuba(SampleRate);
        public static WaveSample Sax => new Sax(SampleRate);
        public static WaveSample Trumpet => new Trumpet(SampleRate);
        public static WaveSample Oboe => new Oboe(SampleRate);
    }
}
