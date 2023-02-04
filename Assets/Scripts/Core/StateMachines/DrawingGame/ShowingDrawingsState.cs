using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingDrawingsState : BaseState<DrawingGameStates, DrawingGamePrefs>
{
    private class PlayerDataMessage
    {
        public string playerName;
        public string title;
    }
    private class PlayerDrawingToShowMsg
    {
        public string playerName;
        public bool last;
    }

    private ShowingDrawingsPanel panel;

    private float timer;
    private float startingTimer;

    public ShowingDrawingsState(ShowingDrawingsPanel panel)
    {
        this.panel = panel;
        timer = 10;
        startingTimer = 2;
    }

    public override void OnEnterState()
    {
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.RoomStateUpdate, OnPlayerDrawingToShow);

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("state", "ShowingDrawings")
        };
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);

        StateData.currentDrawingTitles = new List<DrawingTitle>();

        StateData.ResetPlayers();

        foreach (var p in StateData.Players)
        {
            msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", p.playerName),
                new WebSocketMessageParam("ready", "False"),
                new WebSocketMessageParam("ownDrawingShown", "False")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }

        panel.Show();
    }

    private void OnPlayerDataReceived(string playerDataJSON)
    {
        var msgObj = JsonUtility.FromJson<PlayerDataMessage>(playerDataJSON);

        DrawingPlayer player;

        if (StateData.FindPlayer(msgObj.playerName, out player))
        {
            if (player.ready)
                return;

            player.currentTitleWritten = msgObj.title;
            player.ready = true;

            var titleAlreadyAdded = false;

            foreach(var t in StateData.currentDrawingTitles)
            {
                if(t.title == msgObj.title)
                {
                    titleAlreadyAdded = true;
                    t.playerNames.Add(player.playerName);
                }
            }

            if(!titleAlreadyAdded)
                StateData.currentDrawingTitles.Add(new DrawingTitle(msgObj.title, player.playerName));

            var msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", player.playerName),
                new WebSocketMessageParam("ready", "True")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }
    }
    private void OnPlayerDrawingToShow(string playerNameJSON)
    {
        var msgObj = JsonUtility.FromJson<PlayerDrawingToShowMsg>(playerNameJSON);

        DrawingPlayer player;
        if (StateData.FindPlayer(msgObj.playerName, out player))
        {
            panel.SetCurrentDrawing(player.currentDrawing);
   
            player.currentTitleWritten = player.currentTitleToDraw;
            player.ownDrawingShown = true;
            player.ready = true;

            StateData.currentDrawingPlayer = player;
            StateData.lastDrawing = msgObj.last;

            var msgParams = new List<WebSocketMessageParam>()
            {
                new WebSocketMessageParam("toPlayer", player.playerName),
                new WebSocketMessageParam("ownDrawingShown", "True")
            };
            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.RoomStateUpdate, OnPlayerDrawingToShow);

        panel.Hide();
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;
        

        if (StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;

            panel.SetTimer("Starting in :" + (int)startingTimer);

            if(startingTimer<=0)
                ChangeState(DrawingGameStates.ShowingTitles);
        }else
        {
            panel.SetTimer("Time left :" + (int)timer);
        }
    }
}
