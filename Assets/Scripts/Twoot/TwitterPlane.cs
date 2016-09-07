using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GOOST
{
    public class TwitterPlane : MonoBehaviour
    {
        public List<Twitter.API.Tweet> availableTweets = new List<Twitter.API.Tweet>();

        [SerializeField]
        float moveSpeed = .05f;
        //[SerializeField]
        //TextMesh bannerText;
        [SerializeField]
        Text bannerText;
        bool currentDirection;
        public string consumerKey, consumerSecret, AcessToken;

        Twitter.RequestTokenResponse m_RequestTokenResponse;
        Twitter.AccessTokenResponse m_AccessTokenResponse;

        void Awake()
        {
            //LoadTwitterUserInfo();
            StartCoroutine(Twitter.API.GetTwitterAccessToken(consumerKey, consumerSecret, this));
        }

        public void GetTweets()
        {
            switch (Random.Range(0, 6))
            {
                case 0:
                    StartCoroutine(Twitter.API.SearchForTweets("%23RansomWare", AcessToken, 2, this));
                    break;
                case 1:
                    StartCoroutine(Twitter.API.SearchForTweets("%23cybersecurity", AcessToken, 2, this));
                    break;
                case 2:
                    StartCoroutine(Twitter.API.SearchForTweets("%23Infosec", AcessToken, 2, this));
                    break;
                case 3:
                    StartCoroutine(Twitter.API.SearchForTweets("%23Malware", AcessToken, 2, this));
                    break;
                case 4:
                    StartCoroutine(Twitter.API.SearchForTweets("%23Hacking", AcessToken, 2, this));
                    break;
                case 5:
                    StartCoroutine(Twitter.API.SearchForTweets("%23Security", AcessToken, 10, this));
                    break;
                case 6:
                    StartCoroutine(Twitter.API.SearchForTweets("%23Hack", AcessToken, 2, this));
                    break;
                case 7:
                    StartCoroutine(Twitter.API.SearchForTweets("%23PlayGoose", AcessToken, 2, this));
                    break;
                case 8:
                    StartCoroutine(Twitter.API.SearchForTweets("%23CyberCrime", AcessToken, 2, this));
                    break;
            }
        }

        public void fly(bool dir)
        {
            Vector2 startPos;
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
                startPos = Camera.main.ViewportToWorldPoint(new Vector2((dir) ? 1.5f : -0.5f, Random.Range(.7f, .85f)));
            else
                startPos = Camera.main.ViewportToWorldPoint(new Vector2((dir) ? 1.5f : -0.5f, Random.Range(.6f, .7f)));

            Vector2 endPos = Camera.main.ViewportToWorldPoint(new Vector2((!dir) ? 1.5f : -0.5f, startPos.y));
            currentDirection = dir;
            transform.localScale = dir ? new Vector3(-1, 1, 1) : Vector3.one;
            transform.position = startPos;

            //Text Mesh things
            //bannerText.alignment = dir ? TextAlignment.Left : TextAlignment.Right;
            //bannerText.anchor = dir ? TextAnchor.UpperLeft : TextAnchor.UpperRight;

            bannerText.alignment = dir ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;

            bannerText.transform.localScale = dir ? new Vector3(-0.01f, 0.01f, 0.01f) : new Vector3(0.01f, 0.01f, 0.01f);
            bannerText.rectTransform.pivot = !dir ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f);

            int rand = Random.Range(0, availableTweets.Count);
            moveSpeed = 0.03f + ((1 - (availableTweets[rand].text.Length / 140f)) * 0.03f);

            string text = availableTweets[rand].text;

            string[] words;
            words = text.Split(" "[0]); //Split the string into separate words

            for (int index = 0; index < words.Length; index++)
            {
                var word = words[index].Trim();

                if (word.StartsWith("http:/") || word.StartsWith("https:/"))
                    word = "[LINK]";

                if (index == 0)
                {
                    text = words[0];
                }

                if (index > 0)
                {
                    text += " " + word;
                }
            }

            bannerText.text = text.Replace("&amp", "&") + " <color=blue>@" + availableTweets[rand].user.screen_name + "</color>";
            availableTweets.Remove(availableTweets[rand]);
        }

        public bool gettingTweets;
        [SerializeField]
        float flybyCooldown = 3;

        //Only play in city and menu
        //Don't display any tweets with mad line breaks

        void Update()
        {
            if (GameStateManager.instance.GetState() == GameStates.STATE_MENU || GameStateManager.instance.GetState() == GameStates.STATE_READYUP || (MainMenu.instance.getLevel() == 2 && GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY))
            {
                if (availableTweets.Count > 0)
                {
                    if (flybyCooldown > 0)
                        flybyCooldown -= Time.deltaTime;

                    if (flybyCooldown <= 0)
                    {
                        fly(!currentDirection);
                        //Each flight takes ~ 10 seconds
                        flybyCooldown = Random.Range(25, 30);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!string.IsNullOrEmpty(AcessToken) && availableTweets.Count - 1 < 1 && !gettingTweets)
            {
                gettingTweets = true;
                GetTweets();
            }

            if (!currentDirection)
            {
                transform.Translate(new Vector3(moveSpeed, 0, 0));
                if (transform.position.x > Camera.main.ViewportToWorldPoint(new Vector3(1.5f, 0, 0)).x)
                {
                    moveSpeed = 0;
                }
            }
            else
            {
                transform.Translate(new Vector3(-moveSpeed, 0, 0));

                if (transform.position.x < Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 0)).x)
                {
                    moveSpeed = 0;
                }
            }
        }

        #region totally useless shite 

        const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";
        const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";
        const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";
        const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";



        public void OpenAuthPage()
        {
            Twitter.API.OpenAuthorizationPage("");
        }

        public void registerUser()
        {
            m_RequestTokenResponse = Twitter.API.GetRequestToken(consumerKey, consumerSecret);
        }
        public Texture2D downloadAvatarToTexture(string url)
        {
            WWW web = new WWW(url);
            while (!web.isDone)
            {
                Debug.Log("Downloading image...");
            }
            if (web.error != null)
                Debug.Log(web.error);
            else
                Debug.Log("Avatar downloaded");

            return web.texture;
        }

        void LoadTwitterUserInfo()
        {
            m_AccessTokenResponse = new Twitter.AccessTokenResponse();

            m_AccessTokenResponse.UserId = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
            m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
            m_AccessTokenResponse.Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
            m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

            if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
                !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
                !string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
                !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
            {
                //currentUserScreenName = m_AccessTokenResponse.ScreenName;
                Debug.Log("Authorized as " + m_AccessTokenResponse.ScreenName + ". Ready to post.");

                string log = "LoadTwitterUserInfo - succeeded";
                log += "\n    UserId : " + m_AccessTokenResponse.UserId;
                log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
                log += "\n    Token : " + m_AccessTokenResponse.Token;
                log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
                print(log);
            }
            else
            {
                Debug.Log("Unauthorized, PIN required to post.");
            }
        }

        #endregion

        void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
        {
            if (success)
            {
                string log = "OnAccessTokenCallback - succeeded";
                log += "\n    UserId : " + response.UserId;
                log += "\n    ScreenName : " + response.ScreenName;
                log += "\n    Token : " + response.Token;
                log += "\n    TokenSecret : " + response.TokenSecret;
                Debug.Log(log);

                m_AccessTokenResponse = response;
                //currentUserScreenName = response.ScreenName;

                Debug.Log("Authorized as " + response.ScreenName + ". Ready to post.");

                PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
                PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
                PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
                PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
            }
            else
            {
                Debug.Log("OnAccessTokenCallback - failed.");
            }
        }
    }
}