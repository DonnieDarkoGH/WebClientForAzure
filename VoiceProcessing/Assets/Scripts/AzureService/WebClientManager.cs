using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace AzureServiceManagement{

    public class WebClientManager : MonoBehaviour {

        internal static WebClientManager Instance;

        internal static System.Action<string>                OnProfileListModified;
        internal static System.Action<RequestConfig, string> OnProfileCreated;
        internal static System.Action<string>                OnIdentificationDone;

        [SerializeField]
        private Text TextResult = null;

        [SerializeField]
        private string _subscriptionKey  = "8842b380852146a48496b709e0fbad2a";

        [SerializeField]
        private string _currentProfileId = "2de27537-13d4-465a-bb70-9c1b9156ef1c";

        [SerializeField]
        internal List<DataProfile> _profiles = new List<DataProfile>(0);

        [SerializeField]
        private ProfilesManager profilesManagerRef = null;

        private string opLocation = string.Empty;

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
            ServiceSpeakerManager.OnRequestDone  += InitHttpRequest;

            GetAllProfiles();
        }

        public void ClearResultPanel() {
            Debug.Log("<b>WebClientManager</b> ClearResultPanel ");

            DisplayResponse(string.Empty);
        }

        public void CreateProfile() {
            Debug.Log("<b>WebClientManager</b> CreateProfile ");


            ServiceProfilesManager.CreateProfile();
        }

        public void CreateEnrollment(string profileID, bool shortAudio = false) {
            Debug.Log("<b>WebClientManager</b> CreateProfile ");

            if(profileID == string.Empty)
            {
                DisplayResponse("WARNING ! You must select at least one Speaker for enrollement !");
                return;
            }
                

            ServiceProfilesManager.CreateEnrollment(profileID, shortAudio);
        }

        public void GetAllProfiles() {
            Debug.Log("<b>WebClientManager</b> GetAllProfiles ");

            ServiceProfilesManager.GetAllProfiles();
        }

        public void GetProfile(string profileID) {
            Debug.Log("<b>WebClientManager</b> GetProfile : " + profileID);

            if (profileID == string.Empty)
            {
                DisplayResponse("WARNING ! You must select at least one Speaker to get his Profile !");
                return;
            }

            ServiceProfilesManager.GetProfile(profileID);
        }

        public void DeleteProfile(string profileID) {
            Debug.Log("<b>WebClientManager</b> DeleteProfile : " + profileID);

            if (profileID == string.Empty)
            {
                DisplayResponse("WARNING ! You must select at least one Speaker to delete !");
                return;
            }

            ServiceProfilesManager.DeleteProfile(profileID);
        }

        public void Identification() {
            Debug.Log("<b>WebClientManager</b> Identification");

            string profiles = profilesManagerRef.GetCSVProfiles();

            if (profiles == string.Empty)
            {
                DisplayResponse("WARNING ! You must select at least one Speaker to Identify !");
                return;
            }
            //Debug.Log(profiles);

            ServiceSpeakerManager.Identification(profiles, VoiceRecord.fileBytes, true);
        }


        //private void InitHttpRequest(string method, string url, byte[] byteData = null) {
        //    Debug.Log("<b>WebClientManager</b> InitHttpRequest " + method + " : " + url);

        //    StartCoroutine(DoRequest(method,url));

        //}

        private void InitHttpRequest(RequestConfig requestConfig) {
            Debug.Log("<b>WebClientManager</b> InitHttpRequest " + requestConfig.ToString());

            //if (requestConfig.Data != null)
            //{
            //    Debug.Log(requestConfig.Data.Length);
            //    Debug.Log(requestConfig.Data.GetType());
            //}

            StartCoroutine(DoRequest(requestConfig));

        }

        IEnumerator DoRequest(RequestConfig requestConfig) {

            UnityWebRequest request = new UnityWebRequest(requestConfig.Url, requestConfig.Method);

            request.SetRequestHeader("content-type", "application/json");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", _subscriptionKey);

            request.downloadHandler = new DownloadHandlerBuffer();

            if(requestConfig.Data != null)
            {
                request.uploadHandler = new UploadHandlerRaw(requestConfig.Data);

                if (requestConfig.ServerOperation.Equals(EServerOperation.Identification) || requestConfig.ServerOperation.Equals(EServerOperation.CreateEnrollment))
                    request.uploadHandler.contentType = "application/octet-stream";
                //Debug.Log(request.uploadHandler.contentType);
                //Debug.Log(request.uploadHandler.data.GetType());
                //Debug.Log(request.uploadHandler.data.Length);
            }

            yield return request.Send();

            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(requestConfig.ServerOperation);
                ProcessResponse(request, requestConfig.ServerOperation);
            }

        }

        private void ProcessResponse(UnityWebRequest request, EServerOperation serverOperation) {
            Debug.Log("<b>WebClientManager</b> ProcessResponse " + serverOperation);


            //foreach(var s in request.GetResponseHeaders())
            //{
            //    Debug.Log(s.Key + ", " + s.Value);
            //}
            string json = request.downloadHandler.text;


            switch (serverOperation)
            {
                case EServerOperation.CreateEnrollment:
                    break;

                case EServerOperation.CreateProfile:
                    GetAllProfiles();
                    break;

                case EServerOperation.DeleteProfile:
                    GetAllProfiles();
                    break;

                case EServerOperation.GetAllProfiles:
                    OnProfileListModified(request.downloadHandler.text);
                    break;

                case EServerOperation.GetProfile:
                    break;

                case EServerOperation.GetOperationStatus:
                    //StartCoroutine(WaitForIdentification(request));

                    DataRequest dr = DataRequest.CreateFromJSON(json);
                    Debug.Log(dr.ToString());
                    if((dr.status != "succeeded" && dr.status != "failed"))
                    {
                        ServiceSpeakerManager.GetOperationStatus(opLocation);
                    }
                    else
                    {
                        OnIdentificationDone(dr.processingResult.identifiedProfileId);
                        opLocation = string.Empty;
                    }
                    break;

                case EServerOperation.Identification:
                    opLocation = request.GetResponseHeader("Operation-Location");
                    if (request.responseCode.Equals(202))
                    {
                        Debug.Log("Identification DONE !");
                        ServiceSpeakerManager.GetOperationStatus(opLocation);
                    }
                    break;

                case EServerOperation.Verification:
                    break;

                default:
                    break;
            }

            // Show results as text
            DisplayResponse(request.downloadHandler.text);

            // Or retrieve results as binary data
            //byte[] results = request.downloadHandler.data;

            Debug.Log("Code Response : " + request.responseCode);

        }

        private void DisplayResponse(string textToDisplay) {
            //Debug.Log("<b>WebClientManager</b> DisplayResponse " + textToDisplay);

            TextResult.text = textToDisplay;

            //JsonProfileInterface profiles = JsonProfileInterface.CreateFromJSON(textToDisplay);

            //Debug.Log(profiles.ToString());
        }

        IEnumerator WaitForIdentification(UnityWebRequest request) {

            DataRequest dr = DataRequest.CreateFromJSON(request.downloadHandler.text);
            Debug.Log(dr.ToString());
            
            do
            {
                Debug.Log(request.responseCode);
                yield return new WaitForSeconds(0.5f);
                
            } while (dr.status != "succeeded" && dr.status != "failed");

            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Identification DONE !");
                Debug.Log(dr.processingResult.identifiedProfileId);
                OnIdentificationDone(dr.processingResult.identifiedProfileId);
            }

            //if (dr.status != "succeeded" && dr.status != "failed")
            //{
            //    yield return new WaitForSeconds(0.1f);
            //    Identification();
            //}
            //else
            //{
            //    DisplayResponse(dr.identifiedProfileId);
            //}

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
