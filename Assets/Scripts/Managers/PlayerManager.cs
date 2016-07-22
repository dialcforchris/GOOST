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
  // [SerializeField] private Nest[] nests = null;
    [SerializeField] private Text[] scores = null;

    [SerializeField] private float respawnLength = 1.0f;
    private bool[] playerRespawn = null;
    private float[] respawnTime = null;


    //set this properly when we have a splash screen menu
    private int amountOfPlayers = 2;

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
        /*for (int i = 0; i < Input.GetJoystickNames().Length;i++ )
        {
            if (i<2)
            {
                players[i].gameObject.SetActive(true);
                players[i].playerId = i;
                scores[i].gameObject.SetActive(true);
            }
            else
            {
                break;
            }
        }*/
    }

    public void SetupPlayer(int index)
    {
        players[index].gameObject.SetActive(true);
        players[index].playerId = index;
        scores[index].gameObject.SetActive(true);
        //Some sort of particle effect wouldn't go amiss here
    }

    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_MENU)
        {
            if (MainMenu.instance.currentState == MainMenu.menuState.readyUpScreen)
            {
                if (Input.GetButtonDown("Fly0"))
                {
                    //spawn player 1
                     
                }
                if (Input.GetButtonDown("Fly1"))
                {
                    //spawn player 2
                    players[1].gameObject.SetActive(true);
                    players[1].playerId = 1;
                    scores[1].gameObject.SetActive(true);
                    //Some sort of particle effect wouldn't go amiss here
                }
            }
        }
        else if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            for (int i = 0; i < playerRespawn.Length; ++i)
            {
                if (playerRespawn[i])
                {
                    respawnTime[i] += Time.deltaTime;
                    if (respawnTime[i] >= respawnLength)
                    {
                        playerRespawn[i] = false;
                        players[i].Respawn();
                    }
                }
            }
            UpdateScores();
        }
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

  ////  public Nest GetNest(int _index)
  //  {
  //      return nests[_index];
  //  }


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

    //public void StealEgg(int _playerId)
    //{
    //    Nest n = GetNest(_playerId);
    //    n.EggStolen();
    //}
    //public Nest GetLargestNest()
    //{
    //    int _amount = nests[0].numEggs;
    //    if (players.Length == 2)
    //    {
    //        if(_amount == nests[1].numEggs)
    //        {
    //            if(_amount == 0)
    //            {
    //                return null;
    //            }
    //            else
    //            {
    //                return nests[Random.Range(0, 2)];
    //            }
    //        }
    //        else if(_amount < nests[1].numEggs)
    //        {
    //            return nests[1];
    //        }
    //        else
    //        {
    //            return nests[0];
    //        }
    //    }
    //    else
    //    {
    //        if (_amount == 0)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return nests[0];
    //        }
    //    }
    //}
}