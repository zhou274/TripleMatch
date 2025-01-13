using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class LevelController : MonoBehaviour
{
    [SerializeField] AnimationController animationController;
    [SerializeField] MatchingPad MatchingPad;
    [SerializeField] UIGoalsPad GoalsPad;
    [SerializeField] LevelGoals Goals;
    [SerializeField] LevelTimer Timer;
    [SerializeField] EndLevelScreen Endpanel;
    [SerializeField] GameObject CurtainOverlay;
    public static Action ClearAllCells;

    private bool isGameOver = false;

    public string clickid;
    private StarkAdManager starkAdManager;

    public void OnMatchingObjectClick(MatchingObject spawner)
    {
        if (isGameOver) return;

        MatchingFlyingIcon flyer = animationController.TransitFromObjectToUI(spawner);
        bool gotMatch;

        if (MatchingPad.InsertNewObject(flyer, out gotMatch))
        {
            Destroy(spawner.gameObject); // change to 'return to pooler'?

            // this is the place to progress level
            // the object is placed on the pad, so you can count it already
            // type is very well known (it's flyer.type / spawner.type)
            // if all the goals are finished, call to UI and tell that it's over
            bool goalsAchieved = !Goals.UpdateAndTellIfThereIsAnythingLeft(flyer.type);
            if (goalsAchieved)
            {
                isGameOver = true;
                OnLevelBeaten();
                return;
            }


            if (!gotMatch) // no need to check for game over if there is a match
            {
                isGameOver = MatchingPad.CheckIfGameOver();
                if (isGameOver)
                {
                    var allCells = MatchingPad.GetAllFlyers();
                    // this is the place to call to UI and tell that it's over
                    OnLevelFailed();
                }
            }
        }
        else
        {
            // this is after-game-over situation. so work it just in case
            flyer.ResetAndReturn();
            spawner.Unclick();
        }
    }
    
    public void OnLevelStart(float levelTime = 180)
    {
        Timer.InitAndStart(() => OnLevelFailed(timeOut: true), levelTime);
        animationController.LevelStartedAnimation();
    }

    public void OnLevelFailed(bool timeOut = false)
    {
        MatchingPad.WaitUntilFlyersDieAndCallback(MatchingPad.GetAllFlyers(),
                () => animationController.LevelFailedAnimation());
    }
    public void Start()
    {
        
        
    }
    public void ContinueGame()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    OnLevelStart(Timer.GetTime() + 30);
                    ClearAllCells();
                    //LevelTimer.AddTime();
                    Timer.resume();
                    isGameOver = false;
                    Endpanel.HideLoseScreen();
                    CurtainOverlay.SetActive(false);


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }
    public void OnLevelBeaten()
    {
        Timer.Stop();
        MatchingPad.WaitUntilFlyersDieAndCallback(MatchingPad.GetAllFlyers(), 
                () => animationController.LevelBeatenAnimation(Timer.remainingTime));
    }

    public bool OnBoosterActivation(BoosterType type)
    {
        if (isGameOver)
            return false;

        if (type == BoosterType.MAGNET)
        {
            ActivateMagnet();
            return true;
        }
        return false; // unknown type, impossibru
    }

    private void ActivateMagnet()
    {
        int countToMagnet = Mathf.Min(MatchingPad.GetCellCountByType(MatchingType.EMPTY), 3);

        if (countToMagnet <= 0) { return; } // no place to magnetize anything; it's impossible, but just in case

        var howMuchCanBeMagnetized = new SortedDictionary<MatchingType, int>();
        foreach (var goal in Goals.Goals)
        {
            int theoreticalMax = goal.Value; // empty cells count vs left to collect
            int practicalMax = Mathf.Min(countToMagnet, 3 - MatchingPad.GetCellCountByType(goal.Key));
            howMuchCanBeMagnetized.Add(goal.Key, Mathf.Min(theoreticalMax, practicalMax));
        }

        System.Random rand = new System.Random();

        MatchingType typeToMagnet = MatchingType.EMPTY;
        while (countToMagnet > 0)
        {
            var perfectCandidates = howMuchCanBeMagnetized.Where(candidate => candidate.Value == countToMagnet).ToList();
            if (perfectCandidates.Count > 0)
            {
                typeToMagnet = perfectCandidates[rand.Next(perfectCandidates.Count)].Key;
                break;
            }
            else
            {
                countToMagnet--;
            }
        }

        if (typeToMagnet != MatchingType.EMPTY && countToMagnet != 0)
        {
            var objCandidates = FindObjectsOfType<MatchingObject>().Where(obj => obj.type == typeToMagnet).ToList();

            for (int i = 0; i < countToMagnet; i++)
            {
                if (objCandidates.Count > 0)
                {
                    OnMatchingObjectClick(objCandidates[rand.Next(objCandidates.Count)]);
                }
                else
                {
                    break;
                }
            }
        }

    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }
    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
