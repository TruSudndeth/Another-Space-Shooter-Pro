using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctionsHelper
{
    //public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    //{
    //    return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    //}

    public static int Map(float value, float minInput, float maxInput, int minOutput, int maxOutput)
    {
        float normalizedValue = Mathf.Clamp01((value - minInput) / (maxInput - minInput));
        int mappedValue = minOutput + Mathf.RoundToInt(normalizedValue * (maxOutput - minInput));
        return Mathf.Clamp(mappedValue, minOutput, maxOutput);
    }
    public static float Map(float value, float minInput, float maxInput, float minOutput, float maxOutput)
    {
        float normalizedValue = Mathf.Clamp01((value - minInput) / (maxInput - minInput));
        float mappedValue = minOutput + normalizedValue * (maxOutput - minInput);
        return Mathf.Clamp(mappedValue, minOutput, maxOutput);
    }
}
