using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace AzureServiceManagement {

    internal static class ServiceProfilesManager {

        internal static System.Action<RequestConfig> OnRequestDone;

        private static RequestConfig requestConfig;

        /// <summary>
        /// Create a new speaker identification profile with specified locale.
        /// One subscription can only create 1000 speaker verification/identification profiles at most.
        /// </summary>
        internal static void CreateProfile() {
            Debug.Log("<b>ServiceProfilesManager</b> CreateProfile");

            string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\n\"locale\":\"en-us\",\n}");

            requestConfig = new RequestConfig(HttpMethod.POST, url, byteData);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Get all speaker identification profiles within the subscription.
        /// </summary>
        internal static void GetAllProfiles() {
            Debug.Log("<b>ServiceProfilesManager</b> GetAllProfiles");

            string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles";

            requestConfig = new RequestConfig(HttpMethod.GET, url);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Get a speaker identification profile by identificationProfileId.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        internal static void GetProfile(string identificationProfileId) {
            Debug.Log("<b>ServiceProfilesManager</b> GetProfile : " + identificationProfileId);

            string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?";

            requestConfig = new RequestConfig(HttpMethod.GET, url);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Deletes both speaker identification profile and all associated enrollments permanently from the service.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static void DeleteProfile(string identificationProfileId) {
            Debug.Log("<b>ServiceProfilesManager</b> DeleteProfile : " + identificationProfileId);

            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?";

            requestConfig = new RequestConfig(HttpMethod.DELETE, url);

            OnRequestDone(requestConfig);
        }

        internal static object PopulateProfileList(string json) {

            return JsonUtility.FromJson(json, typeof(DataProfile));
        }
    }
}
