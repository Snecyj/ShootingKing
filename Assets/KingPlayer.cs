using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPlayer : Figure
{
	public Gun gun;

	public GameObject AIM_Triangle;

	public void DEAD()
    {
		foreach (Transform tr in ModelLife.GetComponentInChildren<Transform>())
		{
			Destroy(tr.gameObject);
		}
		Instantiate(ModelDead, ModelLife);
	}
	public void SetPosTarget(Vector3 pos)
	{
		target.position = pos;
		pos = target.position - transform.position;
		if (pos.magnitude > 0.1f)
			gun.transform.rotation = Quaternion.LookRotation(pos);

	}

	public void SetPosAim(Vector3 pos)
	{
		if (pos.magnitude > 0.1f)
			gun.transform.rotation = Quaternion.LookRotation(pos);

	}

	void findshahandmat()
    {
		Vector3 starttargetpos = target.position;
		if (gun.loaded > 0)
			return;
		Vector2Int[] dirsVect = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) };
		bool shahmat = true;
		Figure figure = null;
		int ds = 0;
		foreach (Vector2Int dirIndex in dirsVect)
		{
			Vector3 posz = new Vector3(pos.x, 0, pos.y) + new Vector3(dirIndex.x, 0, dirIndex.y);
			posz.x = Mathf.Clamp(Mathf.Round(posz.x), 0, 7);
			posz.y = Mathf.Round(posz.y);
			posz.z = Mathf.Clamp(Mathf.Round(posz.z), 0, 7);
			target.position = posz;
			if (CanStep(out Vector2Int[] points))
			{
				Vector2Int posq = new Vector2Int(Mathf.RoundToInt(posz.x), Mathf.RoundToInt(posz.z));
				if (world.Get(posq, out figure))
				{
					continue;
				}
				int i = world.nextcan.FindIndex((x) =>
				{
					return x.Undercover(points[0]);
				});
				if (i == -1)
				{
					shahmat = false;
					ds++;
				}
			}
		}
        if (shahmat)
            world.shahmat();
        target.position = starttargetpos;
	}

	public int PlayerStep(out Figure figure)
	{
		findshahandmat();
		figure = null;
		if (CanStep(out Vector2Int[] points))
		{
			Vector2Int pos = new Vector2Int(Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.z));
			if (world.Get(pos, out figure))
			{
				return 1;
			}
			int i = world.nextcan.FindIndex((x) =>
			{
				return x.Undercover(points[0]);
			});
			if (i == -1)
			{
				Step(world.CheckPos);
				GM.AmmoTimer.sprite = GM.AmmoRespawnFull;
				GM.CircleShot.enabled = false;
                if (gun.loaded < 1)
                {
					gun.reloadwaw();
				}
				gun.loaded++;
				GM.CircleShot.gameObject.SetActive(true);
				return 0;
			}
			else
			{
				return 999;
			}
		}
		else
		{
			Vector2Int pos = new Vector2Int(Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.z));
			if (world.Get(pos, out figure))
			{
				if (figure == this)
					return -2;
				else
					return -1;
			}
		}
		return -1;
	}

	internal bool Shoot(Vector3 dir)
	{
		findshahandmat();
		dir.y = 0;
		if (gun.Shoot(dir))
		{
			GM.AmmoUpdate(gun.ammo);
			return true;
		}
		else
		{
			return false;
		}

	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ammo" )
		{
			Destroy(other.gameObject);
			if(gun.ammo < GM.AmmoSlots.Length)
			gun.ammo++;
			GM.AmmoUpdate(gun.ammo);
		}
	}
}
