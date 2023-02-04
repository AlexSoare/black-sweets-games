using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScaleOrthoCamera : MonoBehaviour
{
    private Camera camera;
    public TMP_Text text;
    
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    
    void Update()
    {
        var orthoSize = Screen.height / 2;
        camera.orthographicSize = orthoSize;
        text.text = Screen.height + "";
    }
}
