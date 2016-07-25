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
    [SerializeField]
    private Text[] lives;
    [SerializeField]
    private Text[] coll;
    [SerializeField]
    private Image[] lifeSprite;
    [SerializeField]
    Sprite[] playerSprites;
    [SerializeField]
    Sprite[] collectableSprites;
    [SerializeField]
    private Image[] collectables;
    [SerializeField]
    private Image[] boosts;
    [Range(0, 1)]
    public float fillAmount;
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
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (i < 2)
            {
                players[i].gameObject.SetActive(true);
                players[i].playerId = i;
                scores[i].gameObject.SetActive(true);
            }
            else
            {
                break;
            }
        }
        foreach (Image i in boosts)
        {
            i.fillMethod = Image.FillMethod.Horizontal;
            i.type = Image.Type.Filled;
        }
    }

    public void SetupPlayer(int index)
    {
        players[index].gameObject.SetActive(true);
        players[index].playerId = index;
        scores[index].gameObject.SetActive(true);
        //Some sort of particle effect wouldn't go amiss here

        //Set position
        switch (index)
        {
            case 0:
                players[index].transform.position = new Vector3(-3.5f, 29.5f, 0);
                break;
            case 1:
                players[index].transform.position = new Vector3(3.5f, 29.5f, 0);
                break;
        }
    }

    public void ResetPlayers()
    {
        players[0].GooseyBod.velocity = Vector3.zero;
        players[0].GooseyBod.angularVelocity = 0;
        players[1].GooseyBod.velocity = Vector3.zero;
        players[1].GooseyBod.angularVelocity = 0;


        players[0].gameObject.SetActive(false);
        players[1].gameObject.SetActive(false);
    }

    void Update()
    {
        for (int i = 0; i < playerRespawn.Length; ++i)
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
        UpdateUI();
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

    public void RespawnPlayer(int _index)
    {
        playerRespawn[_index] = true;
        respawnTime[_index] = 0.0f;
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
    void UpdateScores()
    {
        for (int i = 0; i < players.Length; i++)
        {
            scores[i].text = players[i].GetScore().ToString();
        }
    }

    void UpdateLives()
    {
        for (int i = 0; i < players.Length; i++)
        {
            scores[i].text = players[i].eggLives.ToString();
        }
    }
    void UpdateCollectables()
    {
        for (int i = 0; i < players.Length; i++)
        {
            scores[i].text = players[i].collectable.ToString();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < players.Length; i++)
        {
            scores[i].text = players[i].GetScore().ToString();
            lives[i].text = "X"+players[i].eggLives.ToString();
            coll[i].text = "X"+players[i].collectable.ToString();
            lifeSprite[i].sprite = playerSprites[players[i].playerType == PlayerType.GOODGUY ? 0 : 1];
            collectables[i].sprite = collectableSprites[players[i].playerType == PlayerType.GOODGUY ? 0 : 1];
            boosts[i].fillAmount = players[i].dashcool;
        }
       
    }
}