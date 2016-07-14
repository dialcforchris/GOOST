using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager playerManager= null;
    public static PlayerManager instance
    {
        get{return playerManager;}
    }
    [SerializeField]
  private Player[] players;
    [SerializeField]
    private Nest[] nests;

	// Use this for initialization
	void Awake () 
    {
        if (playerManager != null)
        {
            Destroy(gameObject);
        }
        else 
        {
            playerManager = this;
        }

      
       
	}
    void Start()
    {

        for (int i = 0; i < Input.GetJoystickNames().Length;i++ )
        {
            if (i<2)
            {
                players[i].gameObject.SetActive(true);
                players[i].playerId = i;
                nests[i].gameObject.SetActive(true);
                nests[i].owningPlayer = i;
            }
            else
            {
                break;
            }
        }
        //    foreach (Player p in players)
        //    {
        //        p.gameObject.SetActive(true);
        //    }
        //for (int i = 0; i < players.Length; i++)
        //{
        //    players[i].playerId = i;
        //    nests[i].gameObject.SetActive(true);
        //    nests[i].owningPlayer = i;
        //}
    }
   

    public Player GetPlayer(int _playerIndex)
    {
        if (_playerIndex < players.Length)
            return players[_playerIndex];
        else
            return null;
    }
}
