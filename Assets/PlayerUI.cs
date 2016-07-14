using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour 
{
    [SerializeField]
    Player player;
    [SerializeField]
    Slider eggSlide;
	
	// Update is called once per frame
	void Update () 
    {
        if (player.eggMash > 0)
            eggSlide.gameObject.SetActive(true);
        else
            eggSlide.gameObject.SetActive(false);
        eggSlide.value = player.eggMash;
	}
}
