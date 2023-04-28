using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingDrawingsState : BaseState<DrawingGameStates, DrawingGameStateData>
{
    private ShowingDrawingsPanel panel;

    private float timer;
    private float startingTimer;

    public ShowingDrawingsState(ShowingDrawingsPanel panel)
    {
        this.panel = panel;

        timer = 20;
        startingTimer = 2;
    }

    public override void OnEnterState()
    {
        ServerAPI.AddWebSocketMessageCallback<PlayerTitleMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        StateData.SetState(DrawingGameStates.ShowingDrawings.ToString());

        if (!StateData.DrawingsInited)
            StateData.Drawings = new List<Drawing>();

        StateData.CurrentTitles = new List<Title>();
        StateData.ResetPlayersState();

        foreach (var p in StateData.Players)
        {
            p.OwnDrawingShown = false;
            if (!StateData.DrawingsInited)
                StateData.Drawings.Add(p.Drawing);
        }

        StateData.DrawingsInited = true;

        var currentDrawing = DrawingGame.GetNextDrawingToShow(StateData.Drawings);

        if(currentDrawing != null)
        {
            var p = StateData.GetPlayer(currentDrawing.Uid);
            p.OwnDrawingShown = true;
            p.Ready = true;

            StateData.CurrentDrawing = currentDrawing;
            StateData.CurrentTitles.Add(currentDrawing.Title);

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);

            panel.Show();
            panel.SetCurrentDrawing(DrawingGame.GetDrawingSprite(currentDrawing.Base64Texture));
        }
    }

    private void OnPlayerDataReceived(PlayerTitleMsg playerTitle)
    {
        Player tempPlayer;
        if (StateData.PlayerInRoom(playerTitle.Uid, out tempPlayer))
        {
            playerTitle.Title = playerTitle.Title.ToUpper();

            Title title;

            if(StateData.TitleAlreadyWritten(playerTitle.Title, out title))
            {
                title.WrittedByUid.Add(playerTitle.Uid);
            }else
            {
                title = new Title(playerTitle.Title, playerTitle.Uid);
            }

            StateData.CurrentTitles.Add(title);
            tempPlayer.Ready = true;

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
        }
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback<PlayerTitleMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        panel.Hide();
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        if (StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;

            panel.SetTimer("Rezultate in :" + (int)startingTimer);

            if (startingTimer <= 0)
                ChangeState(DrawingGameStates.ShowingTitles);
        }
        else
        {
            panel.SetTimer("Timp ramas :" + (int)timer);
        }
    }
}
