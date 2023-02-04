using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum DrawingGameStates
{
    Lobby,
    WaitingForDrawings,
    ShowingDrawings,
    ShowingTitles,
    ShowingRoundScore,
    ShowingTotalScore,
}

public class DrawingPlayer
{
    public string playerName;
    public bool ready;
    public Sprite avatar;
    public Sprite currentDrawing;
    public string currentTitleToDraw;
    public string currentTitleWritten;
    public string currentChosenTitle;
    public bool ownDrawingShown;
    public int score;

    public DrawingPlayer(string name, Sprite avatar)
    {
        playerName = name;
        this.avatar = avatar;
    }
}
public class DrawingTitle
{
    public string title;
    public List<string> playerNames;
    public List<string> chosenBy;

    public DrawingTitle(string title, string playerName)
    {
        this.title = title;
        playerNames = new List<string>();
        playerNames.Add(playerName);
    }

}
public class DrawingGamePrefs
{
    public List<DrawingPlayer> Players;
    public List<Sprite> playersAvatarsPool;
    public DrawingPlayer currentDrawingPlayer;

    public bool lastDrawing;

    public List<DrawingTitle> currentDrawingTitles;
    public List<string> currentRoundWinners;

    public DrawingGamePrefs()
    {
        Players = new List<DrawingPlayer>();
    }

    public void ResetPlayers()
    {
        foreach (var p in Players)
            p.ready = false;
    }
    public void ResetPlayersScore()
    {
        foreach (var p in Players)
            p.score = 0;
    }

    public bool AllPlayersReady()
    {
        foreach (var p in Players)
            if (!p.ready)
                return false;
        return true;
    }

    public bool FindPlayer(string playerName, out DrawingPlayer player)
    {
        foreach (var p in Players)
            if (p.playerName == playerName)
            {
                player = p;
                return true;
            }

        player = null;
        return false;
    }

}

public class DrawingGame : StateMachineBase<DrawingGameStates, DrawingGamePrefs>
{
    [SerializeField] private LobbyPanel lobbyPanel;
    [SerializeField] private WaitingForDrawingsPanel waitingForDrawingsPanel;
    [SerializeField] private ShowingDrawingsPanel showingDrawingPanel;
    [SerializeField] private ShowingTitlesPanel showingTitlesPanel;
    [SerializeField] private ShowingRoundScorePanel showingRoundScorePanel;
    [SerializeField] private ShowingTotalScorePanel showingTotalScorePanel;

    private void Start()
    {
        InitStates();

        ChangeState(DrawingGameStates.Lobby);
    }

    private void InitStates()
    {
        AddState(DrawingGameStates.Lobby, new LobbyState(lobbyPanel));
        AddState(DrawingGameStates.WaitingForDrawings, new WaitingForDrawingsState(waitingForDrawingsPanel));
        AddState(DrawingGameStates.ShowingDrawings, new ShowingDrawingsState(showingDrawingPanel));
        AddState(DrawingGameStates.ShowingTitles, new ShowingTitlesState(showingTitlesPanel));
        AddState(DrawingGameStates.ShowingRoundScore, new ShowingRoundScoreState(showingRoundScorePanel));
        AddState(DrawingGameStates.ShowingTotalScore, new ShowingTotalScoreState(showingTotalScorePanel));
    }
}
