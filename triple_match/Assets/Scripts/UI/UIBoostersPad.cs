using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class UIBoostersPad : MonoBehaviour
{
    private SortedDictionary<BoosterType, UIBoostersPadCell> cells;
    [SerializeField] SpriteLibrary SpriteLibrary;
    private Action<BoosterType> clickCallback;

    // again, not full fledged but MVP version
    public void InitAndShow(Dictionary<BoosterType, int> boosters, Action<BoosterType> clickCallback)
    {
        var UICells = GetComponentsInChildren<UIBoostersPadCell>(includeInactive: true);
        if (UICells.Length > 0)
        {
            cells = new SortedDictionary<BoosterType, UIBoostersPadCell>();
            this.clickCallback = clickCallback;

            SortedDictionary<BoosterType, int> sortedBoosters = new SortedDictionary<BoosterType, int>(boosters);
            int cellIndex = 0;

            foreach (var booster in sortedBoosters)
            {
                var cell = UICells[cellIndex++];
                cell.Build(SpriteLibrary.GetSpriteByBoosterType(booster.Key), booster.Value, OnCellClicked);
                cells.Add(booster.Key, cell);
                if (cellIndex >= UICells.Length) { break; } // for now: just not bothering if there are more boosters than cells
            }

            // to make sure there's nothing unwanted on the pad
            if (cellIndex < UICells.Length)
            {
                for (int i = cellIndex; i < UICells.Length; i++)
                {
                    UICells[i].Hide();
                }
            }
        }
    }

    public void OnCellClicked(UIBoostersPadCell UICell)
    {
        if (cells.Count == 0)
            return;

        var cell = cells.FirstOrDefault(cell => cell.Value == UICell);
        if (!cell.Equals(default(KeyValuePair<BoosterType, UIBoostersPadCell>)))
        {
            clickCallback?.Invoke(cell.Key);
        }
    }

    public void OnBoosterUse(BoosterType type)
    {
        if (cells.ContainsKey(type))
        {
            cells[type].DecrementBooster();
        }
    }
}
