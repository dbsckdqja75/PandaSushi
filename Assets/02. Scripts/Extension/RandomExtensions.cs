using UnityEngine;

public class RandomExtensions : MonoBehaviour
{
    public static bool RandomBool()
    {
        return (Random.Range(0, 1+1) == 1);
    }

    public static bool ProbabilityBool(float chance)
    {
        return (Random.value <= chance);
    }
}
