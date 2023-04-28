using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingTotalScoreState : BaseState<DrawingGameStates, DrawingGameStateData>
{
    private ShowingTotalScorePanel panel;

    public ShowingTotalScoreState(ShowingTotalScorePanel panel)
    {
        this.panel = panel;
    }

    bool done;

    public override void OnEnterState()
    {
        StateData.SetState(DrawingGameStates.ShowingTotalScore.ToString());

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);

        StateData.Players.Sort((p1, p2) => { return p2.Score.CompareTo(p1.Score); });

        panel.Init(StateData.Players, OnRestart);
        panel.Show();

        CoroutineHelper.Start(Next());
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(5);
        StateData.Drawings.Remove(StateData.CurrentDrawing);

        if (StateData.Drawings.Count > 0)
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
        StateData.ResetRound();
        StateData.SetState(DrawingGameStates.WaitingForDrawings.ToString());

        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, StateData, StateData.Players);

        ChangeState(DrawingGameStates.WaitingForDrawings);
    }
}
