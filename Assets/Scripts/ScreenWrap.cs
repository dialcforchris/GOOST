using UnityEngine;
using System.Collections;

    public class ScreenWrap : MonoBehaviour
    {

       
        Vector2 position;
        Vector2 screenToWorldMax;
        Vector2 screenToWorldMin;

    [SerializeField] private float wrapMin = -0.01f;
    [SerializeField] private float wrapMax = 1.01f;
        // Use this for initialization
        void Start()
        {
           
            screenToWorldMax = Camera.main.ViewportToWorldPoint(new Vector2(1.01f, 1.01f));
            screenToWorldMin = Camera.main.ViewportToWorldPoint(new Vector2(-0.01f, -0.01f));
        }

        // Update is called once per frame
        void Update()
        {
            position = gameObject.transform.position;

            if (position.x > screenToWorldMax.x)
            {
               transform.position = new Vector2(screenToWorldMin.x, position.y);
            }
            if (position.x < screenToWorldMin.x)
            {
               transform.position = new Vector2(screenToWorldMax.x, position.y);
            }
        }

        void Wrap(Vector2 newPos)
        {
            gameObject.transform.position = newPos;
        }
}