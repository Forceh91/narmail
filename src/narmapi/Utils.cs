using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace narmapi
{
    public class HTTPResponse
    {
        public string response;
        public int rateLimitUsed;
        public double rateLimitRemaining;
        public int rateLimitReset;
    }

    public static class Utils
    {
        private static readonly string userAgent = "Windows 10:Narmail:v0.0.1 by /u/forceh";
        private static string rateLimitRemaining;
        private static string rateLimitReset;
        private static string rateLimitUsed;

        public static async Task<HTTPResponse> getHTTPString(Uri requestURI, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) == true)
                return null;

            using (HttpClient httpClient = new HttpClient())
            {
                // add some headers such as the bearer and user agent
                httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

                // get what we want
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestURI);
                if (httpResponseMessage.IsSuccessStatusCode == false)
                    return null;

                // get the rate limiting stuff from the response
                stripRateHeaders(httpResponseMessage);

                // get the response
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                // return the response
                HTTPResponse httpResponse = new HTTPResponse()
                {
                    rateLimitRemaining = Convert.ToDouble(rateLimitRemaining),
                    rateLimitReset = Convert.ToInt32(rateLimitReset),
                    rateLimitUsed = Convert.ToInt32(rateLimitUsed),
                    response = response
                };

                return httpResponse;
            }
        }

        public static async Task<HTTPResponse> postHTTPString(Uri requestURI, string accessToken, string jsonString)
        {
            if (string.IsNullOrEmpty(accessToken) == true)
                return null;

            using (HttpClient httpClient = new HttpClient())
            {
                // add some headers such as the bearer and user agent
                httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

                // the form data to send
                HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/x-www-form-urlencoded");

                // get what we want
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestURI, httpContent);
                if (httpResponseMessage.IsSuccessStatusCode == false)
                    return null;

                // get the rate limiting stuff from the response
                stripRateHeaders(httpResponseMessage);

                // get the response
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                // return the response
                HTTPResponse httpResponse = new HTTPResponse()
                {
                    rateLimitRemaining = Convert.ToDouble(rateLimitRemaining),
                    rateLimitReset = Convert.ToInt32(rateLimitReset),
                    rateLimitUsed = Convert.ToInt32(rateLimitUsed),
                    response = response
                };

                return httpResponse;
            }
        }

        public static async Task<string> postHTTPCodeString(Uri requestURI, string clientID, string codeString)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // generate the password thingy
                string clientIDString = clientID + ": ";
                string userPassBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientIDString));

                // send the basic auth stuff
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + userPassBase64);

                // the form data to send
                HttpContent httpContent = new StringContent(codeString, Encoding.UTF8, "application/x-www-form-urlencoded");

                // get what we want
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestURI, httpContent);
                if (httpResponseMessage.IsSuccessStatusCode == false)
                    return null;
                
                // return the string we get back
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        private static void stripRateHeaders(HttpResponseMessage responseMessage)
        {
            rateLimitRemaining = responseMessage.Headers.GetValues("x-ratelimit-remaining").FirstOrDefault();
            rateLimitReset = responseMessage.Headers.GetValues("x-ratelimit-reset").FirstOrDefault();
            rateLimitUsed = responseMessage.Headers.GetValues("x-ratelimit-used").FirstOrDefault();
        }
    }
}
