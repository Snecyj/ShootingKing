using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject lerncirkle;

    public GameObject GreeyAndRay;

    public GameObject DeadCirkle;

    public Transform _canvas;

    public GameObject FigureHP;

    public Button[] ArrowButtons;

    public GameObject Arrow;

    public bool AskArrow;

    public World W;

    public GameObject cpoint;

    public int dead;

    public PlayerControlUI CircleShot, CircleMove;

    public Image[] AmmoSlots;
    public Sprite AmmoFull, AmmoEmpty, AmmoRespawnFull, AmmoRespawnEmpty;
    public Image AmmoTimer;

    public Image AmmoTimerRespawn;

    public int Money;

    public int CurLevelMoney;

    public int IndLevel;
    public GameObject[] Levels;
    public GameObject CurLevel;

    public int ccid_levels;

    int lastlvl;
    int lastlastlvl;

    public ParticleSystem _FinishSpray1;
    public ParticleSystem _FinishSpray2;

    public CoinManager CM;

    public float timer;

    public float timetonext;

    float timertonext;

    public bool next;

    public bool clicktonext;

    public TMP_Text endtext;

    public TMP_Text endtextpresent;

    public TMP_Text Timer;

    public GameObject[] Win;

    public GameObject[] Lost;

    public GameObject[] StartButton;

    public TMP_Text LevelLabel;

    public GameObject ProgressBarBack;
    public GameObject ProgressBarForward;
    public GameObject ProgressBarLine;

    public GameObject CoinLabel;
    public GameObject CoinImage;
    public GameObject CoinBack;

    public GameObject EndLevelLabel;

    public cam_noise CN;

    public void clicktonexttrue()
    {
        clicktonext = true;
    }

    public void nextfalse()
    {
        next = false;
    }

    public void createfigurehp(int coun, Vector3 position)
    {
        GameObject temp = Instantiate(FigureHP, _canvas);
        temp.GetComponent<FigureHPScript>().Tmp.text = "-" + coun.ToString();
        temp.transform.position = transform.position = Camera.main.WorldToScreenPoint(position + Vector3.up * 2f); ;
    }

    public void createlevel(bool first)
    {
        _FinishSpray1.Stop();
        _FinishSpray2.Stop();
        AmmoTimer.fillAmount = 1f;
        CurLevelMoney = 0;
        foreach (GameObject a in Win)
        {
            a.SetActive(false);
        }
        foreach (GameObject a in Lost)
        {
            a.SetActive(false);
        }
        ProgressBarBack.SetActive(true);
        ProgressBarForward.SetActive(true);
        ProgressBarLine.SetActive(true);
        timer = 0;
        if (!first)
        {
            PlayerPrefs.SetInt("IndLevel", IndLevel);
            PlayerPrefs.SetInt("lastlvl", lastlvl);
            PlayerPrefs.SetInt("lastlastlvl", lastlastlvl);
            PlayerPrefs.SetInt("dead", dead);
            PlayerPrefs.SetInt("Money", Money);
            Destroy(CurLevel);
        }
        Random.InitState(ccid_levels);
        int indc;
        if (dead > 0)
        {
            CurLevel = Instantiate(Levels[lastlvl]);
        }
        else
        {
            if (IndLevel >= Levels.Length)
            {

                indc = Random.Range(0, Levels.Length - 1);
                while (indc == lastlvl || indc == lastlastlvl)
                {
                    indc = Random.Range(0, Levels.Length - 1);
                }
                lastlastlvl = lastlvl;
                lastlvl = indc;
                CurLevel = Instantiate(Levels[indc]);
            }
            else
            {
                lastlastlvl = lastlvl;
                lastlvl = Mathf.Clamp(IndLevel, 0, Levels.Length - 1);
                CurLevel = Instantiate(Levels[Mathf.Clamp(IndLevel, 0, Levels.Length - 1)]);
            }
        }
        W = CurLevel.GetComponentInChildren<World>();
        cpoint = W.CircPoint.gameObject;
        dead = 0;
        if (!first)
        {
            timertonext = 0;
            CurLevelMoney = 0;
            next = false;
            //ProgressBarLine.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
        }
        LevelLabel.gameObject.SetActive(true);
        LevelLabel.text = "Level " + (IndLevel + 1).ToString();
    }

    public void StartGame()
    {
        CoinImage.SetActive(true);
        CoinLabel.SetActive(true);
        CoinBack.SetActive(true);
        if (AskArrow)
        {
            Arrow.SetActive(true);
        }
        else
        {
            CircleMove.gameObject.SetActive(true);
            CircleMove.enabled = true;
        }
        foreach (Image i in AmmoSlots)
        {
            i.sprite = AmmoFull;
            i.gameObject.SetActive(true);
        }
        foreach (GameObject a in StartButton)
        {
            a.SetActive(false);
        }
        foreach (Image i in AmmoSlots)
        {
            i.enabled = true;
            i.sprite = AmmoFull;
        }
        CircleShot.enabled = true;
        CircleShot.gameObject.SetActive(true);
        AmmoTimer.gameObject.SetActive(true);
        AmmoTimerRespawn.gameObject.SetActive(true);
        AmmoTimerRespawn.fillAmount = 0f;
        AmmoTimer.enabled = true;
        W.StartAINextStep();
    }

    public void ShowStartButton()
    {
        foreach (GameObject a in StartButton)
        {
            a.SetActive(true);
        }
    }

    void Start()
    {
        IndLevel = PlayerPrefs.GetInt("IndLevel", 0);
        lastlvl = PlayerPrefs.GetInt("lastlvl", 0);
        lastlastlvl = PlayerPrefs.GetInt("lastlastlvl", 0);
        dead = PlayerPrefs.GetInt("dead", 0);
        createlevel(true);
    }

    public void dis_arrow_button(bool a)
    {
        foreach(Button b in ArrowButtons)
        {
            b.enabled = a;
        }
    }

    public void endscenegame()
    {
        timertonext = 0;
        clicktonext = false;
        GreeyAndRay.SetActive(false);
        foreach (GameObject a in Win)
        {
            a.SetActive(false);
        }
        W.endscenegame();
        IndLevel++;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (clicktonext)
        {
            timertonext++;
            if (timertonext > timetonext)
            {
                GreeyAndRay.SetActive(true);
                //endtextpresent.text = W.present_text;
                LevelLabel.gameObject.SetActive(false);
                ProgressBarBack.SetActive(false);
                ProgressBarForward.SetActive(false);
                ProgressBarLine.SetActive(false);
                CoinImage.SetActive(false);
                CoinLabel.SetActive(false);
                CoinBack.SetActive(false);
                _FinishSpray1.Stop();
                _FinishSpray2.Stop();
                CircleMove.gameObject.SetActive(false);
                CircleShot.gameObject.SetActive(false);
                CircleMove.enabled = false;
                CircleShot.enabled = false;
                Arrow.SetActive(false);
                AmmoTimer.gameObject.SetActive(false);
                AmmoTimerRespawn.gameObject.SetActive(false);
                foreach (Image i in AmmoSlots)
                {
                    i.enabled = false;
                }
                GameObject[] tempcoin = GameObject.FindGameObjectsWithTag("Coin");
                foreach (GameObject an in tempcoin)
                {
                    Destroy(an);
                    Money++;
                }
                CM.coinlabel.text = Money.ToString();
                endtext.text = (CurLevelMoney).ToString();
                foreach (GameObject a in Win)
                {
                    a.SetActive(true);
                }
            }
        }

        if (dead > 0)
        {
            timertonext++;
            if (timertonext == 80)
            {
                LevelLabel.gameObject.SetActive(false);
                ProgressBarBack.SetActive(false);
                ProgressBarForward.SetActive(false);
                ProgressBarLine.SetActive(false);
                CoinImage.SetActive(false);
                CoinLabel.SetActive(false);
                CoinBack.SetActive(false);
                CircleMove.gameObject.SetActive(false);
                CircleShot.gameObject.SetActive(false);
                CircleMove.enabled = false;
                CircleShot.enabled = false;
                Arrow.SetActive(false);
                AmmoTimer.gameObject.SetActive(false);
                AmmoTimer.sprite = AmmoRespawnFull;
                AmmoTimerRespawn.gameObject.SetActive(false);
                foreach (Image i in AmmoSlots)
                {
                    i.enabled = false;
                }
                foreach (GameObject a in Lost)
                {
                    a.SetActive(true);
                }
                GameObject[] tempcoin = GameObject.FindGameObjectsWithTag("Coin");
                foreach (GameObject an in tempcoin)
                {
                    Destroy(an);
                }
            }
        }
    }

    public void StartAim()
    {
        W.playerKing.AIM_Triangle.SetActive(true);
        Arrow.SetActive(false);
    }

    public void EndAim()
    {
        W.playerKing.AIM_Triangle.SetActive(false);
        if (!W.first)
        {
            Arrow.SetActive(true);
        }
    }

    public void Aim()
    {
        W.Aim();
    }

    public void Shoot()
    {
        if (AskArrow)
        {
            //Arrow.SetActive(true);
        }
        W.Shoot();
        W.playerKing.AIM_Triangle.SetActive(false);
    }

    public void StartMove()
    {
        cpoint.SetActive(true);
    }

    public void Move()
    {
        W.Move();
    }

    public void EndMove()
    {
        cpoint.SetActive(false);
        W.WorldStep();
    }

    public void ArrowCenterUp()
    {
        W.ArrowCenterUp();
    }

    public void ArrowLeftUp()
    {
        W.ArrowLeftUp();
    }

    public void ArrowRightUp()
    {
        W.ArrowRightUp();
    }

    public void ArrowRightCenter()
    {
        W.ArrowRightCenter();
    }

    public void ArrowRightDown()
    {
        W.ArrowRightDown();
    }

    public void ArrowLeftCenter()
    {
        W.ArrowLeftCenter();
    }

    public void ArrowLeftDown()
    {
        W.ArrowLeftDown();
    }

    public void ArrowCenterDown()
    {
        W.ArrowCenterDown();
    }

    public void AmmoUpdate (int count)
    {
        for(int i = 0; i < AmmoSlots.Length; i++)
        {
            if (i < count )
            {
                AmmoSlots[i].sprite = AmmoFull;
            }
            else
            {
                AmmoSlots[i].sprite = AmmoEmpty;
            }
        }
    }

    public void EndStep(Figure figure)
    {
        if (figure != null)
        {
            if (figure.team == Figure.Team.Team2)
            {
                CircleShot.enabled = false;
                CircleShot.gameObject.SetActive(false);
                CircleMove.enabled = false;
               // dis_arrow_button(false);
                W.cansteptrue();
                W.AIWorldStep();
            }
            else
            {
                //CircleShot.enabled = true;
                if (AskArrow)
                {
                    dis_arrow_button(true);
                }
                else
                {
                    CircleMove.enabled = true;
                    CircleMove.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            //CircleShot.enabled = true;
            if (AskArrow)
            {
                dis_arrow_button(true);
            }
            else
            {
                CircleMove.enabled = true;
                CircleMove.gameObject.SetActive(true);
            }
        }
    }

    public void StartStep(Figure figure)
    {
        if (figure.team == Figure.Team.Team2)
        {
            W.canstepfalse();
        }
        else
        {

        }
    }
}
