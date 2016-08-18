using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BrightnessModifier : MonoBehaviour {

    [Range(0,1)]
    public float brightness;
    float oldBrightness;
    [SerializeField]
    SpriteRenderer sr;
    
    void Update()
    {
        if (sr)
        {
            if (oldBrightness != brightness)
            {
                sr.color = new Color(brightness, brightness, brightness, 1);
                oldBrightness = brightness;
            }
        }
    }
}
