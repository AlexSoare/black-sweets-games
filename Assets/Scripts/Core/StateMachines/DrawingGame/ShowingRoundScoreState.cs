using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingRoundScoreState : BaseState<DrawingGameStates, DrawingGamePrefs>
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

        StateData.currentRoundWinners = new List<string>();

        foreach (var title in StateData.currentDrawingTitles)
        {
            title.chosenBy = new List<string>();

            foreach (var p in StateData.Players)
            {
                if (p.currentChosenTitle == title.title)
                {
                    title.chosenBy.Add(p.playerName);
                }
            }
        }

        foreach (var p in StateData.Players)
        {
            if (p.currentChosenTitle == StateData.currentDrawingPlayer.currentTitleToDraw)
            {
                StateData.currentRoundWinners.Add(p.playerName);
                p.score += 1000;
            }
        }

        StateData.currentDrawingPlayer.score += 1000 * StateData.currentRoundWinners.Count;

        StateData.currentDrawingTitles.Sort((t1, t2) => { return t2.chosenBy.Count.CompareTo(t1.chosenBy.Count); });

        panel.SetDrawing(StateData.currentDrawingPlayer.currentDrawing);
        panel.Show();

        CoroutineHelper.Start(ShowTitlesRoutine());
    }

    private IEnumerator ShowTitlesRoutine()
    {
        foreach(var title in StateData.currentDrawingTitles)
        {
            if (title.chosenBy.Count == 0)
                continue;


            yield return CoroutineHelper.Start(panel.SetTitle(title.title,title.chosenBy,title.playerNames, false));
        }

        yield return CoroutineHelper.Start(panel.SetTitle(StateData.currentDrawingPlayer.currentTitleToDraw, StateData.currentRoundWinners, new List<string> { StateData.currentDrawingPlayer.playerName }, true));

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
