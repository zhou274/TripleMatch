using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelController))]
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] ObjectGenerator objectGenerator;
    [SerializeField] LevelController controller;
    [SerializeField] LevelGoals goals;
    [SerializeField] PlayerBoosters boosters;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var level = LevelToLoad.level;
        Generate(level);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Generate(Level level)
    {
        Dictionary<MatchingType, int> Objects = new Dictionary<MatchingType, int>();
        Dictionary<MatchingType, int> Goals = new Dictionary<MatchingType, int>();
        foreach (var obj in level.Objects)
        {
            if (obj.count > 0)
                Objects.Add(obj.type, obj.count);
            if (obj.goal > 0)
            {
                int goal = obj.goal / 3 * 3;
                Goals.Add(obj.type, goal);
            }
        }

        objectGenerator.Generate(Objects, controller.OnMatchingObjectClick);
        goals.InitGoals(Goals);

        Dictionary<BoosterType, int> Boosters = new Dictionary<BoosterType, int>();

        foreach (var obj in level.Boosters)
        {
            if (obj.count > 0)
                Boosters.Add(obj.type, obj.count);
        }
        boosters.Init(Boosters);

        int time = Mathf.Max(60, level.Time);
        time = Mathf.Min(time, 300);
        controller.OnLevelStart(time);

    }

}
