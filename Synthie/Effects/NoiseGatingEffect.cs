﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthie.Effects
{
    public class NoiseGatingEffect : Effect
    {
        bool attack = false;
        bool hold = false;
        bool release = false;

        //float time = 0;
        //float timeperframe = 0;
        float threshold = 0;
        float time_stamp;
        
        public NoiseGatingEffect(float tpf, float th)
        {
            timeperframe = tpf;
            time = 0;
            attack = false;
            hold = false;
            release = false;
            threshold = th;
        }

        public override double Apply(double frame)
        {
            if(frame >= threshold)
            {
                if (!attack)
                {
                    attack = true;
                    hold = false;
                    release = false;
                    time_stamp = time;
                }
                return threshold + (frame - threshold) * Math.Min((time - time_stamp) / 0.5f, 1);
            }
            else if (attack || (hold && time - time_stamp < 0.5f))
            {
                if (!hold)
                {
                    attack = false;
                    hold = true;
                    release = false;
                    time_stamp = time;
                }
                return frame;
            }
            else
            {
                if (!release)
                {
                    attack = false;
                    hold = false;
                    release = true;
                    time_stamp = time;
                }
                return frame * Math.Max(0.5f - (time - time_stamp), 0);
            }
            return frame;
        }

        public override void UpdateTime()
        {
            time += timeperframe;
        }
    }

   
}