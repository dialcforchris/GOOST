using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameLogic : MonoBehaviour
{

    [SerializeField]
    GameObject gameOver;
    [SerializeField]
    Text winner;
    [SerializeField]
    Text timer;
    [SerializeField]
    private GameObject entername;
    [SerializeField]
    private GameObject statUI;
    float fade = 1;

    void Update()
    {
        AnnounceWinner();
        timer.text = "Game Over";
        StartCoroutine(DoAFade());
        //if (Input.anyKey)
        //{
        //    end = true;
        //}
        //if (end)
        //{ 
        //    if (statUI.gameObject.activeInHierarchy)
        //    {
        //        for (int i = 0; i < statUI.transform.childCount; i++)
        //        {
        //            fade -= Time.deltaTime/2;
        //            statUI.transform.GetChild(i).GetComponent<Text>().color =
        //                new Color(statUI.transform.GetChild(i).GetComponent<Text>().color.r,
        //                statUI.transform.GetChild(i).GetComponent<Text>().color.g, statUI.transform.GetChild(i).GetComponent<Text>().color.b, fade);
        //        }
        //        if (fade <= 0)
        //        {
        //            entername.SetActive(true);
        //            statUI.gameObject.SetActive(false);
        //        }

        //    }
        //}
    }
    IEnumerator DoAFade()
    {
        yield return new WaitForSeconds(2);

        while (!Input.anyKey)
        {
            if (Input.anyKey)
                break;
            yield return null;
        }
        if (statUI.gameObject.activeInHierarchy)
        {
            for (int i = 0; i < statUI.transform.childCount; i++)
            {
                fade -= Time.deltaTime / 2;
                statUI.transform.GetChild(i).GetComponent<Text>().color =
                    new Color(statUI.transform.GetChild(i).GetComponent<Text>().color.r,
                    statUI.transform.GetChild(i).GetComponent<Text>().color.g, statUI.transform.GetChild(i).GetComponent<Text>().color.b, fade);
            }
            if (fade <= 0)
            {
                entername.SetActive(true);
                statUI.gameObject.SetActive(false);
            }
        }
        yield return null;
    }
    void AnnounceWinner()
    {
        winner.gameObject.SetActive(true);
        if (PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore())
        {
            winner.text = "Player 1 Wins";
            StatTracker.instance.stats.ransomWins++;
        }
        else
        {
            winner.text = "Player 2 Wins";
            StatTracker.instance.stats.ITGuyWins++;
        }
    }
    //[SerializeField]
    //EnterName[] enterName;
    //public static EndGameLogic instance = null;

    //void Start()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}



    //public void EndGame()
    //{
    //    timer.SetActive(false);
    //    gameOver.SetActive(true);
    //    string win;
    //    if (PlayerManager.instance.NumberOfPlayers() > 1)
    //        win = PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore() ? "THE HACKER " : "THE I.T GUY ";
    //    else
    //        win = "THE HACKER ";
    //    winner.text = win + "Wins";
    //    for (int i = 0; i < PlayerManager.instance.NumberOfPlayers(); i++)
    //    {
    //        enterName[i].gameObject.SetActive(true);
    //        enterName[i].EnableIt(i);
    //    }
    //}
}
