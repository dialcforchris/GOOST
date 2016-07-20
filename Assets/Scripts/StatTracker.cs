using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StatTracker : MonoBehaviour {

    public static StatTracker instance;
    public GameStatistics stats;

    [Header("Stats UI")]
    public Text[] textElements;

	// Use this for initialization
	void Start ()
    {
        //SaveStatsToFile();
        LoadStats();
        instance = this;
	}

    void SaveStatsToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenWrite(Application.persistentDataPath + "/stats.dat");
        
        //Fill in all the deets here

        bf.Serialize(file, stats);
        file.Close();
    }

    void OnApplicationQuit()
    {
        SaveStatsToFile();
    }

    void LoadStats()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/stats.dat", FileMode.OpenOrCreate);
        stats = (GameStatistics)bf.Deserialize(file);
        file.Close();
    }
    
    void Update ()
    {
        string[] time = new string[4];

        time[0] = "" + (int)(stats.playtime / 86400);
        time[1] = ""+(int)((stats.playtime / 3600)%24);
        time[2] = "" +(int)((stats.playtime / 60)%60);
        time[3] = "" +(int)(stats.playtime % 60);

        int i = 0;
        foreach (string s in time)
        {
            if (s.Length < 2)
                time[i] = "0" + s;
            i++;
        }

        textElements[0].text = time[0] + ":" + time[1] + ":" + time[2] + ":" + time[3];
        stats.playtime += Time.fixedDeltaTime;

        textElements[1].text = "" + stats.totalFlaps;
    }
}

[System.Serializable]
public class GameStatistics
{
    [SerializeField]
    public float playtime=0;
    [SerializeField]
    public int eggsLaid=0;
    [SerializeField]
    public int roundsPlayed = 0;
    [SerializeField]
    public int silverSpentOnEggs=0;
    [SerializeField]
    public int magpiesKilled = 0;
    [SerializeField]
    public int totalFlaps = 0;
    [SerializeField]
    public int eggsStolen = 0;

    //Rounds won/lost?
}

