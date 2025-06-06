using UnityEngine;

public static class Util
{
    public static void AssertObjectNotNull(Object obj, string message)
    {
        if (obj == null)
        {
            Debug.LogError(message);
#if UNITY_EDITOR
            throw new UnityException(message);
#endif
        }
    }
}
