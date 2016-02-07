using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Cutscene : MonoBehaviour {
    public MovieTexture mov;
    public GameObject cube;
	// Use this for initialization
	void Start () {
        cube.GetComponent<Renderer>().material.mainTexture = mov;
        mov.Play();
        Invoke("NextLevel", mov.duration);
	}

    void NextLevel()
    {
        SceneManager.LoadScene("Scene 3");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
