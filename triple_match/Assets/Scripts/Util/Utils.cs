using UnityEngine;
public static class Utils
{
    public static string TimeToText(float time)
    {
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.CeilToInt(time % 60);
        if (sec == 60)
        {
            sec = 0;
            min++;
        }
        return string.Format("{0}:{1:00}", min, sec);
    }
}
