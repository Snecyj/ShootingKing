using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameManager GM;

    public TMP_Text coinlabel;

    public Transform Image;

    public GameObject Coin;

    private void Start()
    {
        GM.Money = PlayerPrefs.GetInt("Money", 0);
        coinlabel.text = GM.Money.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Image.localScale.x > 1.01f)
        {
            Image.localScale = Vector3.Lerp(Image.localScale, Vector3.one, 0.1f);
        }
    }

    public void addcoin()
    {
        GM.Money++;
        coinlabel.text = GM.Money.ToString();
        Image.localScale += Vector3.one * 0.05f;
    }
}
