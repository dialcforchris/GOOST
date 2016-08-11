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

                if (i < 10)
                {
                    rank[i].color = Color.green;
                    scores[i].color = Color.green;
                    names[i].color = Color.green;
                }
                else if (i < 20)
                {
                    rank[i].color = Color.yellow;
                    scores[i].color = Color.yellow;
                    names[i].color = Color.yellow;
                }
                else
                {
                    rank[i].color = Color.red;
                    scores[i].color = Color.red;
                    names[i].color = Color.red;
                }
                //rank[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
                //scores[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
                //names[i].color = Color.Lerp(Color.green, Color.red, (float)(i * (1f / (float)k.Count)));
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
            if (i < 10)
            {
                rank[i].color = Color.Lerp(Color.green, Color.black, (float)i / 25f);
                scores[i].color = Color.Lerp(Color.green, Color.black, (float)i / 25f);
                names[i].color = Color.Lerp(Color.green, Color.black, (float)i / 25f);
            }
            else if (i < 20)
            {
                rank[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i-5 )/ 25f);
                scores[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i - 5) / 25f);
                names[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i - 5) / 25f);
            }
            else
            {
                rank[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i) / 25f);
                scores[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i) / 25f);
                names[i].color = Color.Lerp(Color.yellow, Color.red, ((float)i) / 25f);
            }
        }
    }
}