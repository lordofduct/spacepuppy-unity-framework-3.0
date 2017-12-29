using System;

namespace com.spacepuppy.Anim.Legacy
{

    public class AnimationInvalidAccessException : System.InvalidOperationException
    {

        public AnimationInvalidAccessException()
            : base("Can not access SPAnimationController until it has been initialized.")
        {

        }

    }

}
