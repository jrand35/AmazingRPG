using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour {
    public Text text;
	// Use this for initialization
	void Start () {
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (SceneManager.GetActiveScene().name == "Scene")
            {
                SceneManager.LoadScene("Scene 2");
            }
        }
	}

    IEnumerator Run()
    {
        for (int i = 1; i <= 15; i++)
        {
            text.transform.localScale = Vector3.one * (float)i / 15;
            yield return 0;
        }
    }
}
