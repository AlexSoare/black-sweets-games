using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITextTween : BaseTween
{
    public float from = 1;
    public float to = 2;
    private TextMeshPro tmPro;
    private TextMeshProUGUI tmProUgui;

    new void Awake()
    {
        tmPro = GetComponent<TextMeshPro>();
        tmProUgui = GetComponent<TextMeshProUGUI>();
    }

    public override void Init() {
        if (tmPro != null)
            tmPro.fontSize = lerpFactor * to + @from * (1 - lerpFactor);
        if (tmProUgui != null)
            tmProUgui.fontSize = lerpFactor * to + @from * (1 - lerpFactor);
    }

    public override void Update()
    {
        if (!Application.isPlaying || base.State == TweenState.Idle) return;
        base.Update();
        if (tmPro != null)
            tmPro.fontSize = lerpFactor*to + @from*(1 - lerpFactor);
        if (tmProUgui != null)
            tmProUgui.fontSize = lerpFactor * to + @from * (1 - lerpFactor);

    }

    [ContextMenu("PlayForward")]
    public void MenuPlayForward()
    {
        base.PlayForward();
    }
}