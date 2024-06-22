using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public int timetodestroy;
    int time;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        time++;
        if (time > timetodestroy)
            Destroy(gameObject);
    }
}
