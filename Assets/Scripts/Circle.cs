using UnityEngine;
using System.Collections;

public class Circle : MonoBehaviour {
    int duration = 40;
    Color col;
    float initAlpha;
    Vector3 startScale;
    Renderer r;
	// Use this for initialization
	void Start () {
        startScale = transform.localScale;
        r = GetComponent<Renderer>();
        col = r.material.GetColor("_TintColor");
        initAlpha = col.a;
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        float alpha = 0f;
        for (int i = 0; i < duration; i++)
        {
            r.material.EnableKeyword("_TINTCOLOR");
            alpha = initAlpha * ((float)i / duration);
            col.a = alpha;
            r.material.SetColor("_TintColor", col);
            transform.localScale = startScale * (1f - (float)i / duration);
            yield return 0;
        }
        Destroy(gameObject);
    }
}
