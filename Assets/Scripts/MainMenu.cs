using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

    [Header("Main menu")]
    public Text[] mainMenuElements;
    public Image[] mainMenuCursor;

    public GameObject[] menuScreens;

    [Header("Options menu")]
    public Text[] optionsElements;
    public Image[] optionsCursor;

    int mainMenuIndex,optionsIndex;
    [SerializeField]
    bool transitioning;

    menuState currentState;
    enum menuState
    {
        mainMenu,
        readyUpScreen,
        optionsMenu,
        leaderboardsMenu,
        statsScreen,
    }

    IEnumerator rotateMenus(Vector3 start, Vector3 end)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            start = transform.rotation.eulerAngles;
            transform.rotation =  Quaternion.Euler(new Vector3(Mathf.LerpAngle(start.x,end.x,lerpy), Mathf.LerpAngle(start.y, end.y, lerpy), Mathf.LerpAngle(start.z, end.z, lerpy)));
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
                    transitioning = true;
                    currentState = menuState.readyUpScreen;
                    menuScreens[3].GetComponent<Canvas>().enabled = false;
                    menuScreens[4].SetActive(true);
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
                        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
                        {
                            switchMenus(0);
                        }
                        break;
                    case menuState.leaderboardsMenu:
                        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
                        {
                            switchMenus(0);
                        }
                        break;
                    case menuState.statsScreen:
                        if (Input.GetAxis("Interact0") > 0 || Input.GetButtonDown("Interact0") || Input.GetAxis("Interact1") > 0 || Input.GetButtonDown("Interact1"))
                        {
                            switchMenus(0);
                        }
                        break;
                    case menuState.optionsMenu:
                        optionsMenu();
                        break;
                }
            }
        }
    }

    void changeSelection(Image[] cursors,Text[] menuElements,ref int menuIndex)
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
            switch(mainMenuIndex)
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
