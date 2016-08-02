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
  

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
  
    public void FadeInText(int index)
    {
        //do a thing
    }

    public void ShowStats()
    {
        flags.SetActive(true);

        if (PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore())
        {
            winnerFlags[0].enabled = false;
        }
        else
        {
            winnerFlags[1].enabled = false;
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