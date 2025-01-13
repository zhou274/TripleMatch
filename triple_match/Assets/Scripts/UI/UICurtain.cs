using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICurtain : MonoBehaviour
{
    [SerializeField] Color color = Color.black;
    [SerializeField] float alphaForOverlay = 0.5f;
    public Tween FadeOut(float time, bool halfTransparent = true)
    {
        if (GetComponent<Image>() is Image curtain && isActiveAndEnabled)
        {
            return curtain.DOFade(0f, time).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        return null;
    }
    public Tween FadeIn(float time, bool halfTransparent = true)
    {
        gameObject.SetActive(true);
        if (GetComponent<Image>() is Image curtain)
        {
            return curtain.DOFade(halfTransparent ? alphaForOverlay : 1f, time);
        }
        return null;
    }

}
