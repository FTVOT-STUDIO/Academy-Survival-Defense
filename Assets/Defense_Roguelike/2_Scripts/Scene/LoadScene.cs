using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadScene : MonoBehaviour
{
    [SerializeField] Slider loadingBar;
    [SerializeField] TMP_Text progressText;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        progressText.text = "데이터를 불러오는 중..";
        AsyncOperation operation;

        operation = SceneManager.LoadSceneAsync("Lobby_Scene");

        DataLoadManager.Inst.isSceneInitialized = false;
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(1);

        float fakeProgress = 0f;
        float targetProgress = 0f;

        while (!operation.isDone)
        {
            yield return null;

            if (operation.progress < 0.9f)
            {
                targetProgress = operation.progress;
            }
            else
            {
                targetProgress = 1f;
            }

            fakeProgress = Mathf.MoveTowards(fakeProgress, targetProgress, Time.unscaledDeltaTime * 0.4f);
            loadingBar.value = fakeProgress;
            progressText.text = (fakeProgress * 100f).ToString("F0") + "%";

            if (fakeProgress >= 1f)
            {
                operation.allowSceneActivation = true;
                DataLoadManager.Inst.isSceneInitialized = true;
                yield break;
            }
        }
    }

}

