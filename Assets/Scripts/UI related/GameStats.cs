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
    private Text[] tScore = new Text[2];
    [SerializeField]
    private Text[] tCollectables = new Text[2];
    [SerializeField]
    private Text[] tEggs = new Text[2];
    [SerializeField]
    private Text[] tAttack = new Text[2];
    [SerializeField]
    private GameObject theUIPart;
  

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowStats()
    {

        theUIPart.SetActive(true);
        for (int i=0;i<2;i++)
        {
            score[i] = PlayerManager.instance.GetPlayer(i).GetScore();
            tScore[i].text = "Player " + (i+1) + " Scored "+score[i].ToString();
            tCollectables[i].text = "Player " + (i + 1) + " Collected " + collectables[i].ToString() + " Things";
            tEggs[i].text = "Player " + (i + 1) + " Collected " + eggs[i].ToString() + " Eggs";
            tAttack[i].text = "Player " + (i + 1) + "Viscously Attacked "+ "Player " + i + " "+attack[i].ToString()+" Times";
        }
    }
}