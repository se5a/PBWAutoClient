using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using PBW2AutoPlrClient.Properties;


namespace PBW2AutoPlrClient
{
	class PBW_ComsHandler
    {		
        /// <summary>
		/// logs into pbw http.
		/// </summary>
		/// <param name="login_address">The full login path.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>
		/// true if succes, false if fail.
		/// </returns>
		/// <remarks>
		/// Todo: handle errors. better.
		/// </remarks>
		public static bool connectPBW(string login_address, string username, string password)
		{
			bool success = false;
			HttpWebRequest request;
			HttpWebResponse response;
			CookieContainer chocolatecookies = new CookieContainer();
			logger.logwrite("attempting to connect to PBW \r\n");
			logger.logwrite("using address " + login_address + "\r\n");
			try
			{
				
				request = (HttpWebRequest)WebRequest.Create(login_address);
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
					ServerSettingsObj.Connection_Status = "Connected";
				}
				logger.logwrite("Status Code = " + response.StatusCode + "\r\n");
				logger.logwrite("Status Description = " + response.StatusDescription + "\r\n");
				logger.logwrite("cookies = " + response.Cookies + "\r\n");
				logger.logwrite("server = " + response.Server + "\r\n");

				chocolatecookies = request.CookieContainer;
				success = true;

                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                response.Close();
                string headers = response.Headers.ToString();
                //logger.logwrite("LOGIN RESPONSE");
                //logger.logwrite(fullResponse);
                logger.logwrite("CONNECT HEADER RESPONSE");
                logger.logwrite(headers);
                if (fullResponse.Contains("Player file received."))
                {
                    success = true;
                }
                
			}
			catch (WebException e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message + " While logging in");
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

                request.UserAgent = "PBWAutoClient/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
                System.Windows.Forms.MessageBox.Show(e.Message + " While getting XmlData");
            }
            finally
            {
                resStream.Close();
            }
			return returnString;
		}


		public static void downloadGame(CookieContainer cookies, string fulldownloadpath, string downloadfilename)
		{
			logger.logwrite("attempting download at address " + fulldownloadpath + "\r\n");
            FileStream fileStream = File.Create(downloadfilename);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fulldownloadpath);
                request.CookieContainer = cookies;
                request.Method = WebRequestMethods.Http.Get;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                logger.logwrite("response StatusCode is " + response.StatusCode + "\r\n");
                logger.logwrite("response StatusDescription is " + response.StatusDescription + "\r\n");

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
                System.Windows.Forms.MessageBox.Show(e.Message + " While downloading turn file");

            }
            finally
            {
                fileStream.Close();
            }
		}


        public static bool uploadTurn(CookieContainer cookies, string file, string uploadurl, string uploadFormParam)
        {
            //adapted from and many thanks to: http://www.briangrinstead.com/blog/multipart-form-post-in-c
            logger.logwrite("attempting upload of " + file + " at url " + uploadurl + " with form data " + uploadFormParam + "\r\n");
            bool was_succeess = false;

            string filename = Path.GetFileName(file);
            string fileformat = uploadFormParam; 
            string maxfilesize = "67108864";
            string content_type = "application/octet-stream";

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
                postParameters.Add(uploadFormParam, new FormUpload.FileParameter(data, filename, content_type)); 

                // Create request and receive response
                string postURL = uploadurl;
                string userAgent = "PBWAutoClient/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(cookies, postURL, userAgent, postParameters);

                // Process response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                webResponse.Close();
                //Response.Write(fullResponse);
                //logger.logwrite(fullResponse);
                string headers = webResponse.Headers.ToString();
                logger.logwrite(headers);
                if (fullResponse.Contains("Player file received."))
                { 
                    was_succeess = true; 
                }
            }

            catch (Exception e)
            {
                logger.logwrite(e.Message.ToString() + " While attempting upload");
            }
            finally
            {
                
            }

            return was_succeess;
        }
	}
}
