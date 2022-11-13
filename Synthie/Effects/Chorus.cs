using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Effects
{
    public class Chorus : Effect
    {
        private double[,] queue;
        private float max_delay;
        private int wr_loc = 55;
        private int rd_loc = 1;
        private int queue_size = 0;
        private int taps = 0;
        private int push_val = 1;
        private int c = 0;

        public Chorus(int channels_cnt, float samplerate)
        {
            max_delay = 0.01f;
            taps = 3;
            sample_rate = samplerate;
            channels = channels_cnt;
            queue_size = (int)((sample_rate * max_delay) + 0.5) + 1;

            queue = new double[queue_size, channels];

            for (int i = 0; i < channels; i++)
                for (int j = 0; j < queue_size; j++)
                    queue[j, i] = 0;

            push_val = (queue_size - 1) / taps;
            c = 0;
        }
        public override double Apply(double data)
        {
            queue[wr_loc, c] = data;
            data = (queue[rd_loc, c] + queue[(rd_loc + 55) % queue_size, c] + 
                queue[(rd_loc + 110) % queue_size, c] + queue[(rd_loc + 165) % queue_size, c]);

            c = (c + 1) % channels;
            return data;
        }

        public override void UpdateTime()
        {
            wr_loc = (wr_loc + 1) % queue_size;
            rd_loc = (rd_loc + 1) % queue_size;
        }
    }
}
