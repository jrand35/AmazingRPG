using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
    int scaleDuration = 12;
    Renderer r;
    Vector3 initScale;
	// Use this for initialization
	void Start () {
        initScale = transform.localScale;
        r = GetComponent<Renderer>();
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        for (int i = 1; i <= scaleDuration; i++)
        {
            transform.localScale = initScale * (float)i / scaleDuration;
            Vector3 pos = transform.position;
            pos.y = r.bounds.extents.y;
            transform.position = pos;
            yield return 0;
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 1; i <= scaleDuration; i++)
        {
            transform.localScale = initScale * (1f - (float)i / scaleDuration);
            Vector3 pos = transform.position;
            pos.y = r.bounds.extents.y;
            transform.position = pos;
            yield return 0;
        }
        Destroy(gameObject);
    }
}
