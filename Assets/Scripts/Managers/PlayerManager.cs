﻿using UnityEngine;
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

        for (int i=0;i<players.Length;i++)
        {
            players[i].playerId = i;
            nests[i].gameObject.SetActive(true);
            nests[i].owningPlayer = i;
        }
       
	}
    void Start()
    {
        foreach (Player p in players)
        {
            p.gameObject.SetActive(true);
        }
    }
   

    public Player GetPlayer(int _playerIndex)
    {
        if (_playerIndex < players.Length)
            return players[_playerIndex];
        else
            return null;
    }
}