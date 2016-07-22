using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

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
        TimerText.text = ""+currentTime;
        StartCoroutine(TextInOut(true));
    }

    bool countdown;

    void Update()
    {
        if (counting)
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

            //Seconds + minutes interface
            /*if (currentTime % 60 > 10)
            TimerText.text = "0"+(int)(currentTime / 60) + ":" + (int)(currentTime % 60);
            else
                TimerText.text = "0"+(int)(currentTime / 60) + ":0" + (int)(currentTime % 60);*/

            //Just seconds
            TimerText.text = ""+(int)(currentTime);
        }
    }
    IEnumerator TextInOut(bool InOut)
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
            countdownTextAnimator.gameObject.SetActive(false);
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
            countdownTextAnimator.gameObject.SetActive(false);
            counting = true;
        }
        countdown = false;
    }
}
