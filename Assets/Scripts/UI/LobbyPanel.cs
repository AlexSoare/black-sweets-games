using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerPrefab;

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

    public void SetPlayerConnected(string playerName, Sprite avatar)
    {
        var newPlayer = Instantiate(playerPrefab, playersParent);

        newPlayer.gameObject.SetActive(true);
        newPlayer.SetInfo(playerName);
        newPlayer.SetAvatar(avatar);
    }

    public void OnStartButton()
    {

        onStartGameCallback?.Invoke();
    }
}
