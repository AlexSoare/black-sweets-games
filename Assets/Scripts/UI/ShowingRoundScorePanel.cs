using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ShowingRoundScorePanel : MonoBehaviour
{
    [SerializeField] private Image drawing;
    [SerializeField] private TitleView titleView;

    [SerializeField] private Transform playersParent;
    [SerializeField] private LobbyPlayerView playerView;

    [SerializeField] private Transform realPlayersParent;
    [SerializeField] private LobbyPlayerView realPlayerView;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject writtenByBkg;

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

    public IEnumerator SetTitle(Title title, List<Player> players, List<Player> realPlayers, bool real)
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

        writtenByBkg.gameObject.SetActive(false);
        // -------------

        titleText.text = "ALES DE ...";
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ShowPlayersRoutine(players));

      
        if (real)
        {
            titleText.text = "TITLUL REAL";
            titleView.SetReal();

            foreach (var p in currentPlayers)
                p.SetScore("+1000");
        }
        else
        {
            writtenByBkg.gameObject.SetActive(true);

            titleText.text = "SCRIS DE ...";
            titleView.SetFalse();

            var score = 1000 * players.Count;

            foreach (var p in realPlayers)
            {
                var tempRealPlayer = Instantiate(realPlayerView, realPlayersParent);

                tempRealPlayer.Init(p);
                tempRealPlayer.SetScore("+" + score);

                tempRealPlayer.gameObject.SetActive(true);

                currentRealPlayers.Add(tempRealPlayer);
            }
        }

        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator ShowPlayersRoutine(List<Player> players)
    {
        foreach(var p in players)
        {
            var tempPlayer = Instantiate(playerView, playersParent);
            tempPlayer.gameObject.SetActive(true);
            tempPlayer.Init(p);

            currentPlayers.Add(tempPlayer);

            yield return new WaitForSeconds(1.5f);
        }
    }
}
