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

    private Action onStartGameCallback;
    private List<LobbyPlayerView> playersView;

    public void Init(Action onStartGameCallback)
    {
        playersView = new List<LobbyPlayerView>();
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
        newPlayer.Init(player);
        newPlayer.SetLoading();

        playersView.Add(newPlayer);
    }
    public void SetPlayerAvatar(Player player, Sprite avatar)
    {
        foreach (var p in playersView)
            if (p.player.Uid == player.Uid)
                p.SetAvatar(avatar);
    }
    public void SetPlayerDone(Player player)
    {
        foreach (var p in playersView)
            if (p.player.Uid == player.Uid)
                p.SetDone();
    }


    public void OnStartButton()
    {

        onStartGameCallback?.Invoke();
    }
}
