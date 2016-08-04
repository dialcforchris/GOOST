using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayLeaderboard : MonoBehaviour
{
    public Text[] scores;
    public Text[] names;
    public Text[] rank;
    bool once = false;

    public void Update()
    {
        if (!once)
        {
            List<KeyValuePair<string, int>> k = LeaderBoard.instance.ReturnLeaderBoard();
            for (int i = 0; i < k.Count; i++)
            {
                rank[i].text = (i + 1).ToString() + ".";
                scores[i].text = k[i].Value + "";
                names[i].text = k[i].Key;
                rank[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
                scores[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
                names[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
            }
            once = true;
        }
    }
    public void ShowTheDarnThing()
    {
        List<KeyValuePair<string, int>> k = LeaderBoard.instance.ReturnLeaderBoard();
        for (int i = 0; i < k.Count; i++)
        {
            rank[i].text = (i + 1).ToString() + ".";
            scores[i].text = k[i].Value + "";
            names[i].text = k[i].Key;
            rank[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
            scores[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
            names[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
        }
    }
}