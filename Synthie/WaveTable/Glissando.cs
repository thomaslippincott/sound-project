using System;
using System.Xml;

namespace Synthie.WaveTable
{
    public class Glissando
    {
        public double newFreq;
        public double start;
        public double count;
        public Glissando(XmlNode xml)
        {
            foreach (XmlAttribute attr in xml.Attributes)
            {
                if (attr.Name == "start")
                {
                    start = Convert.ToDouble(attr.Value) - 1;
                }

                if (attr.Name == "count")
                {
                    count = Convert.ToDouble(attr.Value);
                }

                if (attr.Name == "note")
                {
                    string pitch = attr.Value;
                    newFreq = Notes.NoteToFrequency(pitch);
                }
            }
        }
    }
}
