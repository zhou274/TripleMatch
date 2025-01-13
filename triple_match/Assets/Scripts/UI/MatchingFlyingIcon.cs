using System;
using UnityEngine;
using UnityEngine.Events;

public class MatchingFlyingIcon : UIFlyingIcon
{
    public MatchingType type;

    private bool isHatching = false;
    public bool isFlying { get; private set; } = false;
    public bool isDying { get; private set; } = false;

    private Vector2 destination;

    public MatchingPadCell motherCell;

    // not sure if needed
    private Action finalCallback;
    public UnityEvent LandingEvent = new UnityEvent();
    public UnityEvent DyingEvent = new UnityEvent();

    public void Prepare(MatchingType type, Sprite sprite)
    {
        isFlying = false;
        isDying = false;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        if (sprite != null && Icon != null)
        {
            AssignSprite(sprite);
        }
        this.type = type;
        destination = Vector2.zero;
        isHatching = true;
    }

    public void SendFlying(Vector2 destination, Action landingCallback = null)
    {
        // Debug.Log($"sent {this.type} from {rectTransform.anchoredPosition} to {destination}");
        this.destination = destination;

        isFlying = true;

        if (landingCallback != null)
        {
            // be careful if you need callbacks ?they will be re-written every time you're changing destinations
            finalCallback = landingCallback;
        }
    }

    void Update()
    {
        /*if (isDying)
            return;*/
        if (isHatching)
        {
            isHatching = Fade(false) < 1f;
            return;
        }
        if (isFlying)
        {
            if (FlyAndTellIfLanded())
            {
                isFlying = false;
                transform.SetParent(motherCell.transform);
                rectTransform.anchoredPosition = Vector2.zero;
                if (finalCallback != null)
                {
                    finalCallback.Invoke();
                    finalCallback = null;
                }
                LandingEvent.Invoke();
            }
        }
    }

    private float Fade(bool away)
    {
        Color oldColor = Icon.color;
        float newAlpha = oldColor.a + (away ? -1 : 1) * AnimationSettings.FlyingIconFadeOutTime;
        if (newAlpha > 1f)
            newAlpha = 1f;
        else if (newAlpha < 0f)
            newAlpha = 0f;
        Icon.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
        return newAlpha;
    }

    private bool FlyAndTellIfLanded()
    {
        bool hasLanded = false;
        float movementForTheFrame = Time.deltaTime * AnimationSettings.FlyingIconSpeed;
        Vector2 direction = (destination - rectTransform.anchoredPosition).normalized;
        Vector2 moveVector = direction * movementForTheFrame;
        if (Vector2.Distance(destination, rectTransform.anchoredPosition) is float dist && dist < 2 * movementForTheFrame)
        {

            moveVector = direction * dist;
            // rectTransform.anchoredPosition = Vector2.zero;
            hasLanded = true;
        }
        rectTransform.Translate(moveVector);
        return hasLanded;
    }

    public void PrepareToDie() => isDying = true;

    public void TellEveryoneYouAreDead()
    {
        DyingEvent.Invoke();
    }

}
