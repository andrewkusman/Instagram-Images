using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace instaFuck
{


    class User : Instagram
    {
        public string login;
        public Uri profileImage;
        private string accessTokenUser = acTok;
        private string nextUrl;
        private string prevCount;
        private string countStr;
        public bool isClosed;
        public List<Image> GetImages(int offset, int count)
        {
            List<Image> testList = new List<Image>();
            string loginId = GetId(login, accessTokenUser);
            string requestData;
            countStr = "count=" + count;
            if (offset == 1)
            {
                requestData = "https://api.instagram.com/v1/users/" + loginId + "/media/recent/" +
                              "?access_token=" + accessTokenUser +
                              "&" + countStr;
            }
            else requestData = nextUrl.Replace(prevCount, countStr);
            prevCount = countStr;
            try
            {

                var content = GetHtml(requestData);
                dynamic all = JsonConvert.DeserializeObject(content);
                nextUrl = all.pagination.next_url;
                foreach (var i in all.data)
                {
                    testList.Add(new Image
                    {
                        thumbNail = i.images.thumbnail.url,
                        highResolution = i.images.standard_resolution.url
                    });
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            return testList;
        }

        public User GetUserInformation(User incUser)
        {
            string id = GetId(incUser.login, accessTokenUser);
            if (id == null)
            {
                return null;
            }
            string requestUrl = MakeUlr("userInformation", id, accessTokenUser);
            try
            {
                string content = GetHtml(requestUrl);
                if (content != null)
                {
                    dynamic information = JsonConvert.DeserializeObject(content);
                    User user = new User()
                    {
                        login = information.data.username,
                        profileImage = information.data.profile_picture,
                        isClosed = false
                    };
                    return user;
                }
                else
                {
                    User user = new User()
                    {
                        login = "hui",
                        isClosed = true,
                        profileImage = null
                    };
                    return user;
                }
            }
            catch
            {
                User user = new User()
                {
                    login = "hui",
                    isClosed = true,
                    profileImage = null
                };
                return user;
            }
            //return null;
        }

    }
    class Instagram
    {
        public string accsessToken;
        protected static string acTok;

        public Instagram(string incToken)
        {
            accsessToken = incToken;
            acTok = incToken;
        }

        public Instagram()
        {

        }

        protected string GetHtml(string requestUrl)
        {
            try
            {
                var req =
                    (HttpWebRequest)
                        WebRequest.Create(requestUrl);
                var resp = (HttpWebResponse)req.GetResponse();
                var sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                var content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
            catch (Exception e)
            {
                Console.WriteLine(">> ERROR: " + e);
                return null;
            }
        }

        protected string MakeUlr(string task, string userId, string accessToken)
        {
            string accessTokenUri = "access_token=" + accessToken;
            string finalUri = "https://api.instagram.com/v1/users/";
            switch (task)
            {
                case "userInformation":
                    return finalUri + userId + "/?" + accessTokenUri;
                case "recent":
                    return finalUri + userId + "/media/recent/?" + accessTokenUri;
                case "search":
                    return finalUri + "search?q=" + userId + "&" + accessTokenUri;
            }
            return null;
        }


        protected string GetId(string login, string accessToken)
        {
            string requestUrl = MakeUlr("search", login, accessToken);
            dynamic all = JsonConvert.DeserializeObject(GetHtml(requestUrl));
            if (all.meta.code == 200)
            {
                try
                {
                    return all.data[0].id;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        public List<User> SearchUsers(string login)
        {
            List<User> mainList = new List<User>();
            string request = MakeUlr("search", login, accsessToken);
            string response = GetHtml(request);
            try
            {
                dynamic all = JsonConvert.DeserializeObject(response);
                if (all.meta.code == 200)
                {
                    foreach (var i in all.data)
                    {
                        mainList.Add(new User()
                        {
                            login = i.username,
                            profileImage = i.profile_picture
                        });
                    }
                    return mainList;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class Image
    {
        public Uri thumbNail;
        public Uri highResolution;
    }
}
