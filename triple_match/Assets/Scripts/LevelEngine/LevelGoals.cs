using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGoals : MonoBehaviour
{
    public Dictionary<MatchingType, int> Goals { get; private set; }
    [SerializeField] UIGoalsPad UIGoalsPad;

    private static readonly object goalsShiftLock = new object();

    public void InitGoals(Dictionary<MatchingType, int> goals)
    {
        Goals = goals;
        UIGoalsPad.SetGoals(goals);
    }

    public bool UpdateAndTellIfThereIsAnythingLeft(MatchingType clickedType)
    {
        lock (goalsShiftLock)
        {
            if (Goals.ContainsKey(clickedType))
            {
                Goals[clickedType]--;
                if (Goals[clickedType] < 0)
                {
                    Goals[clickedType] = 0;
                }
                UIGoalsPad.UpdateGoal(clickedType, Goals[clickedType]);
            }
            return Goals.Aggregate(0, (sum, goal) => sum + goal.Value) > 0;
        }
    }

}
