using System;

namespace com.spacepuppy.UserInput
{

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

    public enum CompositeAxlePrecedence
    {
        
        Largest = 0,
        Smallest = 1

    }

}
