using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void OnLevelLoadButton(Level level)
    {
        LevelToLoad.level = level;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("TripleMatchCanoe", LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
            yield return null;
        yield return new WaitForEndOfFrame();
    }

}
