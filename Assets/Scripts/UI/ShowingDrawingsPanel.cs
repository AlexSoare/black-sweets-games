using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowingDrawingsPanel : MonoBehaviour
{
    [SerializeField] private Image drawingImg;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetCurrentDrawing(Sprite drawingSprite)
    {
        drawingImg.sprite = drawingSprite;
    }
    public void SetTimer(string timer)
    {
        timerText.text = timer;
    }
}
