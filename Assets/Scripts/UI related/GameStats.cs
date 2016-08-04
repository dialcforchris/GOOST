using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStats : MonoBehaviour
{
    public static GameStats instance = null;

    //player stats
    public int[] score = new int[2];
    public int[] collectables = new int[2];
    public int[] eggs = new int[2];
    public int[] attack = new int[2];
    public int[] flaps = new int[2];

    //UI shit
    [SerializeField]
    GameObject flags;
    [SerializeField]
    SpriteRenderer[] winnerFlags;
    [SerializeField]
    private Text[] tScore = new Text[2];
    [SerializeField]
    private Text[] tCollectables = new Text[2];
    [SerializeField]
    private Text[] tEggs = new Text[2];
    [SerializeField]
    private Text[] tAttack = new Text[2];
    [SerializeField]
    private GameObject theUIPart;
    [SerializeField]
    private Text press;
    float fade = 1;

    public SpriteRenderer[] WinnerFlags
    {
        get
        {
            return winnerFlags;
        }
        set
        {
            winnerFlags = value;
        }
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
  
    public void RevealText(int index)
    {
        //do a thing
        switch (index)
        {
            case 0:
                for (int i = 0; i < 2; i++)
                {
                    score[i] = PlayerManager.instance.GetPlayer(i).GetScore();
                    tScore[i].text = score[i] + "";
                    tCollectables[i].text = collectables[i] + "";
                    tEggs[i].text = eggs[i] + "";
                    tAttack[i].text = attack[i] + "";
                }
                StartCoroutine(fadeInText(tAttack[0], tAttack[0].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tAttack[1], tAttack[1].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tAttack[0].GetComponentsInParent<Text>()[1], tAttack[0].GetComponentsInParent<Outline>()[1]));
                break;
            case 1:
                StartCoroutine(fadeInText(tEggs[0], tEggs[0].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tEggs[1], tEggs[1].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tEggs[0].GetComponentsInParent<Text>()[1], tEggs[0].GetComponentsInParent<Outline>()[1]));
                break;
            case 2:
                StartCoroutine(fadeInText(tCollectables[0], tCollectables[0].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tCollectables[1], tCollectables[1].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tCollectables[0].GetComponentsInParent<Text>()[1], tCollectables[0].GetComponentsInParent<Outline>()[1]));
                break;
            case 3:
                StartCoroutine(fadeInText(tScore[0], tScore[0].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tScore[1], tScore[1].GetComponent<Outline>()));
                StartCoroutine(fadeInText(tScore[0].GetComponentsInParent<Text>()[1], tScore[0].GetComponentsInParent<Outline>()[1]));
                break;
        }
    }

    public void ResetText()
    {
        //Hi Shaun
        StartCoroutine(fadeInText(tAttack[0], tAttack[0].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tAttack[1], tAttack[1].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tAttack[0].GetComponentsInParent<Text>()[1], tAttack[0].GetComponentsInParent<Outline>()[1], 5, false));
        StartCoroutine(fadeInText(tEggs[0], tEggs[0].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tEggs[1], tEggs[1].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tEggs[0].GetComponentsInParent<Text>()[1], tEggs[0].GetComponentsInParent<Outline>()[1], 5, false));
        StartCoroutine(fadeInText(tCollectables[0], tCollectables[0].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tCollectables[1], tCollectables[1].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tCollectables[0].GetComponentsInParent<Text>()[1], tCollectables[0].GetComponentsInParent<Outline>()[1], 5, false));
        StartCoroutine(fadeInText(tScore[0], tScore[0].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tScore[1], tScore[1].GetComponent<Outline>(), 5, false));
        StartCoroutine(fadeInText(tScore[0].GetComponentsInParent<Text>()[1], tScore[0].GetComponentsInParent<Outline>()[1], 5, false));
    }

    IEnumerator fadeInText(Text t, Outline o, float speed = 5, bool inOut = true)
    {
        Color TextCol;
        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * speed;
            TextCol = t.color;
            TextCol.a = Mathf.Lerp(inOut ? 0 : 1, inOut ? 1 : 0, lerpy);
            t.color = TextCol;
            TextCol = o.effectColor;
            TextCol.a = Mathf.Lerp(inOut ? 0 : 1, inOut ? 1 : 0, lerpy);
            o.effectColor = TextCol;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ShowStats()
    {
        flags.SetActive(true);

        if (PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore())
        {
            WinnerFlags[0].enabled = false;
        }
        else
        {
            WinnerFlags[1].enabled = false;
        }
        theUIPart.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            string colour = i == 0 ? "orange" : "green";
            score[i] = PlayerManager.instance.GetPlayer(i).GetScore();
            tScore[i].text = "<color=" + colour + ">" + score[i].ToString() + "</color>";
            //   tScore[i].color = colour; 
            tCollectables[i].text = "<color=" + colour + ">" + collectables[i].ToString() + "</color>";
            // tCollectables[i].color = colour; 
            tEggs[i].text = "<color=" + colour + ">" + eggs[i].ToString() + "</color>";
            //tEggs[i].color = colour;
            tAttack[i].text = "<color=" + colour + ">" + attack[i].ToString() + "</color>";// +" Times";
            //tAttack[i].color = colour;
        }
    }
}