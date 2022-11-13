using System;
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
        float cutoff = 0;
        float prev_frame;
        
        public NoiseGatingEffect(float thresh_hold, float time_per_frame)
        {
            timeperframe = time_per_frame;
            time = 0;
            attack = false;
            hold = false;
            release = false;
            threshold = thresh_hold;
            cutoff = threshold - (threshold*0.3f);
            prev_frame = 0;
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
                return frame;
            }
            else if ((attack || (hold && time - time_stamp < 0.05f)) && frame >= cutoff)
            {
                if (!hold)
                {
                    attack = false;
                    hold = true;
                    release = false;
                    time_stamp = time;

                }

                prev_frame = (float)frame;
                return prev_frame;
            }
            else if ((hold || (release && time - time_stamp < 0.05f)) && frame > 0)
            {
                if (!release)
                {
                    attack = false;
                    hold = false;
                    release = true;
                    time_stamp = time;
                }
                return prev_frame * Math.Max(0.05f - (time - time_stamp), 0);
            }
            return 0;
        }

        public override void UpdateTime()
        {
            time += timeperframe;
        }
    }

   
}
