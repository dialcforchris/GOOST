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
    [SerializeField] private Player[] players = null;
    [SerializeField] private Nest[] nests = null;
    [SerializeField] private Text[] scores = null;

    [SerializeField] private float respawnLength = 1.0f;
    private bool[] playerRespawn = null;
    private float[] respawnTime = null;


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
            respawnTime = new float[amountOfPlayers];
            playerRespawn = new bool[amountOfPlayers];
            for(int i = 0; i < amountOfPlayers; ++i)
            {
                respawnTime[i] = 0.0f;
                playerRespawn[i] = false;
            }
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
                nests[i].enabled = true;
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
        for(int i = 0; i < playerRespawn.Length; ++i)
        {
            if (playerRespawn[i])
            {
                respawnTime[i] += Time.deltaTime;
                if(respawnTime[i] >= respawnLength)
                {
                    playerRespawn[i] = false;
                    players[i].Respawn();
                }
            }
        }
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

    public void RespawnPlayer(int _index)
    {
        playerRespawn[_index] = true;
        respawnTime[_index] = 0.0f;
    }

    public Nest GetNest(int _index)
    {
        return nests[_index];
    }

    public Player GetClosestPlayer(Vector3 _pos)
    {
        float _distance = Vector3.SqrMagnitude(players[0].transform.position - _pos);
        if(players.Length == 2)
        {
            if(Vector3.SqrMagnitude(players[1].transform.position - _pos) < _distance)
            {
                return players[1];
            }
        }
        return players[0];
    }
}