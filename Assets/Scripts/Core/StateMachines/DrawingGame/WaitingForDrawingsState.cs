using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Convert = System.Convert;
using MemoryStream = System.IO.MemoryStream;

public class WaitingForDrawingsState : BaseState<DrawingGameStates, DrawingGameStateData>
{
    private class PlayerDrawing
    {
        public string playerName;
        public string titleToDraw;
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
        StateData.ResetPlayers();

        ServerAPI.AddWebSocketMessageCallback<PlayerDrawingMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        // Init drawings
        var drawings = DrawingGame.GenerateTitlesToDraw(StateData.Players.Count);

        for (int i = 0; i < StateData.Players.Count; i++)
        {
            StateData.Players[i].State = DrawingGameStates.WaitingForDrawings.ToString();
            StateData.Players[i].TitleToDraw = new Title(drawings[i], StateData.Players[i].Uid);
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
            };

            tempPlayer.Ready = true;

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);

            byte[] imageBytes = Convert.FromBase64String(playerDrawing.DrawingBase64);

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

            panel.PlayerSentHisDrawing(playerDrawing.Uid, sprite);

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        //if (StateData.AllPlayersReady())
        //{
        //    startingTimer -= Time.deltaTime;

        //    panel.SetTimer("Starting in :" + (int)startingTimer);

        //    if (startingTimer <= 0)
        //        ChangeState(DrawingGameStates.ShowingDrawings);
        //}
        //else
        //{
        //    panel.SetTimer("Time left :" + (int)timer);
        //}
    }
}
