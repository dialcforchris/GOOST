using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
    public AudioClip[] stomps,screaming;
	// How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

    Vector3 originalPos;
	
	void OnEnable()
	{
        instance = this;
        originalPos = new Vector3(0, 0, -10);
    }
    
    public void GetGoosed()
    {
        StatTracker.instance.stats.gooseZillaSightings++;
        GetComponent<Animator>().Play("gooseZilla");
    }

    public void PlayScreamingSound()
    {
        SoundManager.instance.playSound(screaming[0],0.25f);
    }

    int stompIndex = 0;
    public void PlayStompSound()
    {
        SoundManager.instance.playSound(stomps[stompIndex]);
        stompIndex++;
        if (stompIndex > stomps.Length - 1)
            stompIndex = 0;
    }

    public void Shake()
    {
        PlayStompSound();
        shakeDuration += .5f;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Camera.main.transform.localPosition = (Vector3.up * shakeAmount) + originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;

            if (shakeDuration <= 0)
            {
                Camera.main.transform.position = new Vector3(0, 0, -10);
            }
        }
    }
}

[CustomEditor(typeof(CameraShake))]
public class GOOSEZILLA  : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraShake scriptToControl = (CameraShake)target;
        if (GUILayout.Button("Get Goosed"))
        {
            scriptToControl.GetGoosed();
        }

    }
}