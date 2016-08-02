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
	  
           
            for (int i=0;i<PlayerManager.instance.NumberOfPlayers();++i)
            {

            if (!enterNames[i].check && LeaderBoard.instance.CheckIfHighScore(PlayerManager.instance.GetPlayer(i).GetScore()))
                {
                    enterNames[i].gameObject.SetActive(true);
                    enterNames[i].EnableIt(i);
                }
            }
            //if (PlayerManager.instance.NumberOfPlayers()<done.Length)
            //{
            //    done[PlayerManager.instance.NumberOfPlayers()+1] = true;
            //}

            if (done[0]&&done[1])
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString());
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
