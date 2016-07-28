using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform[] camTransforms;
	
	// How long the object should shake for.
	public float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	List<Vector3> originalPos = new List<Vector3>();
	
	void OnEnable()
	{
        instance = this;
        originalPos.Clear();
        foreach(Transform v in camTransforms)
        {
            originalPos.Add(v.localPosition);
        }
	}

    void Update()
    {
        if (shakeDuration > 0)
        {
            for (int i = 0; i < camTransforms.Length; i++)
            {
                camTransforms[i].localPosition = originalPos[i] + Random.insideUnitSphere * shakeAmount;
            }
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;

            for (int i = 0; i < camTransforms.Length; i++)
            {
                camTransforms[i].localPosition = originalPos[i];
            }
        }
    }
}
