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

    public void playFootstepSound(int type)
    {
        switch (type)
        {
            case 0:
                //Play grass sounds
                int thisGrass = Random.Range(0, grassSteps.Count);
                SoundManager.instance.playSound(grassSteps[thisGrass], 0.25f);
                AudioClip mostRecentGrassSound = grassSteps[thisGrass];
                if (lastGrassStep)
                    grassSteps.Add(lastGrassStep);
                grassSteps.Remove(mostRecentGrassSound);
                lastGrassStep = mostRecentGrassSound;
                break;
            case 1:
                //Play wood sounds
                int thisWood = Random.Range(0, woodSteps.Count);
                SoundManager.instance.playSound(woodSteps[thisWood],0.25f);
                AudioClip mostRecentWoodSound = woodSteps[thisWood];
                if (lastWoodStep)
                    woodSteps.Add(lastWoodStep);
                woodSteps.Remove(mostRecentWoodSound);
                lastWoodStep = mostRecentWoodSound;
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
