using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingRoundScoreState : BaseState<DrawingGameStates, DrawingGameStateData>
{
    private ShowingRoundScorePanel panel;

    public ShowingRoundScoreState(ShowingRoundScorePanel panel)
    {
        this.panel = panel;
    }

    bool done;

    public override void OnEnterState()
    {
        done = false;

        StateData.SetState(DrawingGameStates.ShowingRoundScore.ToString());
        StateData.ResetPlayersState();

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
        panel.SetDrawing(DrawingGame.GetDrawingSprite(StateData.CurrentDrawing.Base64Texture));

        CoroutineHelper.Start(ShowTitlesRoutine());
    }

    private IEnumerator ShowTitlesRoutine()
    {
        StateData.CurrentTitles.Sort((t1, t2) => t2.ChosenByUid.Count.CompareTo(t1.ChosenByUid.Count));

        var chosedBy = new List<Player>();
        var writtenBy = new List<Player>();

        foreach (var title in StateData.CurrentTitles)
        {
            if (title.Original || title.ChosenByUid.Count == 0)
                continue;

            chosedBy = StateData.GetPlayers(title.ChosenByUid);
            writtenBy = StateData.GetPlayers(title.WrittedByUid);

            foreach (var p in writtenBy)
                p.Score += (chosedBy.Count * 1000);

            yield return CoroutineHelper.Start(panel.SetTitle(title, chosedBy, writtenBy, false));
        }

        var originalTitle = StateData.CurrentTitles.Find(t => t.Original);

        chosedBy = StateData.GetPlayers(originalTitle.ChosenByUid);
        var originalPlayer = StateData.GetPlayer(originalTitle.WrittedByUid[0]);

        foreach (var p in chosedBy)
            p.Score += 1000;

        originalPlayer.Score += (chosedBy.Count * 1000);

        yield return CoroutineHelper.Start(panel.SetTitle(originalTitle, chosedBy,new List<Player> { originalPlayer }, true));

        yield return new WaitForSeconds(2);

        done = true;
    }

    public override void OnExitState()
    {
        panel.Hide();
    }

    public override void StateUpdate()
    {
        if (done)
            ChangeState(DrawingGameStates.ShowingTotalScore);
    }
}
