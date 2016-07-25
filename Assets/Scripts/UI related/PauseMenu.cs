using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu instance;

    int index = 0;
    [SerializeField]
    private Text[] options;
    [SerializeField]
    private Slider[] soundsSliders;

    [SerializeField]
    Color highlightColour;

    void Start()
    {
        instance = this;
    }
    bool scrolling;
    void Update()
    {
        #region move selection up/down
        if ((Input.GetAxis("Vertical0") > 0 || Input.GetAxis("Vertical1") > 0) && !scrolling)
        {
            scrolling = true;
            index--;
            if (index < 0)
                index = options.Length - 1;
        }
        else if ((Input.GetAxis("Vertical0") < 0 || Input.GetAxis("Vertical1") < 0) && !scrolling)
        {
            scrolling = true;
            index++;
            if (index > options.Length - 1)
                index = 0;
        }
        else if ((Input.GetAxis("Vertical0") == 0 && Input.GetAxis("Vertical1") == 0) && scrolling)
        {
            scrolling = false;
        }
        #endregion

        #region sound sliders
        if (Input.GetAxis("Horizontal0") < 0 || Input.GetAxis("Horizontal1") < 0)
        {
            switch (index)
            {
                case 1:
                    soundsSliders[0].value -= Time.fixedDeltaTime;
                    break;
                case 2:
                    soundsSliders[1].value -= Time.fixedDeltaTime;
                    break;
            }
        }
        if (Input.GetAxis("Horizontal0") > 0 || Input.GetAxis("Horizontal1") > 0)
        {
            switch (index)
            {
                case 1:
                    soundsSliders[0].value += Time.fixedDeltaTime;
                    break;
                case 2:
                    soundsSliders[1].value += Time.fixedDeltaTime;
                    break;
            }
        }
        #endregion

        #region text highlighting
        foreach (Text t in options)
        {
            t.color = Color.white;
        }
        options[index].color = highlightColour;
        #endregion

        if (Input.GetButton("Interact0") || Input.GetButton("Interact1"))
        {
            switch (index)
            {
                case 0:
                    //Resume
                    GameStateManager.instance.unPause();
                    break;
                case 3:
                    //Quit to menu
                    CameraController.instance.switchViews(true);
                    GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
                    break;
            }
        }
    }
}
