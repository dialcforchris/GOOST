using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

public class EndGameLogic : MonoBehaviour
{

    public static EndGameLogic instance = null;
    [SerializeField]
    SpriteRenderer Shield;
    [SerializeField]
    Sprite[] ShieldSprites; //0 for simon, 1 for handsomeware
    [SerializeField]
    Animator FlagAnimator,ContinueTextAnimator;
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
        //AnnounceWinner();
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

    IEnumerator EndGameScreenReveal(bool winner)//False for highway man, True for Simon
    {
        if (!winner)
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
            TextCol = Backdrop.color;
            TextCol.a = Mathf.Lerp(0, .75f, lerpy);
            Backdrop.color = TextCol;
            lerpy += Time.deltaTime * 1.0f;
            yield return new WaitForEndOfFrame();
        }

        //Change shield sprite
        Shield.sprite = ShieldSprites[winner ? 0 : 1];
        //Enable the right flag
        GameStats.instance.WinnerFlags[1].enabled = !winner;
        GameStats.instance.WinnerFlags[0].enabled = winner;

        //Play flag animation
        FlagAnimator.Play("flag_erect");//hehe

        //Play appluase sounds
        yield return new WaitForSeconds(3);
        SoundManager.instance.playSound(AudienceSounds[winner ? 0 : 1]);

        //When concluded, allow input, play animation for "Press any button" text prompt
        ContinueTextAnimator.Play("fadeTExt");

        while (!Input.anyKey)
        {
            yield return null;
        }

        lerpy = 0;
        while (lerpy < 1)
        {
            TextCol = Backdrop.color;
            TextCol.a = Mathf.Lerp(.75f, 0, lerpy);
            Backdrop.color = TextCol;
            lerpy += Time.deltaTime * 5.0f;
            yield return new WaitForEndOfFrame();
        }

        MainMenu.instance.transform.rotation = Quaternion.Euler(Vector3.zero);
        MainMenu.instance.switchMenus(0);
        MainMenu.instance.currentState = MainMenu.menuState.mainMenu;
        GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
        CameraController.instance.switchViews(true);

        //Wait a moment before resetting everything, just to make sure it's not in the camera view
        yield return new WaitForSeconds(2);
        FlagAnimator.Play("flag_idle_down");
        ContinueTextAnimator.Play("fade_text_idle");
        GameStats.instance.ResetText();

        for (int i = 0; i < EnemyManager.instance.AllEnemies.Count; i++)
        {
            EnemyManager.instance.AllEnemies[i].GetComponent<Rigidbody2D>().isKinematic = false;
            EnemyManager.instance.AllEnemies[i].poolData.ReturnPool(EnemyManager.instance.AllEnemies[i]);
        }
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
            winner.text = "Player 1 Wins";
        }
        else
        {
            winner.text = "Player 2 Wins";
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


[CustomEditor(typeof(EndGameLogic))]
public class MainMenuTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EndGameLogic scriptToControl = (EndGameLogic)target;
        if (GUILayout.Button("bounce"))
        {
            scriptToControl.TriggerGameEnd(false);
        }
    }
}
