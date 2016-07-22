using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterName : MonoBehaviour
{
    public Text[] box;
    int[] currentCharacter;
    public Text score;
    int selectBox = 0;
    int selectChar= 65;
    float coolDown = 0;
    float maxCool = 0.2f;
    string theName = string.Empty;
	
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
            MenuInput();
            box[selectBox].text = ((char)currentCharacter[selectBox]).ToString();
            ChangeTextColour();
        //    score.text = "Your Score: " + Player.instance.score.ToString();
            if (Input.GetButtonDown("Fire1"))
                SelectName();
    }

    void MenuInput()
    {
        if (ConvertToPos())
        {
            if (Input.GetAxis("Horizontal") != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Horizontal") > 0)
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
                else if (Input.GetAxis("Horizontal") < 0)
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
            if (Input.GetAxis("Vertical") != 0 && SelectCoolDown())
            {
                if (Input.GetAxis("Vertical") < 0)
                {
                    if (selectChar > 65)
                        selectChar--;

                    else
                        selectChar = 90;
                }
                else if (Input.GetAxis("Vertical") > 0)
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
        for (int i = 0; i < box.Length; i++)
        {
            theName = theName + box[i].text;
        }
       
        theName =  theName.Insert(3, "&");
        LeaderBoard.instance.SetName(theName);
        LeaderBoard.instance.AddNewScoreToLB();
        transform.parent.gameObject.SetActive(false);
    }

    bool ConvertToPos()
    {
        float hori = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");
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
}
