using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private Image avatarImg;

    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject doneLoading;
    [SerializeField] private Image drawingImg;
    

    public string playerName;

    public void SetAvatar(Sprite sprite)
    {
        avatarImg.sprite = sprite;
    }

    public void SetInfo(string name)
    {
        playerName = name;

        nameTxt.text = name;
    }

    public void SetWaitingForDrawing()
    {
        loading.SetActive(true);
    }

    public void SetDone(Sprite drawing)
    {
        loading.SetActive(false);
        doneLoading.SetActive(true);
        drawingImg.sprite = drawing;
    }

    public void SetScore(string score)
    {
        scoreTxt.text = score;
    }
}
