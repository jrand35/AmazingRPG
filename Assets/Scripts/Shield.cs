using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    int duration = 20;
    public bool Active { get; private set; }
    Vector3 initScale;

	// Use this for initialization
	void Start () {
        Active = false;
        initScale = transform.localScale;
        transform.localScale = Vector3.zero;
	}

    public IEnumerator Enter()
    {
        Debug.Log("Shield active");
        Active = true;
        float xscale = 0f;
        float zscale = 0f;
        for (int i = 1; i <= duration; i++)
        {
            xscale = 2f - 1f * (i / (float)duration);
            zscale = ((float)i / duration);
            transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
            yield return 0;
        }
        transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
    }

    public IEnumerator Exit()
    {
        Active = false;
        float xscale = 0f;
        float zscale = 0f;
        for (int i = 1; i <= duration; i++)
        {
            xscale = 1f - ((float)i / duration);
            zscale = 1f + 1f * (i / (float)duration);
            transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
            yield return 0;
        }
        transform.localScale = new Vector3(initScale.x * xscale, initScale.y * 1f, initScale.z * zscale);
    }
}
