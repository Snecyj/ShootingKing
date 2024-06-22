using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBox : MonoBehaviour
{
    // Start is called before the first frame update

    public Color start_color;

    public MeshRenderer MR;

    public bool red;

    public Animator anim;

    public int timer;

    int timetostart;

    public void reset_color()
    {
        MR.material.color = start_color;
        red = false;
    }

    public void red_color()
    {
        MR.material.color = MR.material.color * 0.5f + Color.red * 0.5f;
    }
    void Start()
    {
        timetostart = Random.Range(0, 50);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (timer > timetostart)
        {
            anim.enabled = true;
        }
        else
        {
            timer++;
        }
    }
}
