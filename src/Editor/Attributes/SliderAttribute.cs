using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Attributes
{
    public class SliderAttribute : Attribute
    {
        public float LowestValue { get; }
        public float HighestValue { get; }

        public SliderAttribute(float lowestValue, float highestValue)
        {
            LowestValue = lowestValue;
            HighestValue = highestValue;
        }
    }
}
