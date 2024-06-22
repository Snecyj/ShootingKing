using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField]
	Rigidbody rigidbody;

	Vector3 LaunchVel;

	Vector3 LaunchPos;

	float time = 0;
	public void Launch(Vector3 dir,float time)
	{
		gameObject.SetActive(true);
		rigidbody.velocity = dir;
		LaunchVel = dir;
		LaunchPos = transform.position;
		this.time = time;
	}
	private void FixedUpdate()
	{
		time -= Time.fixedDeltaTime;
		if (time < 0)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Figure figure = collision.collider.GetComponentInParent<Figure>();
		if (figure != null)
		{
			figure.stephp++;
			figure.AddPower(LaunchPos + Vector3.up , rigidbody.velocity);
			figure.hp--;
			figure.transform.position += rigidbody.velocity * 0.015f;
			figure.BulletRed();
			gameObject.SetActive(false);
			return;
		}
		Present present = collision.collider.GetComponentInParent<Present>();
		if (present != null)
        {
			present.present_shoot(LaunchVel.normalized);
		}
	}
}
