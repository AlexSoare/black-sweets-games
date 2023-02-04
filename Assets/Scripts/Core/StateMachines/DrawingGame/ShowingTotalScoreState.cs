using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingTotalScoreState : BaseState<DrawingGameStates, DrawingGamePrefs>
{
    private ShowingTotalScorePanel panel;

    public ShowingTotalScoreState(ShowingTotalScorePanel panel)
    {
        this.panel = panel;
    }

    bool done; 

    public override void OnEnterState()
    {
        var scoring = new List<string>();

        StateData.Players.Sort((p1, p2) => { return p2.score.CompareTo(p1.score); });

        foreach(var p in StateData.Players)
        {
            scoring.Add(p.playerName + ": " + p.score);
        }

        panel.Init(scoring, OnRestart);
        panel.Show();

        CoroutineHelper.Start(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(5);
        if (!StateData.lastDrawing)
            ChangeState(DrawingGameStates.ShowingDrawings);
        else
            panel.ShowEndText();
    }
    public override void OnExitState()
    {
        panel.Hide();
    }

    public override void StateUpdate()
    {
        
    }

    private void OnRestart()
    {
        StateData.ResetPlayers();
        StateData.ResetPlayersScore();

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("state", "GameReset")
        };

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);

        foreach (var p in StateData.Players)
        {
            msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", p.playerName),
                new WebSocketMessageParam("ready", "False")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }

        ChangeState(DrawingGameStates.WaitingForDrawings);
    }
}
