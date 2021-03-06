﻿using UnityEngine;
using System.Collections;

namespace GOOST
{
    //Create tiny pauses for certain events
    //i.e. clashing of beaks, defeat of geese etc.
    public class frameHolder : MonoBehaviour
    {

        public static frameHolder instance;

        public void holdFrame(float time)
        {
            StartCoroutine(hold(time));
        }

        IEnumerator hold(float duration)
        {
            Time.timeScale = 0;

            float timer = 0;

            while (timer < duration)
            {
                timer += Time.fixedDeltaTime;
                yield return new WaitForEndOfFrame();
            }
            Time.timeScale = 1;
        }

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}