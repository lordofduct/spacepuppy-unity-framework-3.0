using System;

namespace com.spacepuppy.SPInput
{

    public enum AxleValueConsideration : byte
    {
        Positive = 0,
        Negative = 1,
        Absolute = 2
    }

    public enum ButtonState : sbyte
    {
        None = 0,
        Down = 1,
        Held = 2,
        Released = -1
    }

    public enum DeadZoneCutoff
    {
        Scaled = 0,
        Shear = 1
    }

    public enum MergedAxlePrecedence
    {

        Largest = 0,
        Smallest = 1

    }

}
