using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FigureHPScript : MonoBehaviour
{
    public TextMeshProUGUI Tmp;
    int timer;
    Vector3 startscale;
    void Start()
    {
        startscale = transform.localScale* 0.75f;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer++;
        transform.localScale = Vector3.Lerp(transform.localScale, startscale, 0.1f);
        transform.position += Vector3.up * 3;
        if (timer > 45)
        {
            Tmp.color = new Color(Tmp.color.r, Tmp.color.g, Tmp.color.b, Mathf.Lerp(Tmp.color.a, 0, 0.25f));
            if (Tmp.color.a < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }
}
