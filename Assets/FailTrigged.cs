using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailTrigged : MonoBehaviour
{
	public Image[] images;

	public Color off, on;

	public int fails {
		set
		{
			for (int i = 0; i < images.Length; i++)
			{
				images[i].color = i >= value ? off : on;
			}
		}
	}

	private void OnValidate()
	{
		fails = 0;
	}
}
