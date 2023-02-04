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

    public void Init(List<string> playerNames)
    {
        if (playersView != null)
        {
            foreach (var p in playersView)
                Destroy(p.gameObject);
            playersView.Clear();
        }
        else
            playersView = new List<LobbyPlayerView>();

        foreach (var p in playerNames)
        {
            var tempPlayer = Instantiate(playerPrefab, playersParent);
            tempPlayer.SetInfo(p);
            tempPlayer.SetWaitingForDrawing();
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

    public void PlayerSentHisDrawing(string playerName, Sprite drawing)
    {
        foreach (var p in playersView)
            if (p.playerName == playerName)
                p.SetDone(drawing);
    }

    public void SetTimer(string time)
    {
        timerText.text = time;
    }
}
