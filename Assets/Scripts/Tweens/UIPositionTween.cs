using UnityEngine;

public class UIPositionTween : BaseTween
{

    public Vector2 from;
    public Vector2 to;
    private RectTransform myRect;

    public void OnEnable()
    {
        myRect = transform.GetComponent<RectTransform>();
    }
    
    public void GetStart()
    {
        from = GetComponent<RectTransform>().anchoredPosition;
    }

    public void GetEnd()
    {
        to = GetComponent<RectTransform>().anchoredPosition;
    }
    
    public override void Init() {
        myRect.anchoredPosition = @from * (1 - lerpFactor) + to * lerpFactor;
    }

    public override void Update()
    {
        if (!Application.isPlaying || State == TweenState.Idle) return;
        base.Update();
        //myRect.sizeDelta
        myRect.anchoredPosition = @from * (1 - lerpFactor) + to * lerpFactor;
    }
}
