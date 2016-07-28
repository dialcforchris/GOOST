using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class footstepPlayer : MonoBehaviour {

    [SerializeField]
    List<AudioClip> grassSteps = new List<AudioClip>();
    AudioClip lastGrassStep;
    [SerializeField]
    List<AudioClip> woodSteps= new List<AudioClip>();
    AudioClip lastWoodStep;
    [SerializeField]
    Actor ActorComponent;

    public void playFootstepSound()
    {
        switch (ActorComponent.currentSurface)
        {
            case platformManager.platformTypes.grass:
                //Play grass sounds
                int thisGrass = Random.Range(0, grassSteps.Count);
                SoundManager.instance.playSound(grassSteps[thisGrass], ActorComponent.playerType == PlayerType.ENEMY ? 0.075f : 0.15f);
                AudioClip mostRecentGrassSound = grassSteps[thisGrass];
                if (lastGrassStep)
                    grassSteps.Add(lastGrassStep);
                grassSteps.Remove(mostRecentGrassSound);
                lastGrassStep = mostRecentGrassSound;
                break;
            case platformManager.platformTypes.wood:
                //Play wood sounds
                int thisWood = Random.Range(0, woodSteps.Count);
                SoundManager.instance.playSound(woodSteps[thisWood], ActorComponent.playerType == PlayerType.ENEMY ? 0.15f : 0.3f);
                AudioClip mostRecentWoodSound = woodSteps[thisWood];
                if (lastWoodStep)
                    woodSteps.Add(lastWoodStep);
                woodSteps.Remove(mostRecentWoodSound);
                lastWoodStep = mostRecentWoodSound;
                break;
            default:
                break;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
