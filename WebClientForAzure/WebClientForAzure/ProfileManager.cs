using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;

namespace WebClientForAzure
{

    public static class ProfileManager {

        /// <summary>
        /// Callback sent when a HttpResponseMessage is returned from the server
        /// </summary>
        public static System.Action<string> OnResponse;

        /// <summary>
        /// Subscription key which provides access to this API. Found in Cognitive Services accounts.
        /// </summary>
        private static string                _subscriptionKey;

        /// <summary>
        /// Set the Subscription Key which provides access to this API. Found in Cognitive Services accounts.
        /// </summary>
        /// <param name="key"></param>
        public static void SetSubscriptionKey(string key) {
            _subscriptionKey = key; 
        }

        /// <summary>
        /// Create a new speaker identification profile with specified locale.
        /// One subscription can only create 1000 speaker verification/identification profiles at most.
        /// </summary>
        public static async void CreateProfile() {
            Console.Write("ProfileManager :: CreateProfile\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\n\"locale\":\"en-us\",\n}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            SendResponseCallback(response);
        }

        /// <summary>
        /// Enrollment for speaker identification is text-independent, which means that there are no restrictions on what the speaker says in the audio. 
        /// The speaker's voice is recorded, and a number of features are extracted to form a unique voiceprint.
        /// </summary>
        public static async void CreateEnrollment() {
            Console.Write("ProfileManager :: CreateEnrollment\n");

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            // Request parameters
            queryString["shortAudio"] = "{boolean}";
            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/{identificationProfileId}/enroll?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            SendResponseCallback(response);

        }

        /// <summary>
        /// Get all speaker identification profiles within the subscription.
        /// </summary>
        public static async void GetAllProfiles() {
            Console.Write("ProfileManager :: GetAllProfiles\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            HttpResponseMessage response;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles?" + queryString;

            response = await client.GetAsync(uri);

            SendResponseCallback(response);
        }

        /// <summary>
        /// Get a speaker identification profile by identificationProfileId.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static async void GetProfile(string identificationProfileId) {
            Console.Write("ProfileManager :: GetProfile (" + identificationProfileId + ")\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            HttpResponseMessage response;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?" + queryString;

            response = await client.GetAsync(uri);

            SendResponseCallback(response);
        }

        /// <summary>
        /// Deletes both speaker identification profile and all associated enrollments permanently from the service.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static async void DeleteProfile(string identificationProfileId) {
            Console.Write("ProfileManager :: DeleteProfile (" + identificationProfileId + ")\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            HttpResponseMessage response;

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?" + queryString;

            response = await client.DeleteAsync(uri);

            SendResponseCallback(response);
        }

        /// <summary>
        /// Deletes all enrollments associated with the given speaker identification profile permanently from the service.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static async void ResetEnrollments(string identificationProfileId) {
            Console.Write("ProfileManager :: ResetEnrollments (" + identificationProfileId + ")\n");

            var client      = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "/reset?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            SendResponseCallback(response);
        }

        /// <summary>
        /// Parse the content of the result as a string and send it with the OnResponse Callback 
        /// </summary>
        private static async void SendResponseCallback(HttpResponseMessage response) {
            Console.Write("ProfileManager :: SendResponseCallback\n");

            string result = await response.Content.ReadAsStringAsync();

            OnResponse(result);
        }
    }
}
