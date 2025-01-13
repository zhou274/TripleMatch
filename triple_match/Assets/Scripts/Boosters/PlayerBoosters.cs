using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoosters: MonoBehaviour
{
    public Dictionary<BoosterType, int> boosters { get; private set; }
    [SerializeField] UIBoostersPad UIBoostersPad;
    [SerializeField] LevelController controller;

    public void Init(Dictionary<BoosterType, int> boosters)
    {
        if (this.boosters == null)
            this.boosters = new Dictionary<BoosterType, int>();
        else 
            this.boosters.Clear();
        
        this.boosters = boosters;
        UIBoostersPad.InitAndShow(boosters, UseBooster);
    }

    public void Clear()
    {
        boosters.Clear();
    }

    public void AddBooster(BoosterType type, int count = 1)
    {
        if (boosters.ContainsKey(type))
        {
            boosters[type] += count;
        }
        else
        {
            boosters.Add(type, count);
        }
    }

    public void UseBooster(BoosterType type)
    {
        if (!boosters.ContainsKey(type) || boosters[type] <= 0)
            return;
        else
        {
            boosters[type]--;
            if (controller.OnBoosterActivation(type))
            {
                UIBoostersPad.OnBoosterUse(type);
            }
        }
    }
}
