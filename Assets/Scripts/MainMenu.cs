using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

    [Header("Main menu")]
    public Text[] menuElements;
    public Image[] cursor;
    public GameObject[] menuScreens;
    int menuIndex;
    bool transitioning;

    menuState currentState;
    enum menuState
    {
        mainMenu,
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
                    StartCoroutine(rotateMenus(transform.rotation.eulerAngles, new Vector3(90, 0, 0)));
                    break;
            }
        }
    }

    bool scrolling;
    void Update()
    {
        if (!transitioning)
        {
            if (currentState == menuState.mainMenu)
            {
                mainMenu();
            }
            else if (currentState == menuState.leaderboardsMenu)
            {
                if (Input.GetAxis("Fire1") > 0)
                {
                    switchMenus(0);
                }
            }
            else if (currentState == menuState.statsScreen)
            {
                if (Input.GetAxis("Fire1") > 0)
                {
                    switchMenus(0);
                }
            }
            else if (currentState == menuState.optionsMenu)
            {
                if (Input.GetAxis("Fire1") > 0)
                {
                    switchMenus(0);
                }
            }
        }
    }

    void mainMenu()
    {
        #region Move up/down
        if (Input.GetAxis("Vertical") < 0 && !scrolling)
        {
            scrolling = true;
            menuIndex++;

            if (menuIndex > menuElements.Length - 1)
                menuIndex = 0;

            //move cursor
            cursor[0].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition + new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 100, 32);
            cursor[1].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition - new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 100, -32);
            Invoke("allowMove", 0.25f);

        }
        else if (Input.GetAxis("Vertical") > 0 && !scrolling)
        {
            scrolling = true;
            menuIndex--;

            if (menuIndex < 0)
                menuIndex = menuElements.Length - 1;

            //move cursor
            cursor[0].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition + new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 100, 32);
            cursor[1].rectTransform.anchoredPosition = menuElements[menuIndex].rectTransform.anchoredPosition - new Vector2(menuElements[menuIndex].rectTransform.sizeDelta.x / 2 + 100, -32);
            Invoke("allowMove", 0.25f);

        }
        else if (Input.GetAxis("Vertical") == 0 && scrolling)
        {
            CancelInvoke("allowMove");
            scrolling = false;
        }
        #endregion

        if (Input.GetAxis("Fire1") > 0)
        {
            switch(menuIndex)
            {
                case 0:
                    Camera.main.GetComponent<CameraController>().switchViews(false);
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

    void allowMove()
    {
        scrolling = false;
    }

}

[CustomEditor(typeof(MainMenu))]
public class tempUIMenuMover : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MainMenu scriptToControl = (MainMenu)target;
        if (GUILayout.Button("main menu"))
        {
            scriptToControl.switchMenus(0);
        }
        if (GUILayout.Button("options"))
        {
            scriptToControl.switchMenus(1);
        }
        if (GUILayout.Button("leaderboards"))
        {
            scriptToControl.switchMenus(2);
        }
        if (GUILayout.Button("stats"))
        {
            scriptToControl.switchMenus(3);
        }
    }
}
