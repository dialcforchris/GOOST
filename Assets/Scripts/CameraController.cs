using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    Vector3 up = new Vector3(0, 30, -10);
    Vector3 down = new Vector3(0, 0, -10);

    public void Awake()
    {
        instance = this;
    }

    public void switchViews(bool upDown)
    {
        StopAllCoroutines();
        StartCoroutine(moveCamera(transform.position, upDown ? up : down));
    }

    IEnumerator moveCamera(Vector3 start, Vector3 end)
    {
        float lerpy = 0;
        while (lerpy < 0.15f)
        {
            transform.position = Vector3.Lerp(transform.position, end, lerpy);
            lerpy += Time.deltaTime * 0.1f;
            yield return new WaitForEndOfFrame();
        }
        if (end == up)
            GameStateManager.instance.ChangeState(GameStates.STATE_MENU);
        Actor.worldMaxY = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1.01f));
    }
}