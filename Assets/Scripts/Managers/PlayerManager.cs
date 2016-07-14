using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    [SerializeField]
    private Text[] scores;


    //set this properly when we have a splash screen menu
    private int amountOfPlayers = 1;

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
                scores[i].gameObject.SetActive(true);
            }
            else
            {
                break;
            }
        }
    }
   
    void Update()
    {
        UpdateScores();
    }
    public Player GetPlayer(int _playerIndex)
    {
        if (_playerIndex < players.Length)
            return players[_playerIndex];
        else
            return null;
    }

    public int NumberOfPlayers()
    {
        return amountOfPlayers;
    }

    void UpdateScores()
    {
        for (int i = 0; i < players.Length;i++ )
        {
            scores[i].text = players[i].GetScore().ToString();
        }
    }
}