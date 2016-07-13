using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

    public int maxSeconds;
    [SerializeField]
    float currentTime;
    [SerializeField]
    bool counting,upDown;
    [SerializeField]
    private Text TimerText;

	void Start ()
    {

    }

    void Update()
    {
        if (counting)
        {
            if (upDown)
                currentTime += Time.deltaTime;
            else
            {
                currentTime -= Time.deltaTime;
                if (currentTime < 0)
                {
                    counting = false;
                    currentTime = 0;
                }
            }
            if (currentTime % 60 > 10)
            TimerText.text = (int)(currentTime / 60) + ":" + (int)(currentTime % 60);
            else
                TimerText.text = (int)(currentTime / 60) + ":0" + (int)(currentTime % 60);
        }
    }
}
