using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumber : MonoBehaviour {
    public Image Back;
    public Text text;
    private int duration = 15;
    private int damage;
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
            text.text = value.ToString();
            int digits = NumDigits(value);
            Back.transform.localScale = new Vector3(1f + 0.3f * (digits - 2), 1f, 1f);
        }
    }
    public Color Color
    {
        get
        {
            return text.color;
        }
        set
        {
            text.color = value;
        }
    }
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 2f);
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        Vector3 initPos = transform.localPosition;
        for (int i = 1; i <= duration; i++)
        {
            Vector3 newPos = initPos;
            newPos.y = initPos.y + 40f * Mathf.Sin(i * Mathf.PI / duration);
            transform.localPosition = newPos;
            yield return 0;
        }
    }

    int NumDigits(int num)
    {
        int digits = 1;
        while (num >= 10)
        {
            num /= 10;
            digits++;
        }
        return digits;
    }
}
