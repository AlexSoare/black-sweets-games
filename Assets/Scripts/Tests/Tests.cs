using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Tests : MonoBehaviour
{
    public int value;

    [ContextMenu("Test1")]
    public void Test1()
    {
        int[] arr1 = new int[value];
        int[] arr2 = new int[value];

        for(int i = 0;i<value;i++)
        {
            arr1[i] = i;
            arr2[i] = i;
        }

        Stopwatch watch = new Stopwatch();
        watch.Start();
        
        for(int i = 0;i<value;i++)
        {
            arr1[i] *= 3;
            arr2[i] *= 3;
        }

        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds.ToString());
    }

    [ContextMenu("Test2")]
    public void Test2()
    {
        int[] arr1 = new int[value];
        int[] arr2 = new int[value];

        for (int i = 0; i < value; i++)
        {
            arr1[i] = i;
            arr2[i] = i;
        }

        Stopwatch watch = new Stopwatch();
        watch.Start();

        for (int i = 0; i < value; i++)
        {
            arr1[i] *= 3;
        }
        for (int i = 0; i < value; i++)
        {
            arr2[i] *= 3;
        }

        watch.Stop();
        UnityEngine.Debug.Log(watch.ElapsedMilliseconds.ToString());
    }
}
