using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_noise : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 start_pos;
    float timer;
    void Start()
    {
        start_pos = transform.position;
    }

    public void random_pos()
    {
        timer = 0;
        transform.position += Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)))* 0.2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

            transform.position = Vector3.Lerp(transform.position, start_pos, 0.2f);

    }
}
