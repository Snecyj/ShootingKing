using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator ani;

    public World world;

    [SerializeField] Rigidbody RB;

    [SerializeField] GameObject Arrow;

    [SerializeField] GameObject SHOOT_ME_text;

    [SerializeField] GameObject present_figure;

    Vector3 start_pos;
    Vector3 start_pos_gloabal;

    bool end;
    void Start()
    {
        start_pos = transform.localPosition;
        start_pos_gloabal = transform.position;
    }

    public void present_shoot(Vector3 dir)
    {
        ani.enabled = true;
        ani.SetBool("SHOOT ME", true);
        world.pwin();
        RB.isKinematic = false;
        RB.velocity = dir * 25 + Vector3.up * 25;
        RB.AddTorque(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.VelocityChange);
        Arrow.SetActive(false);
        SHOOT_ME_text.SetActive(false);
        present_figure.SetActive(true);
    }

    public void present_hide()
    {
        present_figure.SetActive(false);
        ani.SetBool("EndScene", true);
        RB.isKinematic = true;
        end = true;
        transform.position = start_pos_gloabal;
    }

    void FixedUpdate()
    {
        if (end)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, start_pos, 0.1f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
        }
    }
}
