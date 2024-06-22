using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class ChessLogicBase
{

	protected static Vector2Int[] dirsVect = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1) };
	protected static byte[] hps = new byte[] { 3, 3, 4, 4, 5, 8 };
	//protected static byte[] hps = new byte[] { 1, 1, 1, 1, 1, 1 };
	public virtual byte hp { get; }

	protected virtual int[] dirs
	{
		get
		{
			return new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
		}
	}

	public static void Dir(ref int x)
	{
		x = x == 0 ? (int)Mathf.Sign(UnityEngine.Random.value - .5f) : (int)Mathf.Sign(x);
	}
	protected virtual int[] GetDirs(Vector2Int targetDir)
	{
		List<int> dir = new List<int>(dirs);
		dir.Sort((x, y) => { return Vector2.Dot(dirsVect[y], targetDir).CompareTo(Vector2.Dot(dirsVect[x], targetDir)); });
		return dir.ToArray();
	}
	public abstract bool GetResults(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result);
	public abstract bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target);
}

[System.Serializable]
public class Chess_Pawn : ChessLogicBase
{
	public Vector2Int dir;
	public override byte hp => hps[0];
	public Chess_Pawn()
	{
		this.dir = Vector2Int.down;
	}
	public Chess_Pawn(Vector2Int dir)
	{
		this.dir = dir;
	}
	public override bool GetResults(Func<Vector2Int,bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result)
	{
		result = new Vector2Int[1];
		result[0] = position + dir;
		return CheckPoint(result[0]);
	}

	public override bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target)
	{
		return (position + dir + Vector2Int.left == target || position + dir + Vector2Int.right == target);
	}
}
[System.Serializable]
public class Chess_Horse : ChessLogicBase
{
	Vector2Int[] points = new Vector2Int[] { new Vector2Int(2, -1), new Vector2Int(2, 1), new Vector2Int(1, 2), new Vector2Int(-1, 2), new Vector2Int(-2, 1), new Vector2Int(-2, -1), new Vector2Int(-1, -2), new Vector2Int(1, -2) };

	public override byte hp => hps[1];

	public override bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target)
	{
		target = target - position;
		foreach (var point in points)
		{
			if (point == target)
				return true;
		}
		return false;
	}

	public override bool GetResults(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result)
	{
		List<Vector2Int> vector2Ints = new List<Vector2Int>(points);
		int i = 0;
		while(i < vector2Ints.Count)
		{
			vector2Ints[i] = vector2Ints[i++] + position;
		}
		vector2Ints.Sort((x, y) =>
		{
			return Vector2Int.Distance(x, target).CompareTo(Vector2Int.Distance(y, target));
		});
		if (CheckPoint!=null)
			vector2Ints.RemoveAll((x)=>{ return !CheckPoint(x); });
		result = vector2Ints.ToArray();
		return vector2Ints.Count>0;
	}
}
public class Chess_Bishop : ChessLogicBase
{
	public override byte hp => hps[2];
	protected override int[] dirs
	{
		get
		{
			return new int[] { 1, 3, 5, 7 };
		}
	}

	public override bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target)
	{
		bool b = GetResults(CheckPoint, position, target, out Vector2Int[] result);
		if (b)
		{
			foreach (var point in result)
			{
				if (point == target)
					return true;
			}
			return false;
		}
		else
			return false;
	}

	public override bool GetResults(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result)
	{
		Vector2Int dir = target - position;
		int[] dirs = GetDirs(dir);

		List<Vector2Int> points = new List<Vector2Int>();

		foreach (int dirIndex in dirs)
		{
			Vector2Int d = dirsVect[dirIndex];
			int dist = (int)Mathf.Abs(Vector2.Dot(dir, d)) / 2;
			Vector2Int point = Vector2Int.zero;
			for (int i = 1; i <= dist; i++)
			{
				if (!CheckPoint(position + d * i))
					break;
				point = d * i;
			}
			if (point != Vector2Int.zero)
			{
				points.Add(position + point);
			}
		}
		points.Sort((x, y) =>
		{
			return Vector2Int.Distance(x, target).CompareTo(Vector2Int.Distance(y, target));
		});
		result = points.ToArray();
		return points.Count > 0;
	}
}
public class Chess_Rook : ChessLogicBase
{
	public override byte hp => hps[3];
	protected override int[] dirs
	{
		get
		{
			return new int[] { 0, 2, 4, 6 };
		}
	}
	public override bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target)
	{
		bool b = GetResults(CheckPoint, position, target, out Vector2Int[] result);
		if (b)
		{
			foreach (var point in result)
			{
				if (point == target)
					return true;
			}
			return false;
		}
		else
			return false;
	}
	public override bool GetResults(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result)
	{
		Vector2Int dir = target - position;
		int[] dirs = GetDirs(dir);

		List<Vector2Int> points = new List<Vector2Int>();

		foreach (int dirIndex in dirs)
		{
			Vector2Int d = dirsVect[dirIndex];
			int dist = (int)Mathf.Abs(Vector2.Dot(dir, d));
			Vector2Int point = Vector2Int.zero;
			for (int i = 1; i <= dist; i++)
			{
				if (!CheckPoint(position + d * i))
					break;
				point = d * i;
			}
			if (point != Vector2Int.zero)
			{
				points.Add(position + point);
			}
		}
		points.Sort((x, y) =>
		{
			return Vector2Int.Distance(x, target).CompareTo(Vector2Int.Distance(y, target));
		});
		result = points.ToArray();
		return points.Count > 0;
	}
}

[System.Serializable]
public class Chess_Queen : ChessLogicBase
{
	protected int maxDist = 10;
	public override byte hp => hps[4];
	public Chess_Queen()
	{
	}
	public Chess_Queen(int max)
	{
		maxDist = max;
	}
	public override bool CanAttack(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target)
	{
		bool b = GetResults(CheckPoint, position, target, out Vector2Int[] result);
		if (b)
		{
			foreach (var point in result)
			{
				if (point == target)
					return true;
			}
			return false;
		}
		else
			return false;
	}
	public override bool GetResults(Func<Vector2Int, bool> CheckPoint, Vector2Int position, Vector2Int target, out Vector2Int[] result)
	{
		Vector2Int dir = target - position;
		int[] dirs = GetDirs(dir);

		List<Vector2Int> points = new List<Vector2Int>();

		foreach (int dirIndex in dirs)
		{
			Vector2Int d = dirsVect[dirIndex];
			int dist = (int)Mathf.Abs(Vector2.Dot(dir, d));
			if (dirIndex % 2 == 1)
				dist /= 2;
			dist = Mathf.Min(dist, maxDist);
			Vector2Int point = Vector2Int.zero;
			for (int i = 1; i <= dist; i++)
			{
				if (!CheckPoint(position + d * i))
					break;
				point = d * i;
			}
			if (point != Vector2Int.zero)
			{
				points.Add(position + point);
			}
		}
		points.Sort((x, y) =>
		{
			return Vector2Int.Distance(x, target).CompareTo(Vector2Int.Distance(y, target));
		});
		result = points.ToArray();
		return points.Count > 0;
	}
}
[System.Serializable]
public class Chess_King : Chess_Queen
{
	public override byte hp => hps[5];
	public Chess_King():base(1)
	{}
}