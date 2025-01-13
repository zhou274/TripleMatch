using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// this class is responsible solely for the matching pad logic
// animations are done elsewhere (but some of them are called from here)
public class MatchingPad : MonoBehaviour
{
    [SerializeField] MatchingPadCell[] cells;
    [SerializeField] AnimationController AnimationController;

    private static readonly object cellShiftLock = new object();

    private void Awake()
    {
        LevelController.ClearAllCells += ClearAllCell;
    }
    private void OnDestroy()
    {
        LevelController.ClearAllCells -= ClearAllCell;
    }
    private void Start()
    {
        
    }
    public bool InsertNewObject(MatchingFlyingIcon flyer, out bool areMatched)
    {
        lock (cellShiftLock)
        {
            areMatched = false;
            int cellIndex = GetIndexLastCellOfType(flyer.type);
            if (cellIndex == -1)
            {
                cellIndex = GetIndexFirstFreeCell();
                if (cellIndex == -1) { return false; }
            }
            else
            {
                cellIndex++;                    // actually I need the next cell.
                if (cellIndex >= cells.Length)  // ..but if there's no such cell
                    return false;               // then, again, no place to insert
            }

            // left path: no need to shift || right part: shifted succesfully
            if (cellIndex == GetIndexFirstFreeCell() || ShiftCellsRight(cellIndex, 1))
            {
                var cellToPair = cells[cellIndex];
                flyer.transform.SetParent(this.transform);

                ApplyFlyerToCell(flyer, cellToPair);
                MatchedThree matchedFlyers = null;
                areMatched = CheckAndMarkIfThreeMatched(cellIndex, flyer.type, out matchedFlyers);

                Action callback = null;
                if (areMatched)
                {
                    var allFlyers = new List<MatchingFlyingIcon>() { matchedFlyers.left, matchedFlyers.mid, matchedFlyers.right };
                    foreach (var matched in allFlyers)
                        matched.PrepareToDie();
                    callback = () => WaitUntilFlyersLandAndCallback(
                        allFlyers,
                        () => MatchThree(matchedFlyers)
                    );
                }

                flyer.SendFlying(cellToPair.GetComponent<RectTransform>().anchoredPosition, callback);
                return true;
            }
            return false; // if we're here, we have no place to shift. it is kinda impossible, but still.
        }
    }

    private int GetIndexFirstFreeCell(int start = 0) => GetIndexFirstCellOfType(MatchingType.EMPTY, start);

    private int GetIndexFirstCellOfType(MatchingType type, int start = 0)
    {
        for (int i = start; i < cells.Length; i++)
        {
            if (cells[i].type == type)
                return i;
        }
        return -1;
    }


    private int GetIndexLastCellOfType(MatchingType type)
    {
        for (int i = cells.Length - 1; i >= 0; i--)
        {
            if (cells[i].type == type)
                return i;
        }
        return -1;
    }

    private bool ShiftCellsRight(int firstToShift, int shift)
    {
        int lastToShift = GetIndexFirstFreeCell();
        if (lastToShift == -1)
            return false;
        else
            lastToShift--;

        if (lastToShift + shift >= cells.Length)
            return false;

        for (int i = lastToShift; i >= firstToShift; i--)
        {
            SwapCellsAndSendFlyer(cells[i], cells[i + shift]);
        }
        return true;
    }

    private void DensifyCellsLeft()
    {
        int firstFreeCell = GetIndexFirstFreeCell();
        Debug.Log($"densifying. first free cell: {firstFreeCell}");
        if (firstFreeCell == -1) return;
        for (int i = firstFreeCell + 1; i < cells.Length; i++)
        {
            if (firstFreeCell >= cells.Length)
                break;
            if (cells[i].type != MatchingType.EMPTY)
            {
                SwapCellsAndSendFlyer(cells[i], cells[firstFreeCell]);
                firstFreeCell++;
            }
        }
    }


