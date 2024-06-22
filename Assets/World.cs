using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

	public bool first;

    bool win;

	public GameObject present_pref;

	public GameObject present;

	public string present_text;

	GameManager GM;

	public Animator RingAnim;

	[SerializeField] GameObject Ammo;

	public int AmmoDelay;

	int wstep;

	bool canstep = true;

	Vector3 wpos;

	Vector2 input;

	public string axisName = "PlayerAxis";

	Figure[,] elements = new Figure[8, 8];

	Vector3Int direction_move;

	public Transform CircPoint;

	public List<Figure> nextcan;

	public Dictionary<Figure.Team, List<Figure>> TeamFigures = new Dictionary<Figure.Team, List<Figure>>()
	{
		{ Figure.Team.Team1,new List<Figure>() },
		{ Figure.Team.Team2,new List<Figure>() }
	};
	public KingPlayer playerKing;
	Plane plane;
	public int currentError = 0;
	public int maxError = 3;

	public ApplicationStruct.EventInt fail;

	public Transform board;

	public GameObject TableBoxBlack;

	public GameObject TableBoxWhite;

	public Transform[,] TableBoxes = new Transform[8,8];

	public void AINextStep(byte max)
    {
		nextcan = new List<Figure>(TeamFigures[Figure.Team.Team1]);
		nextcan.RemoveAll((x) => x.team == Figure.Team.Team2 || !x.CanStep() || !x.isActiveAndEnabled || x.NextStep || x.dead);
		int trys = 2;
		int c = nextcan.Count;
		while (nextcan.Count > max && trys-- > 0)
		{
			nextcan.RemoveAll((x) => {
				bool b = c > max && Random.value > 0.5f;
				if (b) c--;
				return b;
			});
		}
		List<Figure> can = new List<Figure>(TeamFigures[Figure.Team.Team1]);
		foreach (Figure f in can)
		{
			f.NextStep = false;
		}
		foreach (Transform box in TableBoxes)
		{
			box.GetComponent<TableBox>().reset_color();
		}
		foreach (Figure f in nextcan)
		{
			f.NextStep = true;
			Vector3 p = f.transform.position;
			Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
			p = f.target.position;
			foreach (Transform box in TableBoxes)
			{
				Vector2Int target = new Vector2Int(Mathf.RoundToInt(box.position.x), Mathf.RoundToInt(box.position.z));
				TableBox tempb = box.GetComponent<TableBox>();
				if (f.logic.CanAttack(CheckPos, pos, target))
				{
					if (!tempb.red)
					{
						if (CheckPos(target))
						{
							tempb.red = true;
							box.GetComponent<TableBox>().red_color();
						}
					}
				}
			}
		}
	}

	public void AIStep()
	{
        if (win)
        {
			if (GM.clicktonext == false)
				//GM.EndStep(null);
			return;
		}
		List<Figure> can = new List<Figure>(TeamFigures[Figure.Team.Team1]);
		if (can.Count<1 || can.Find(x => x.type == Figure.LogicType.King) == null)
        {
			foreach (Figure f in can)
			{
				f.hp = 0;
			}
			/*present = Instantiate(present_pref, GM.CurLevel.transform);
			present.GetComponentInChildren<Present>().world = this;*/
			pwin();
			win = true;
			//GM.EndStep(null);
			return;
		}
		bool AIcanmove = false;
		foreach (Figure f in nextcan)
		{
			Vector3 p = f.transform.position;
			Vector2Int pos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
			p = f.target.position;
			Vector2Int target = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
			if (f.type == Figure.LogicType.Pawn)
            {
				if (f.logic.GetResults(CheckPosPawn, pos, target, out Vector2Int[] points))
				{
					AIcanmove = true;
				}
			}
            else
            {
				if (f.logic.GetResults(CheckPos, pos, target, out Vector2Int[] points))
				{
					AIcanmove = true;
				}
			}
			if (f.logic.CanAttack(CheckPos, pos, target))
			{
				if (!f.dead)
				{
					GM.CircleMove.enabled = false;
					GM.CircleShot.enabled = false;
					StartCoroutine(MatWait(Vector3.Normalize(f.transform.position - playerKing.transform.position), f));
				}
				return;
			}
		}

        if (!AIcanmove)
        {
			GM.EndStep(null);
		}

		foreach(Figure f in nextcan)
		{
			if (!f.dead)
			{
				if (f.type == Figure.LogicType.Pawn)
				{
					f.Step(CheckPosPawn);
				}
				else
				{
					f.Step(CheckPos);
				}
			}
		}
	}

	public void shahmat()
    {
		GM.CircleMove.enabled = false;
		GM.CircleShot.enabled = false;
		StartCoroutine(shahmati());
	}

	IEnumerator shahmati()
	{
		Instantiate(GM.DeadCirkle, playerKing.transform);
		yield return new WaitForSeconds(1.5f);
		GM.dead = 1;
		playerKing.DEAD();
		Rigidbody[] rbs = playerKing.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in rbs)
		{
			rb.isKinematic = false;
			rb.AddForce(Vector3.up * 3, ForceMode.VelocityChange);
		}
	}

	public void pwin()
    {
		GM.CircleMove.enabled = false;
		GM.CircleShot.enabled = false;
		GM.lerncirkle.SetActive(false);
		GM.CircleShot.gameObject.SetActive(false);
		GM.CircleShot.enabled = false;
		GM.Arrow.SetActive(false);
		StartCoroutine(Win());
	}

	public void endscenegame()
    {
		//present.GetComponentInChildren<Present>().present_hide();
		StartCoroutine(Iendscenegame());
	}

	IEnumerator Iendscenegame()
    {
		//yield return new WaitForSeconds(1.0f);
		playerKing.anim.SetBool("End", true);
		yield return new WaitForSeconds(1.0f);
		RingAnim.SetBool("End", true);
        foreach (Transform box in TableBoxes)
        {
            box.GetComponent<TableBox>().anim.enabled = false;
            box.GetComponent<TableBox>().anim.SetBool("End", true);
            box.GetComponent<TableBox>().timer = 0;
        }
        foreach (GameObject a in GameObject.FindGameObjectsWithTag("Ammo"))
        {
            Destroy(a);
        }
		yield return new WaitForSeconds(2.0f);
		GM.createlevel(false);
	}

	IEnumerator Win()
	{
		//playerKing.anim.SetBool("End", true);
		//yield return new WaitForSeconds(1.0f);
		//RingAnim.SetBool("End", true);
/*		foreach (Transform box in TableBoxes)
		{
			box.GetComponent<TableBox>().anim.enabled = false;
			box.GetComponent<TableBox>().anim.SetBool("End", true);
			box.GetComponent<TableBox>().timer = 0;
		}*/
		/*foreach (GameObject a in GameObject.FindGameObjectsWithTag("Ammo"))
        {
			Destroy(a);
        }*/
		yield return new WaitForSeconds(0.25f);
		if (GM.clicktonext == false)
		{
			float count = GM.CurLevelMoney;
			GameObject temp;
			Vector3 moneypos = Vector3.zero;// Camera.main.transform.position - Vector3.up * 20f - Vector3.back * 8;
			for (int i = 0; i < count; i++)
			{
				temp = Instantiate(GM.CM.Coin, gameObject.transform);
				temp.transform.localPosition = moneypos;
				//temp.transform.parent = Camera.main.transform;
				float angle = Random.Range(0, Mathf.PI*2f);
				float dist = Random.Range(0, 3.5f);
				temp.GetComponent<money>().selftarget = temp.transform.localPosition + new Vector3(Mathf.Cos(angle) * dist, 0, Mathf.Sin(angle) * dist) + Vector3.up * 2;
			}
			GM.clicktonext = true;
			GM._FinishSpray1.Play();
			GM._FinishSpray2.Play();
		}
	}


	IEnumerator MatWait(Vector3 pow, Figure f)
	{
		Instantiate(GM.DeadCirkle, f.transform);
		GM.CircleMove.enabled = false;
		GM.CircleShot.enabled = false;
		yield return new WaitForSeconds(0.5f);
		GM.CircleMove.enabled = false;
		GM.CircleShot.enabled = false;
		f.StepMat();
		yield return new WaitForSeconds(1.0f);
		GM.CircleMove.enabled = false;
		GM.CircleShot.enabled = false;
		GM.dead = 1;
		playerKing.DEAD();

	}


	public void StartAINextStep()
    {
		AINextStep((byte)Random.Range(1, 5));
		GM.EndStep(null);
		if (first)
		{
			GM.CircleShot.gameObject.SetActive(false);
		}
	}

	public bool InPlace(Vector2Int pos)
	{
		return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
	}

	public bool CheckPos(Vector2Int pos)
	{
		return InPlace(pos) && (elements[pos.x, pos.y] == null || elements[pos.x, pos.y].team == Figure.Team.Team2);
	}

	public bool CheckPosPawn(Vector2Int pos)
	{
		return InPlace(pos) && (elements[pos.x, pos.y] == null);
	}

	public bool Set(Vector2Int pos,Figure element)
	{
		bool b = element == null || CheckPos(pos);
		if (b)
			elements[Mathf.Clamp(pos.x, 0, 7) , Mathf.Clamp(pos.y, 0, 7)] = element;
		return b;
	}
	public bool Get(Vector2Int pos, out Figure element)
	{
		if (InPlace(pos) && elements[pos.x, pos.y] != null)
		{
			element = elements[pos.x, pos.y];
			return true;
		}
		else
		{
			element = null;
			return false;
		}
	}
	private void Start()
	{
		VirtualsInputs.VirtualAxis.AxisInput<Vector2> axis;
		if (VirtualInputManager.instance.inputs.GetVirtualInput(axisName, out axis))
		{
			axis.onAction += UpdateInput;
		}
		plane = new Plane(transform.up, transform.position);
		GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		if (playerKing == null)
			playerKing = TeamFigures[Figure.Team.Team2].Find((x) => x.team == Figure.Team.Team2) as KingPlayer;

		int odd = 1;
		for(int i = 0; i < 8; i++)
        {
			for (int k = 0; k < 8; k++)
			{
				if (odd == 1)
				{
					GameObject tempObj = Instantiate(TableBoxWhite, board);
					TableBoxes[k, i] = tempObj.transform;
                }
                else
                {
					GameObject tempObj = Instantiate(TableBoxBlack, board);
					TableBoxes[k, i] = tempObj.transform;
				}
				odd *= -1;
				TableBoxes[k, i].position = new Vector3(k, board.position.y, i);
			}
			odd *= -1;
		}
	}

	public void timetostartfigure()
    {
		int timestart = 50;
		List<Figure> can = new List<Figure>(TeamFigures[Figure.Team.Team1]);
		foreach (Figure f in can)
		{
			f.timetostart = timestart;
			timestart += 5;
		}
		timestart += 5;
		can = new List<Figure>(TeamFigures[Figure.Team.Team2]);
		foreach (Figure f in can)
		{
			f.timetostart = timestart;
			timestart += 5;
		}
	}

	private void OnDestroy()
	{
		VirtualsInputs.VirtualAxis.AxisInput<Vector2> axis;
		if (VirtualInputManager.instance.inputs.GetVirtualInput(axisName, out axis))
		{
			axis.onAction -= UpdateInput;
		}
	}

	void UpdateInput(Vector2 input, VirtualsInputs.AxisState state)
	{
		this.input = input;
		if (state == VirtualsInputs.AxisState.End)
		{
			this.input = Vector2.zero;
		}
	}

	public void ArrowCenterUp()
    {
		direction_move = new Vector3Int(0, 0, 1);
	}

	public void ArrowLeftUp()
	{
		direction_move = new Vector3Int(-1, 0, 1);
	}

	public void ArrowRightUp()
	{
		direction_move = new Vector3Int(1, 0, 1);
	}

	public void ArrowRightCenter()
	{
		direction_move = new Vector3Int(1, 0, 0);
	}

	public void ArrowRightDown()
	{
		direction_move = new Vector3Int(1, 0, -1);
	}

	public void ArrowLeftCenter()
	{
		direction_move = new Vector3Int(-1, 0, 0);
	}

	public void ArrowLeftDown()
	{
		direction_move = new Vector3Int(-1, 0, -1);
	}

	public void ArrowCenterDown()
	{
		direction_move = new Vector3Int(0, 0, -1);
	}

	public void cansteptrue()
    {
		canstep = true;
    }

	public void canstepfalse()
	{
		canstep = false;
	}

	public void Move()
	{
		Vector3 movevec = Vector3.Normalize( new Vector3(input.x, 0, input.y) );
		direction_move = new Vector3Int(Mathf.RoundToInt(movevec.x), 0, Mathf.RoundToInt(movevec.z));
		if (playerKing == null)
			playerKing = TeamFigures[Figure.Team.Team2].Find((x) => x.team == Figure.Team.Team2) as KingPlayer;
		CircPoint.position = playerKing.transform.position + direction_move;
		CircPoint.position = new Vector3(Mathf.Clamp(CircPoint.position.x, 0, 7), 0, Mathf.Clamp(CircPoint.position.z, 0, 7));
	}

	IEnumerator ShootWait()
	{
		yield return new WaitForSeconds(0.3f);
		List<Figure> can = new List<Figure>(TeamFigures[Figure.Team.Team1]);
		foreach (Figure f in can)
		{
            if (f.stephp > 0)
            {
				GM.createfigurehp(f.stephp, f.transform.position);
				f.stephp = 0;
			}
		}
		yield return new WaitForSeconds(0.4f);
		wstep = 12;
		AIWorldStep();
		//AIStep();
		//AINextStep((byte)Random.Range(0, 3));
		canstep = true;
	}

	public void Shoot()
	{
		if (!canstep)
			return;
		if (playerKing.Shoot(new Vector3(input.x, 0, input.y)))
		{
			if (first)
			{
				GM.lerncirkle.SetActive(false);
				GM.Arrow.SetActive(true);
				//first = false;
				Debug.Log("ага");
			}
			GM.AmmoTimer.sprite = GM.AmmoRespawnEmpty;
			canstep = false;
			GM.CircleMove.enabled = false;
			GM.CircleShot.enabled = false;
			GM.CircleShot.gameObject.SetActive(false);
			GM.dis_arrow_button(false);
			GM.CN.random_pos();
			StartCoroutine(ShootWait());
		}
	}

	public void Aim()
	{
		if (playerKing == null)
			playerKing = TeamFigures[Figure.Team.Team2].Find((x) => x.team == Figure.Team.Team2) as KingPlayer;
		playerKing.SetPosAim(new Vector3(input.x, 0, input.y));
	}

	public void AIWorldStep()
    {
		switch (wstep)
		{
			case -1:
				if ((wpos - playerKing.transform.position).magnitude > float.Epsilon)
				{
					currentError++;
					fail.Invoke(currentError);
				}
				break;
			case 0:
				if (playerKing.gun.ammo < GM.AmmoSlots.Length / 2)
				{
					AmmoDelay++;
					if (AmmoDelay > 2)
					{
						AmmoDelay = 0;
						GameObject temp = Instantiate(Ammo, GM.CurLevel.transform);
						temp.transform.position = new Vector3(Random.Range(0, 7), 0.1f, Random.Range(0, 7));
					}
					GM.AmmoTimerRespawn.fillAmount = (AmmoDelay/3f);
                }
                else
                {
					GM.AmmoTimerRespawn.fillAmount = 0f;

				}
                if (first)
                {
					if (playerKing.gun.ammo > 0)
					{
						GM.lerncirkle.SetActive(true);
						GM.CircleShot.gameObject.SetActive(true);
						GM.CircleShot.enabled = true;
						GM.Arrow.SetActive(false);
					}
				}
				//AIStep();
				//AINextStep((byte)Random.Range(0, 3));
				currentError = 0;
				fail.Invoke(currentError);
				break;
			case 12:
				if (first)
				{
					GM.lerncirkle.SetActive(false);
					GM.CircleShot.gameObject.SetActive(false);
					GM.CircleShot.enabled = false;
					GM.Arrow.SetActive(true);
				}
				AIStep();
				AINextStep((byte)Random.Range(0, 3));
				currentError = 0;
				fail.Invoke(currentError);
				break;
		}
	}

	public void WorldStep()
    {
		if (!canstep)
			return;
		if (currentError < maxError)
		{
			if (playerKing == null)
				playerKing = TeamFigures[Figure.Team.Team2].Find((x) => x.team == Figure.Team.Team2) as KingPlayer;
			Vector3 pos = new Vector3(playerKing.pos.x, 0, playerKing.pos.y) + direction_move;
			pos.x = Mathf.Clamp(Mathf.Round(pos.x), 0, 7);
			pos.y = Mathf.Round(pos.y);
			pos.z = Mathf.Clamp(Mathf.Round(pos.z), 0, 7);

			playerKing.SetPosTarget(pos);
			wstep = playerKing.PlayerStep(out Figure figure);
            if (wstep == 0)
            {
				if (playerKing.gun.ammo > 0)
				{
					GM.lerncirkle.SetActive(true);
					GM.Arrow.SetActive(false);
                }
                else
                {
					GM.CircleShot.gameObject.SetActive(false);
				}
			}
			wpos = pos;
		}
	}

}
