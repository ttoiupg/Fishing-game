using UnityEngine;

namespace Halfmoon.Utilities
{
    public class NumberRange
    {
        public float min { get; set; }
        public float max { get; set; }

        protected NumberRange(float value) {min = 0; max = value; }
        protected NumberRange(float a, float b)
        {
            if (a > b)
            { min = b; max = a; }
            else
            { min = a; max = b; }
        }
    }
}