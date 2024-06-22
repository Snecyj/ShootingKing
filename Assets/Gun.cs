using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] AudioSource _audio;
	[SerializeField] AudioClip _Shoot, _Reload;
	int m_loaded;

	public ParticleSystem Sparkle;

	public GameObject Gilsa;
	public int loaded
	{
		get { return m_loaded; }
		set { m_loaded = Mathf.Min(value, loadedMax); }
	}
	public int loadedMax = 1;
	public int count;
	public int ammo;
	public float spd = 15;
	public float spread;
	public float time = .5f;

	public Bullet pref;

	List<Bullet> preInst = new List<Bullet>();

	public Transform target;

	private void Awake()
	{
		int i = 0;
		while (i++ < count)
		{
			Bullet bullet = Instantiate(pref);
			bullet.gameObject.SetActive(false);
			preInst.Add(bullet);
		}
		loaded = loadedMax;
	}

	public void reloadwaw()
    {
		_audio.clip = _Reload;
		_audio.Play();
	}

	public bool Shoot(Vector3 dir)
	{
		if (ammo > 0)
		if (loaded > 0)
		{
			float i = 0;
			float r = spread / preInst.Count / 2;
			foreach (Bullet go in preInst)
			{
				go.transform.position = target.transform.position;
				go.Launch(Quaternion.Euler(0, -spread / 2 + (i++ / preInst.Count) * spread + Random.Range(-r, r), 0) * (dir.normalized * spd), time);
			}
			loaded--;
		    ammo--;
				_audio.clip = _Shoot;
				_audio.Play();
			Sparkle.Play();
				GameObject temp = Instantiate(Gilsa, target);
				temp.GetComponent<Rigidbody>().AddForce(Vector3.up*2 + target.forward * -2 + target.right * 2, ForceMode.VelocityChange);
				temp.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.VelocityChange);
				temp.transform.parent = null;
				return true;
			
		}
		return false;
	}

}
