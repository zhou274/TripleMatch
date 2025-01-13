using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIGoalsPad : MonoBehaviour
{
    public SortedDictionary<MatchingType, UIGoal> Goals { get; private set; }
    [SerializeField] UIGoalsPadCell[] Cells;

    [SerializeField] AnimationController animationController;
    private Queue<Sequence> AnimationQueue = new Queue<Sequence>();

    private static readonly object cellShiftLock = new object();

    public void SetGoals(Dictionary<MatchingType, int> levelGoals)
    {
        Goals = new SortedDictionary<MatchingType, UIGoal>() { };
        int cellIndex = 0;
        foreach (var goal in levelGoals)
        {
            Goals.Add(goal.Key, null);

            if (++cellIndex > Cells.Length)
            {
                Debug.LogWarning($"Too much goals. Only {Cells.Length} were assigned");
                break;
            }
        }
        lock (cellShiftLock)
        {

            cellIndex = 0;
            var goalKeys = new List<MatchingType>(Goals.Keys);
            foreach (var goal in goalKeys)
            {
                var cell = Cells[cellIndex++];
                UIGoal uiGoal = animationController.CreateUIGoal(cell.rectTransform, goal, levelGoals[goal]);
                Goals[goal] = uiGoal;
                cell.type = goal;
                cell.goal = uiGoal;
            }
        }

        // here you need to start an animation to show all the goals
        Sequence seq = DOTween.Sequence();
        seq.Pause();
        seq.Append(animationController.ShowUIGoals(Goals.Values.ToList()));
        QueueAnimationSequence(seq);
    }

    public void UpdateGoal(MatchingType type, int value)
    {
        lock (cellShiftLock)
        {
            if (Goals[type] is UIGoal goalToUpdate)
            {
                goalToUpdate.SetValue(value);

                if (value <= 0 && !goalToUpdate.isHidden)       // goal isn't hidden but should be, so let's hide it
                {
                    HideCellAndShiftLeft(goalToUpdate);
                }
                else if (value > 0 && goalToUpdate.isHidden)    // goal is hidden but shouldn't be, so let's show it
                {
                    ShowCellAndShiftRight(goalToUpdate);
                }
            }
        }
    }

    private void HideCellAndShiftLeft(UIGoal goalToHide)
    {
        lock (cellShiftLock)
        {
            int cellIndex = 0;

            Sequence seq = DOTween.Sequence();
            seq.Pause();
            goalToHide.isHidden = true;

            seq.Append(animationController.DissapearGoal(goalToHide));
            seq.AppendInterval(0);

            foreach (var (type, goal) in Goals)
            {
                if (!goal.isHidden)
                {
                    if (Cells[cellIndex].goal != goal)
                    {
                        Cells[cellIndex].goal = goal;
                        Cells[cellIndex].type = type;
                        seq.Join(animationController.MoveGoal(goal, Cells[cellIndex]));
                    }
                    cellIndex++;
                    if (cellIndex > Cells.Length) break; // it is impossible by design, but just in case
                }
            }

            QueueAnimationSequence(seq);

        }
    }

    // TODO when 'ctrl+Z' booster is in the game
    private void ShowCellAndShiftRight(UIGoal goalToShow)
    {

    }

    private void QueueAnimationSequence(Sequence seq)
    {
        seq.OnComplete(OnAnimationSequenceComplete);
        AnimationQueue.Enqueue(seq);
        if (AnimationQueue.Count == 1)
        {
            AnimationQueue.Peek().Play();
        }
    }

    private void OnAnimationSequenceComplete()
    {
        AnimationQueue.Dequeue();
        if (AnimationQueue.Count > 0)
        {
            AnimationQueue.Peek().Play();
        }
    }
}
