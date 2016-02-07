using UnityEngine;
using System.Collections;

public class Comb : MonoBehaviour {
    int fadeDuration = 90;
    Material m;
	// Use this for initialization
	void Start () {
        m = GetComponent<Renderer>().material;
        StartCoroutine(Run());
        Destroy(gameObject, 5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        Transparent(m);
        for (int i = 1; i <= fadeDuration; i++)
        {
            float r = Mathf.Pow(i - fadeDuration, 2) / 15f;
            transform.localRotation = Quaternion.Euler(0f, r, 0f);
            Color col = Color.white * (float)i / fadeDuration;
            m.color = col;
            yield return 0;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 50; i++)
        {
            float dZ = i * -0.05f;
            transform.Translate(0f, 0f, dZ);
            yield return 0;
        }
    }

    void Opaque(Material material)
    {
        material.SetFloat("_Mode", 0);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    void Transparent(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
