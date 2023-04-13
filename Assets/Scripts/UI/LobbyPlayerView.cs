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

    public Player player;

    public void SetAvatar(Sprite sprite)
    {
        avatarImg.sprite = sprite;
    }
    public void SetDrawing(Sprite sprite)
    {
        drawingImg.sprite = sprite;
    }
    public void Init(Player player)
    {
        this.player = player;

        nameTxt.text = player.Name;
    }

    public void SetLoading()
    {
        loading.SetActive(true);
    }

    public void SetDone()
    {
        loading.SetActive(false);
        doneLoading.SetActive(true);
    }

    public void SetScore(string score)
    {
        scoreTxt.text = score;
    }
}
