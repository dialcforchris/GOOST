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

	void Start ()
    {
        instance = this;
        if (currentTime % 60 > 10)
            TimerText.text = (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
        else
            TimerText.text = (int)(currentTime / 60) + ":0" + (int)(currentTime % 60);

        //StartCoroutine(TextInOut(true));
    }

    bool countdown;

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

            if ((int)currentTime == 60 && MainMenu.instance.getLevel() == 1)
            {
                if (Random.value == 42)
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
        StopAllCoroutines();
        countdownTextAnimator.gameObject.SetActive(false);
    }

    public IEnumerator TextInOut(bool InOut)
    {
        countdown = true;
        countdownTextAnimator.gameObject.SetActive(true);
        if (!InOut)
        {
            for (int i = 5; i > 0; i--)
            {
                countdownText.text = "" + i;

                countdownTextAnimator.Play("text_in");
                while (currentTime > i)
                    yield return null;
            }

            countdownText.text = "Time!";
            while (Time.timeScale > 0)
            {
                Time.timeScale -= Time.deltaTime * 2;
                yield return new WaitForEndOfFrame();
                if (Time.timeScale < 0.01f)
                    Time.timeScale = 0;
            }
            counting = false;
            Time.timeScale = 1;
        }
        else
        {
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = "" + i;

                countdownTextAnimator.Play("text_in");
                yield return new WaitForSeconds(1);
            }

            countdownText.text = "Go!";
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);

            yield return new WaitForSeconds(.95f);
            counting = true;
        }
        countdownTextAnimator.gameObject.SetActive(false);
        countdown = false;
    }
}
