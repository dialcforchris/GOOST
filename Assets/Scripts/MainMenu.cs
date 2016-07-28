using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;

    [Header("Main menu")]
    [SerializeField]
    Text[] mainMenuElements;
    [SerializeField]
    Image[] mainMenuCursor;
    [SerializeField]
    Animator TutorialAnimator;

    [SerializeField]
    GameObject[] menuScreens;
    //Main menu
    //Options
    //Leaderboard
    //Stats
    //Ready up
    //Level Select
    [SerializeField]
    AudioClip woodLandingSound;

    [SerializeField]
    AudioClip[] wooshNoises;
    bool whichWoosh;

    [Header("Options menu")]
    [SerializeField]
    Text[] optionsElements;
    [SerializeField]
    Image[] optionsCursor;

    [Header("Level Selection")]
    [SerializeField]
    Image[] levelSelectionElements;
    [SerializeField]
    Image[] levelSelectionCursor;

    [Header("Ready up menu")]
    [SerializeField]
    Text[] readyTextPrompts;
    [SerializeField]
    GameObject readyUpBounds,leftCloudPlatform,rightCloudPlatform;
    [SerializeField]
    Image returnImage;
    [SerializeField]
    Image[] readyIndicator, characterImg,clouds;
    [SerializeField]
    Vector2[] characterImgStart, characterImgEnd;

    [Header("Cosmetics")]
    [SerializeField]
    Image[] CustomisableSlots;
    [SerializeField]
    Sprite[] backpacks, hats;
    [SerializeField]
    GameObject[] CustomGeese;

    int mainMenuIndex, optionsIndex,levelSelectionIndex;
    bool transitioning;

    public menuState currentState;
    public enum menuState
    {
        mainMenu,
        readyUpScreen,
        optionsMenu,
        leaderboardsMenu,
        statsScreen,
        levelSelection,
        tutorial,
    }

    public void Awake()
    {
        instance = this;
        ready = new bool[2] { false, false };
        pressingReady = new bool[2] { false, false };
        scrolling = new bool[2] { false, false };
        customised = new bool[2] { false, false };
        bigHead = new bool[2] { true, true };
        cosmeticIndex = new int[2] { 0, 0 };
    }

    IEnumerator rotateMenus(Vector3 start, Vector3 end)
    {
        Vector3 difference = end - start;
        float lerpy = 0;
        if (end.x != -180 && start.y != 180)
        {
            while (lerpy < 1 && start.y != 180)
            {
                start = transform.rotation.eulerAngles;
                //transform.rotation = Quaternion.Euler(Vector3.Slerp(start, end, lerpy));
                transform.rotation = Quaternion.Euler(new Vector3(Mathf.LerpAngle(start.x, end.x, lerpy), Mathf.LerpAngle(start.y, end.y, lerpy), Mathf.LerpAngle(start.z, end.z, lerpy)));

                lerpy += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (start.y != 180 )//|| start.y != 270)
        {
            while (lerpy < 18)
            {
                lerpy++;
                transform.Rotate(Vector3.left, 5);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (lerpy < 18)
            {
                lerpy++;    
                transform.Rotate(Vector3.right, 5);
                yield return new WaitForEndOfFrame();
            }
        }
        transitioning = false;
    }

    public void switchMenus(int menu)
    {
        if (!transitioning)
        {
            SoundManager.instance.playSound(wooshNoises[whichWoosh ? 0 : 1]);
            whichWoosh = !whichWoosh;
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
                    menuScreens[4].SetActive(false);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(0, -90, 0)));
                    break;
                case 2:
                    transitioning = true;
                    currentState = menuState.leaderboardsMenu;
                    LeaderBoard.instance.display.update();
                    menuScreens[2].SetActive(true);
                    menuScreens[1].SetActive(false);
                    menuScreens[4].SetActive(false);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(0, 90, 0)));
                    break;
                case 3:
                    transitioning = true;
                    currentState = menuState.statsScreen;
                    menuScreens[3].GetComponent<Canvas>().enabled = true;
                    menuScreens[4].SetActive(false);
                    menuScreens[5].SetActive(false);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(90, 0, 0)));
                    break;
                case 4:
                    //no button pressing allowed during this time
                    transitioning = true;

                    currentState = menuState.readyUpScreen;
                    GameStateManager.instance.ChangeState(GameStates.STATE_READYUP);
                    
                    #region a lot of nonsense
                    //Make sure the players can't escape the screen
                    readyUpBounds.SetActive(true);

                    //Reset the everything
                    readyTextPrompts[0].text = "Press        \nto spawn";
                    readyTextPrompts[1].text = "Press \n to spawn";
                    customised[0] = false;
                    customised[1] = false;
                    cosmeticIndex[0] = 0;
                    cosmeticIndex[1] = 0;
                    bigHead[0] = true;
                    bigHead[1] = true;
                    CustomGeese[0].SetActive(false);
                    CustomGeese[1].SetActive(false);
                    clouds[0].gameObject.SetActive(true);
                    clouds[1].gameObject.SetActive(true);

                    //Turn some off/on to help aid visibility
                    menuScreens[3].GetComponent<Canvas>().enabled = false;
                    menuScreens[4].SetActive(true);

                    //Reset return text;
                    returnTimer = 0;
                    returnImage.fillAmount = 0;

                    //Tween in character images
                    StartCoroutine(CharacterImageTransition(true, 0));
                    StartCoroutine(CharacterImageTransition(true, 1));
                    #endregion
                    //Display the correct menu
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(-180, 0, 0)));
                    break;
                case 5:
                    transitioning = true;
                    currentState = menuState.levelSelection;
                    menuScreens[3].GetComponent<Canvas>().enabled = false;
                    menuScreens[5].SetActive(true);
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(-90, 0, 0)));
                    break;
            }
        }
    }

    bool[] scrolling;
    void Update()
    {
        if (TutorialAnimator.GetCurrentAnimatorStateInfo(0).IsName("tutorial_idle") && transitioning)
            transitioning = false;
        if (TutorialAnimator.GetCurrentAnimatorStateInfo(0).IsName("tutorial_out_idle") && currentState == menuState.tutorial)
        {
            transitioning = false;
            currentState = menuState.mainMenu;
        }
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
                    case menuState.levelSelection:
                        LevelSelectionMenu();
                        break;
                    case menuState.tutorial:
                        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
                        {
                            TutorialAnimator.Play("tutorial_out");
                            transitioning = true;
                        }
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
    bool[] ready; //When both players are ready, start game
    bool[] bigHead; //Is there still a big head on screen. True for yes, false for no.
    bool[] customised; //Cosmetics stage completed?
    bool[] pressingReady; //QoL for button pressing
    int[] cosmeticIndex;

    void ReadyUpScreen()
    {
        //Press button
        //Get rid of faces
        //Display goose customisability bit
        //Comfirm to spawn goose
        //Then standard ready up bs

        if (Input.GetJoystickNames().Length<PlayerManager.instance.NumberOfPlayers())
        {
            ready[1] = true;
        }

        if (Input.GetButtonDown("Fly0") && bigHead[0])
        {
            StartCoroutine(CharacterImageTransition(false, 0));
        }
        if (Input.GetButtonDown("Fly1") && bigHead[1])
        {
            StartCoroutine(CharacterImageTransition(false, 1));
        }

        #region select cosmetics
        if (Mathf.Abs(Input.GetAxis("Horizontal0")) > 0 && !bigHead[0] && !customised[0] && !scrolling[0])
        {
            //Scroll control
            scrolling[0] = true;
            StopCoroutine(allowMove(0));
            StartCoroutine(allowMove(0));

            //Change cosmetic index
            cosmeticIndex[0] += (Input.GetAxis("Horizontal0") > 0) ? 1 : -1;
            if (cosmeticIndex[0] > hats.Length - 1)
                cosmeticIndex[0] = 0;
            else if (cosmeticIndex[0] < 0)
                cosmeticIndex[0] = hats.Length - 1;

            //Change cosmetic
            CustomisableSlots[0].sprite = hats[cosmeticIndex[0]];
        }
        if (scrolling[0] && Input.GetAxis("Horizontal0") == 0)
        {
            StopCoroutine(allowMove(0));
            scrolling[0] = false;
        }
        if (Mathf.Abs(Input.GetAxis("Horizontal1")) > 0 && !bigHead[1] && !customised[1] && !scrolling[1])
        {
            //Scroll control
            scrolling[1] = true;
            StopCoroutine(allowMove(1));
            StartCoroutine(allowMove(1));

            cosmeticIndex[1] += (Input.GetAxis("Horizontal1") > 0) ? 1 : -1;
            if (cosmeticIndex[1] > backpacks.Length - 1)
                cosmeticIndex[1] = 0;
            else if (cosmeticIndex[1] < 0)
                cosmeticIndex[1] = backpacks.Length - 1;

            CustomisableSlots[1].sprite = backpacks[cosmeticIndex[1]];
        }
        if (scrolling[1] && Input.GetAxis("Horizontal1") == 0)
        {
            StopCoroutine(allowMove(1));
            scrolling[1] = false;
        }
        if (Input.GetButtonDown("Fly0") && characterImg[0].color.a < 0.1f && !customised[0])
        {
            customised[0] = true;
            //Setup player
            CustomGeese[0].SetActive(false);
            PlayerManager.instance.SetupPlayer(0);
            PlayerManager.instance.GetPlayer(0).headSprite.sprite = hats[cosmeticIndex[0]];
            readyTextPrompts[0].text = "Press Dash and Fly\n buttons to ready up";
            clouds[0].gameObject.SetActive(true);
            leftCloudPlatform.SetActive(true);
        }
        if (Input.GetButtonDown("Fly1") && characterImg[1].color.a < 0.1f && !customised[1])
        {
            customised[1] = true;
            //Setup player
            CustomGeese[1].SetActive(false);
            PlayerManager.instance.SetupPlayer(1);
            PlayerManager.instance.GetPlayer(1).backpack.sprite = backpacks[cosmeticIndex[1]];

            readyTextPrompts[1].text = "Press Dash and Fly\n buttons to ready up";
            clouds[1].gameObject.SetActive(true);
            rightCloudPlatform.SetActive(true);
        }
        #endregion

        #region ready up
        //Player 1
        if (Input.GetButton("Fly0") && Input.GetButton("Interact0") && !pressingReady[0] && customised[0])
        {
            pressingReady[0] = true;
            ready[0] = !ready[0];
            readyIndicator[0].color = ready[0] ? Color.yellow : Color.white;
            readyTextPrompts[0].text = "Press Dash and Fly\n again to cancel";
            //Mb play a sound?
        }
        else if (!Input.GetButton("Fly0") && !Input.GetButton("Interact0") && customised[0])
            pressingReady[0] = false;

        //Player 2
        if (Input.GetButton("Fly1") && Input.GetButton("Interact1") && !pressingReady[1] && customised[1])
        {
            pressingReady[1] = true;
            ready[1] = !ready[1];
            readyIndicator[1].color = ready[1] ? Color.yellow : Color.white;
            readyTextPrompts[1].text = "Press Dash and Fly\n again to cancel";
            //Mb play a sound?
        }
        else if (!Input.GetButton("Fly1") && !Input.GetButton("Interact1") && customised[0])
            pressingReady[1] = false;

        #endregion

        #region start game when both players are ready
        if (ready[0] & ready[1])
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
            currentState = menuState.mainMenu;

            //Reset ready up booleans
            ready[0] = false;
            ready[1] = false;

            //Let the geese fall down
            readyUpBounds.gameObject.SetActive(false);

            PlayerManager.instance.ResetGameStart();
            PlayerManager.instance.GetPlayer(0).TakeOffFromPlatform();
            PlayerManager.instance.GetPlayer(1).TakeOffFromPlatform();

            StartCoroutine(BounceyGeese(0));
            StartCoroutine(BounceyGeese(1));

            //Pan camera towards ground
            Camera.main.GetComponent<CameraController>().switchViews(false);
        }
        #endregion

        #region holding dash to quit
        if (Input.GetButton("Interact1") || Input.GetButton("Interact0"))
        {
            returnTimer += Time.deltaTime * .5f;
            returnImage.fillAmount = returnTimer;
            if (returnTimer > 1)
            {
                switchMenus(5);
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

    public IEnumerator BounceyGeese(int index)
    {
        yield return new WaitForSeconds(2);
        float lerpy = 0;

        //Don't allow any other forces to act on the goose, we're in control now.
        PlayerManager.instance.GetPlayer(index).GooseyBod.isKinematic = true;

        SoundManager.instance.playSound(woodLandingSound);
        while (lerpy < 1)
        {
            Vector3 pos = PlayerManager.instance.GetPlayer(index).transform.position;
            PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index > 0 ? 6 : -6, bounceNess.Evaluate(lerpy) + 2.15f, pos.z);
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

    //Moves big character images out of screen, sets custom geese to active
    IEnumerator CharacterImageTransition(bool inOut, int index)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            if (inOut)
            {
                if (index == 0)
                    leftCloudPlatform.SetActive(false);
                else
                    rightCloudPlatform.SetActive(false);

                clouds[index].gameObject.SetActive(false);
                //Fade in character image
                Color col = characterImg[index].color;
                col.a = lerpy;
                characterImg[index].color = col;
                //Move character image out of screen
                characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImgEnd[index], characterImgStart[index], lerpy);

                CustomGeese[index].SetActive(false);
            }
            else
            {
                Color col = characterImg[index].color;
                col.a = 1 - lerpy;
                characterImg[index].color = col;
                characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImgStart[index], characterImgEnd[index], lerpy);

                CustomGeese[index].SetActive(true);
            }
            lerpy += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        if (!inOut)
            bigHead[index] = false;
    }

    void changeSelection(Image[] cursors, RectTransform[] menuElements, ref int menuIndex,float yOffset = 0)
    {
        if (((Input.GetAxis("Vertical0") < 0 && !scrolling[0]) || (Input.GetAxis("Vertical1") < 0) && !scrolling[1]))
        {
            if (Input.GetAxis("Vertical0") < 0)
            {
                scrolling[0] = true;
                StartCoroutine(allowMove(0));
            }
            else
            {
                scrolling[1] = true;
                StartCoroutine(allowMove(1));
            }
            menuIndex++;

            if (menuIndex > menuElements.Length - 1)
                menuIndex = 0;

            //move cursor
            cursors[0].rectTransform.anchoredPosition = menuElements[menuIndex].anchoredPosition + new Vector2(menuElements[menuIndex].sizeDelta.x / 2 + 75, yOffset);
            cursors[1].rectTransform.anchoredPosition = menuElements[menuIndex].anchoredPosition - new Vector2(menuElements[menuIndex].sizeDelta.x / 2 + 75, -yOffset);

        }
        else if (((Input.GetAxis("Vertical0") > 0 && !scrolling[0])|| (Input.GetAxis("Vertical1") > 0) && !scrolling[1]))
        {
            if (Input.GetAxis("Vertical0") > 0)
            {
                scrolling[0] = true;
                StartCoroutine(allowMove(0));
            }
            else
            {
                scrolling[1] = true;
                StartCoroutine(allowMove(1));
            }
            menuIndex--;

            if (menuIndex < 0)
                menuIndex = menuElements.Length - 1;

            //move cursor
            cursors[0].rectTransform.anchoredPosition = menuElements[menuIndex].anchoredPosition + new Vector2(menuElements[menuIndex].sizeDelta.x / 2 + 75, yOffset);
            cursors[1].rectTransform.anchoredPosition = menuElements[menuIndex].anchoredPosition - new Vector2(menuElements[menuIndex].sizeDelta.x / 2 + 75, -yOffset);
        }
        else if (Input.GetAxis("Vertical0") == 0 && scrolling[0])
        {
            StopCoroutine(allowMove(1));
            scrolling[0] = false;
        }
        else if (Input.GetAxis("Vertical1") ==0 && scrolling[1])
        {
            StopCoroutine(allowMove(1));
            scrolling[1] = false;
        }
    }

    void mainMenu()
    {         
        changeSelection(mainMenuCursor, GetRectTransforms(mainMenuElements), ref mainMenuIndex,42);

        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
        {
            switch (mainMenuIndex)
            {
                case 0:
                    switchMenus(5);
                    //Camera.main.GetComponent<CameraController>().switchViews(false);
                    //Start game
                    break;
                case 1:
                    //Tutorial
                    transitioning = true;
                    currentState = menuState.tutorial;
                    TutorialAnimator.Play("tutorial_in");
                    break;
                case 2:
                    //View leaderboards
                    switchMenus(2);
                    break;
                case 3:
                    //View stats
                    switchMenus(3);
                    break;
                case 4:
                    //View options
                    switchMenus(1);
                    break;
                case 5:
                    //Quit game
                    Application.Quit();
                    break;
            }
        }
    }

    RectTransform[] GetRectTransforms(Text[] textElements)
    {
        RectTransform[] RTs = new RectTransform[textElements.Length];

        int i = 0;
        foreach (Text t in textElements)
        {
            RTs[i] = t.rectTransform;
            i++;
        }
        return RTs;
    }
    RectTransform[] GetRectTransforms(Image[] textElements)
    {
        RectTransform[] RTs = new RectTransform[textElements.Length];

        int i = 0;
        foreach (Image t in textElements)
        {
            RTs[i] = t.rectTransform;
            i++;
        }
        return RTs;
    }

    void optionsMenu()
    {
        changeSelection(optionsCursor, GetRectTransforms(optionsElements), ref optionsIndex,42);

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

    void LevelSelectionMenu()
    {
        //Select level
        changeSelection(levelSelectionCursor, GetRectTransforms(levelSelectionElements), ref levelSelectionIndex);

        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
        {
            switch (levelSelectionIndex)
            {
                case 0:
                    switchMenus(4);
                    //Select woodland level
                    break;
                case 1:
                    switchMenus(4);
                    //City level
                    break;
                case 2:
                    //Back to main menu
                    switchMenus(0);
                    break;
            }
        }
    }

    IEnumerator allowMove(int index)
    {
        yield return new WaitForSeconds(0.25f);
        scrolling[index] = false;
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