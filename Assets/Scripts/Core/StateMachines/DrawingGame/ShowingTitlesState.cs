using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowingTitlesState : BaseState<DrawingGameStates, DrawingGameStateData>
{
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
        ServerAPI.AddWebSocketMessageCallback<PlayerChosenTitleMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        StateData.SetState(DrawingGameStates.ShowingTitles.ToString());
        StateData.ResetPlayers();

        StateData.GetPlayer(StateData.CurrentDrawing.Uid).Ready = true;

        foreach (var p in StateData.Players)
        {
            p.TitlesToChooseFrom = new List<Title>();
            foreach (var t in StateData.CurrentTitles)
            {
                bool writtenByThisPlayer = false;
                foreach (var w in t.WrittedByUid)
                    if (p.Uid == w)
                    {
                        writtenByThisPlayer = true;
                        break;
                    }
                if (!writtenByThisPlayer)
                    p.TitlesToChooseFrom.Add(t);
            }
        }

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);

        panel.Show();
        panel.Init(StateData.CurrentTitles, DrawingGame.GetDrawingSprite(StateData.CurrentDrawing.Base64Texture));
    }

    public override void OnExitState()
    {
        ServerAPI.RemoveWebSocketMessageCallback<PlayerChosenTitleMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerDataReceived);

        panel.Hide();
    }

    private void OnPlayerDataReceived(PlayerChosenTitleMsg playerMsg)
    {
        Player tempPlayer;
        if (StateData.PlayerInRoom(playerMsg.Uid, out tempPlayer))
        {
            var title = StateData.GetTitle(playerMsg.ChosenTitleUid);
            if (title != null)
            {
                tempPlayer.ChosenTitle = title;
                tempPlayer.Ready = true;

                title.ChosenByUid.Add(tempPlayer.Uid);

                ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
            }
        }
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;

        if (StateData.AllPlayersReady())
        {
            startingTimer -= Time.deltaTime;
            panel.SetTimer("Incepem in: " + (int)startingTimer);

            if (startingTimer <= 0)
                ChangeState(DrawingGameStates.ShowingRoundScore);
        }
        else
        {
            panel.SetTimer("Timp ramas: " + (int)timer);
        }
    }
}
