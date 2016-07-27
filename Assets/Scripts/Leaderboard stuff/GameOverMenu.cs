using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverMenu : MenuSelect 
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
	if (Input.GetButton("Fly0")||Input.GetButton("Fly1"))
        switch(selectBox)
        {
            case 0:
                {
                    SceneManager.LoadScene(3);
                    break;
                }
            case 1:
                {
                    SceneManager.LoadScene(2);
                    break; 
                }
            case 2:
                {
                    Application.Quit();
                    break;
                }
        }

	}
}
