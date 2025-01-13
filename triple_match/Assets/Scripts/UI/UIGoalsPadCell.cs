using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoalsPadCell : MonoBehaviour
{
    public RectTransform rectTransform;
    public MatchingType type;
    public UIGoal goal;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
