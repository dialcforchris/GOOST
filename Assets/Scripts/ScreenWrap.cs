﻿using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class ScreenWrap : MonoBehaviour
    {
        public delegate void ScreenWrapped(bool _wrap);
        private ScreenWrapped screenWrapped = null;

        private Vector2 screenToWorldMax;
        private Vector2 screenToWorldMin;
        [SerializeField]
        Actor ActorComponent;
        [SerializeField]
        private float wrapMin = -0.01f;
        [SerializeField]
        private float wrapMax = 1.01f;

        private void Start()
        {
            screenToWorldMax = Camera.main.ViewportToWorldPoint(new Vector2(wrapMax, wrapMax));
            screenToWorldMin = Camera.main.ViewportToWorldPoint(new Vector2(wrapMin, wrapMin));
        }

        private void Update()
        {
            if (transform.position.x > screenToWorldMax.x)
            {
                transform.position = new Vector2(screenToWorldMin.x, transform.position.y);
                Wrapped();
            }
            else if (transform.position.x < screenToWorldMin.x)
            {
                transform.position = new Vector2(screenToWorldMax.x, transform.position.y);
                Wrapped();
            }
        }

        private void Wrapped()
        {
            if (ActorComponent)
            {
                //Pure scum
                transform.position -= Vector3.down * 0.01f;
            }

            if (screenWrapped != null)
            {
                screenWrapped(true);
            }
        }

        public void AddScreenWrapCall(ScreenWrapped _wrap)
        {
            screenWrapped += _wrap;
        }
    }
}