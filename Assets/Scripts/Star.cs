using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
    private Rigidbody rb;
    static Vector3 cameraPos;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(0f, 0f, -5f);
	}

    static Star()
    {
        cameraPos = Camera.main.transform.position;
    }
	
	// Update is called once per frame

    void OnCollisionEnter(Collision col)
    {
        StartCoroutine(CameraShake());
    }
	void Update () {
	}

    IEnumerator CameraShake()
    {
        float maxAmpl = 1.5f;
        float ampl = maxAmpl;
        int duration = 15;
        for (int i = duration; i >= 0; i--)
        {
            ampl = maxAmpl * ((float)i / duration);
            Camera.main.transform.position = cameraPos + Random.insideUnitSphere * ampl;
            yield return 0;
        }
        Camera.main.transform.position = cameraPos;
    }
}
