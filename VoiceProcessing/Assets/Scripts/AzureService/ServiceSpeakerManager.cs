using System.Text;
using UnityEngine;

namespace AzureServiceManagement {

    /// <summary>
    /// This class manages the requests of the SPEAKER RECOGNITION API
    /// see : https://westus.dev.cognitive.microsoft.com/docs/services/563309b6778daf02acc0a508/operations/5645c725ca73070ee8845bd6
    /// </summary>
    internal static class ServiceSpeakerManager {

        // Event triggered 
        internal static System.Action<RequestConfig> OnRequestDone;

        private static RequestConfig requestConfig;

        /// <summary>
        /// Get operation status or result. The operation should be created by Speaker Recognition - Identification or Identification Profile - Create Enrollment. 
        /// And the URL should be retrieved from Operation-Location header of initial POST 202 response
        /// </summary>
        /// <param name="operationId">The operation Id, created by Speaker Recognition - Identification or Identification Profile - Create Enrollment.</param>
        internal static void GetOperationStatus(string operationId) {
            Debug.Log("<b>SpeakerManager</b> GetOperationStatus");

            //string url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/operations/" + operationId + "?";
            string url = operationId;

            requestConfig = new RequestConfig(HttpMethod.GET, url, EServerOperation.GetOperationStatus);

            OnRequestDone(requestConfig);
        }


        /// <summary>
        /// To automatically identify who is speaking given a group of speakers.
        /// </summary>
        /// <param name="identificationProfileIds">Comma-delimited identificationProfileIds, the id should be Guid.
        /// It can only support at most 10 profiles for one identification request.</param>
        /// <param name="shortAudio">Instruct the service to waive the recommended minimum audio limit needed for identification. 
        /// Set value to “true” to force identification using any audio length (min. 1 second).</param>
        public static void Identification(string identificationProfileIds, byte[] audioBytes, bool shortAudio = false) {
            Debug.Log("<b>SpeakerManager</b> Identification : " + identificationProfileIds + " (short audio : " + shortAudio +")");

            // Request parameters
            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identify?identificationProfileIds=" + identificationProfileIds + "&shortAudio=" + shortAudio;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");
            //VoiceRecord.StreamAudio();

            //requestConfig = new RequestConfig(HttpMethod.POST, url, EServerOperation.Identification, VoiceRecord.fileBytes);
            requestConfig = new RequestConfig(HttpMethod.POST, url, EServerOperation.Identification, audioBytes);

            OnRequestDone(requestConfig);
        }

        /// <summary>
        /// To automatically verify and authenticate users using their voice or speech.
        /// </summary>
        /// <param name="verificationProfileId">ID of speaker verification profile. It should be a GUID.</param>
        static void Verification(string verificationProfileId) {
            Debug.Log("<b>SpeakerManager</b> Verification : " + verificationProfileId);

            var url = "https://westus.api.cognitive.microsoft.com/spid/v1.0/verify?verificationProfileId={" + verificationProfileId + "}&";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            requestConfig = new RequestConfig(HttpMethod.POST, url, EServerOperation.Verification, byteData);

            OnRequestDone(requestConfig);
        }
    }
}