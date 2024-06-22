using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Figure : MonoBehaviour
{
	[SerializeField] private protected GameObject ModelDead;
	[SerializeField] private protected Transform ModelLife;

	[SerializeField] MeshRenderer ModelLifeMR;

	Color StartColor;

	bool newfigure;

	bool nowstep;

	public int stephp;

	public int timetostart;

	int timer;

	public Animator anim;

	public GameObject[] Fig;
	Rigidbody rigidbody;

	Vector3 pow, lpos;

	public GameManager GM;

	public Team team = Team.Team1;
	byte _hp;

	[SerializeField] Collider collider;


	public World world;
	public ChessLogicBase logic;
	public LogicType type;

	public Transform target;

	public Vector2Int pos;

	public bool NextStep;

	public bool dead;

	public enum LogicType
	{
		Pawn,
		Horse,
		Bishop,
		Rook,
		Queen,
		King
	}

	public enum Team
	{
		Team1,
		Team2
	}

	public byte hp
	{
		get
		{
			return _hp;
		}
		set
		{
			_hp = value;
			if (_hp < 1)
			{
				world.TeamFigures[team].Remove(this);
				world.Set(pos, null);
				NextStep = false;
				dead = true;
				collider.enabled = false;
				if (type == LogicType.Pawn) { GM.CurLevelMoney += 10; }
				if (type == LogicType.Horse) { GM.CurLevelMoney += 50; }
				if (type == LogicType.Bishop) { GM.CurLevelMoney += 50; }
				if (type == LogicType.Rook) { GM.CurLevelMoney += 50; }
				if (type == LogicType.Queen) { GM.CurLevelMoney += 100; }
				if (type == LogicType.King) { GM.CurLevelMoney += 100; }
				foreach (Transform tr in ModelLife.GetComponentInChildren<Transform>())
				{
					Destroy(tr.gameObject);
				}
				Instantiate(ModelDead, ModelLife);
				rigidbody.isKinematic = true;
				collider.enabled = false;
				Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody rb in rbs)
				{
					rb.AddForce(pow, ForceMode.VelocityChange);
				}
				if (stephp > 0)
				    GM.createfigurehp(stephp, transform.position);
				Invoke("SelfDestroy", 2);
				return;
			}
		}
	}

	void SelfDestroy()
	{
		GM.EndStep(this);
		Destroy(gameObject);
	}

	public void AddPower(Vector3 point,Vector3 power)
	{
		pow = power;
		lpos = point;
	}

	private void OnValidate()
	{
		if (TryGetComponent(out rigidbody))
		{
			rigidbody.isKinematic = true;
		}
		else
		{
			rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
		}
		switch (type)
		{
			case LogicType.Pawn:
				logic = new Chess_Pawn();
				break;
			case LogicType.Horse:
				logic = new Chess_Horse();
				break;
			case LogicType.Bishop:
				logic = new Chess_Bishop();
				break;
			case LogicType.Rook:
				logic = new Chess_Rook();
				break;
			case LogicType.Queen:
				logic = new Chess_Queen();
				break;
			case LogicType.King:
				logic = new Chess_King();
				break;
		}
		timer = 0;
		timetostart = 1000;
	}
	private void OnDrawGizmos()
	{
		if (this.target == null)
			return;
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		p = this.target.position;
		Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		if (logic.GetResults(world.CheckPos, pos,target,out Vector2Int[] points))
		{
			int i = points.Length;
			foreach (var point in points)
			{
				Gizmos.DrawWireSphere(new Vector3(point.x, 0, point.y), .1f + .4f * (((float)i--) / points.Length));
			}
		}
	}

	private void Start()
	{
		OnValidate();
		GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		Vector3 p = transform.position;
		pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		world.Set(pos, this);
		world.TeamFigures[team].Add(this);
		hp = logic.hp;
		rigidbody.isKinematic = true;
		timer = 0;
		timetostart = 1000;
        if (timetostart > 999)
        {
			world.timetostartfigure();
        }
        if (newfigure)
        {
			timetostart = 0;
		}
		StartColor = ModelLifeMR.material.color;
	}
	private void OnDestroy()
	{
		world.TeamFigures[team].Remove(this);
		if (world.Get(pos,out Figure figure) && figure == this)
		{
			world.Set(pos, null);
		}
	}

	public bool CanStep()
	{
		if (this.target == null)
			return false;
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		p = this.target.position;
		Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		return logic.GetResults(world.CheckPos, pos, target, out Vector2Int[] points);
	}
	public bool CanStep(out Vector2Int[] points)
	{
		if (this.target == null)
		{
			points = new Vector2Int[0];
			return false;
		}
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		p = this.target.position;
		Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		return logic.GetResults(world.CheckPos, pos, target, out points);
	}

	[ContextMenu("Step")]

	IEnumerator StepMini(Vector3 p)
	{
		int i = 0;
		float dist = Vector3.Distance(transform.position, p);
		Vector3 tempp = transform.position;
		while (Vector3.Distance(transform.position, p) > 0.1f)
        {
			transform.position += (new Vector3(p.x, 0, p.z) - new Vector3(tempp.x, 0, tempp.z)) * 0.025f;
			transform.position = new Vector3(transform.position.x, Mathf.Sin(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(p.x,p.z)) / dist * Mathf.PI) * 1.5f, transform.position.z);
			yield return new WaitForSeconds(0.01f);
			i++;
			if (i > 100)
            {
				break;
            }
		}
		GM.EndStep(this);
		if (type == LogicType.Pawn)
		{
			Vector2Int pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            if (pos.y <= 0)
            {
				GameObject temp = Instantiate(Fig[UnityEngine.Random.Range(0, Fig.Length-1)], GM.CurLevel.transform);
				temp.transform.position = this.transform.position;
				temp.GetComponent<Figure>().world = world;
				temp.GetComponent<Figure>().target = target;
				//temp.GetComponent<Figure>().anim.transform.position = Vector3.zero;
				temp.GetComponent<Figure>().newfigure = true;
				SelfDestroy();
            }
        }
		nowstep = false;
	}

	IEnumerator StepMiniMat(Vector3 p)
	{
		int i = 0;
		float dist = Vector3.Distance(transform.position, p);
		Vector3 tempp = transform.position;
		while (Vector3.Distance(transform.position, p) > 0.02f)
		{
			transform.position += (new Vector3(p.x, 0, p.z) - new Vector3(tempp.x, 0, tempp.z)) * 0.01f;
			transform.position = new Vector3(transform.position.x, Mathf.Sin(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(p.x, p.z)) / dist * Mathf.PI) * 1.5f, transform.position.z);
			yield return new WaitForSeconds(0.01f);
			i++;
			if (i > 1000)
			{
				break;
			}
		}
		nowstep = false;
	}

	public void Step(Func<Vector2Int, bool> CheckPoint)
	{
		if (this.target == null)
			return;
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		p = this.target.position;
		Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		if (logic.GetResults(CheckPoint, pos, target, out Vector2Int[] points))
		{
			if (world.Set(points[0], this))
			{
				world.Set(this.pos, null);
				this.pos = points[0];
				p.x = points[0].x;
				p.z = points[0].y;
				GM.StartStep(this);
				StartCoroutine(StepMini(p));
				nowstep = true;
				pow = Vector3.zero;
				lpos = Vector3.zero;
			}
		}
	}

	public void StepMat()
	{
		if (this.target == null)
			return;
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		p = this.target.position;
		Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		if (world.Set(target, this))
		{
			world.Set(this.pos, null);
			this.pos = target;
			p.x = target.x;
			p.z = target.y;
			GM.StartStep(this);
			nowstep = true;
			StartCoroutine(StepMiniMat(p));
			pow = Vector3.zero;
			lpos = Vector3.zero;
		}
	}
	public bool Undercover(params Vector2Int[] points)
	{
		Vector3 p = transform.position;
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
		foreach (Vector2Int point in points)
		{
			if (logic.CanAttack(world.CheckPos, pos, point))
				return true;
		}
		return false;
	}

	public void BulletRed()
    {
		ModelLifeMR.material.color = Color.red;
		Invoke("ResetColor", 0.17f);
	}

    void ResetColor()
    {
		if(ModelLifeMR != null)
		    ModelLifeMR.material.color = StartColor;
	}

    public void FixedUpdate()
    {
		if (!nowstep)
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, transform.position.y, pos.y), 0.25f);
		}
		if (NextStep)
        {
			transform.position += Vector3.right * (Mathf.Sin(Time.fixedTime * 25) * 0.025f);
        }
		if (timer > timetostart)
		{
            if (anim != null)
            {
				if (team == Team.Team2 && anim.enabled == false)
				{
					GM.ShowStartButton();
				}
				anim.enabled = true;
			}
		}
		else
		{
			timer++;
		}

    }
}
