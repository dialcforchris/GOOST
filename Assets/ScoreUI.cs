using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour 
{
    [SerializeField]
    private Text[] scores;
    Player[] players;
	// Use this for initialization
	void Start () 
    {
        for (int i=0;i<PlayerManager.instance.NumberOfPlayers();i++)
        {
            scores[i].gameObject.SetActive(true);
            players[i] = PlayerManager.instance.GetPlayer(i);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //UpdateScores();
	}

    void UpdateScores()
    {
        for (int i = 0; i < players.Length;i++ )
        {
            Debug.Log(i);
            scores[i].text = players[i].GetScore().ToString();
        }
    }
}
