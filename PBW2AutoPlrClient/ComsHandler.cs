using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PBW2AutoPlrClient
{
	class PbwComsHandler
    {		
        /// <summary>
		/// logs into pbw http.
		/// </summary>
		/// <param name="loginAddress">The full login path.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>
		/// true if succes, false if fail.
		/// </returns>
		/// <remarks>
		/// Todo: handle errors. better.
		/// </remarks>
		public static bool ConnectPbw(string loginAddress, string username, string password)
		{
			bool success = false;
			HttpWebRequest request;
			HttpWebResponse response;
			CookieContainer chocolatecookies = new CookieContainer();
			Logger.Logwrite("attempting to connect to PBW \r\n");
			Logger.Logwrite("using address " + loginAddress + "\r\n");
			try
			{
				
				request = (HttpWebRequest)WebRequest.Create(loginAddress);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII))
				{
					writer.Write("username=" + username + "&password=" + password);
                    
				}
				request.CookieContainer = new CookieContainer();
				response = (HttpWebResponse)request.GetResponse();
       
				if (response.StatusCode.ToString() == "OK")
				{
					ServerSettingsObj.ConnectionStatus = "Connected";
				}
				Logger.Logwrite("Status Code = " + response.StatusCode + "\r\n");
				Logger.Logwrite("Status Description = " + response.StatusDescription + "\r\n");
				Logger.Logwrite("cookies = " + response.Cookies + "\r\n");
				Logger.Logwrite("server = " + response.Server + "\r\n");

				chocolatecookies = request.CookieContainer;
				success = true;

                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                response.Close();
                string headers = response.Headers.ToString();
                //logger.logwrite("LOGIN RESPONSE");
                //logger.logwrite(fullResponse);
                Logger.Logwrite("CONNECT HEADER RESPONSE");
                Logger.Logwrite(headers);
                if (fullResponse.Contains("Player file received."))
                {
                    success = true;
                }
                
			}
			catch (WebException e)
			{
                Logger.Logwrite(e.Message + " While logging in");
				//MessageBox.Show(e.Message + " While logging in");
				success = false;
			}

			ServerSettingsObj.CookieJar = chocolatecookies;
			return success;
		}

		/// <summary>
		/// gets xml data from pbw.
		/// </summary>
		/// <param name="cookies">cookies from a previous authentication.</param>
        /// <param name="url">url of page.</param>
		/// <returns>
		/// xml games list or null
		/// </returns>
		/// <remarks>
		/// Todo: 
		/// handle errors,
		/// login if not, etc.
		/// </remarks>
		public static string get_PbwXmlData(CookieContainer cookies, string url)
		{
			string returnString = null;

			StringBuilder sb = new StringBuilder();
			byte[] buf = new byte[8192];

			HttpWebRequest request;	
			HttpWebResponse response;
			Stream resStream = null;
			string tempString = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookies;

                request.UserAgent = "PBWAutoClient/" + Assembly.GetExecutingAssembly().GetName().Version;
                response = (HttpWebResponse)request.GetResponse();
                resStream = response.GetResponseStream();

                int count = 0;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);

                returnString = sb.ToString();
            }
            catch (WebException e)
            {
                Logger.Logwrite(e.Message + " While getting XmlData");
                //MessageBox.Show(e.Message + " While getting XmlData");
            }
            finally
            {
                if (resStream != null)
                { resStream.Close(); }
            }
			return returnString;
		}


		public static void DownloadGame(CookieContainer cookies, string fulldownloadpath, string downloadfilename)
		{
			Logger.Logwrite("attempting download at address " + fulldownloadpath + "\r\n");
            FileStream fileStream = File.Create(downloadfilename);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fulldownloadpath);
                request.CookieContainer = cookies;
                request.Method = WebRequestMethods.Http.Get;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                Logger.Logwrite("response StatusCode is " + response.StatusCode + "\r\n");
                Logger.Logwrite("response StatusDescription is " + response.StatusDescription + "\r\n");

                int buffersize = 1024;
                byte[] buffer = new byte[buffersize];
                int bytesRead = 0;
                WebHeaderCollection headders = response.Headers;

                while ((bytesRead = responseStream.Read(buffer, 0, buffersize)) != 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                } // end while
                
                //long expectedsize = long.Parse(headders["Content-Length"]);
                //FileInfo finfo = new FileInfo(downloadfilename);
                //long actualsize = finfo.Length;
                //if (expectedsize == actualsize)
                //{
                //    //great success.
                //}
            }

            catch (WebException e)
            {
                Logger.Logwrite(e.Message + " While downloading turn file");
                //MessageBox.Show(e.Message + " While downloading turn file");

            }
            finally
            {
                fileStream.Close();
            }
		}


        public static bool UploadTurn(CookieContainer cookies, string file, string uploadurl, string uploadFormParam)
        {
            //adapted from and many thanks to: http://www.briangrinstead.com/blog/multipart-form-post-in-c
            Logger.Logwrite("attempting upload of " + file + " at url " + uploadurl + " with form data " + uploadFormParam + "\r\n");
            bool wasSucceess = false;

            string filename = Path.GetFileName(file);
            string fileformat = uploadFormParam; 
            string maxfilesize = "67108864";
            string contentType = "application/octet-stream";

            try
            {
                // Read file data
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                // Generate post objects
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("MAX_FILE_SIZE", maxfilesize);
                postParameters.Add("fileformat", fileformat);
                postParameters.Add(uploadFormParam, new FormUpload.FileParameter(data, filename, contentType)); 

                // Create request and receive response
                string postUrl = uploadurl;
                string userAgent = "PBWAutoClient/" + Assembly.GetExecutingAssembly().GetName().Version;
                HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(cookies, postUrl, userAgent, postParameters);

                // Process response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                webResponse.Close();
                //Response.Write(fullResponse);
                //logger.logwrite(fullResponse);
                string headers = webResponse.Headers.ToString();
                Logger.Logwrite(headers);
                if (fullResponse.Contains("Player file received."))
                { 
                    wasSucceess = true; 
                }
            }

            catch (Exception e)
            {
                Logger.Logwrite(e.Message + " While attempting upload");
            }
            finally
            {
                
            }

            return wasSucceess;
        }
	}
}
