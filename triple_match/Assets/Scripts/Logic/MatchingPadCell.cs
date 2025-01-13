using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingPadCell : MonoBehaviour
{
    public MatchingFlyingIcon flyer = null;
    public MatchingType type;

    public void Empty()
    {
        Debug.Log($"emptying {this}");
        flyer = null;
        type = MatchingType.EMPTY;
    }
}
