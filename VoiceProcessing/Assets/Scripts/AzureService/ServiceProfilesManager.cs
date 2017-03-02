using System.Text;
using UnityEngine;


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

            requestConfig = new RequestConfig(HttpMethod.POST, url, EServerOperation.CreateEnrollment, byteData);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Enrollment for speaker identification is text-independent, which means that there are no restrictions on what the speaker says in the audio. 
        /// The speaker's voice is recorded, and a number of features are extracted to form a unique voiceprint.
        /// </summary>
        public static void CreateEnrollment(string identificationProfileId, bool shortAudio = false) {
            Debug.Log("<b>ServiceProfilesManager</b> CreateEnrollment");

            // Request parameters
            //queryString["shortAudio"] = "{boolean}";
            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/"+ identificationProfileId + "/enroll?" + "&shortAudio=" + shortAudio;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");
            VoiceRecord.StreamAudio();

            requestConfig = new RequestConfig(HttpMethod.POST, url, EServerOperation.CreateEnrollment, VoiceRecord.fileBytes);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Get all speaker identification profiles within the subscription.
        /// </summary>
        internal static void GetAllProfiles() {
            Debug.Log("<b>ServiceProfilesManager</b> GetAllProfiles");

            string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles";

            requestConfig = new RequestConfig(HttpMethod.GET, url, EServerOperation.GetAllProfiles);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Get a speaker identification profile by identificationProfileId.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        internal static void GetProfile(string identificationProfileId) {
            Debug.Log("<b>ServiceProfilesManager</b> GetProfile : " + identificationProfileId);

            string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?";

            requestConfig = new RequestConfig(HttpMethod.GET, url, EServerOperation.GetProfile);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Deletes both speaker identification profile and all associated enrollments permanently from the service.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static void DeleteProfile(string identificationProfileId) {
            Debug.Log("<b>ServiceProfilesManager</b> DeleteProfile : " + identificationProfileId);

            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "?";

            requestConfig = new RequestConfig(HttpMethod.DELETE, url, EServerOperation.DeleteProfile);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// Deletes all enrollments associated with the given speaker identification profile permanently from the service.
        /// </summary>
        /// <param name="identificationProfileId">ID of speaker identification profile. GUID returned from Identification Profile - Create Profile API</param>
        public static void ResetEnrollments(string identificationProfileId) {
            Debug.Log("<b>ServiceProfilesManager</b> ResetEnrollments : " + identificationProfileId);

            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/" + identificationProfileId + "/reset?";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            requestConfig = new RequestConfig(HttpMethod.DELETE, url, EServerOperation.ResetEnrollments, byteData);

            OnRequestDone(requestConfig);
        }

        internal static object PopulateProfileList(string json) {

            return JsonUtility.FromJson(json, typeof(DataProfile));
        }
    }
}
