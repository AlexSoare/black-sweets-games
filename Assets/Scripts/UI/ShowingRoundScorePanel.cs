using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ShowingRoundScorePanel : MonoBehaviour
{
    [SerializeField] private Image drawing;
    [SerializeField] private TitleView titleView;

    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerView;

    [SerializeField] private Transform realPlayersParent;
    [SerializeField] private LobbyPlayerView realPlayerView;

    private List<LobbyPlayerView> currentPlayers;
    private List<LobbyPlayerView> currentRealPlayers;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetDrawing(Sprite drawing)
    {
        this.drawing.sprite = drawing;
    }

    public IEnumerator SetTitle(string title, List<string> players, List<string> realPlayers, bool real)
    {
        // Clean up
        titleView.SetTitle(title);
        titleView.SetNormal();

        if (currentPlayers != null)
            foreach (var p in currentPlayers)
                Destroy(p.gameObject);
        else currentPlayers = new List<LobbyPlayerView>();
        currentPlayers.Clear();

        if (currentRealPlayers != null)
            foreach (var p in currentRealPlayers)
                Destroy(p.gameObject);
        else currentRealPlayers = new List<LobbyPlayerView>();
        currentRealPlayers.Clear();
        // -------------

        yield return StartCoroutine(ShowPlayersRoutine(players));

        if (real)
            titleView.SetReal();
        else
            titleView.SetFalse(realPlayers);

        if (real)
        {
            foreach (var p in currentPlayers)
                p.SetInfo(p.playerName + " +1000");
        }

        var score = 1000 * players.Count;

        /*foreach (var p in realPlayers)
        {
            var tempRealPlayer = Instantiate(realPlayerView, realPlayersParent);

            tempRealPlayer.SetInfo(p);
            tempRealPlayer.SetScore("+" + score);

            tempRealPlayer.gameObject.SetActive(true);

            currentRealPlayers.Add(tempRealPlayer);
        }*/

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowPlayersRoutine(List<string> players)
    {
        foreach(var p in players)
        {
            var tempPlayer = Instantiate(playerView, playersParent);
            tempPlayer.gameObject.SetActive(true);
            tempPlayer.SetInfo(p);

            currentPlayers.Add(tempPlayer);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
