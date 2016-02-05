using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverScreen : MonoBehaviour {
    public Image black;
    public Text text;
    public Button button;
    public Text buttonText;
    public int duration = 130;

    void Start()
    {
        StartCoroutine(Run());
    }

    public void Retry()
    {
        SceneManager.LoadScene("Scene");
    }

    IEnumerator Run()
    {
        Color initBlackColor = black.color;
        Color initTextColor = text.color;
        Color initButtonColor = button.image.color;
        Color initButtonTextColor = buttonText.color;
        for (int i = 0; i < duration; i++)
        {
            Color newBlackColor = initBlackColor;
            newBlackColor.a = initBlackColor.a * ((float)i / duration);
            Color newTextColor = initTextColor;
            newTextColor.a = initTextColor.a * ((float)i / duration);
            Color newButtonColor = initTextColor;
            newButtonColor.a = initButtonColor.a * ((float)i / duration);
            Color newButtonTextColor = initButtonTextColor;
            newButtonTextColor.a = initButtonTextColor.a * ((float)i / duration);
            black.color = newBlackColor;
            text.color = newTextColor;
            button.image.color = newButtonColor;
            buttonText.color = newButtonTextColor;
            yield return 0;
        }
    }
}
