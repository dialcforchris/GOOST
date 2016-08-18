using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
    Text[] optionsElements,TextToFadeOut;
    [SerializeField]
    Slider[] soundsSliders;
    [SerializeField]
    Image[] optionsCursor,ImagesToFadeOut;
    [SerializeField]
    Image backdrop;
    [SerializeField]
    Text Credits;
        
    [Header("Level Selection")]
    [SerializeField]
    RectTransform[] levelSelectionElements;
    [SerializeField]
    Image[] levelSelectionCursor;
    [SerializeField]
    GameObject LevelSelector;

    [Header("Ready up menu")]
    [SerializeField]
    Text[] readyTextPrompts,nameFields;
    [SerializeField]
    GameObject readyUpBounds, leftCloudPlatform, rightCloudPlatform;
    [SerializeField]
    Image returnImage,versus;
    [SerializeField]
    Image[] characterImg, clouds;
    [SerializeField]
    Vector2[] characterImgStart, characterImgEnd;

    [Header("Cosmetics")]
    [SerializeField]
    Image[] CustomisableSlots;
    [SerializeField]
    Sprite[] backpacks, hats;
    [SerializeField]
    GameObject[] CustomGeese;

    [Header("Misc")]
    [SerializeField]
    Image ScreenFade;
    [SerializeField]
    float maxIdleTime, idleTime=0;

    int mainMenuIndex, optionsIndex, levelSelectionIndex;
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
        StartCoroutine(FadeScreenInOut(true));
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
                lerpy += Time.deltaTime;
                start = transform.rotation.eulerAngles;
                //transform.rotation = Quaternion.Euler(Vector3.Slerp(start, end, lerpy));
                transform.rotation = Quaternion.Euler(new Vector3(Mathf.LerpAngle(start.x, end.x, lerpy), Mathf.LerpAngle(start.y, end.y, lerpy), Mathf.LerpAngle(start.z, end.z, lerpy)));

                yield return new WaitForEndOfFrame();
            }
        }
        else if (start.y != 180)//|| start.y != 270)
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
                    LeaderBoard.instance.display.ShowTheDarnThing();
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

                    PlayerManager.instance.ResetPlayers();
                    EggPool.instance.Reset();
                    #region a lot of nonsense
                    //Make sure the players can't escape the screen
                    readyUpBounds.SetActive(true);

                    //Reset the everything
                    customised[0] = false;
                    customised[1] = false;
                    cosmeticIndex[0] = 0;
                    nameFields[0].text = "Mr  Handsomeware\n&\nHarold";
                    CustomisableSlots[0].sprite = hats[0];
                    cosmeticIndex[1] = 0;
                    nameFields[1].text = "Simon\n&\ntheir trusty goose";
                    CustomisableSlots[1].sprite = backpacks[0];
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
        #region menu navigation
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
                        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
                        {
                            TutorialAnimator.Play("tutorial_out");
                            transitioning = true;
                        }
                        break;
                    default:
                        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
                        {
                            switchMenus(0);
                        }
                        break;
                }
            }
        }
        if (GameStateManager.instance.GetState() == GameStates.STATE_READYUP && !transitioning)
        {
            ReadyUpScreen();
        }
        #endregion

        if (!Input.anyKey && GameStateManager.instance.GetState() != GameStates.STATE_GAMEPLAY
            && Input.GetAxis("Horizontal0") == 0 && Input.GetAxis("Horizontal1") == 0
             && Input.GetAxis("Vertical0") == 0 && Input.GetAxis("Vertical1") == 0)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            idleTime = 0;
        }

        if (idleTime > maxIdleTime && GameStateManager.instance.GetState() != GameStates.STATE_GAMEOVER)
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEOVER);
            StartCoroutine(FadeScreenInOut(false));
        }
    
    }

    IEnumerator FadeScreenInOut(bool inOut)//True for in, false for out
    {
        if (!inOut)
            ScreenFade.color = new Color(0, 0, 0, 0);

        float lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;
            ScreenFade.color = inOut ? new Color(1, 1, 1, 1 - lerpy) : new Color(0, 0, 0, lerpy);
            yield return new WaitForEndOfFrame();
        }
        if (!inOut)
            SceneManager.LoadScene(0);
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
        if (Input.GetButtonDown("Interact0") && bigHead[0] && !charTransitionP1)
        {
            charTransitionP1 = true;
            StartCoroutine(CharacterImageTransition(false, 0));
        }
        if (Input.GetButtonDown("Interact1") && bigHead[1] && !charTransitionP2)
        {
            charTransitionP2 = true;
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

            switch(cosmeticIndex[0])
            {
                case 0:
                    nameFields[0].text = "Mr  Handsomeware\n&\nHarold";
                    break;
                case 1:
                    nameFields[0].text = "Mr  Handsomeware\n&\nRiker";
                    break;
                case 2:
                    nameFields[0].text = "Mr  Handsomeware\n&\nBiggust goosus";
                    break;
                case 3:
                    nameFields[0].text = "Mr  Handsomeware\n&\nMr Handsomegoose";
                    break;
                case 4:
                    nameFields[0].text = "Mr  Handsomeware\n&\nleonard";
                    break;
                case 5:
                    nameFields[0].text = "Mr  Handsomeware\n&\nBilly";
                    break;
                case 6:
                    nameFields[0].text = "Mr  Handsomeware\n&\nbrunel";
                    break; 
                case 7:
                    nameFields[0].text = "Mr  Handsomeware\n&\nCool billy";
                    break;
                case 8:
                    nameFields[0].text = "Mr  Handsomeware\n&\nProf. Goosington";
                    break;
                default:
                    nameFields[0].text = "Mr  Handsomeware\n&\nleonard";
                    break;
            }

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

            switch (cosmeticIndex[1])
            {
                case 0:
                    nameFields[1].text = "Alan\n&\ntheir trusty goose";
                    break;
                case 1:
                    nameFields[1].text = "Trevor\n&\ntheir trusty goose";
                    break;
                case 2:
                    nameFields[1].text = "Geoff\n&\ntheir trusty goose";
                    break;
                case 3:
                    nameFields[1].text = "Steve\n&\ntheir trusty goose";
                    break;
                case 4:
                    nameFields[1].text = "John\n&\ntheir trusty goose";
                    break;
                default:
                    nameFields[1].text = "Jerry\n&\ntheir trusty goose";
                    break;
            }
            CustomisableSlots[1].sprite = backpacks[cosmeticIndex[1]];
        }
        if (scrolling[1] && Input.GetAxis("Horizontal1") == 0)
        {
            StopCoroutine(allowMove(1));
            scrolling[1] = false;
        }
        if (Input.GetButtonDown("Interact0") && characterImg[0].color.a < 0.1f && !customised[0])
        {
            customised[0] = true;
            //Setup player
            CustomGeese[0].SetActive(false);
            PlayerManager.instance.SetupPlayer(0);
            PlayerManager.instance.GetPlayer(0).TakeOffFromPlatform();
            PlayerManager.instance.GetPlayer(0).headSprite.sprite = hats[cosmeticIndex[0]];
            //readyTextPrompts[0].text = "Waiting for\nother player...";
            clouds[0].gameObject.SetActive(true);
            leftCloudPlatform.SetActive(true);

            ready[0] = true;
        }
        if (Input.GetButtonDown("Interact1") && characterImg[1].color.a < 0.1f && !customised[1])
        {
            customised[1] = true;
            //Setup player
            CustomGeese[1].SetActive(false);
            PlayerManager.instance.SetupPlayer(1);
            PlayerManager.instance.GetPlayer(1).TakeOffFromPlatform();
            PlayerManager.instance.GetPlayer(1).backpack.sprite = backpacks[cosmeticIndex[1]];
            
            //readyTextPrompts[1].text = "Waiting for\nother player...";
            clouds[1].gameObject.SetActive(true);
            rightCloudPlatform.SetActive(true);

            ready[1] = true;
        }
        #endregion

        #region ready up
        //Cool I'll just throw this all in the bin. 
        /*
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
         */
        #endregion

        #region start game when both players are ready
        if (ready[0] & ready[1])
        {
            //Reset ready up booleans
            ready[0] = false;
            ready[1] = false;
            StartCoroutine(GameStart());
        }
        #endregion

        #region holding dash to quit
        if ((Input.GetButton("Fly1") || Input.GetButton("Fly0")) && !Timer.instance.countdown)
        {
            returnTimer += Time.deltaTime * .5f;
            returnImage.fillAmount = returnTimer;
            if (returnTimer > 1)
            {
                switchMenus(5);
                StartCoroutine(CharacterImageTransition(false, 0));
                StartCoroutine(CharacterImageTransition(false, 1));
                ready[0] = false;
                ready[1] = false;
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

    IEnumerator GameStart()
    {
        Timer.instance.Reset();
        StartCoroutine(Timer.instance.TextInOut(true));
        //readyTextPrompts[1].text = "Game starting!";
        //readyTextPrompts[0].text = "Game starting!";
        yield return new WaitForSeconds(2.5f);
        GameStateManager.instance.ChangeState(GameStates.STATE_TRANSITIONING);
        currentState = menuState.mainMenu;

        //Let the geese fall down
        readyUpBounds.gameObject.SetActive(false);

        PlayerManager.instance.ResetGameStart();
        PlayerManager.instance.GetPlayer(0).TakeOffFromPlatform();
        PlayerManager.instance.GetPlayer(1).TakeOffFromPlatform();
        
        StartCoroutine(BounceyGeese(0, level));
        StartCoroutine(BounceyGeese(1, level));

        //Pan camera towards ground
        Camera.main.GetComponent<CameraController>().switchViews(false);
    }

    public IEnumerator BounceyGeese(int index, int level)
    {
        yield return new WaitForSeconds(2);
        float lerpy = 0;

        //Don't allow any other forces to act on the goose, we're in control now.
        PlayerManager.instance.GetPlayer(index).GooseyBod.isKinematic = true;

        SoundManager.instance.playSound(woodLandingSound);
        while (lerpy < 1)
        {
            Vector3 pos = PlayerManager.instance.GetPlayer(index).transform.position;
            if (level == 0)
            {
                PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index > 0 ? 6 : -6, bounceNess.Evaluate(lerpy) + 2.15f, pos.z);
            }
            else if (level==1)
            {
                PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index > 0 ? 7.75f : -7.75f, (bounceNess.Evaluate(lerpy) * 1.25f) - 0.4f, pos.z);
            }
            else
            {
                if (index == 0)
                    PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index > 0 ? 6 : -6, bounceNess.Evaluate(lerpy) + 0.75f, pos.z);
                else
                    PlayerManager.instance.GetPlayer(index).transform.position = new Vector3(index > 0 ? 6 : -6, (bounceNess.Evaluate(lerpy) * 1.25f) - 1, pos.z);
            }
            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Give control back to the goose, since he's been so good.
        PlayerManager.instance.GetPlayer(index).GooseyBod.isKinematic = false;

        //Make sure the countdown text is only triggered once
        if (index == 0)
        {
            //Timer.instance.Reset();
            //StartCoroutine(Timer.instance.TextInOut(true));
        }
    }

    public AnimationCurve bounceNess;
    [SerializeField]
    AudioClip clash;
    IEnumerator versusImageInOut(bool inOut)//TRUE for IN, FALSE for OUT
    {
        while (transitioning)
            yield return null;

        float lerpy = 0;
        while (lerpy<1)
        {
            lerpy += Time.fixedDeltaTime * 1.5f;

            Color col = versus.color;
            if (inOut)
            {
                col.a = lerpy;
                versus.color = col;
                versus.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(65, 1750), new Vector2(65, 250), lerpy);
            }
            else
            {
                col.a = 1 - lerpy;
                versus.color = col;
                versus.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(65, 250), new Vector2(65, 1750), lerpy);
            }
            yield return new WaitForEndOfFrame();
        }
        if (inOut)
        {
            ScreenFade.color = Color.white;
            SoundManager.instance.playSound(clash);
            yield return new WaitForEndOfFrame();
            Color colour = ScreenFade.color;
            colour.a = 0;
            ScreenFade.color = colour;
        }
    }

    bool charTransitionP1, charTransitionP2;
    //Moves big character images out of screen, sets custom geese to active
    IEnumerator CharacterImageTransition(bool inOut, int index)
    {
        if (!inOut && versus.rectTransform.anchoredPosition.y < 251)
        {
            StopCoroutine("versusImageInOut");
            StartCoroutine(versusImageInOut(inOut));
        }
        else if (inOut)
        {
            StopCoroutine("versusImageInOut");
            StartCoroutine(versusImageInOut(inOut));
        }

        float lerpy = 0;
        CustomGeese[index].SetActive(!inOut);
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * 1.5f;

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
            }
            else
            {
                Color col = characterImg[index].color;
                col.a = 1 - lerpy;
                characterImg[index].color = col;
                characterImg[index].rectTransform.anchoredPosition = Vector2.Lerp(characterImgStart[index], characterImgEnd[index], lerpy);
            }

            yield return new WaitForEndOfFrame();

        }
        if (!inOut)
            bigHead[index] = false;
        else
            bigHead[index] = true;

        if (index == 0)
        {
            charTransitionP1 = false;
        }
        else if (index == 1)
        {
            charTransitionP2 = false;
        }
    }

    void changeSelection(Image[] cursors, RectTransform[] menuElements, ref int menuIndex, float yOffset = 0)
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
        else if (((Input.GetAxis("Vertical0") > 0 && !scrolling[0]) || (Input.GetAxis("Vertical1") > 0) && !scrolling[1]))
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
        else if (Input.GetAxis("Vertical1") == 0 && scrolling[1])
        {
            StopCoroutine(allowMove(1));
            scrolling[1] = false;
        }
    }

    void mainMenu()
    {
        changeSelection(mainMenuCursor, GetRectTransforms(mainMenuElements), ref mainMenuIndex, 42);

        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
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
                    //Application.Quit();
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
        changeSelection(optionsCursor, GetRectTransforms(optionsElements), ref optionsIndex, 42);

        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
        {
            switch (optionsIndex)
            {
                case 2:
                    //credits
                    transitioning = true;
                    StartCoroutine(DisplayCredits());
                    break;
                case 3:
                    switchMenus(0);
                    break;
            }
        }
        #region sound sliders
        if (Input.GetAxis("Horizontal0") < 0 || Input.GetAxis("Horizontal1") < 0)
        {
            switch (optionsIndex)
            {
                case 0:
                    soundsSliders[0].value -= Time.fixedDeltaTime;
                    break;
                case 1:
                    soundsSliders[1].value -= Time.fixedDeltaTime;
                    break;
            }
        }
        if (Input.GetAxis("Horizontal0") > 0 || Input.GetAxis("Horizontal1") > 0)
        {
            switch (optionsIndex)
            {
                case 0:
                    soundsSliders[0].value += Time.fixedDeltaTime;
                    break;
                case 1:
                    soundsSliders[1].value += Time.fixedDeltaTime;
                    break;
            }
        }
        #endregion
    }

    IEnumerator DisplayCredits()
    {
        float lerpy = 0;
        #region fade out everything
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;
            if (lerpy > 1)
                lerpy = 1;
            foreach (Image i in ImagesToFadeOut)
            {
                Color ImageCol = i.color;
                ImageCol.a = Mathf.Lerp(1, 0, lerpy);
                i.color = ImageCol;
                if (i.GetComponent<Outline>())
                {
                    ImageCol = i.GetComponent<Outline>().effectColor;
                    ImageCol.a = Mathf.Lerp(1, 0, lerpy);
                    i.GetComponent<Outline>().effectColor = ImageCol;
                }
            }
            foreach (Text t in TextToFadeOut)
            {
                Color ImageCol = t.color;
                ImageCol.a = Mathf.Lerp(1, 0, lerpy);
                t.color = ImageCol;
                if (t.GetComponent<Outline>())
                {
                    ImageCol = t.GetComponent<Outline>().effectColor;
                    ImageCol.a = Mathf.Lerp(1, 0, lerpy);
                    t.GetComponent<Outline>().effectColor = ImageCol;
                }
            }

            Color backdropCol = backdrop.color;
            backdropCol.a = Mathf.Lerp(0, .5f, lerpy);
            backdrop.color = backdropCol;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        //Scroll credits up
        lerpy = 0;
        while (Credits.rectTransform.anchoredPosition.y < 2150)
        {
            lerpy += Time.deltaTime;
            Credits.rectTransform.anchoredPosition = Vector2.Lerp(-Vector2.up * 1250, Vector2.up * 2150, lerpy / 25);
            if (Input.GetButton("Interact0") || Input.GetButton("Interact1"))
                lerpy += Time.deltaTime * 5;

            yield return new WaitForEndOfFrame();
        }

        //Reset credit location
        Credits.rectTransform.anchoredPosition = -Vector2.up * 1250;


        lerpy = 0;
        #region fade in everything
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;
            foreach (Image i in ImagesToFadeOut)
            {
                Color ImageCol = i.color;
                ImageCol.a = Mathf.Lerp(0, 1, lerpy);
                i.color = ImageCol;
                if (i.GetComponent<Outline>())
                {
                    ImageCol = i.GetComponent<Outline>().effectColor;
                    ImageCol.a = Mathf.Lerp(0, 1, lerpy);
                    i.GetComponent<Outline>().effectColor = ImageCol;
                }
            }
            foreach (Text t in TextToFadeOut)
            {
                Color ImageCol = t.color;
                ImageCol.a = Mathf.Lerp(0, 1, lerpy);
                t.color = ImageCol;
                if (t.GetComponent<Outline>())
                {
                    ImageCol = t.GetComponent<Outline>().effectColor;
                    ImageCol.a = Mathf.Lerp(0, 1, lerpy);
                    t.GetComponent<Outline>().effectColor = ImageCol;
                }
            }
            Color backdropCol = backdrop.color;
            backdropCol.a = Mathf.Lerp(.5f, 0, lerpy);
            backdrop.color = backdropCol;
            yield return new WaitForEndOfFrame();
        }
        #endregion
        transitioning = false;
    }

    int level;
    public int getLevel()
    {
        return level;
    }
    void LevelSelectionMenu()
    {
        //Select level
        changeSelection(levelSelectionCursor, levelSelectionElements, ref levelSelectionIndex);

        if (Input.GetButtonDown("Interact0") || Input.GetButtonDown("Interact1"))
        {
            switch (levelSelectionIndex)
            {
                case 0:
                    switchMenus(4);
                    //Select woodland level
                    level = 0;
                    LevelSelector.transform.position = new Vector3(-50, 0, 0);
                    break;
                case 1:
                    switchMenus(4);
                    //Labs level
                    level = 1;
                    LevelSelector.transform.position = new Vector2 (50,0);
                    break;
                case 2:
                    switchMenus(4);
                    //City level
                    level = 2;
                    LevelSelector.transform.position = Vector3.zero;//change this
                    break;
            }
        }
        if (Input.GetButtonDown("Fly1") || Input.GetButtonDown("Fly0"))
        {
            //Back to main menu
            switchMenus(0);
        }
    }

    IEnumerator allowMove(int index)
    {
        yield return new WaitForSeconds(0.25f);
        scrolling[index] = false;
    }
}   