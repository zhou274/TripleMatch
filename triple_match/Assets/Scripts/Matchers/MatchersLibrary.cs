using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Matchers Library")]
public class MatchersLibrary : ScriptableObject
{
    [SerializeField] MatchingObject[] Matchers;

    public GameObject GetMatcherByType(MatchingType type)
    {
        return Array.Find(Matchers, matcher => matcher.type == type).gameObject;
    }
}
