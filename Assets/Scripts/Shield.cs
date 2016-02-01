using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    int duration = 20;
    Vector3 initScale;

	// Use this for initialization
	void Start () {
        initScale = transform.localScale;
        StartCoroutine(Scale());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Scale()
    {
        float xscale = 0f;
        float zscale = 0f;
        for (int i = 1; i < duration; i++)
        {
            xscale = 2f - 1f * (i / (float)duration);
            zscale = ((float)i / duration);
            transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
            yield return 0;
        }
        transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
    }
}
