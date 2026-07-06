using UnityEngine;
using System.Collections;
using TMPro;

public class NumberCounter : MonoBehaviour
{
    public static void StartCounting(MonoBehaviour runner, int count, float startNum, float duration, TMP_Text text, string prefix = "", string suffix = "")
    {
        runner.StartCoroutine(NumberCountRoutine(count, startNum, duration, text, prefix, suffix));
    }

    private static IEnumerator NumberCountRoutine(int count, float startNum, float duration, TMP_Text text, string prefix = "", string suffix = "")
    {
        if (Mathf.Approximately(startNum, count) || duration <= 0f)
        {
            text.text = $"{prefix}{count:n0}{suffix}";
            yield break;
        }

        float timeElapsed = 0f;
        float start = startNum;
        float end = count;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float current = Mathf.Lerp(start, end, t);
            text.text = $"{prefix}{(int)current:n0}{suffix}";
            yield return null;
        }

        text.text = $"{prefix}{count:n0}{suffix}";
    }
}


