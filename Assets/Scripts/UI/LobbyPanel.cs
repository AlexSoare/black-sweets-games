using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerPrefab;

    public Image testImage;
    public string imgBase64;

    [ContextMenu("Test")]
    public void Test()
    {
        var sprite = DrawingGame.GetDrawingSprite(imgBase64,true);
        testImage.sprite = sprite;
    }

    private Action onStartGameCallback;

    public void Init(Action onStartGameCallback)
    {
        this.onStartGameCallback = onStartGameCallback;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerConnected(Player player)
    {
        var newPlayer = Instantiate(playerPrefab, playersParent);

        newPlayer.gameObject.SetActive(true);
        newPlayer.SetInfo(player.Uid,player.Name);
        //newPlayer.SetAvatar(avatar);
    }

    public void OnStartButton()
    {

        onStartGameCallback?.Invoke();
    }
}
