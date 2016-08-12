using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{

    public static Timer instance;

    [SerializeField]
    private Animator countdownTextAnimator;
    [SerializeField]
    private Text countdownText;
    
    public int maxSeconds;
    [SerializeField]
    float currentTime;
    [SerializeField]
    bool counting,upDown;
    [SerializeField]
    private Text TimerText;
    [SerializeField]
    private AudioClip[] countDownSound;
    [SerializeField]
    private AudioClip stop;
    [SerializeField]
    private AudioClip go;
    [SerializeField]
    private AudioClip game;

    bool goosed;

	void Start ()
    {
        instance = this;
        if (currentTime % 60 > 10)
            TimerText.text = (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
        else
            TimerText.text = (int)(currentTime / 60) + ":0" + (int)(currentTime % 60);

        //StartCoroutine(TextInOut(true));
    }

    public bool countdown;

    void Update()
    {
        if (counting)
        {
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
            {
                if (upDown)
                    currentTime += Time.deltaTime;
                else
                {
                    currentTime -= Time.deltaTime;
                    if (currentTime < 6 && !countdown)
                        StartCoroutine(TextInOut(false));

                    if (currentTime < 0)
                    {
                        counting = false;
                        currentTime = 0;
                    }
                }
            }
            //Seconds + minutes interface

            if ((int)currentTime == 60 && MainMenu.instance.getLevel() == 2 && !goosed)
            {
                goosed = true;
                if (Random.value > .7f)
                {
                    //OH SHIT HERE COMES DAT BOI!
                    CameraShake.instance.GetGoosed();
                }
            }

            if (currentTime % 60 > 10)
                TimerText.text = (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
            else
                TimerText.text = (int)(currentTime / 60) + ":0" + (int)(currentTime % 60);

            //Just seconds
            //TimerText.text = ""+(int)(currentTime);
        }
    }

    public void Reset()
    {
        counting = false;
        currentTime = maxSeconds;

        if (currentTime % 60 > 10)
            TimerText.text = (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
        else
            TimerText.text = (int)(currentTime / 60) + ":0" + (int)(currentTime % 60);

        StopAllCoroutines();
        countdownTextAnimator.gameObject.SetActive(false);
    }
    
    public IEnumerator MidGameOver()
    {
        if (currentTime > 3.5f)
        {
            counting = false;
            StatTracker.instance.stats.roundsPlayed++;

            countdownTextAnimator.gameObject.SetActive(true);
            countdownTextAnimator.Play("text_in");
            countdownText.text = "Game!";
            SoundManager.instance.playSound(game);
            while (Time.timeScale > 0)
            {
                Time.timeScale -= Time.deltaTime * 2;
                yield return new WaitForEndOfFrame();
                if (Time.timeScale < 0.01f)
                    Time.timeScale = 0;
            }
            countdownTextAnimator.gameObject.SetActive(false);
            EndGameLogic.instance.TriggerGameEnd(false);
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEOVER);
            Time.timeScale = 1;
        }
    }

    public IEnumerator TextInOut(bool InOut)
    {
        goosed = false;
        countdown = true;
        countdownTextAnimator.gameObject.SetActive(true);
        if (!InOut)
        {
            for (int i = 5; i > 0; i--)
            {
                countdownText.text = "" + i;
                SoundManager.instance.playSound(countDownSound[i-1]);
                countdownTextAnimator.Play("text_in");
                while (currentTime > i)
                    yield return null;
            }

            StatTracker.instance.stats.roundsPlayed++;

            SoundManager.instance.playSound(stop);

            countdownText.text = "Time!";
            while (Time.timeScale > 0)
            {
                Time.timeScale -= Time.deltaTime * 2;
                yield return new WaitForEndOfFrame();
                if (Time.timeScale < 0.01f)
                    Time.timeScale = 0;
            }
            counting = false;
            EndGameLogic.instance.TriggerGameEnd(true);
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEOVER);
            Time.timeScale = 1;
        }
        else
        {
            for (int i = 6; i > 0; i--)
            {
                countdownText.text = "" + i;
                if (i<=3)
                {
                    SoundManager.instance.playSound(countDownSound[i - 1]);
                }
                countdownTextAnimator.Play("text_in");
                yield return new WaitForSeconds(1);
            }
            SoundManager.instance.playSound(go);
            countdownText.text = "Go!";
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);

            yield return new WaitForSeconds(.95f);
            counting = true;
        }
        countdownTextAnimator.gameObject.SetActive(false);
        countdown = false;
    }
}
