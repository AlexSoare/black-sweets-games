using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AlphaTween : BaseTween
{
    public float from = 0;
    public float to = 1;
    private SpriteRenderer[] _sprites;
    private TextMeshPro[] _texts;
    private TextMeshProUGUI[] _textsGui;
    private Image[] _images;
    public bool DestroyAfter = false;

    public void OnEnable()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>();

        _texts = GetComponentsInChildren<TextMeshPro>();

        _textsGui = GetComponentsInChildren<TextMeshProUGUI>();

        _images = GetComponentsInChildren<Image>();
    }

    public override void Init()
    {
        foreach (var sprite in _sprites)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var text in _texts)
            text.color = new Color(text.color.r, text.color.g, text.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var text in _textsGui)
            text.color = new Color(text.color.r, text.color.g, text.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var img in _images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, from * (1 - lerpFactor) + to * lerpFactor);
    }

    public override void Update()
    {
        if (!Application.isPlaying || State == TweenState.Idle) return;
        base.Update();
        foreach (var sprite in _sprites)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var text in _texts)
            text.color = new Color(text.color.r, text.color.g, text.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var text in _textsGui)
            text.color = new Color(text.color.r, text.color.g, text.color.b, from * (1 - lerpFactor) + to * lerpFactor);
        foreach (var img in _images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, from * (1 - lerpFactor) + to * lerpFactor);


        if (DestroyAfter && Mathf.Abs(lerpFactor - 1) < 0.01f)
        {
            foreach (var sprite in _sprites)
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, to);
            foreach (var text in _texts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, to);
            Destroy(this);
        }
    }

    [ContextMenu("PlayForward")]
    public void MenuPlayForward()
    {
        base.PlayForward();
    }

    [ContextMenu("PlayReverse")]
    public void MenuPlayReverse()
    {
        base.PlayReverse();
    }
}
