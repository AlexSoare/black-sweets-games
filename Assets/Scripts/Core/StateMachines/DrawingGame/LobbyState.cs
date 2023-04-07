using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyState : BaseState<DrawingGameStates, DrawingGameStateData>
{
    private LobbyPanel panel;

    public LobbyState(LobbyPanel panel)
    {
        this.panel = panel;
    }

    public override void OnEnterState()
    {
        StateData.Clear();
        StateData.State = DrawingGameStates.Lobby.ToString();

        ServerAPI.AddWebSocketMessageCallback<PlayerConnectedMsg>(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
        ServerAPI.AddWebSocketMessageCallback<PlayerDrawingMsg>(WebSocketMessageType.PlayerInputUpdate, OnPlayerAvatarDrawing);

        this.panel.Init(OnStartGame);
        this.panel.Show();
    }

    public override void OnExitState()
    {
        this.panel.Hide();
        ServerAPI.RemoveWebSocketMessageCallback<PlayerConnectedMsg>(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
    }

    private void OnPlayerConnected(PlayerConnectedMsg newPlayer)
    {
        Player tempPlayer;

        if(!StateData.PlayerInRoom(newPlayer.Uid,out tempPlayer))
        {
            tempPlayer = new Player(newPlayer.Uid, newPlayer.Name);
            tempPlayer.State = DrawingGameStates.Lobby.ToString();

            StateData.AddPlayer(tempPlayer);
            panel.SetPlayerConnected(tempPlayer);
        }

        // type, roomState, playersState
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
    }

    public void OnPlayerAvatarDrawing(PlayerDrawingMsg playerDrawing)
    {
        Player tempPlayer;

        if (StateData.PlayerInRoom(playerDrawing.Uid, out tempPlayer))
        {
            tempPlayer.Avatar = new Drawing()
            {
                Uid = tempPlayer.Uid,
                Base64Texture = playerDrawing.DrawingBase64,
            };

            tempPlayer.Ready = true;

            ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);
        }
    }

    private void OnStartGame()
    {
        //CoroutineHelper.Start(BackgroundManager.Instance.UpdateGridRow(0.01f, 1f));
        ChangeState(DrawingGameStates.WaitingForDrawings);
    }

    public override void StateUpdate()
    {

    }
}
