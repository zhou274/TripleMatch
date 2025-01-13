using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    [SerializeField] TMP_Text timer;
    
    public void UpdateText(float time)
    {
        timer.text = Utils.TimeToText(time);
    }
}
