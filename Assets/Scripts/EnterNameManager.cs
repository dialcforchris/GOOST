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
        ended = (done[0] & done[1]);
    }
    public void ShowEnterName()
    {
        for (int i = 0; i < PlayerManager.instance.NumberOfPlayers(); ++i)
        {

            if (!enterNames[i].check && LeaderBoard.instance.CheckIfHighScore(PlayerManager.instance.GetPlayer(i).GetScore()))
            {
                enterNames[i].gameObject.SetActive(true);
                enterNames[i].EnableIt(i);
            }
            else
            {
                Done(i);
                continue;
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
}
