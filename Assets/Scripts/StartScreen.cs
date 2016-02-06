using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartScreen : MonoBehaviour {
    public Text text;

    void Start()
    {
        StartCoroutine(Run());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene("Scene");
        }
    }

    IEnumerator Run()
    {
        float count = 0f;
        Color col = text.color;
        Vector3 scale = text.transform.localScale;
        while (true)
        {
            count += 0.1f;
            float color = 0.5f + 0.25f * Mathf.Sin(count);
            col.b = color;
            scale.x = 0.2f + 0.01f * Mathf.Sin(count);
            scale.y = 0.2f + 0.01f * Mathf.Sin(count);
            text.color = col;
            text.transform.localScale = scale;
            yield return 0;
        }
    }
}
