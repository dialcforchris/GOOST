using UnityEngine;
using UnityEditor;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public void switchViews(bool upDown)
    {
        StopAllCoroutines();
        Vector3 up= new Vector3(0, 30, -10);
        Vector3 down = new Vector3(0, 0, -10);
        StartCoroutine(moveCamera(transform.position, upDown ? up : down));
    }

    IEnumerator moveCamera(Vector3 start, Vector3 end)
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            transform.position = Vector3.Lerp(transform.position, end, lerpy);
            lerpy += Time.deltaTime * 0.1f;
            yield return new WaitForEndOfFrame();
        }
    }
}

[CustomEditor(typeof(CameraController))]
public class tempUIcammover : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraController scriptToControl = (CameraController)target;
        if (GUILayout.Button("move camera up"))
        {
            scriptToControl.switchViews(true);
        }
        if (GUILayout.Button("move camera down"))
        {
            scriptToControl.switchViews(false);
        }
    }
}