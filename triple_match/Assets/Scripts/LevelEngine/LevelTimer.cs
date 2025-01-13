using System;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    private bool isRunning = false;
    private Action endCallback;
    [SerializeField] UITimer uiTimer;
    public float remainingTime { get; private set; }
    public static Action AddTime;
    private void Awake()
    {
        AddTime += AddTimer;
    }
    private void OnDestroy()
    {
        AddTime -= AddTimer;
    }
    public void InitAndStart(Action callback, float time = 180f)
    {
        remainingTime = time;
        isRunning = true;
        endCallback = callback;
        uiTimer.UpdateText(time);
    }
    public void AddTimer()
    {
        remainingTime += 30;
    }
    public float GetTime()
    {
        return remainingTime;
    }
    public void Stop()
    {
        isRunning = false;
    }
    public void resume()
    {
        isRunning = true;
    }
    void Update()
    {
        if (isRunning && remainingTime > 0)
        {
            float newTime = remainingTime - Time.deltaTime;
            if (newTime <= 0)
            {
                newTime = 0;
                isRunning = false;
                endCallback?.Invoke();
            }
            if (Mathf.CeilToInt(newTime) < remainingTime)
            {
                uiTimer.UpdateText(newTime);
            }
            remainingTime = newTime;
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            remainingTime += 10;
        }
    }

}
