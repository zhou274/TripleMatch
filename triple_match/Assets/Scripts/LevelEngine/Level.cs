using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    [System.Serializable]
    public class BoosterWithCount
    {
        public BoosterType type;
        public int count;
    }

    public ObjectCountGoals[] Objects;
    public BoosterWithCount[] Boosters;

    public int Time;
}
