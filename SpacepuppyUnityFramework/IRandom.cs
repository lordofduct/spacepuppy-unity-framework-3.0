

namespace com.spacepuppy
{

    /// <summary>
    /// Interface/contract for a random number generator.
    /// </summary>
    public interface IRandom
    {

        float Next();
        double NextDouble();
        int Next(int size);
        int Next(int low, int high);

    }

}
