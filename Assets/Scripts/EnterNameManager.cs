using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterNameManager : MonoBehaviour
{
    public static EnterNameManager instance = null;
    [SerializeField]
    GameObject gameOver;
    [SerializeField]
    Text winner;
    [SerializeField]
    GameObject timer;
    [SerializeField]
    private EnterName[] enterNames;
    [SerializeField]
    bool[] done = new bool[2];
	
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
   	// Update is called once per frame
	void Update ()
    {
	    if (GameStateManager.instance.GetState()==GameStates.STATE_GAMEOVER)
        {
            gameOver.SetActive(true);
            timer.SetActive(false);
            ShowWinner();

           
            for (int i=0;i<PlayerManager.instance.NumberOfPlayers();i++)
            {
                if (!enterNames[i].check)
                {
                    enterNames[i].gameObject.SetActive(true);
                    enterNames[i].EnableIt(i);
                }
            }
            if (PlayerManager.instance.NumberOfPlayers()<done.Length)
            {
                done[PlayerManager.instance.NumberOfPlayers()+1] = true;
            }

            if (done[0]&&done[1])
            {
                SceneManager.LoadScene(0);
            }
        }
	}
    public void Done(int playerIndex)
    {
        done[playerIndex] = true;
    }
    void ShowWinner()
    {
        string win = "";
        if (PlayerManager.instance.NumberOfPlayers() > 1)
            win = PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore() ? "THE HACKER " : "THE I.T GUY ";
        else
            win = "THE HACKER ";

        winner.text = win + "Wins";
    }
}
