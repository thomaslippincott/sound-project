using System.Drawing;

namespace AudioProcess
{
    class SoundView
    {
        //Draw options
        private Pen axisColor;
        private Brush backgroundColor;
        private Brush barColor;
        private Rectangle drawArea;
        private Pen channelSepColor;
        private double heightPercent;
        private Brush infoColor;
        private int offset;
        private int pixelsPerPeak;
        private int samplesPerPeak;
        private int spacerPixels;

        private Sound sound;

        public SoundView()
        {
            SetDefault();
        }

        public Pen AxisColor { get => axisColor; set => axisColor = value; }
        public Brush BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
        public Brush BarColor { get => barColor; set => barColor = value; }
        public Rectangle DrawArea { set => drawArea = value; }
        public double HeightPercent { get => heightPercent; set => heightPercent = value; }
        public Brush InfoColor { get => infoColor; set => infoColor = value; }
        public int Offset { get => offset; set {
                if (value < 0)
                    offset = 0;
                else 
                    offset = value; 
            } 
        }
        public int PixelsPerPeak { get => pixelsPerPeak; set => pixelsPerPeak = value; }
        public int SamplesPerPeak { get => samplesPerPeak; set => samplesPerPeak = value; }
        public Sound Sound {set => sound = value; }
        public int SpacerPixels { get => spacerPixels; set => spacerPixels = value; }
        public Pen ChannelSepColor { get => channelSepColor; set => channelSepColor = value; }

        public void SetDefault()
        {
            drawArea = new Rectangle(0, 0, 400, 300);
            pixelsPerPeak = 3;
            spacerPixels = 1;
            SamplesPerPeak = 10;
            offset = 0;
            heightPercent = 0.8;
            MakeDarkFormat();
        }


         /// <summary>
        /// Updates the display offset to start at a given percentage
        /// </summary>
        /// <param name="percent"></param>
        public void SetOffsetFromPercent(double percent)
        {
            int maxBars = drawArea.Width / (pixelsPerPeak + spacerPixels);
            int barsInFile = sound.FrameCount / (SamplesPerPeak);
            int maxOffsetBar = barsInFile - maxBars;
            int maxOffset = maxOffsetBar * SamplesPerPeak;

            if (barsInFile < maxBars)
                Offset = 0;

            Offset = (int)(percent * sound.FrameCount);
            if (Offset > maxOffset)
                Offset = maxOffset;

        }

        #region Pre made color formats

        public void MakeDarkFormat()
        {
            infoColor = new SolidBrush(Color.Yellow);
            backgroundColor = new SolidBrush(Color.FromArgb(50, 50, 50));
            axisColor = new Pen(Color.Black, 5);
            barColor = new SolidBrush(Color.Red);
            channelSepColor = new Pen(Color.Yellow, 10);
        }

        public void MakeLightFormat()
        {
            infoColor = new SolidBrush(Color.Black);
            backgroundColor = new SolidBrush(Color.FromArgb(225,225,225));
            axisColor = new Pen(Color.FromArgb(50,50,50), 5);
            barColor = new SolidBrush(Color.DarkGray);
            channelSepColor = new Pen(Color.Black, 10);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draws a given channel in the area given
        /// </summary>
        /// <param name="g">Graphics pointer</param>
        /// <param name="channelArea">area to draw in</param>
        /// <param name="channel">the channel to draw</param>
        private void Render(Graphics g, Rectangle channelArea, int channel)
        {
            if (sound == null)
                return;

            int start = offset;

            //parameters
            int drawHeight = channelArea.Height;
            int drawWidth = channelArea.Width;
            int midPoint = (drawHeight) / 2 + channelArea.Y;
            int heightRange = (int)(heightPercent * drawHeight) / 2;

            //background
            g.FillRectangle(backgroundColor,
                new RectangleF(channelArea.X, channelArea.Y, drawWidth, drawHeight));

            //draw samples
            int x = 0;
            float currMax = MaxInRange(start, samplesPerPeak, channel);
            float currMin = MinInRange(start, samplesPerPeak, channel);
            start += SamplesPerPeak * sound.Channels;
            while (x < channelArea.Width && (start + SamplesPerPeak) < sound.FrameCount)
            {
                float nextMax = MaxInRange(start, samplesPerPeak, channel);
                float nextMin = MinInRange(start, samplesPerPeak, channel);

                //draw max and min bars
                var lineHeight = heightRange * currMax;
                g.FillRectangle(barColor,
                    new RectangleF(x, midPoint - lineHeight, pixelsPerPeak, lineHeight));
                lineHeight = heightRange * currMin;
                g.FillRectangle(barColor,
                    new RectangleF(x, midPoint, pixelsPerPeak, -lineHeight));

                //move to next bin
                x += pixelsPerPeak;
                x += spacerPixels;
                start += SamplesPerPeak;
                currMax = nextMax;
                currMin = nextMin;
            }

            //draw the axis
            g.DrawLine(axisColor, channelArea.X, midPoint, channelArea.Width, midPoint);
        }

        /// <summary>
        /// Pull basic sound ifor and print at the top left
        /// </summary>
        /// <param name="g"></param>
        private void RenderFormat(Graphics g)
        {
            string s = "Channels: " + sound.Channels + "\t";
            s += "Sample Rate: " + sound.SampleRate + "\t";
            s += "Sample Frames: " + sound.FrameCount + "\t";
            s += "Duration: " + sound.Duration + "\t";
            s += "Current Pos: " + (float)offset/(sound.SampleRate * sound.Channels);

            g.DrawString(s, new Font(FontFamily.GenericSerif, 10), infoColor, new Point(drawArea.X + 20, drawArea.Y));
        }

        public void OnPaint(Graphics g)
        {

            int drawHeight = drawArea.Height;
            Rectangle channelArea = drawArea;
            channelArea.Height = drawHeight / sound.Channels;

            //draw all channels
            for (int i = 0; i < sound.Channels; i++)
            {
                //figure our chanels draw area, then draw
                channelArea.Y = i * channelArea.Height;
                Render(g, channelArea, i);

                //draw channel separator
                if (i != 0)
                {
                    g.DrawLine(channelSepColor, channelArea.X, channelArea.Y, channelArea.Width, channelArea.Y);
                }

            }

            RenderFormat(g); //draw sound info
        }
        #endregion


        #region Helper functions
        public float MaxInRange(int start, int numSample, int channel)
        {
            float max = -2;
            int i = start;
            int stop = start + numSample;

            if (stop > sound.FrameCount - 1)
                stop = sound.FrameCount - 1;

            for (; i < stop; i ++)
            {
                if (sound.GetFrame(i)[channel] > max)
                    max = sound.GetFrame(i)[channel];
            }
            if (max > 1)
                max = 1;
            return max;
        }

        public float MinInRange(int start, int numSamples, int channel)
        {
            float min = 2;
            int i = start ;
            int stop = start + numSamples;

            if (stop > sound.FrameCount - 1)
                stop = sound.FrameCount - 1;

            for (; i < stop; i ++)
            {
                if (sound.GetFrame(i)[channel] < min)
                    min = sound.GetFrame(i)[channel];
            }
            if (min < -1)
                min = -1;
            return min;
        }
        #endregion
    }
}
