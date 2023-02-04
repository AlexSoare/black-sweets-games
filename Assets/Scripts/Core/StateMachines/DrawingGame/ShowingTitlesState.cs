using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowingTitlesState : BaseState<DrawingGameStates, DrawingGamePrefs>
{
    private class PlayerDataMessage
    {
        public string playerName;
        public string title;
    }

    private ShowingTitlesPanel panel;

    private float timer;
    private float startingTimer;

    public ShowingTitlesState(ShowingTitlesPanel panel)
    {
        this.panel = panel;
        timer = 30;
        startingTimer = 5;
    }

    public override void OnEnterState()
    {
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("state", "ShowingTitles")
        };
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);

        StateData.ResetPlayers();
        StateData.currentDrawingPlayer.ready = true;

        foreach (var p in StateData.Players)
        {
            msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", p.playerName),
                new WebSocketMessageParam("ready", "False")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }


        var titlesList = new List<string>();
        foreach(var p in StateData.Players)
        {
            titlesList.Add(p.currentTitleWritten);
            /*if (!p.ownDrawingShown)
            {
                titlesList.Add(p.currentTitleWritten);
            }*/
        }

        titlesList = titlesList.OrderBy(i => Guid.NewGuid()).ToList();

        panel.Init(titlesList, StateData.currentDrawingPlayer.currentDrawing);
        panel.Show();
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);

        panel.Hide();
    }

    private void OnPlayerDataReceived(string playerDataJSON)
    {
        var msgObj = JsonUtility.FromJson<PlayerDataMessage>(playerDataJSON);

        DrawingPlayer player;

        if (StateData.FindPlayer(msgObj.playerName, out player))
        {
            if (player.ready)
                return;

            player.currentChosenTitle = msgObj.title;
            player.ready = true;

            var msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", player.playerName),
                new WebSocketMessageParam("ready", "True")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        if(StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;
            panel.SetTimer("Starting in: " + (int)startingTimer);

            if (startingTimer <= 0)
                ChangeState(DrawingGameStates.ShowingRoundScore);
        }
        else
        {
            panel.SetTimer("Time left: " + (int)timer);
        }
    }
}
