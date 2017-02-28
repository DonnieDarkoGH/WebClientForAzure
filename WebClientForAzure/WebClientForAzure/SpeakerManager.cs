using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;

namespace WebClientForAzure {

    public class SpeakerManager {

        /// <summary>
        /// Callback sent when a HttpResponseMessage is returned from the server
        /// </summary>
        public static System.Action<string> OnResponse;

        /// <summary>
        /// Subscription key which provides access to this API. Found in Cognitive Services accounts.
        /// </summary>
        private static string _subscriptionKey;

        /// <summary>
        /// Set the Subscription Key which provides access to this API. Found in Cognitive Services accounts.
        /// </summary>
        /// <param name="key"></param>
        public static void SetSubscriptionKey(string key) {
            _subscriptionKey = key;
        }

        /// <summary>
        /// Get operation status or result. The operation should be created by Speaker Recognition - Identification or Identification Profile - Create Enrollment. 
        /// And the URL should be retrieved from Operation-Location header of initial POST 202 response
        /// </summary>
        /// <param name="operationId">The operation Id, created by Speaker Recognition - Identification or Identification Profile - Create Enrollment.</param>
        public static async void GetOperationStatus(string operationId) {
            Console.Write("SpeakerManager :: GetOperationStatus\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/operations/" + operationId + "?" + queryString;

            var response = await client.GetAsync(uri);

            SendResponseCallback(response);
        }

        /// <summary>
        /// To automatically identify who is speaking given a group of speakers.
        /// </summary>
        /// <param name="identificationProfileIds">Comma-delimited identificationProfileIds, the id should be Guid.
        /// It can only support at most 10 profiles for one identification request.</param>
        /// <param name="shortAudio">Instruct the service to waive the recommended minimum audio limit needed for identification. 
        /// Set value to “true” to force identification using any audio length (min. 1 second).</param>
        public static async void Identification(string identificationProfileIds, bool shortAudio = false) {
            Console.Write("SpeakerManager :: Identification (" + identificationProfileIds + ", " + shortAudio + ")\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            // Request parameters
            queryString["shortAudio"] = "{" + shortAudio + "}";
            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identify?identificationProfileIds=" + identificationProfileIds + "&" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("<application/json >");
                response = await client.PostAsync(uri, content);
            }

        }

        /// <summary>
        /// To automatically verify and authenticate users using their voice or speech.
        /// </summary>
        /// <param name="verificationProfileId">ID of speaker verification profile. It should be a GUID.</param>
        static async void Verification(string verificationProfileId) {
            Console.Write("SpeakerManager :: Verification ("+ verificationProfileId + ")\n");

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/verify?verificationProfileId={verificationProfileId}&" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
                response = await client.PostAsync(uri, content);
            }

        }

        /// <summary>
        /// Parse the content of the result as a string and send it with the OnResponse Callback 
        /// </summary>
        private static async void SendResponseCallback(HttpResponseMessage response) {
            Console.Write("SpeakerManager :: SendResponseCallback\n");

            string result = await response.Content.ReadAsStringAsync();

            OnResponse(result);
        }
    }
}
