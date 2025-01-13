using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoostersPadCell : MonoBehaviour
{
    [SerializeField] Button Button;
    [SerializeField] Image Icon;
    [SerializeField] Image CounterText; // well.. Image!
    [SerializeField] SpriteLibrary SpriteLibrary; // to make it easier in this proto-version, bind spritelibrary
    private int counter;
    public Action<UIBoostersPadCell> ClickCallback { get; private set; }

    // pretty prototypey, with bare bones functionality
    public void Build(Sprite icon, int counter, Action<UIBoostersPadCell> callback)
    {
        gameObject.SetActive(true);
        if (counter < 0 || counter > 2)
        {
            counter = 0; // let it be like that
        }
        this.counter = counter;
        Icon.sprite = icon;
        AssignCounterSprite(counter);
        Button.interactable = counter > 0;
        ClickCallback = callback;
    }

    public void IncrementBooster()
    {
        if (counter + 1 > 2)
            return; // so it goes!
        counter++;
        AssignCounterSprite(counter);
        Button.interactable = true;
    }
    public void DecrementBooster()
    {
        if (counter == 0)
            return; // so it goes!
        counter--;
        AssignCounterSprite(counter);
        Button.interactable = counter > 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        ClickCallback?.Invoke(this);
    }

    public void AssignCounterSprite(int counter)
    {
        if (SpriteLibrary.GetSpriteForBoosterCounter(counter) is Sprite sprite)
        {
            CounterText.gameObject.SetActive(true);
            CounterText.sprite = sprite;
        }
        else
        {
            CounterText.gameObject.SetActive(false);
        }
    }
}
