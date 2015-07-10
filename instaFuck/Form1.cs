using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace instaFuck
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string token = "";
        string step1url = "https://instagram.com/oauth/authorize/?client_id=c08b5e9ea6204a0fa64569af46e20ab5&redirect_uri=http://localhost&response_type=token";
        string step2url = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3Dc08b5e9ea6204a0fa64569af46e20ab5%26redirect_uri%3Dhttp%3A//localhost%26response_type%3Dtoken";
        string step3url = "https://instagram.com/accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3Dc08b5e9ea6204a0fa64569af46e20ab5%26redirect_uri%3Dhttp%3A//localhost%26response_type%3Dtoken";
        string step4url = "https://instagram.com/oauth/authorize/?client_id=c08b5e9ea6204a0fa64569af46e20ab5&redirect_uri=http://localhost&response_type=token";

        string csrftoken, mid, sessionid;

        string login = "", password = "";

        void resetForm()
        {
            progressBar1.Value = 0;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            browserLocation = webBrowser1.Location;
        }

        Point browserLocation;

        void showBrowser()
        {
            for(int i = 765; i>=0;i-=15)
            {
                Thread.Sleep(5);

                Invoke(new Action(() =>
                {
                    browserLocation.X = i;
                }));
                Invoke(new Action(() =>
                {
                    webBrowser1.Location = browserLocation;
                }));
            }
        }

        void hideBrowser()
        {
            for (int i = 0; i <= 765; i+=15)
            {
                Thread.Sleep(1);

                Invoke(new Action(() =>
                {
                    browserLocation.X = i;
                })); 
                Invoke(new Action(() =>
                {
                    webBrowser1.Location = browserLocation;
                })); 
            }
        }

        void make1step()
        {
            Uri uri = new Uri(step1url);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            webRequest.AllowAutoRedirect = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            webRequest.Host = "instagram.com";
            webRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:38.0) Gecko/20100101 Firefox/38.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            WebResponse webResponse = webRequest.GetResponse();
            string cookiedata = webResponse.Headers.Get("Set-Cookie");
            csrftoken = getParameter(cookiedata, "csrftoken=", ";");
            mid = getParameter(cookiedata, "mid=", ";");
            webResponse.Close();
        }

        void make2step()
        {
            Uri uri = new Uri(step2url);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            webRequest.AllowAutoRedirect = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            webRequest.Host = "instagram.com";
            webRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:38.0) Gecko/20100101 Firefox/38.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.Headers.Add(HttpRequestHeader.Cookie, "csrftoken=" + csrftoken + "; mid=" + mid + ";");
            WebResponse webResponse = webRequest.GetResponse();
            webResponse.Close();
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        void make3step()
        {
            Uri uri = new Uri(step3url);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            webRequest.AllowAutoRedirect = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            string postData = "csrfmiddlewaretoken=" + csrftoken + "&username=" + login + "&password=" + password;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            webRequest.Method = "POST";
            webRequest.Host = "instagram.com";
            webRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:38.0) Gecko/20100101 Firefox/38.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Referer = step3url;
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.Headers.Add(HttpRequestHeader.Cookie, "csrftoken=" + csrftoken + "; mid=" + mid + ";");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse webResponse = webRequest.GetResponse();
            string cookiedata = webResponse.Headers.Get("Set-Cookie");

            if (!cookiedata.Contains("sessionid="))
            {
                webResponse.Close();
                throw new WrongLoginPassword();
            }

            csrftoken = getParameter(cookiedata, "csrftoken=", ";");
            sessionid = getParameter(cookiedata, "sessionid=", ";");
            webResponse.Close();
        }


        void make4step()
        {
            Uri uri = new Uri(step4url);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            webRequest.AllowAutoRedirect = false;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            webRequest.Host = "instagram.com";
            webRequest.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:38.0) Gecko/20100101 Firefox/38.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Referer = step3url;
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.Headers.Add(HttpRequestHeader.Cookie, "csrftoken=" + csrftoken + "; mid=" + mid + "; " + "sessionid=" + sessionid + ";");
            WebResponse webResponse = webRequest.GetResponse();

            string cookiedata = webResponse.Headers.Get("Set-Cookie");
            webResponse.Close();
            if(cookiedata.Contains("ds_user_id"))
            {
                new Thread(showBrowser).Start();

                ds_user_id = getParameter(cookiedata, "ds_user_id=", ";");

                string[] Cookies = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
                int notDeleted = 0;
                foreach (string CookieFile in Cookies)
                {
                    try
                    {
                        System.IO.File.Delete(CookieFile);

                    }
                    catch
                    {
                        notDeleted++;
                    }

                }

                MessageBox.Show((Cookies.Length - notDeleted).ToString() + " Cookies Deleted, " + notDeleted.ToString() + " Cookies Not Deleted", "Cookies");

                InternetSetCookie("https://instagram.com", "ig_pr", "1");
                InternetSetCookie("https://instagram.com", "ig_vw", "1440");
                InternetSetCookie("https://instagram.com", "csrftoken", csrftoken);
                InternetSetCookie("https://instagram.com", "mid", mid);
                InternetSetCookie("https://instagram.com", "sessionid", sessionid);
                InternetSetCookie("https://instagram.com", "ds_user_id", ds_user_id);

                webBrowser1.Navigated += webBrowser1_DocumentCompleted;

                webBrowser1.Navigate("https://instagram.com/oauth/authorize/?client_id=c08b5e9ea6204a0fa64569af46e20ab5&redirect_uri=http%3A%2F%2Flocalhost&response_type=token");

            }
            else
            {
                string locationdata = webResponse.Headers.Get("Location");
                token = getParameter(locationdata, "#access_token=", "");
                progressBar1.Visible = false;
            }
        }

        string  ds_user_id;

        string getParameter(string data, string FROM, string TO)
        {
            if (TO != "")
                return (data = data.Substring(data.IndexOf(FROM) + FROM.Length)).Substring(0, data.IndexOf(TO));
            else
                return data.Substring(data.IndexOf(FROM) + FROM.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login = textBox1.Text;
            password = textBox2.Text;

            if (login.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Fill in all fields", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                make1step();
                progressBar1.Increment(25);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message, "Network error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetForm();
                return;
            }
            try{
                make2step();
                progressBar1.Increment(25);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message, "Network error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetForm();
                return;
            }
            try
            {
                make3step();
                progressBar1.Increment(25);
            }
            catch (WrongLoginPassword)
            {
                MessageBox.Show("Wrong username/password", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetForm();
                return;
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message, "Network error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetForm();
                return;
            }
            try
            {
                make4step();
                progressBar1.Increment(25);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message, "Network error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetForm();
                return;
            }

            textBox1.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;

            textBox3.Visible = true;
            button2.Visible = true;
        }

        void follow()
        {
            string[] Cookies = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
            int notDeleted = 0;
            foreach (string CookieFile in Cookies)
            {
                try
                {
                    System.IO.File.Delete(CookieFile);

                }
                catch
                {
                    notDeleted++;
                }

            }


            MessageBox.Show((Cookies.Length - notDeleted).ToString() + " Cookies Deleted, " + notDeleted.ToString() + " Cookies Not Deleted", "Cookies");

            InternetSetCookie("https://instagram.com", "ig_pr", "1");
            InternetSetCookie("https://instagram.com", "ig_vw", "1440");
            InternetSetCookie("https://instagram.com", "csrftoken", csrftoken);
            InternetSetCookie("https://instagram.com", "mid", mid);
            InternetSetCookie("https://instagram.com", "sessionid", sessionid);
            // InternetSetCookie("https://instagram.com", "ds_user_id", ds_user_id);
            webBrowser1.Navigated += webBrowser1_Navigated_2;

            string address = "https://instagram.com/" + textBox3.Text;

            webBrowser1.Navigate(address);
        }

        public class myPictureBox : PictureBox
        {
            public string link;
        }

        private void button2_Click_2(object sender, EventArgs e)
        {

            Instagram test = new Instagram(token);

            User user = new User();
            user.login = textBox3.Text;

            if (user.GetUserInformation(user).isClosed)
            {
                follow();
            }
            else
            {
                List<Image> list = user.GetImages(1, 12);
                //list = user.GetImages(12, 12);

                for(int x = 0; x< 4; x++)
                {
                    for(int y = 0; y<3;y++)
                    {
                        if (list.Count > y * 4 + x)
                        {
                            myPictureBox box = new myPictureBox();
                            box.Size = new Size(150, 150);
                            box.Click += box_Click;
                            box.link = list[y * 4 + x].highResolution.ToString();
                            box.Cursor = Cursors.Hand;
                            box.Location = new Point(20 + x * (150 + 40), 62 + y * (150 + 20));
                            box.ImageLocation = list[y * 4 + x].thumbNail.ToString();
                            this.Controls.Add(box);
                        }
                        else
                        {
                           //
                        }
                    }
                }
            }

        }

        void box_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ты выбрал фотку: " + ((myPictureBox)sender).link);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName, string lpszCookieData);


        void webBrowser1_Navigated_2(object sender, WebBrowserNavigatedEventArgs e)
        {
            
            webBrowser1.Navigate("javascript:document.getElementsByTagName('button')[0].click()");
            //new Thread(showBrowser).Start();
        }

        bool visited = false;

        void webBrowser1_DocumentCompleted(object sender, WebBrowserNavigatedEventArgs e)
        {
            //if(webBrowser1.Url.ToString() != "https://instagram.com/oauth/authorize/?client_id=c08b5e9ea6204a0fa64569af46e20ab5&redirect_uri=http://localhost&response_type=token")
            if (visited)
            {
                new Thread(hideBrowser).Start();
                resetForm();
                button1_Click(sender, null);
                webBrowser1.Navigated -= webBrowser1_DocumentCompleted;
            }
            else visited = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //webBrowser1.Visible = true;
            webBrowser1.Navigate("javascript:document.getElementsByTagName('button')[0].click()");

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
                e.Handled = e.SuppressKeyPress = true;

            }
        }
    }

    public class WrongLoginPassword : Exception
    {

    }

    
}
