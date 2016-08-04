using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterName : MonoBehaviour
{
    public Text[] box;
    [SerializeField] private int playerNumber = 0;
    public Text playerName;
    int[] currentCharacter;
    public Text score;
    int selectBox = 0;
    int selectChar= 65;
    float coolDown = 0;
    float maxCool = 0.2f;
    string theName = string.Empty;
    public Canvas enterCan;
    public bool check = false;
	
    // Use this for initialization
	void Awake () 
    {
        coolDown = maxCool;
        currentCharacter = new int[box.Length];
        for (int i = 0; i < currentCharacter.Length;i++ )
        {
            currentCharacter[i] = 65;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
       StartCoroutine( HoldIt());
        // if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEOVER)
        {
            if (!check)
            {
                enterCan.enabled = true;
                if (LeaderBoard.instance.CheckIfHighScore(PlayerManager.instance.GetPlayer(playerNumber).GetScore()) && PlayerManager.instance.GetPlayer(playerNumber).GetScore() > 0)
                {
                    playerName.text = "Player " + (playerNumber + 1) + " Enter Name:";
                    MenuInput();
                    box[selectBox].text = ((char)currentCharacter[selectBox]).ToString();
                    ChangeTextColour();
                    score.text = "Player " + (playerNumber + 1) + " Score: " + PlayerManager.instance.GetPlayer(playerNumber).GetScore().ToString();
                    if (Input.GetButton("Fly" + playerNumber))
                    {
                        SelectName();
                        check = true;
                    }
                }
                else
                {
                    check = true;
                }
            }
            else
            {
                EnterNameManager.instance.Done(playerNumber);
                theName = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }
    IEnumerator HoldIt()
    {
        yield return new WaitForSeconds(1);
    }
    void MenuInput()
    {
        if (ConvertToPos())
        {
            if (Input.GetAxis("Horizontal"+playerNumber) != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Horizontal"+playerNumber) > 0)
                {
                    if (selectBox == box.Length - 1)
                    {
                        selectBox = 0;
                    }
                    else
                    {
                        selectBox++;
                    }
                }
                else if (Input.GetAxis("Horizontal"+playerNumber) < 0)
                {
                    if (selectBox == 0)
                    {
                        selectBox = box.Length - 1;
                    }
                    else
                    {
                        selectBox--;
                    }
                }
                selectChar = currentCharacter[selectBox];
                coolDown = 0;
            }
        }
        else
        {
            if (Input.GetAxis("Vertical"+playerNumber) != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Vertical"+playerNumber) < 0)
                {
                    if (selectChar > 65)
                        selectChar--;

                    else
                        selectChar = 90;
                }
                else if (Input.GetAxis("Vertical"+playerNumber) > 0)
                {
                    if (selectChar < 90)
                        selectChar++;

                    else
                        selectChar = 65;
                }
                coolDown = 0;
                currentCharacter[selectBox] = selectChar;
            }
        }
    }

    bool SelectCoolDown()
    {
        if (coolDown<maxCool)
        {
            coolDown += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }
    }

    void ChangeTextColour()
    {
       for (int i=0;i<box.Length;i++)
       {
           if (selectBox == i)
           {
               box[i].color = Color.green;
           }
           else
           {
               box[i].color = Color.white;
           }
        }
    }
    void SelectName()
    {
        if (theName!=null)
        {
            theName = null;
        }
        for (int i = 0; i < box.Length; i++)
        {
            theName = theName + box[i].text;
        }
        LeaderBoard.instance.AddNewScoreToLB(PlayerManager.instance.GetPlayer(playerNumber).GetScore(),theName);
        theName = string.Empty;
    }

    bool ConvertToPos()
    {
        float hori = Input.GetAxis("Horizontal"+playerNumber);
        float verti = Input.GetAxis("Vertical" + playerNumber);
        if (hori < 0)
        {
            hori -= hori * 2;
        }
        if (verti < 0)
        {
            verti -= verti * 2;
        }
        if (hori > verti)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void EnableIt(int _playerId)
    {
        playerNumber = _playerId;
        check = false;
        enterCan.gameObject.SetActive(true);
    }
  
}
