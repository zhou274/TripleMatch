using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    private void Start()
    {
        if (GetComponent<RectTransform>() is RectTransform RT)
        {
            AnimationSettings.FlyingIconSpeed = RT.sizeDelta.y;
        }
    }
}
