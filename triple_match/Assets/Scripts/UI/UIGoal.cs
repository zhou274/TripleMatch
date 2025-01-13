using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGoal : UIFlyingIcon
{
    public TMP_Text counterText;
    public Image achieved;
    public bool isHidden;

    public void SetValue(int value)
    {
        if (value > 0)
        {
            HideAchievement();
            counterText.text = value.ToString();
        }
        else
        {
            counterText.text = "";
            ShowAchievement();
        }
    }

    private void HideAchievement()
    {
        if (achieved.color.a != 0f)
        {
            achieved.color = new Color(achieved.color.r, achieved.color.g, achieved.color.b, 0);
        }
    }

    private void ShowAchievement()
    {
        if (achieved.color.a < 1f)
        {
            achieved.color = new Color(achieved.color.r, achieved.color.g, achieved.color.b, 1);
        }
    }
    override public void ResetAndReturn()
    {
        achieved.color = new Color(achieved.color.r, achieved.color.g, achieved.color.b, 0);
        counterText.text = "";
        rectTransform.localScale = Vector3.one;
        base.ResetAndReturn();
    }
}
