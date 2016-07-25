using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;

    [Header("Main menu")]
    public Text[] mainMenuElements;
    public Image[] mainMenuCursor;

    public GameObject[] menuScreens;

    [Header("Options menu")]
    public Text[] optionsElements;
    public Image[] optionsCursor;

    [Header("Ready up menu")]
    [SerializeField]
    Text[] readyTextPrompts;
    [SerializeField]
    GameObject readyUpBounds;
    [SerializeField]
    Image returnImage;
    [SerializeField]
    Image[] readyIndicator, characterImg;
    [SerializeField]
    Vector2[] characterImgStart, characterImgEnd;

    int mainMenuIndex, optionsIndex;
    bool transitioning;

    public menuState currentState;
    public enum menuState
    {
        mainMenu,
        readyUpScreen,
        optionsMenu,
        leaderboardsMenu,
        statsScreen,
    }

    public void Awake()
    {
        instance = this;
        ready = new bool[2] { false, false };
        pressingReady = new bool[2] { false, false };
    }

    IEnumerator rotateMenus(Vector3 start, Vector3 end)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            start = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(Mathf.LerpAngle(start.x, end.x, lerpy), Mathf.LerpAngle(start.y, end.y, lerpy), Mathf.LerpAngle(start.z, end.z, lerpy)));
            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transitioning = false;
    }

    public void switchMenus(int menu)
    {
        if (!transitioning)
        {
            switch (menu)
            {
                case 0:
                    transitioning = true;
                    currentState = menuState.mainMenu;
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, Vector3.zero));
                    break;
                case 1:
                    transitioning = true;
                    currentState = menuState.optionsMenu;
                    menuScreens[2].SetActive(false);
                    menuScreens[1].SetActive(true);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(0, -90, 0)));
                    break;
                case 2:
                    transitioning = true;
                    currentState = menuState.leaderboardsMenu;
                    menuScreens[2].SetActive(true);
                    menuScreens[1].SetActive(false);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(0, 90, 0)));
                    break;
                case 3:
                    transitioning = true;
                    currentState = menuState.statsScreen;
                    menuScreens[3].GetComponent<Canvas>().enabled = true;
                    menuScreens[4].SetActive(false);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(90, 0, 0)));
                    break;
                case 4:
                    //no button pressing allowed during this time
                    transitioning = true;

                    currentState = menuState.readyUpScreen;
                    GameStateManager.instance.ChangeState(GameStates.STATE_READYUP);

                    //Make sure the players can't escape the screen
                    readyUpBounds.SetActive(true);
                    readyTextPrompts[0].text = "Press the fly button\n to spawn";
                    readyTextPrompts[1].text = "Press the fly button\n to spawn";

                    //Turn some off/on to help aid visibility
                    menuScreens[3].GetComponent<Canvas>().enabled = false;
                    menuScreens[4].SetActive(true);

                    //Reset return text;
                    returnTimer = 0;
                    returnImage.fillAmount = 0;

                    //Tween in character images
                    StartCoroutine(CharacterImageTransition(true, 0));
                    StartCoroutine(CharacterImageTransition(true, 1));

                    //Display the correct menu
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(-90, 0, 0)));
                    break;
            }
        }
    }

    bool scrolling;
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_MENU)
        {
            if (!transitioning)
            {
                switch (currentState)
                {
                    case menuState.mainMenu:
                        mainMenu();
                        break;
                    case menuState.readyUpScreen:
                        break;
                    case menuState.optionsMenu:
                        optionsMenu();
                        break;
                    default:
                        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
                        {
                            switchMenus(0);
                        }
                        break;
                }
            }
        }
        if (GameStateManager.instance.GetState() == GameStates.STATE_READYUP)
        {
            ReadyUpScreen();
        }
    }

    float returnTimer;
    [SerializeField]
    bool[] ready;
    bool[] pressingReady;
    void ReadyUpScreen()
    {
        #region spawn geese
        if (Input.GetButtonDown("Fly0") && !PlayerManager.instance.GetPlayer(0).gameObject.activeSelf)
        {
            StartCoroutine(CharacterImageTransition(false, 0));
            PlayerManager.instance.SetupPlayer(0);
            readyTextPrompts[0].text = "Press Dash and Fly\n buttons to ready up";
        }
        if (Input.GetButtonDown("Fly1") && !PlayerManager.instance.GetPlayer(1).gameObject.activeSelf)
        {
            StartCoroutine(CharacterImageTransition(false, 1));
            PlayerManager.instance.SetupPlayer(1);
            readyTextPrompts[1].text = "Press Dash and Fly\n buttons to ready up";
        }
        #endregion

        #region ready up
        //Player 1
        if (Input.GetButton("Fly0") && Input.GetButton("Interact0") && !pressingReady[0])
        {
            pressingReady[0] = true;
            ready[0] = !ready[0];
            readyIndicator[0].color = ready[0] ? Color.yellow : Color.white;
            readyTextPrompts[0].text = "Press Dash and Fly\n again to cancel";
            //Mb play a sound?
        }
        else if (!Input.GetButton("Fly0") && !Input.GetButton("Interact0"))
            pressingReady[0] = false;

        //Player 2
        if (Input.GetButton("Fly1") && Input.GetButton("Interact1") && !pressingReady[1])
        {
            pressingReady[1] = true;
            ready[1] = !ready[1];
            readyIndicator[1].color = ready[1] ? Color.yellow : Color.white;
            readyTextPrompts[1].text = "Press Dash and Fly\n again to cancel";
            //Mb play a sound?
        }
        else if(!Input.GetButton("Fly1") && !Input.GetButton("Interact1"))
            pressingReady[1] = false;

        #endregion

        if (ready[0] & ready[1])
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
            currentState = menuState.mainMenu;

            //Reset ready up booleans
            ready[0] = false;
            ready[1] = false;

            //Let the geese fall down
            readyUpBounds.gameObject.SetActive(false);

            PlayerManager.instance.GetPlayer(0).TakeOffFromPlatform();
            PlayerManager.instance.GetPlayer(1).TakeOffFromPlatform();

            StartCoroutine(BounceyGeese(0));
            StartCoroutine(BounceyGeese(1));

            //Pan camera towards ground
            Camera.main.GetComponent<CameraController>().switchViews(false);
        }

        #region holding dash to quit
        if (Input.GetButton("Interact1") || Input.GetButton("Interact0"))
        {
            returnTimer += Time.deltaTime*.5f;
            returnImage.fillAmount = returnTimer;
            if (returnTimer > 1)
            {
                switchMenus(0);
                ready[0] = false;
                ready[1] = false;
                readyIndicator[0].color = Color.white;
                readyIndicator[1].color = Color.white;
                GameStateManager.instance.ChangeState(GameStates.STATE_MENU);
                PlayerManager.instance.ResetPlayers();
            }
        }
        else if (returnTimer > 0)
        {
            returnTimer -= Time.deltaTime * 2.5f;
            returnImage.fillAmount = returnTimer;
        }
        #endregion
    }

   public  IEnumerator BounceyGeese(int index)
    {
        yield return new WaitForSeconds(2);
        float lerpy = 0; 

        //Don't allow any other forces to act on the goose, we're in control now.
        PlayerManager.instance.GetPlayer(index).GooseyBod.isKinematic = true;

        while (lerpy < 1)
        {
            Vector3 pos =PlayerManager.instance.GetPlayer(index).transform.position;
            PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index>0 ? 6 : -6, bounceNess.Evaluate(lerpy)+2.15f, pos.z);
            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Give control back to the goose, since he's been so good.
        PlayerManager.instance.GetPlayer(index).GooseyBod.isKinematic = false;

        yield return new WaitForSeconds(0);
        //Make sure the countdown text is only triggered once
        if (index == 0)
            StartCoroutine(Timer.instance.TextInOut(true));
    }

    public AnimationCurve bounceNess;

    IEnumerator CharacterImageTransition(bool inOut, int index)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            if (inOut)
            {
                Color col = characterImg[index].color;
                col.a = lerpy;
                characterImg[index].color = col;
                characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImgEnd[index], characterImgStart[index], lerpy);
            }
            else
            {
                Color col = characterImg[index].color;
                col.a = 1 - lerpy;
                characterImg[index].color = col;
                characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImgStart[index], characterImgEnd[index], lerpy);
                //characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImg[index].rectTransform.anchoredPosition, characterImgEnd[index], lerpy);
            }
            lerpy += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
    }

    void changeSelection(Image[] cursors, Text[] menuElements, ref int menuIndex)
    {
        if ((Input.GetAxis("Vertical0") < 0 || Input.GetAxis("Vertical1") < 0) && !scrolling)
        {
            scrolling = true;
            menuIndex++;

            if (menuIndex > menuElements.Length - 1)
                menuIndex = 0;

            //move cursor
            cursors[0].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition + new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 75, 42);
            cursors[1].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition - new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 75, -42);
            Invoke("allowMove", 0.25f);

        }
        else if ((Input.GetAxis("Vertical0") > 0 || Input.GetAxis("Vertical1") > 0) && !scrolling)
        {
            scrolling = true;
            menuIndex--;

            if (menuIndex < 0)
                menuIndex = menuElements.Length - 1;

            //move cursor
            cursors[0].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition + new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 75, 42);
            cursors[1].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition - new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 75, -42);
            Invoke("allowMove", 0.25f);

        }
        else if (Input.GetAxis("Vertical0") == 0 && Input.GetAxis("Vertical1") == 0 && scrolling)
        {
            CancelInvoke("allowMove");
            scrolling = false;
        }
    }

    void mainMenu()
    {
        changeSelection(mainMenuCursor, mainMenuElements, ref mainMenuIndex);

        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
        {
            switch (mainMenuIndex)
            {
                case 0:
                    switchMenus(4);
                    //Camera.main.GetComponent<CameraController>().switchViews(false);
                    //Start game
                    break;
                case 1:
                    //View leaderboards
                    switchMenus(2);
                    break;
                case 2:
                    //View stats
                    switchMenus(3);
                    break;
                case 3:
                    //View options
                    switchMenus(1);
                    break;
                case 4:
                    //Credits sequence
                    break;
                case 5:
                    //Quit game
                    Application.Quit();
                    break;
            }
        }
    }

    void optionsMenu()
    {
        changeSelection(optionsCursor, optionsElements, ref optionsIndex);

        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
        {
            switch (optionsIndex)
            {
                case 0:
                    //do a thing
                    break;
                case 1:
                    //do a thing
                    break;
                case 2:
                    //do a thing
                    break;
                case 3:
                    //do a thing
                    break;
                case 4:
                    switchMenus(0);
                    break;
            }
        }
    }

    void allowMove()
    {
        scrolling = false;
    }
}

[CustomEditor(typeof(MainMenu))]
public class MainMenuTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MainMenu scriptToControl = (MainMenu)target;
        if (GUILayout.Button("bounce"))
        {
            scriptToControl.StartCoroutine(scriptToControl.BounceyGeese(0));
        }

    }
}