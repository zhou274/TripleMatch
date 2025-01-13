using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour
{
    [SerializeField] UI UI;
    [SerializeField] UICurtain UICurtain;
    [SerializeField] EndLevelScreen EndLevelScreen;
    [SerializeField] GameObject FlyerPrefab;
    [SerializeField] GameObject UIGoalPrefab;
    [SerializeField] SpriteLibrary SpriteLibrary;

    public MatchingFlyingIcon TransitFromObjectToUI(MatchingObject spawner)
    {
        GameObject flyerGO = ObjectPooler.Create(FlyerPrefab, Vector3.zero, transform.rotation);

        MatchingFlyingIcon flyerComp = flyerGO.GetComponent<MatchingFlyingIcon>();
        Image flyerImage = flyerComp.Icon;

        if (flyerImage == null || flyerComp == null)
        {
            throw new MissingFieldException("Required component are missing.");
        }

        ApplyStartingDisposition(flyerGO.GetComponent<RectTransform>(), spawner.transform);
        flyerComp.Prepare(spawner.type, SpriteLibrary.GetSpriteByMatchingType(spawner.type));

        return flyerComp;
    }

    internal void ApplyStartingDisposition(RectTransform flyingObject, Transform spawner)
    {
        flyingObject.transform.SetParent(UI.transform);

        Vector2 pos = Camera.main.WorldToViewportPoint(spawner.transform.position);

        flyingObject.pivot = Vector2.one / 2;
        flyingObject.anchorMin = Vector2.one / 2;
        flyingObject.anchorMax = Vector2.one / 2;

        var UIRT = UI.GetComponent<RectTransform>();

        flyingObject.anchoredPosition = new Vector2((pos.x - 0.5f) * UIRT.sizeDelta.x, (pos.y - 0.5f) * UIRT.sizeDelta.y);



        /*Vector2 pos = Camera.main.WorldToScreenPoint(spawner.transform.position);

        flyingObject.pivot = Vector2.one / 2;

        flyingObject.anchorMin = Vector2.zero;
        flyingObject.anchorMax = Vector2.zero;
        flyingObject.anchoredPosition = pos;

        pos = flyingObject.localPosition;

        flyingObject.anchorMin = Vector2.one / 2;
        flyingObject.anchorMax = Vector2.one / 2;
        flyingObject.localPosition = pos;*/
    }

    public void MatchThree(Image left, Image mid, Image right, Action callback)
    {
        float distanceToTheGrave = mid.rectTransform.position.x - left.rectTransform.position.x;
        Sequence animationSequence = DOTween.Sequence();

        animationSequence.Append(left.rectTransform.DOAnchorPosX(-distanceToTheGrave * 0.25f, AnimationSettings.FastTime).SetEase(Ease.InOutSine));
        animationSequence.Join(right.rectTransform.DOAnchorPosX(distanceToTheGrave * 0.25f, AnimationSettings.FastTime).SetEase(Ease.InOutSine));
        animationSequence.Append(mid.rectTransform.DOScale(AnimationSettings.FlyingIconScaleCoefficient,
                                                                AnimationSettings.MediumTime));
        animationSequence.Append(left.rectTransform.DOAnchorPosX(distanceToTheGrave * 1.25f, AnimationSettings.FastTime).SetEase(Ease.InOutSine));
        animationSequence.Join(right.rectTransform.DOAnchorPosX(-distanceToTheGrave * 1.25f, AnimationSettings.FastTime).SetEase(Ease.InOutSine));

        animationSequence.OnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    public Tween DissapearGoal(UIGoal goal)
    {
        return DOTween.Sequence().AppendInterval(AnimationSettings.SnailSlowTime)
                                 .Append(goal.rectTransform.DOScale(0, AnimationSettings.SlowTime))
                                 .AppendCallback(() => ObjectPooler.Return(goal.gameObject));
    }

    public Tween MoveGoal(UIGoal goal, UIGoalsPadCell newHome)
    {
        return DOTween.Sequence().AppendCallback(() => goal.gameObject.transform.SetParent(newHome.transform))
                                 .Append(goal.rectTransform.DOAnchorPos(Vector2.zero, AnimationSettings.FastTime));
    }

    public UIGoal CreateUIGoal(RectTransform parentCell, MatchingType type, int number)
    {
        GameObject uiGoalGO = ObjectPooler.Create(UIGoalPrefab, parentCell.position, Quaternion.Euler(0, 90, 0));

        if (uiGoalGO != null && uiGoalGO.GetComponent<UIGoal>() is UIGoal uiGoal)
        {
            uiGoal.AssignSprite(SpriteLibrary.GetSpriteByMatchingType(type));
            uiGoal.SetValue(number);
            uiGoal.transform.SetParent(parentCell.transform);
            uiGoal.rectTransform.localScale = Vector3.one;
            uiGoal.rectTransform.anchoredPosition = Vector2.zero;
            uiGoal.isHidden = false;
            return uiGoal;
        }
        return null;
    }

    public Tween ShowUIGoals(List<UIGoal> goals)
    {
        Sequence revealingAnimation = DOTween.Sequence();
        foreach (var goal in goals)
        {
            revealingAnimation.Join(goal.transform.DORotate(new Vector3(0, 0, 0), AnimationSettings.FastTime));
        }
        return revealingAnimation;
    }

    public void LevelStartedAnimation(Action endCallback = null)
    {
        EndLevelScreen.Deconstruct();
        UICurtain.FadeOut(AnimationSettings.MediumTime);
    }

    public void LevelFailedAnimation(Action endCallback = null)
    {
        UICurtain.FadeIn(AnimationSettings.MediumTime).OnComplete(() =>
        {
            EndLevelScreen.InitLoseScreen();
            endCallback?.Invoke();
        });
    }

    public void LevelBeatenAnimation(float timeLeft, Action endCallback = null)
    {
        UICurtain.FadeIn(AnimationSettings.MediumTime).OnComplete(() =>
        {
            EndLevelScreen.InitWinScreen(Utils.TimeToText(timeLeft));
            endCallback?.Invoke();
        });
    }

}
