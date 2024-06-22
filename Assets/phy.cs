using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phy : MonoBehaviour
{

    public Rigidbody RB;
    public Vector3 vel;

    public float av;

    public void setvel()
    {
        RB.velocity = vel;
    }

    public void FixedUpdate()
    {
       /* RB.velocity += new Vector3(0, av, 0) * 0.02f;*/
        //RB.velocity *= 0.99f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            //RB.velocity += new Vector3(0, av, 0);
            Rigidbody temp_rb = collision.collider.GetComponent<Rigidbody>();
            
            RB.AddForce(-temp_rb.velocity * RB.mass, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            RB.AddForce(-collision.impulse, ForceMode.Impulse);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            RB.AddForce(-collision.impulse, ForceMode.Impulse);
        }
    }

}
