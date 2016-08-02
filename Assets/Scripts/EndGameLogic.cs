using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameLogic : MonoBehaviour
{


    [SerializeField]
    private GameObject entername;
    [SerializeField]
    private GameObject statUI;
    float fade = 1;
    bool end = false;
    void Update()
    {
        StartCoroutine(WaitForSecs());
        if (end)
        {
            StartCoroutine(DoAFade());
        }
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

    IEnumerator WaitForSecs()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(WaitForInput(Input.anyKey));
        
    }

    IEnumerator WaitForInput(bool anykey)
    {
      
      while(!anykey)
        {
            yield return null;
        }
        end = true; 
       
    }
    IEnumerator DoAFade()
    {
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
