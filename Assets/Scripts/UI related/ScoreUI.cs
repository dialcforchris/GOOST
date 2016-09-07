using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GOOST
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        private Text[] scores;
        Player[] players;
        [SerializeField]
        private Text[] Lives;
        [SerializeField]
        private Text[] coll;
        [SerializeField]
        private Image[] lives;
        [SerializeField]
        private Image[] collectables;

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < PlayerManager.instance.NumberOfPlayers(); i++)
            {
                scores[i].gameObject.SetActive(true);
                players[i] = PlayerManager.instance.GetPlayer(i);
            }
        }

        // Update is called once per frame
        void Update()
        {
            //   UpdateScores();
        }


        void UpdateLives()
        {

        }
    }
}