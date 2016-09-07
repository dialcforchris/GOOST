using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GOOST
{
    public class EndGameLogic : MonoBehaviour
    {
        public static EndGameLogic instance = null;
        [SerializeField]
        SpriteRenderer Shield;
        [SerializeField]
        Sprite[] ShieldSprites; //0 for simon, 1 for handsomeware
        [SerializeField]
        Animator FlagAnimator;
        [SerializeField]
        GameObject ContinueText;
        [SerializeField]
        AudioClip[] AudienceSounds;
        [SerializeField]
        GameObject gameOver;
        [SerializeField]
        Text winner;
        [SerializeField]
        Text timer;
        [SerializeField]
        private GameObject entername;
        [SerializeField]
        private GameObject statUI;
        float fade = 1;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            //timer.text = "Game Over";
            //StartCoroutine(DoAFade());
        }

        public void TriggerGameEnd(bool naturalGameEnd)//If this bool is set to false, a player ran out of lives and triggered game end like that.
        {
            if (naturalGameEnd)
            {
                if (PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore())
                {
                    //Player 1 wins
                    StartCoroutine(EndGameScreenReveal(false));
                }
                else
                {
                    //Player 2 wins
                    StartCoroutine(EndGameScreenReveal(true));
                }
            }
            else
            {
                if (PlayerManager.instance.GetPlayer(0).eggLives > PlayerManager.instance.GetPlayer(1).eggLives)
                {
                    //Player 1 wins
                    StartCoroutine(EndGameScreenReveal(false));
                }
                else
                {
                    //Player 2 wins
                    StartCoroutine(EndGameScreenReveal(true));
                }
            }
        }

        IEnumerator EndGameScreenReveal(bool result)//False for highway man, True for Simon
        {
            if (!result)
                StatTracker.instance.stats.ransomWins++;
            else
                StatTracker.instance.stats.ITGuyWins++;

            for (int i = 0; i < EnemyManager.instance.AllEnemies.Count; i++)
            {
                EnemyManager.instance.AllEnemies[i].GetComponent<Rigidbody2D>().isKinematic = true;
            }
            PlayerManager.instance.GetPlayer(0).GooseyBod.isKinematic = true;
            PlayerManager.instance.GetPlayer(1).GooseyBod.isKinematic = true;

            //Timer to game over
            timer.text = "Game over";

            Image Backdrop = GetComponent<Image>();
            float lerpy = 0;

            //Fade backdrop in
            Color TextCol;
            while (lerpy < 1)
            {
                lerpy += Time.deltaTime * 1.0f;
                TextCol = Backdrop.color;
                TextCol.a = Mathf.Lerp(0, .75f, lerpy);
                Backdrop.color = TextCol;
                yield return new WaitForEndOfFrame();
            }
            EnterNameManager.instance.EnterNameReset();
            //Change shield sprite
            Shield.sprite = ShieldSprites[result ? 0 : 1];
            //Enable the right flag
            GameStats.instance.WinnerFlags[1].enabled = !result;
            GameStats.instance.WinnerFlags[0].enabled = result;

            //Play flag animation
            FlagAnimator.Play("flag_erect");//hehe

            //Play appluase sounds
            yield return new WaitForSeconds(3);
            SoundManager.instance.playSound(AudienceSounds[result ? 0 : 1]);
            winner.gameObject.SetActive(true);
            winner.text = result ? "\nI.T. Data remains Protected" : "\nThe Ransomware bandit stole the data"; // "Player " + (result ? "2" : "1") + " Wins!";
            winner.text += "\n<size=40> Player " + (result ? "2" : "1") + " wins </size>";

            //When concluded, allow input, play animation for "Press any button" text prompt
            //ContinueTextAnimator.Play("fadeTExt");
            ContinueText.SetActive(true);

            while (!Input.anyKey)
            {
                yield return null;
            }

            winner.gameObject.SetActive(false);
            FlagAnimator.Play("flag_idle_down");
            ContinueText.SetActive(false);
            GameStats.instance.ResetText();

            while (Input.anyKey)
            {
                yield return null;
            }


            TextCol = Backdrop.color;
            TextCol.a = 0;
            Backdrop.color = TextCol;
            EnterNameManager.instance.ShowEnterName();
            while (!EnterNameManager.instance.ended)
            {
                yield return null;
            }

            lerpy = 0;
            while (lerpy < 1)
            {
                lerpy += Time.deltaTime * 5.0f;
                TextCol = Backdrop.color;
                TextCol.a = Mathf.Lerp(.75f, 0, lerpy);
                Backdrop.color = TextCol;
                yield return new WaitForEndOfFrame();
            }

            MainMenu.instance.transform.rotation = Quaternion.Euler(Vector3.zero);
            MainMenu.instance.switchMenus(0);
            MainMenu.instance.currentState = MainMenu.menuState.mainMenu;
            GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
            CameraController.instance.switchViews(true);

            //Wait a moment before resetting everything, just to make sure it's not in the camera view
            yield return new WaitForSeconds(2);


            for (int i = 0; i < EnemyManager.instance.AllEnemies.Count; i++)
            {
                EnemyManager.instance.AllEnemies[i].GetComponent<Rigidbody2D>().isKinematic = false;
            }
            EnemyManager.instance.Reset();
            PlayerManager.instance.GetPlayer(0).GooseyBod.isKinematic = false;
            PlayerManager.instance.GetPlayer(1).GooseyBod.isKinematic = false;
        }

        IEnumerator DoAFade()
        {
            yield return new WaitForSeconds(2);

            while (!Input.anyKey)
            {
                if (Input.anyKey)
                    break;
                yield return null;
            }
            if (statUI.gameObject.activeInHierarchy)
            {
                for (int i = 0; i < statUI.transform.childCount; i++)
                {
                    fade -= Time.deltaTime / 2;
                    statUI.transform.GetChild(i).GetComponent<Text>().color =
                        new Color(statUI.transform.GetChild(i).GetComponent<Text>().color.r,
                        statUI.transform.GetChild(i).GetComponent<Text>().color.g, statUI.transform.GetChild(i).GetComponent<Text>().color.b, fade);
                }
                if (fade <= 0)
                {
                    entername.SetActive(true);
                    statUI.gameObject.SetActive(false);
                }
            }
            yield return null;
        }
        void AnnounceWinner()
        {
            winner.gameObject.SetActive(true);
            if (PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore())
            {
                winner.text = "Player 1 Wins!";
            }
            else
            {
                winner.text = "Player 2 Wins!";
                StatTracker.instance.stats.ITGuyWins++;
            }
        }
        //[SerializeField]
        //EnterName[] enterName;

        //public void EndGame()
        //{
        //    timer.SetActive(false);
        //    gameOver.SetActive(true);
        //    string win;
        //    if (PlayerManager.instance.NumberOfPlayers() > 1)
        //        win = PlayerManager.instance.GetPlayer(0).GetScore() > PlayerManager.instance.GetPlayer(1).GetScore() ? "THE HACKER " : "THE I.T GUY ";
        //    else
        //        win = "THE HACKER ";
        //    winner.text = win + "Wins";
        //    for (int i = 0; i < PlayerManager.instance.NumberOfPlayers(); i++)
        //    {
        //        enterName[i].gameObject.SetActive(true);
        //        enterName[i].EnableIt(i);
        //    }
        //}
    }
}