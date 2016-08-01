using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography;

//For use interacting with the twitter API
namespace Twitter
{
    public class RequestTokenResponse
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }

    public class AccessTokenResponse
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string UserId { get; set; }
        public string ScreenName { get; set; }
    }

    public delegate void RequestTokenCallback(bool success, RequestTokenResponse response);
    public delegate void AccessTokenCallback(bool success, AccessTokenResponse response);
    public delegate void PostTweetCallback(bool success);

    public class API
    {
        //Username used in the most recent API call
        private static string currentDisplayName;

        #region Twitter API Methods

        //Authorization set-up
        //Adapted from the following source:
        //SOURCE:
        //http://www.conlanrios.com/2013/10/twitter-application-only-authentication.html

        //PUBLIC API AUTH
        public static string GetTwitterAccessToken(string consumerKey, string consumerSecret)
        {
            string URL_ENCODED_KEY_AND_SECRET = Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey + ":" + consumerSecret));

            byte[] body;
            body = Encoding.UTF8.GetBytes("grant_type=client_credentials");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Basic " + URL_ENCODED_KEY_AND_SECRET;

            WWW web = new WWW("https://api.twitter.com/oauth2/token", body, headers);
            while (!web.isDone)
            {
                Debug.Log("Retrieving access token...");
            }
            Debug.Log("Access token retrieved");
            //Format string response
            string output = web.text.Replace("{\"token_type\":\"bearer\",\"access_token\":\"", "");
            output = output.Replace("\"}", "");

            return output;
        }

        public static IEnumerator GetTwitterAccessToken(string consumerKey, string consumerSecret,TwitterPlane callback)
        {
            string URL_ENCODED_KEY_AND_SECRET = Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey + ":" + consumerSecret));

            byte[] body;
            body = Encoding.UTF8.GetBytes("grant_type=client_credentials");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Basic " + URL_ENCODED_KEY_AND_SECRET;

            WWW web = new WWW("https://api.twitter.com/oauth2/token", body, headers);
            yield return web;

            string output = web.text.Replace("{\"token_type\":\"bearer\",\"access_token\":\"", "");
            output = output.Replace("\"}", "");

            callback.AcessToken = output;
        }

        #region OAuth Token Methods
        // 1. Get Request-Token From Twitter
        // 2. Get PIN from User
        // 3. Get Access-Token from Twitter
        // 4. Use Accss-Token for APIs requriring OAuth 
        // Accss-Token will be always valid until the user revokes the access to your application.

        // Twitter APIs for OAuth process
        private static readonly string RequestTokenURL = "https://api.twitter.com/oauth/request_token";
        private static readonly string AuthorizationURL = "https://api.twitter.com/oauth/authenticate?oauth_token={0}";
        private static readonly string AccessTokenURL = "https://api.twitter.com/oauth/access_token";

        public static RequestTokenResponse GetRequestToken(string consumerKey, string consumerSecret)
        {
            WWW web = WWWRequestToken(consumerKey, consumerSecret);

            while (!web.isDone)
            {
                Debug.Log("Grabbing request token...");
            }

            if (!string.IsNullOrEmpty(web.error))
            {
                Debug.Log(string.Format("GetRequestToken - failed. error : {0}", web.error));
                return null;
            }
            else
            {
                RequestTokenResponse response = new RequestTokenResponse
                {
                    Token = Regex.Match(web.text, @"oauth_token=([^&]+)").Groups[1].Value,
                    TokenSecret = Regex.Match(web.text, @"oauth_token_secret=([^&]+)").Groups[1].Value,
                };

                if (!string.IsNullOrEmpty(response.Token) &&
                    !string.IsNullOrEmpty(response.TokenSecret))
                {
                    Debug.Log("Request token created, opening authorization page");
                    Twitter.API.OpenAuthorizationPage(response.Token);
                    //caller.m_RequestTokenResponse = response;
                    return response;
                }
                else
                {
                    Debug.Log(string.Format("GetRequestToken - failed. response : {0}", web.text));
                    return null;
                }
            }
        }

        public static void OpenAuthorizationPage(string requestToken)
        {
            Application.OpenURL(string.Format(AuthorizationURL, requestToken));
        }

        public static IEnumerator GetAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin, AccessTokenCallback callback)
        {
            WWW web = WWWAccessToken(consumerKey, consumerSecret, requestToken, pin);

            yield return web;

            if (!string.IsNullOrEmpty(web.error))
            {
                Debug.Log(string.Format("GetAccessToken - failed. error : {0}", web.error));
            }
            else
            {
                AccessTokenResponse response = new AccessTokenResponse
                {
                    Token = Regex.Match(web.text, @"oauth_token=([^&]+)").Groups[1].Value,
                    TokenSecret = Regex.Match(web.text, @"oauth_token_secret=([^&]+)").Groups[1].Value,
                    UserId = Regex.Match(web.text, @"user_id=([^&]+)").Groups[1].Value,
                    ScreenName = Regex.Match(web.text, @"screen_name=([^&]+)").Groups[1].Value
                };

                if (!string.IsNullOrEmpty(response.Token) &&
                    !string.IsNullOrEmpty(response.TokenSecret) &&
                    !string.IsNullOrEmpty(response.UserId) &&
                    !string.IsNullOrEmpty(response.ScreenName))
                {
                    callback(true, response);
                }
                else
                {
                    Debug.Log(string.Format("GetAccessToken - failed. response : {0}", web.text));
                }
            }
        }

        private static WWW WWWRequestToken(string consumerKey, string consumerSecret)
        {
            // Add data to the form to post.
            WWWForm form = new WWWForm();
            form.AddField("oauth_callback", "oob");

            // HTTP header
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);
            parameters.Add("oauth_callback", "oob");

            var headers = new Dictionary<string, string>();
            headers["Authorization"] = GetFinalOAuthHeader("POST", RequestTokenURL, parameters);

            return new WWW(RequestTokenURL, form.data, headers);
        }

        private static WWW WWWAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin)
        {
            // Need to fill body since Unity doesn't like an empty request body.
            byte[] dummmy = new byte[1];
            dummmy[0] = 0;

            // HTTP header
            var headers = new Dictionary<string, string>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);
            parameters.Add("oauth_token", requestToken);
            parameters.Add("oauth_verifier", pin);

            headers["Authorization"] = GetFinalOAuthHeader("POST", AccessTokenURL, parameters);

            return new WWW(AccessTokenURL, dummmy, headers);
        }

        private static string GetHeaderWithAccessToken(string httpRequestType, string apiURL, string consumerKey, string consumerSecret, AccessTokenResponse response, Dictionary<string, string> parameters)
        {
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);

            parameters.Add("oauth_token", response.Token);
            parameters.Add("oauth_token_secret", response.TokenSecret);

            return GetFinalOAuthHeader(httpRequestType, apiURL, parameters);
        }

        #endregion

        //Data types used for tweet data as well as detailed time info
        #region screenshotDoing

        public static IEnumerator PostScreenshot(string encodedImage, string consumerKey, string consumerSecret, AccessTokenResponse response)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("media_data", encodedImage);

            // Add data to the form to post.
            WWWForm form = new WWWForm();
            form.AddField("media_data", encodedImage);

            Debug.Log("uploading to twitter...");
            // HTTP header
            var headers = new Dictionary<string, string>();
            headers["Authorization"] = GetHeaderWithAccessToken("POST", "https://upload.twitter.com/1.1/media/upload.json", consumerKey, consumerSecret, response, parameters);
            headers["Content-Transfer-Encoding"] = "base64";
            WWW web = new WWW("https://upload.twitter.com/1.1/media/upload.json", form.data, headers);
            yield return web;

            if (web.error != "Null")
            {
                string mediaID = web.text.Remove(web.text.IndexOf(','), web.text.Length - web.text.IndexOf(','));
                mediaID = mediaID.Remove(0, 12);
                Debug.Log("Upload complete - " + mediaID);
                //secondaryCaller.mediaIDs.Add(new KeyValuePair<string, bool>(mediaID, isBack));
            }
            else
            {
                Debug.Log(web.error);
            }
        }

        public static IEnumerator PostTweet(string text, string mediaID, string consumerKey, string consumerSecret, AccessTokenResponse response)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("status", text);
            parameters.Add("media_ids", mediaID);

            // Add data to the form to post.
            WWWForm form = new WWWForm();
            form.AddField("status", text);
            form.AddField("media_ids", mediaID);

            Debug.Log(mediaID);
            Debug.Log("Posting to Twitter...");
            // HTTP header
            var headers = new Dictionary<string, string>();
            headers["Authorization"] = GetHeaderWithAccessToken("POST", "https://api.twitter.com/1.1/statuses/update.json", consumerKey, consumerSecret, response, parameters);
            WWW web = new WWW("https://api.twitter.com/1.1/statuses/update.json", form.data, headers);
            yield return web;

            Debug.Log(web.text);
            if (web.error != "Null")
            {
                Debug.Log("Upload complete!");
            }
            else
            {
                Debug.Log(web.error);
            }
        }
        #endregion

        [System.Serializable]
        public class tw_DateTime
        {
            public string Weekday;
            public string Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;
            public string Offset;
            public int Year;
        }

        [System.Serializable]
        public class SearchResults
        {
            public Tweet[] statuses;
        }
        public class MetaData
        {
            public string iso_language_code;
            public string result_type;
        }

        [System.Serializable]
        public class Tweet
        {
            public string TweetName;
            public string created_at;
            //public string id;
            //public string id_str;
            public string text;
            //public bool truncated;
            //public MetaData metadata;
            //public string source;
            //public string in_reply_to_status_id;
            //public string in_reply_to_status_id_str;
            //public string in_reply_to_user_id;
            //public string in_reply_to_user_id_str;
            //public string in_reply_to_screen_name;
            public TwitterUserInfo user;
            //public string geo;
            //public string coordinates;
            //public string place;
            //public bool retweeted;
            //public bool favourited;
            //public string contributors;
            //public Tweet retweeted_status;
            //public tw_DateTime FormattedDateTime;
        }

        [System.Serializable]
        public class TwitterUserInfo
        {
            public string screen_name;
            public string id;
            public string name;
            //public string profile_location;
            //public string description;
            //public string url;
            //public int followers_count;
            //public int friends_count;
            //public string created_at;
            //public int favourites_count;
            //public string utc_offset;
            //public string time_zone;
            //public bool geo_enabled;
            //public bool verified;
            //public int statuses_count;
            //public string lang;
            ////public Tweet status;
            //public string profile_image_url;
        }

        //Grab a selection of tweets from a user
        public static List<Tweet> GetUserTimeline(string name, string AccessToken, int count)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            //DO AN API CALL
            //For this program, the parameters used below will likely need to be changed bar username and # of tweets to pull
            WWW web = new WWW("https://api.twitter.com/1.1/statuses/" + "user" + "_timeline.json?screen_name=" + name + "&count=" + count + "&trim_user=1" + "&include_rts=0&exclude_replies=true&contributor_details=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }

            //We have an error x(
            if (web.error != null)
            {
                //Output error
                Debug.Log(web.error);
                return null;
            }
            else
            {
                //Remove this bit 'cause it's more trouble than it's worth.
                List<string> mentions = extractData(web.text, ",\"entities\":", ",\"source\":");
                //If detected, remove.
                string extractMe;
                if (ammendOutputText == null)
                    extractMe = web.text;
                else
                    extractMe = ammendOutputText;

                string[] delim = { "},{" };
                string[] pls = extractMe.Split(delim, StringSplitOptions.None);

                List<Tweet> output = new List<Tweet>();

                foreach (string s in pls)
                {
                    //Little bit of formatting, make sure the JSON utility can do it's job properly
                    string temp = s;

                    if (temp.StartsWith("["))
                        temp = temp.Remove(0, 1);

                    if (temp.EndsWith("]") || temp.EndsWith("}}"))
                        temp = temp.Remove(temp.Length - 1, 1);

                    if (!temp.EndsWith("}"))
                        temp += "}";

                    if (!temp.StartsWith("{"))
                        temp = "{" + temp;

                    Tweet newTweet = JsonUtility.FromJson<Tweet>(temp);
                    //newTweet.FormattedDateTime = formatDateTime(newTweet.created_at);
                    //newTweet.screen_name = currentDisplayName;
                    output.Add(newTweet);
                }
                return output;
            }
        }

        public static tw_DateTime formatDateTime(string dateTime)
        {
            string temp = "";
            List<string> date = new List<string>();
            for (int k = 0; k < dateTime.Length; k++)
            {
                if (dateTime[k] != ' ')
                    temp += dateTime[k];
                else
                {
                    date.Add(temp);
                    temp = "";
                }

                if (k == dateTime.Length - 1)
                    date.Add(temp);
            }
            temp = "";
            List<string> timeOfDay = new List<string>();
            for (int k = 0; k < date[3].Length; k++)
            {
                if (date[3][k] != ':')
                    temp += date[3][k];
                else
                {
                    timeOfDay.Add(temp);
                    temp = "";
                }

                if (k == date[3].Length - 1)
                    timeOfDay.Add(temp);
            }

            Twitter.API.tw_DateTime time = new Twitter.API.tw_DateTime();
            time.Weekday = date[0];
            time.Month = date[1];
            time.Day = int.Parse(date[2]);
            time.Hour = int.Parse(timeOfDay[0]);
            time.Minute = int.Parse(timeOfDay[1]);
            time.Second = int.Parse(timeOfDay[2]);
            time.Year = int.Parse(date[5]);
            time.Offset = date[4];

            return time;
        }

        //Retrieve user profile information
        public static TwitterUserInfo GetProfile(string name, string AccessToken, bool isID)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            //DO AN API CALL
            //For this program, the parameters used below will likely need to be changed bar username and # of tweets to pull
            if (!isID)
            {
                WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?screen_name=" + name + "&include_entities=false", null, headers);
                while (!web.isDone)
                {
                    Debug.Log("Processing request...");
                }

                //We have an error x(
                if (web.error != null)
                {
                    //Output error
                    Debug.Log(web.error);
                    return null;
                }
                else
                {
                    Debug.Log("Profile retrieved");
                    TwitterUserInfo output = JsonUtility.FromJson<TwitterUserInfo>(web.text);
                    if (output.screen_name == " ")
                        output.screen_name = "Somebody with a non-ascii name";

                    //output.profile_image_url = output.profile_image_url.Remove(output.profile_image_url.IndexOf("_normal"), 7);
                    currentDisplayName = output.screen_name;

                    return output;
                }
            }
            else
            {
                WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?user_id=" + name + "&include_entities=false", null, headers);
                while (!web.isDone)
                {
                    Debug.Log("Processing request...");
                }

                //We have an error x(
                if (web.error != null)
                {
                    //Output error
                    Debug.Log(web.error);
                    return null;
                }
                else
                {
                    TwitterUserInfo output = JsonUtility.FromJson<TwitterUserInfo>(web.text);
                    Debug.Log(output.screen_name + " profile retrieved");

                    return output;
                }
            }

        }

        public static IEnumerator GetProfileCorotine(string name, string AccessToken, bool isID)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            //DO AN API CALL
            //For this program, the parameters used below will likely need to be changed bar username and # of tweets to pull
            if (!isID)
            {
                WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?screen_name=" + name + "&include_entities=false", null, headers);
                yield return web;

                //We have an error x(
                if (web.error != null)
                {
                    //Output error
                    Debug.Log(web.error);
                }
                else
                {
                    Debug.Log("Profile retrieved");
                    TwitterUserInfo output = JsonUtility.FromJson<TwitterUserInfo>(web.text);
                    if (output.screen_name == " ")
                        output.screen_name = "Somebody with a non-ascii name";

                    //output.profile_image_url = output.profile_image_url.Remove(output.profile_image_url.IndexOf("_normal"), 7);
                    currentDisplayName = output.screen_name;
                    //callback.user = output;
                }
            }
            else
            {
                WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?user_id=" + name + "&include_entities=false", null, headers);

                yield return web;

                //We have an error x(
                if (web.error != null)
                {
                    //Output error
                    Debug.Log(web.error);
                }
                else
                {
                    TwitterUserInfo output = JsonUtility.FromJson<TwitterUserInfo>(web.text);
                    //callback.StartCoroutine(callback.userProfileCallback(output));
                }
            }
        }

        public static IEnumerator getIDs(string screen_name, string AccessToken)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/followers/ids.json?screen_name=" + screen_name, null, headers);//+"&count=900"

            yield return web;

            string newText = web.text.Remove(0, 8);
            newText = newText.Remove(newText.IndexOf(']', newText.Length - newText.IndexOf(']')));
            string[] ids = newText.Split(',');
            //callback.StartCoroutine(callback.followersCallback(ids));
        }
        
        public static string[] GetFollowerIDs(string screen_name, string AcessToken)
        {
            float startTime = DateTime.Now.Millisecond + (DateTime.Now.Second * 1000) + (DateTime.Now.Minute * 1000 * 60);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AcessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/followers/ids.json?screen_name=" + screen_name, null, headers);

            while (!web.isDone)
            {
                Debug.Log("Retrieving follower IDs...");
            }

            string newText = web.text.Remove(0, 8);
            newText = newText.Remove(newText.IndexOf(']', newText.Length - newText.IndexOf(']')));
            string[] ids = newText.Split(',');
            float timeElapsed = (((DateTime.Now.Millisecond + (DateTime.Now.Second * 1000) + (DateTime.Now.Minute * 1000 * 60)) - startTime) / 1000);

            Debug.Log(ids.Length + " IDs collected from " + screen_name + "'s followers");
            Debug.Log("Time elapsed: " + timeElapsed);
            return ids;
        }

        public static IEnumerator SearchForTweets(string query, string AcessToken, int count, TwitterPlane callback)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AcessToken;
            WWW web = new WWW("https://api.twitter.com/1.1/search/tweets.json?" + "q=" + query + "&result_type=recent&count=" + count + "&include_entities=false", null, headers);

            yield return web;

            if (web.error != null)
            {
                Debug.Log(web.error);
                callback.gettingTweets = false;
            }
            else
            {
                SearchResults newResults = JsonUtility.FromJson<SearchResults>(web.text);
                Debug.Log(web.text);
                int i = callback.availableTweets.Count;
                foreach (Tweet t in newResults.statuses)
                {
                    char[] c = new char[1] { '\n' };
                    string[] s = t.text.Split(c);
                    i++;
                    t.TweetName = i + ". " + query;
                    //Add tweet to a list of available tweets
                    if (s.Length < 3)
                        callback.availableTweets.Add(t);
                    else
                    {
                        //FUCK OFF, I DON'T WANT YOUR SHIT TWEET WITH LOTS OF LINEBREAKS
                    }
                }
                callback.gettingTweets = false;
            }
        }

        #endregion

        //Used for extracting data from API response
        public static List<string> extractData(string outputText, string start, string end)
        {
            #region Find all the position of all mentions of start string
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            int i = 0;
            while ((i = outputText.IndexOf(start, i)) != -1)
            {
                startPos.Add(i);
                i++;
            }
            #endregion

            #region Do the same for end string
            i = 0;
            while ((i = outputText.IndexOf(end, i)) != -1)
            {
                stopPos.Add(i);
                i++;
            }
            #endregion

            //Data to return
            List<string> returnMe = new List<string>();

            //If we have a different number of start and end points, something has gone wrong
            if (startPos.Count != stopPos.Count)
                startPos.Remove(startPos[startPos.Count - 1]);//Try fixing


            for (int j = startPos.Count - 1; j > -1; j--)
            {
                string output = "";
                for (int c = startPos[j]; c < stopPos[j]; c++)
                {
                    output += outputText[c];
                }

                if (start != ",\"entities\":")
                {
                    #region Format output string
                    output = output.Replace(start, "");
                    //Remove emoji type things
                    output = output.Replace("\ud83c[\udf00-\udfff]", " ! ");
                    output = output.Replace("\\\"", "\"");
                    output = output.Replace("\\/", "/");
                    output = output.Replace("&amp;", "&");

                    //Attempt at emoji removal
                    List<int> EmojisOrSimilar = new List<int>();
                    i = 0;
                    while ((i = output.IndexOf("\\u", i)) != -1)
                    {
                        EmojisOrSimilar.Add(i);
                        i++;
                    }

                    for (int u = EmojisOrSimilar.Count - 1; u > -1; u--)
                    {
                        //Emoji text is typically 6 characters long
                        output = output.Remove(EmojisOrSimilar[u], 6);
                        output.Insert(EmojisOrSimilar[u], "*!*");
                    }
                    #endregion
                }
                else if (output != "[]")
                {
                    //Remove text from original input and return
                    //Remove each section of the string STARTING AT THE END AND WORKING BACK
                    outputText = outputText.Remove(startPos[j] + 1, output.Length);// + start.Length - 1);
                    output = null;
                    ammendOutputText = outputText;
                }

                returnMe.Add(output);
            }
            return returnMe;
        }

        //Used with above function
        public static string ammendOutputText = null;

        #region OAuth Help Methods
        // The below help methods are modified from "WebRequestBuilder.cs" in Twitterizer(http://www.twitterizer.net/).
        // Here is its license.

        //-----------------------------------------------------------------------
        // <copyright file="WebRequestBuilder.cs" company="Patrick 'Ricky' Smith">
        //  This file is part of the Twitterizer library (http://www.twitterizer.net/)
        // 
        //  Copyright (c) 2010, Patrick "Ricky" Smith (ricky@digitally-born.com)
        //  All rights reserved.
        //  
        //  Redistribution and use in source and binary forms, with or without modification, are 
        //  permitted provided that the following conditions are met:
        // 
        //  - Redistributions of source code must retain the above copyright notice, this list 
        //    of conditions and the following disclaimer.
        //  - Redistributions in binary form must reproduce the above copyright notice, this list 
        //    of conditions and the following disclaimer in the documentation and/or other 
        //    materials provided with the distribution.
        //  - Neither the name of the Twitterizer nor the names of its contributors may be 
        //    used to endorse or promote products derived from this software without specific 
        //    prior written permission.
        // 
        //  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
        //  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
        //  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
        //  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
        //  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
        //  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
        //  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
        //  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
        //  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
        //  POSSIBILITY OF SUCH DAMAGE.
        // </copyright>
        // <author>Ricky Smith</author>
        // <summary>Provides the means of preparing and executing Anonymous and OAuth signed web requests.</summary>
        //-----------------------------------------------------------------------

        private static readonly string[] OAuthParametersToIncludeInHeader = new[]
                                                          {
                                                              "oauth_version",
                                                              "oauth_nonce",
                                                              "oauth_timestamp",
                                                              "oauth_signature_method",
                                                              "oauth_consumer_key",
                                                              "oauth_token",
                                                              "oauth_verifier"
                                                              // Leave signature omitted from the list, it is added manually
                                                              // "oauth_signature",
                                                          };

        private static readonly string[] SecretParameters = new[]
                                                                {
                                                                    "oauth_consumer_secret",
                                                                    "oauth_token_secret",
                                                                    "oauth_signature"
                                                                };

        private static void AddDefaultOAuthParams(Dictionary<string, string> parameters, string consumerKey, string consumerSecret)
        {
            parameters.Add("oauth_version", "1.0");
            parameters.Add("oauth_nonce", GenerateNonce());
            parameters.Add("oauth_timestamp", GenerateTimeStamp());
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_consumer_secret", consumerSecret);
        }

        private static string GetFinalOAuthHeader(string HTTPRequestType, string URL, Dictionary<string, string> parameters)
        {
            // Add the signature to the oauth parameters
            string signature = GenerateSignature(HTTPRequestType, URL, parameters);

            parameters.Add("oauth_signature", signature);

            StringBuilder authHeaderBuilder = new StringBuilder();
            authHeaderBuilder.AppendFormat("OAuth realm=\"{0}\"", "Twitter API");

            var sortedParameters = from p in parameters
                                   where OAuthParametersToIncludeInHeader.Contains(p.Key)
                                   orderby p.Key, UrlEncode(p.Value)
                                   select p;

            foreach (var item in sortedParameters)
            {
                authHeaderBuilder.AppendFormat(",{0}=\"{1}\"", UrlEncode(item.Key), UrlEncode(item.Value));
            }

            authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));

            return authHeaderBuilder.ToString();
        }

        private static string GenerateSignature(string httpMethod, string url, Dictionary<string, string> parameters)
        {
            var nonSecretParameters = (from p in parameters
                                       where !SecretParameters.Contains(p.Key)
                                       select p);

            // Create the base string. This is the string that will be hashed for the signature.
            string signatureBaseString = string.Format(CultureInfo.InvariantCulture,
                                                       "{0}&{1}&{2}",
                                                       httpMethod,
                                                       UrlEncode(NormalizeUrl(new Uri(url))),
                                                       UrlEncode(nonSecretParameters));

            // Create our hash key (you might say this is a password)
            string key = string.Format(CultureInfo.InvariantCulture,
                                       "{0}&{1}",
                                       UrlEncode(parameters["oauth_consumer_secret"]),
                                       parameters.ContainsKey("oauth_token_secret") ? UrlEncode(parameters["oauth_token_secret"]) : string.Empty);


            // Generate the hash
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(key));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
            return Convert.ToBase64String(signatureBytes);
        }

        private static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
        }

        private static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return new System.Random().Next(123400, int.MaxValue).ToString("X", CultureInfo.InvariantCulture);
        }

        private static string NormalizeUrl(Uri url)
        {
            string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }

            normalizedUrl += url.AbsolutePath;
            return normalizedUrl;
        }

        private static string EscapeString(string originalString)
        {
            int limit = 2000;

            StringBuilder sb = new StringBuilder();
            int loops = originalString.Length / limit;

            for (int i = 0; i <= loops; i++)
            {
                if (i < loops)
                {
                    sb.Append(Uri.EscapeDataString(originalString.Substring(limit * i, limit)));
                }
                else
                {
                    sb.Append(Uri.EscapeDataString(originalString.Substring(limit * i)));
                }
            }
            return sb.ToString();
        }

        private static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = EscapeString(value);
            //value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }

        private static string UrlEncode(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder parameterString = new StringBuilder();

            var paramsSorted = from p in parameters
                               orderby p.Key, p.Value
                               select p;

            foreach (var item in paramsSorted)
            {
                if (parameterString.Length > 0)
                {
                    parameterString.Append("&");
                }

                parameterString.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}={1}",
                        UrlEncode(item.Key),
                        UrlEncode(item.Value)));
            }

            return UrlEncode(parameterString.ToString());
        }

        #endregion   
    }
}