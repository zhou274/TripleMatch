using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] UICurtain Curtain;
    public void OnReturnButtonClick()
    {
        Curtain.transform.SetAsLastSibling();
        Curtain.FadeIn(0.5f, halfTransparent: false).OnComplete(() =>
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        });
    }
}
