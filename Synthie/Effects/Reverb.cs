using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Effects
{
    public class Reverb : Effect
    {
        private double[,] queue;
        private double max_delay;
        private int wr_loc = 0;
        private int rd_loc = 0;
        private int queue_size = 0;
        private int taps = 0;
        private int push_val = 1;
        private int c = 0;
        public Reverb(int channels_cnt, float samplerate)
        {
            max_delay = MainForm.reverb_dlg.Delay;
            taps = MainForm.reverb_dlg.Taps;
            wet_percentage = MainForm.reverb_dlg.WetPercentage;
            sample_rate = samplerate;
            channels = channels_cnt;
            queue_size = (int)((sample_rate * max_delay) + 0.5) + 1;
            queue = new double[queue_size, channels];

            for (int i = 0; i < channels; i++)
                for (int j = 0; j < queue_size; j++)
                    queue[j, i] = 0;

            push_val = (queue_size-1) / taps;
            c = 0;
        }

        public override double Apply(double data)
        {
            
            int tmp = (wr_loc) % queue_size;
            double temp_data;

            temp_data = (1 - wet_percentage) * data + wet_percentage * (queue[rd_loc, c]);


            queue[tmp, c] = data;
            tmp = (tmp + push_val) % queue_size;
            for (int i = 2; i<= taps; i++)
            {
                queue[tmp, c] += data / i;
                tmp = (tmp + push_val) % queue_size;

            }
            

            c = (c + 1) % channels;
            return temp_data;
        }

        public override void UpdateTime()
        {
            wr_loc = (wr_loc + 1) % queue_size;
            rd_loc = (rd_loc + 1) % queue_size;
        }
    }
}
