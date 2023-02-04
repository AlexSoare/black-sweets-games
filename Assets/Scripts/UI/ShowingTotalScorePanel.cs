using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingTotalScorePanel : MonoBehaviour
{
    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerView;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;

    private List<LobbyPlayerView> playerViews;

    private Action onRestartCallback;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Init(List<string> playerScores, Action onRestartCallback)
    {
        endText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        this.onRestartCallback = onRestartCallback;

        if (playerViews != null)
            foreach (var p in playerViews)
                Destroy(p.gameObject);
        else playerViews = new List<LobbyPlayerView>();
        playerViews.Clear();

        foreach (var p in playerScores)
        {
            var tempPlayer = Instantiate(playerView, playersParent);
            tempPlayer.SetInfo(p);
            tempPlayer.gameObject.SetActive(true);

            playerViews.Add(tempPlayer);
        }
    }

    public void ShowEndText()
    {
        endText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        onRestartCallback?.Invoke();
    }
}
