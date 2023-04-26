using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingForDrawingsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerPrefab;

    private List<LobbyPlayerView> playersView;

    public void Init(List<Player> players)
    {
        if (playersView != null)
        {
            foreach (var p in playersView)
                Destroy(p.gameObject);
            playersView.Clear();
        }
        else
            playersView = new List<LobbyPlayerView>();

        foreach (var p in players)
        {
            var tempPlayer = Instantiate(playerPrefab, playersParent);
            tempPlayer.Init(p);
            tempPlayer.SetLoading();
            tempPlayer.gameObject.SetActive(true);

            playersView.Add(tempPlayer);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerDrawing(Player player, Sprite drawing)
    {
        foreach (var p in playersView)
            if (p.player.Uid == player.Uid)
                p.SetDrawing(drawing);
    }
    public void SetPlayerDone(Player player)
    {
        foreach (var p in playersView)
            if (p.player.Uid == player.Uid)
                p.SetDone();
    }


    public void SetTimer(string time)
    {
        timerText.text = time;
    }
}
