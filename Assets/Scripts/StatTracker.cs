using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StatTracker : MonoBehaviour
{

    public static StatTracker instance;
    public GameStatistics stats;

    [Header("Stats UI")]
    public Text[] textElements;
    [SerializeField]
    Text[] totalVictoriesText;
    [SerializeField]
    Slider victoriesSlider;

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
        #region run time
        string[] time = new string[4];

        time[0] = "" + (int)(stats.runTime / 86400);
        time[1] = ""+(int)((stats.runTime / 3600)%24);
        time[2] = "" +(int)((stats.runTime / 60)%60);
        time[3] = "" +(int)(stats.runTime % 60);
        int i = 0;
        foreach (string s in time)
        {
            if (s.Length < 2)
                time[i] = "0" + s;
            i++;
        }
        textElements[0].text = time[0] + ":" + time[1] + ":" + time[2] + ":" + time[3];
        stats.runTime += Time.fixedDeltaTime;
        #endregion

        #region play time
        string[] playtime = new string[4];

        playtime[0] = "" + (int)(stats.playTime / 86400);
        playtime[1] = "" + (int)((stats.playTime / 3600) % 24);
        playtime[2] = "" + (int)((stats.playTime / 60) % 60);
        playtime[3] = "" + (int)(stats.playTime % 60);
        int j = 0;
        foreach (string s in playtime)
        {
            if (s.Length < 2)
                playtime[j] = "0" + s;
            j++;
        }
        textElements[1].text = playtime[0] + ":" + playtime[1] + ":" + playtime[2] + ":" + playtime[3];
        if (GameStateManager.instance.GetState()==GameStates.STATE_GAMEPLAY)
        {
            stats.playTime += Time.fixedDeltaTime;
        }
        #endregion

        textElements[2].text = "" + stats.roundsPlayed;
        textElements[3].text = "" + stats.totalFlaps;
        textElements[4].text = "" + stats.gooseZillaSightings;
        totalVictoriesText[0].text = "" + stats.ransomWins;
        totalVictoriesText[1].text = "" + stats.ITGuyWins;

        if (stats.ITGuyWins > 0 && stats.roundsPlayed > 0)
            victoriesSlider.value = ((float)stats.roundsPlayed - (float)stats.ITGuyWins) / (float)stats.roundsPlayed;
    }
}

[System.Serializable]
public class GameStatistics
{
    [SerializeField]
    public float runTime = 0;
    [SerializeField]
    public float playTime = 0;
    [SerializeField]
    public int roundsPlayed = 0;
    [SerializeField]
    public int totalFlaps = 0;
    [SerializeField]
    public int gooseZillaSightings = 0;
    [SerializeField]
    public int ransomWins=0, ITGuyWins = 0;

    //Rounds won/lost?
}

