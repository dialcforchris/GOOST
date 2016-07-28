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
    bool once =false;

    public void update()
    {
        Debug.Log("called");
       
            Debug.Log("once = false");
            
            List<KeyValuePair<string, int>> k = LeaderBoard.instance.ReturnLeaderBoard();
            for (int i = 0; i < k.Count; i++)
            {

                rank[i].text = (i+1).ToString()+".";
                scores[i].text = k[i].Value.ToString();
                names[i].text = k[i].Key;
                rank[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.03f));
                scores[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.03f));
                names[i].color = Color.Lerp(Color.green, Color.red, (float)(i * 0.03f));
            }
            once = true;
      
    }
}

[CustomEditor(typeof(DisplayLeaderboard))]
public class leaderboardDisplayer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DisplayLeaderboard myScript = (DisplayLeaderboard)target;
        if (GUILayout.Button("do it"))
        {
            myScript.update();
        }
    }
}