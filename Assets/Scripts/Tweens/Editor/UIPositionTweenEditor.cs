//c# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIPositionTween))]
[CanEditMultipleObjects]
public class UIPositionTweenEditor : Editor
{
    private UIPositionTween myTarget;

    void OnEnable()
    {
        myTarget = (UIPositionTween)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Get from"))
        {
            myTarget.from = myTarget.GetComponent<RectTransform>().anchoredPosition;
        }
        if (GUILayout.Button("Get to"))
        {
            myTarget.to = myTarget.GetComponent<RectTransform>().anchoredPosition;
        }

        DrawDefaultInspector();
    }
}