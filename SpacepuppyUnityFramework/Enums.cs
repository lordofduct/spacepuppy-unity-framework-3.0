

namespace com.spacepuppy
{


    /// <summary>
    /// Describe an axis in cartesian coordinates, Useful for components that need to serialize which axis to use in some fashion.
    /// </summary>
    public enum CartesianAxis
    {
        Zneg = -3,
        Yneg = -2,
        Xneg = -1,
        X = 0,
        Y = 1,
        Z = 2
    }

    [System.Flags()]
    public enum ComparisonOperator
    {
        NotEqual = 0,
        LessThan = 1,
        GreaterThan = 2,
        NotEqualAlt = 3,
        Equal = 4,
        LessThanEqual = 5,
        GreatThanEqual = 6,
        Always = 7
    }

    /// <summary>
    /// Search parameter type
    /// </summary>
    public enum SearchBy
    {
        Nothing = 0,
        Tag = 1,
        Name = 2,
        Type = 3
    }


}
