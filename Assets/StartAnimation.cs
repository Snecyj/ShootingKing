using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator Anim;

    int timer;

    public int timetostart;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (timer > timetostart)
        {
            Anim.enabled = true;
        }
        else
        {
            timer++;
        }
    }
}
