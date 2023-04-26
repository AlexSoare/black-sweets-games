using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Convert = System.Convert;
using MemoryStream = System.IO.MemoryStream;

public class WaitingForDrawingsState : BaseState<DrawingGameStates, DrawingGameStateData>
{

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
        StateData.ResetPlayers();
        StateData.SetState(DrawingGameStates.WaitingForDrawings.ToString());

        ServerAPI.AddWebSocketMessageCallback<PlayerDrawingMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        // Init drawings
        var drawings = DrawingGame.GenerateTitlesToDraw(StateData.Players.Count);

        for (int i = 0; i < StateData.Players.Count; i++)
        {
            StateData.Players[i].TitleToDraw = new Title(drawings[i], StateData.Players[i].Uid, true);
        }

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
        //

        timer = 5;

        panel.Init(StateData.Players);
        panel.Show();
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback<PlayerDrawingMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        panel.Hide();
    }

    private void OnPlayerDataReceived(PlayerDrawingMsg playerDrawing)
    {
        Player tempPlayer;

        if (StateData.PlayerInRoom(playerDrawing.Uid, out tempPlayer))
        {
            tempPlayer.Drawing = new Drawing()
            {
                Uid = tempPlayer.Uid,
                Base64Texture = playerDrawing.DrawingBase64,
                Title = tempPlayer.TitleToDraw,
            };

            tempPlayer.Ready = true;

            var drawingSprite = DrawingGame.GetDrawingSprite(playerDrawing.DrawingBase64);

            panel.SetPlayerDone(tempPlayer);
            panel.SetPlayerDrawing(tempPlayer, DrawingGame.GetDrawingSprite(tempPlayer.Avatar.Base64Texture));

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        if (StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;
            panel.SetTimer("Incepem in:" + (int)startingTimer);

            if (startingTimer <= 0)
            {
                ChangeState(DrawingGameStates.ShowingDrawings);
            }

        }
        else
        {
            panel.SetTimer("Timp ramas:" + (int)timer);
        }
    }
}
