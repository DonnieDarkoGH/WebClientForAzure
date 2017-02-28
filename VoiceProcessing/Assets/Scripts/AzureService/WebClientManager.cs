using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace AzureServiceManagement{

    public class WebClientManager : MonoBehaviour {

        public static WebClientManager Instance;

        [SerializeField]
        private Text TextResult = null;

        [SerializeField]
        private string _subscriptionKey  = "8842b380852146a48496b709e0fbad2a";

        [SerializeField]
        private string _currentProfileId = "2de27537-13d4-465a-bb70-9c1b9156ef1c";

        [SerializeField]
        internal List<DataProfile> Profiles = new List<DataProfile>(0);

        public string SubscriptionKey {
            get {
                return _subscriptionKey;
            }

            set {
                _subscriptionKey = value;
            }
        }

        void Awake() {

            Instance = GetComponent<WebClientManager>();

            ServiceProfilesManager.OnRequestDone += InitHttpRequest;
        }

        public void ClearResultPanel() {
            Debug.Log("<b>WebClientManager</b> ClearResultPanel ");

            DisplayResponse(string.Empty);
        }

        public void CreateProfile() {
            Debug.Log("<b>WebClientManager</b> CreateProfile ");

            ServiceProfilesManager.CreateProfile();
        }

        public void GetAllProfiles() {
            Debug.Log("<b>WebClientManager</b> GetAllProfiles ");

            ServiceProfilesManager.GetAllProfiles();
        }

        public void GetProfile(string profileID) {
            Debug.Log("<b>WebClientManager</b> GetProfile : " + profileID);

            ServiceProfilesManager.GetProfile(profileID);
        }

        public void DeleteProfile(string profileID) {
            Debug.Log("<b>WebClientManager</b> DeleteProfile : " + profileID);

            ServiceProfilesManager.DeleteProfile(profileID);
        }


        private void InitHttpRequest(string method, string url, byte[] byteData = null) {
            Debug.Log("<b>WebClientManager</b> InitHttpRequest " + method + " : " + url);

            StartCoroutine(DoRequest(method,url));

        }

        private void InitHttpRequest(RequestConfig requestConfig) {
            Debug.Log("<b>WebClientManager</b> InitHttpRequest " + requestConfig.ToString());

            if(requestConfig.Data == null)
            {
                StartCoroutine(DoRequest(requestConfig.Method, requestConfig.Url));
            }
            else
            {
                StartCoroutine(DoRequest(requestConfig.Method, requestConfig.Url, requestConfig.Data));
            }
                

        }

        IEnumerator DoRequest(string method, string url) {
             
            UnityWebRequest request = new UnityWebRequest(url, method);
            request.SetRequestHeader("content-type", "application/json");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", _subscriptionKey);

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.Send();

            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Show results as text
                DisplayResponse(request.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = request.downloadHandler.data;
            }

        }

        IEnumerator DoRequest(string method, string url, byte[] byteData) {

            UnityWebRequest request = new UnityWebRequest(url, method);

            request.SetRequestHeader("content-type", "application/json");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", _subscriptionKey);

            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler   = new UploadHandlerRaw(byteData);

            yield return request.Send();

            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Show results as text
                DisplayResponse(request.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = request.downloadHandler.data;
            }

        }

        private void DisplayResponse(string textToDisplay) {
            //Debug.Log("<b>WebClientManager</b> DisplayResponse " + textToDisplay);

            TextResult.text = textToDisplay;

            //JsonProfileInterface profiles = JsonProfileInterface.CreateFromJSON(textToDisplay);

            //Debug.Log(profiles.ToString());
        }

        private void OnDisable() {
            //Debug.Log("<b>WebClientManager</b> OnDisable");

            ServiceProfilesManager.OnRequestDone -= InitHttpRequest;
        }

        private void OnDestroy() {
            //Debug.Log("<b>WebClientManager</b> OnDestroy");

            ServiceProfilesManager.OnRequestDone -= InitHttpRequest;
        }

    }


}
