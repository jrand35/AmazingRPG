using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverScreen : MonoBehaviour {
    public Image black;
    public Text text;
    public Text retryText;
    public int duration = 130;

    void Start()
    {
        StartCoroutine(Run());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator Run()
    {
        Color initBlackColor = black.color;
        Color initTextColor = text.color;
        Color initRetryTextColor = retryText.color;
        for (int i = 0; i < duration; i++)
        {
            Color newBlackColor = initBlackColor;
            newBlackColor.a = initBlackColor.a * ((float)i / duration);
            Color newTextColor = initTextColor;
            newTextColor.a = initTextColor.a * ((float)i / duration);
            Color newRetryTextColor = initRetryTextColor;
            newRetryTextColor.a = initRetryTextColor.a * ((float)i / duration);
            black.color = newBlackColor;
            text.color = newTextColor;
            retryText.color = newRetryTextColor;
            yield return 0;
        }
    }
}