    private bool CheckAndMarkIfThreeMatched(int lastIndex, MatchingType type, out MatchedThree matchedFlyers)
    {
        matchedFlyers = null;

        // if inserted object is in the first or the second slot, match is impossible
        // also drop the check if the index is out of bounds for some reason
        // or the cell is empty
        if (lastIndex < 2 || lastIndex >= cells.Length || type == MatchingType.EMPTY)
        {
            return false;
        }

        bool matchedThree = type == cells[lastIndex - 1]?.type && type == cells[lastIndex - 2]?.type;
        if (matchedThree)
        {
            cells[lastIndex - 2].type = MatchingType.MATCHED;
            cells[lastIndex - 1].type = MatchingType.MATCHED;
            cells[lastIndex].type = MatchingType.MATCHED;
            matchedFlyers = new MatchedThree(cells[lastIndex - 2].flyer, cells[lastIndex - 1].flyer, cells[lastIndex].flyer);
        }
        return matchedThree;
    }
    public void WaitForFlyersAndCallback(List<MatchingFlyingIcon> flyers, Action callback, bool flying)
    {
        int stillWaiting = 0;

        void CatchEventAndMoveForwardIfReady()
        {
            if (--stillWaiting == 0)
            {
                callback?.Invoke();
            }
        }

        foreach (var flyer in flyers)
        {
            if (flying && flyer.isFlying)
            {
                stillWaiting++;
                flyer.LandingEvent.AddListener(CatchEventAndMoveForwardIfReady);
            }
            else if (!flying && flyer.isDying)
            {
                stillWaiting++;
                flyer.DyingEvent.AddListener(CatchEventAndMoveForwardIfReady);
            }
        }
        if (stillWaiting == 0) { callback?.Invoke(); }
    }

    public void WaitUntilFlyersLandAndCallback(List<MatchingFlyingIcon> flyers, Action callback) =>
        WaitForFlyersAndCallback(flyers, callback, flying: true);

    public void WaitUntilFlyersDieAndCallback(List<MatchingFlyingIcon> flyers, Action callback) =>
        WaitForFlyersAndCallback(flyers, callback, flying: false);


    private void MatchThree(MatchedThree flyers)
    {
        AnimationController.MatchThree(flyers.left.Icon, flyers.mid.Icon, flyers.right.Icon, () =>
        {
            lock (cellShiftLock)
            {
                List<MatchingFlyingIcon> allFlyers = new List<MatchingFlyingIcon>() { flyers.left, flyers.mid, flyers.right };
                foreach (var flyer in allFlyers)
                {
                    flyer.motherCell.Empty();
                    flyer.TellEveryoneYouAreDead();
                    flyer.ResetAndReturn();
                }

                DensifyCellsLeft();
            }
        });
    }
    public void ClearAllCell()
    {
        //List<MatchingFlyingIcon> allFlyers = new List<MatchingFlyingIcon>();
        for(int i=0;i<cells.Count();i++)
        {
            MatchingFlyingIcon flyer= cells[i].GetComponent<Transform>().GetChild(0).GetComponent<MatchingFlyingIcon>();
            flyer.motherCell.Empty();
            flyer.TellEveryoneYouAreDead();
            flyer.ResetAndReturn();
        }
    }
    private void ApplyFlyerToCell(MatchingFlyingIcon flyer, MatchingPadCell cell)
    {
        cell.type = flyer.type;
        cell.flyer = flyer;
        flyer.motherCell = cell;
    }


    private void SwapCellsAndSendFlyer(MatchingPadCell from, MatchingPadCell to)
    {
        from.flyer.transform.SetParent(this.transform);
        ApplyFlyerToCell(from.flyer, to);
        // NB! if old cell has type other than its flyer, this type has priority
        if (from.type != from.flyer.type)
        {
            to.type = from.type;
        }
        from.flyer.SendFlying(to.GetComponent<RectTransform>().anchoredPosition);

        // a bit redundant but reliable
        from.Empty();
    }

    public bool CheckIfGameOver()
    {
        lock (cellShiftLock)
        {
            foreach (MatchingPadCell cell in cells)
            {
                if (cell.type == MatchingType.EMPTY || cell.type == MatchingType.MATCHED)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public int GetCellCountByType(MatchingType type)
    {
        return cells.Aggregate(0, (count, cell) => cell.type == type ? count + 1 : count);
    }

    public List<MatchingFlyingIcon> GetAllFlyers()
    {
        return GetComponentsInChildren<MatchingFlyingIcon>().ToList();
    }
    //todo
    //public void ClearAllFlyers()
    //{
    //    for(int i=0;i<transform.childCount;i++)
    //    {
    //        MatchingFlyingIcon match = transform.GetChild(i).GetComponent<MatchingFlyingIcon>();
    //        if(match!=null)
    //        {
    //            Destroy(transform.GetChild(i));
    //        }
    //    }
    //}
}
