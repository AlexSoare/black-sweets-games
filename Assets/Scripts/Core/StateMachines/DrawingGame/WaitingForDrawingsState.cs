using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Convert = System.Convert;
using MemoryStream = System.IO.MemoryStream;
//...

public class WaitingForDrawingsState : BaseState<DrawingGameStates, DrawingGamePrefs>
{
    private class PlayerDrawing
    {
        public string playerName;
        public string titleToDraw;
    }

    private class PlayerDataMessage
    {
        public string playerName;
        public string drawingBase64;
    }

    private WaitingForDrawingsPanel panel;

    private float timer;
    private float startingTimer;

    public WaitingForDrawingsState(WaitingForDrawingsPanel panel)
    {
        this.panel = panel;
        startingTimer = 5;
    }

    public override void OnEnterState()
    {
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.RoomStateUpdate, OnPlayersTitles);

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("state","WaitingForDrawings"),
        };
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);

        timer = 5;

        List<string> playerNames = new List<string>();
        foreach (var p in StateData.Players)
            playerNames.Add(p.playerName);

        panel.Init(playerNames);

        panel.Show();
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.PlayerData, OnPlayerDataReceived);
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.RoomStateUpdate, OnPlayersTitles);

        panel.Hide();
    }

    private void OnPlayersTitles(string msg)
    {
        var msgObj = JsonConvert.DeserializeObject<List<PlayerDrawing>>(msg);

        foreach (var p in msgObj)
        {
            DrawingPlayer player;
            if (StateData.FindPlayer(p.playerName, out player))
                player.currentTitleToDraw = p.titleToDraw;
        }
    }

    private void OnPlayerDataReceived(string msg)
    {
        var msgObj = JsonUtility.FromJson<PlayerDataMessage>(msg);

        DrawingPlayer player;

        if (!StateData.FindPlayer(msgObj.playerName, out player) || player.ready)
            return;

        byte[] imageBytes = Convert.FromBase64String(msgObj.drawingBase64);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);

        // Remove white bkg
        /*Color[] pixels = tex.GetPixels(0, 0, tex.width, tex.height, 0);
        for (int p = 0; p < pixels.Length; p++)
        {
            if (pixels[p] == Color.white)
                pixels[p] = new Color(0, 0, 0, 0);
        }
        tex.SetPixels(0, 0, tex.width, tex.height, pixels, 0);
        tex.Apply();*/
        // ----

        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        player.ready = true;
        player.currentDrawing = sprite;

        Debug.LogError(player.currentDrawing);

        panel.PlayerSentHisDrawing(msgObj.playerName, sprite);

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("toPlayer",player.playerName),
            new WebSocketMessageParam("ready","True"),
        };
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);



    }
    private void OnPlayerConnected(string playerName)
    {
        var msgObj = JsonUtility.FromJson<PlayerConnectedMsg>(playerName);

        DrawingPlayer player;
        if (StateData.FindPlayer(msgObj.playerName, out player))
        {
            var msgParams = new List<WebSocketMessageParam>()
                {
                    new WebSocketMessageParam("toPlayer",msgObj.playerName),
                    new WebSocketMessageParam("state","WaitingForDrawings"),
                    new WebSocketMessageParam("ready",player.ready.ToString()),
                };

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        if (StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;

            panel.SetTimer("Starting in :" + (int)startingTimer);

            if (startingTimer <= 0)
                ChangeState(DrawingGameStates.ShowingDrawings);
        }
        else
        {
            panel.SetTimer("Time left :" + (int)timer);
        }
    }
}
