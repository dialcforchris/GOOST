using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOOST
{
    public class footstepPlayer : MonoBehaviour
    {

        [SerializeField]
        List<AudioClip> grassSteps = new List<AudioClip>();
        AudioClip lastGrassStep;
        [SerializeField]
        List<AudioClip> woodSteps = new List<AudioClip>();
        AudioClip lastWoodStep;
        [SerializeField]
        List<AudioClip> metalSteps = new List<AudioClip>();
        AudioClip lastMetalStep;
        [SerializeField]
        List<AudioClip> concreteSteps = new List<AudioClip>();
        AudioClip lastConcreteStep;
        [SerializeField]
        Actor ActorComponent;

        public void playFootstepSound()
        {
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
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
                    case platformManager.platformTypes.metal:
                        //Play wood sounds
                        int thisMetal = Random.Range(0, metalSteps.Count);
                        SoundManager.instance.playSound(metalSteps[thisMetal], ActorComponent.playerType == PlayerType.ENEMY ? 0.15f : 0.3f);
                        AudioClip mostRecentMetalSound = metalSteps[thisMetal];
                        if (lastMetalStep)
                            metalSteps.Add(lastMetalStep);
                        metalSteps.Remove(mostRecentMetalSound);
                        lastMetalStep = mostRecentMetalSound;
                        break;
                    case platformManager.platformTypes.concrete:
                        //Play concrete sounds
                        int thisConcrete = Random.Range(0, concreteSteps.Count);
                        SoundManager.instance.playSound(concreteSteps[thisConcrete], ActorComponent.playerType == PlayerType.ENEMY ? 0.15f : 0.3f);
                        AudioClip mostRecentConcreteSound = concreteSteps[thisConcrete];
                        if (lastConcreteStep)
                            concreteSteps.Add(lastConcreteStep);
                        concreteSteps.Remove(mostRecentConcreteSound);
                        lastConcreteStep = mostRecentConcreteSound;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}