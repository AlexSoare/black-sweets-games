using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper instance;

    private static void CheckInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("CoroutineHelper").AddComponent<CoroutineHelper>();
        }
    }
    public static Coroutine Start(IEnumerator routine)
    {
        CheckInstance();
        
        if (routine == null)
            return null;
        
        return instance.StartCoroutine(routine);
    }

    public static void Stop(IEnumerator routine)
    {
        CheckInstance();
        
        if (routine == null)
            return;
        
        instance.StopCoroutine(routine);
    }
}
