using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class money : MonoBehaviour
{
    // Start is called before the first frame update
    CoinManager target;
    public float scale;
    public Vector3 selftarget;
    bool move;

    void Start()
    {
        target = GameObject.FindWithTag("CoinTarget").GetComponent<CoinManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = Vector3.one/scale * Vector3.Distance(transform.position, Camera.main.transform.position);
          if (!move)
        {
            if (Vector3.Distance(transform.localPosition, selftarget) > 0.1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, selftarget, 0.05f);
            }
            else
            {
                move = true;
            }
        }
        if (move)
        {
            if (!target.GM.next)
                transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.05f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);
        target.addcoin();
    }
}
