using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterNameManager : MonoBehaviour
{
    public static EnterNameManager instance = null;
   
    [SerializeField]
    private EnterName[] enterNames;
    [SerializeField]
    private GameObject enterCanvas;
    [SerializeField]
    bool[] done = new bool[2];
    public bool ended = false;
	
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
        if (GameStateManager.instance.GetState()==GameStates.STATE_MENU)
        {
            EnterNameReset();
        }
        ended = (done[0] & done[1]);
        if (ended)
        {
            for (int i = 0; i < enterNames.Length - 1; i++)
            {
                enterNames[i].gameObject.SetActive(false);
            }
            enterCanvas.SetActive(false);
        }
    }
    public void ShowEnterName()
    {
        EnterNameReset();
        enterCanvas.SetActive(true);
        for (int i = 0; i < PlayerManager.instance.NumberOfPlayers(); i++)
        {

            if (PlayerManager.instance.GetPlayer(i).gameObject.activeInHierarchy)
            {
                if (!enterNames[i].check)
                {
                    if (LeaderBoard.instance.CheckIfHighScore(PlayerManager.instance.GetPlayer(i).GetScore()))
                    {
                        enterNames[i].gameObject.SetActive(true);
                        enterNames[i].EnableIt(i);
                    }
                    else
                    {
                      Done(i);
                    }
                }
            }
            else
            {
                Done(i);
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
    }
   public void EnterNameReset()
    {
        for (int i = 0; i < 2; i++)
        {
            done[i] = false;
            enterNames[i].check = false;
        }
        ended = false;
    }
}
