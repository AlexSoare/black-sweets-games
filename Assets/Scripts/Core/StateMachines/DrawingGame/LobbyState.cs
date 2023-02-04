using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectedMsg
{
    public string playerName;
}

public class LobbyState : BaseState<DrawingGameStates, DrawingGamePrefs>
{
    private LobbyPanel panel;

    public LobbyState(LobbyPanel panel)
    {
        this.panel = panel;
    }

    public override void OnEnterState()
    {
        ServerAPI.AddWebSocketMessageCallback(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
        StateData.playersAvatarsPool = new List<Sprite>(SpritesHolder.PlayerAvatars);

        this.panel.Init(OnStartGame);
        this.panel.Show();
    }

    public override void OnExitState()
    {
        this.panel.Hide();
        ServerAPI.RemoveWebSocketMessageCallback(WebSocketMessageType.PlayerConnected, OnPlayerConnected);
    }

    private void OnPlayerConnected(string playerNameJSON)
    {
        var msgObj = JsonUtility.FromJson<PlayerConnectedMsg>(playerNameJSON);

        DrawingPlayer player;

        if (!StateData.FindPlayer(msgObj.playerName, out player))
        {
            var avatar = StateData.playersAvatarsPool[Random.Range(0, StateData.playersAvatarsPool.Count)];
            StateData.playersAvatarsPool.Remove(avatar);

            player = new DrawingPlayer(msgObj.playerName, avatar);
            StateData.Players.Add(player);

            panel.SetPlayerConnected(msgObj.playerName, avatar);
        }

        var msgParams = new List<WebSocketMessageParam>()
        {
            new WebSocketMessageParam("toPlayer",player.playerName),
            new WebSocketMessageParam("state","Lobby"),
        };

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, msgParams);
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
