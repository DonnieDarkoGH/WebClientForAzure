using System.Text;
using UnityEngine;

namespace AzureServiceManagement {

    internal abstract class JsonToObject : object {

        public static JsonToObject CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<JsonToObject>(jsonString);
        }
    }


    [System.Serializable]
    internal class ProcessingResult : JsonToObject {

        public string identifiedProfileId = string.Empty;
        public string confidence          = string.Empty;
        public string enrollmentStatus    = string.Empty;
        public float remainingEnrollmentSpeechTime = 0.0f;
        public float speechTime           = 0.0f;
        public string message             = string.Empty;

        public static new ProcessingResult CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<ProcessingResult>(jsonString);
        }

        public override string ToString() {

            string str = string.Empty;

            str += "IdentificationProfileId : " + identifiedProfileId + "\n";
            str += "confidence : "              + confidence + "\n";
            str += "enrollmentStatus : "        + enrollmentStatus + "\n";
            str += "RemainingEnrollmentSpeechTime : " + remainingEnrollmentSpeechTime + "\n";
            str += "speechTime : "              + speechTime + "\n";
            str += "message : "                 + message + "\n";

            return str;
        }
    }

    [System.Serializable]
    internal class DataRequest : JsonToObject {

        public string status             = string.Empty;
        public string createdDateTime    = string.Empty;
        public string lastActionDateTime = string.Empty;
        public ProcessingResult processingResult = new ProcessingResult();

        public static new DataRequest CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<DataRequest>(jsonString);
        }


        public override string ToString() {

            string str = string.Empty;

            str += "status : " + status + "\n";
            str += "CreatedDateTime : " + createdDateTime + "\n";
            str += "LastActionDateTime : " + lastActionDateTime + "\n";
            str += "processingResult : " + processingResult.ToString() + "\n";
            str += "*********************************************************************\n";

            return str;
        }
    }

    [System.Serializable]
    internal class DataProfile : JsonToObject {
        public string name = string.Empty;
        public string identificationProfileId = string.Empty;
        public string locale = string.Empty;
        public float enrollmentSpeechTime = 0f;
        public float remainingEnrollmentSpeechTime = 0f;
        public string createdDateTime = string.Empty;
        public string lastActionDateTime = string.Empty;
        public string enrollmentStatus = string.Empty;

        internal DataProfile(string profileName, string id) {
            name = profileName;
            identificationProfileId = id;
        }

        public static new DataProfile CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<DataProfile>(jsonString);
        }

        public override string ToString() {

            string str = string.Empty;

            str += "IdentificationProfileId : " + identificationProfileId + "\n";
            str += "Locale : " + locale + "\n";
            str += "EnrollmentSpeechTime : " + enrollmentSpeechTime + "\n";
            str += "RemainingEnrollmentSpeechTime : " + remainingEnrollmentSpeechTime + "\n";
            str += "CreatedDateTime : " + createdDateTime + "\n";
            str += "LastActionDateTime : " + lastActionDateTime + "\n";
            str += "EnrollmentStatus : " + enrollmentStatus + "\n";
            str += "*********************************************************************\n";

            return str;
        }
    }

    [System.Serializable]
    internal class DataProfileArray : JsonToObject {

        public DataProfile[] DataProfiles;

        public static new DataProfileArray CreateFromJSON(string jsonString) {

            string newJson = "{ \"DataProfiles\": " + jsonString + "}";
            //Debug.Log(newJson);

            return JsonUtility.FromJson<DataProfileArray>(newJson);
        }

        public override string ToString() {
            string str = string.Empty;

            int len = DataProfiles.Length;
            for (int i = 0; i < len; i++)
            {
                str += "IdentificationProfileId : " + DataProfiles[i].identificationProfileId + "\n";
                str += "Locale : " + DataProfiles[i].locale + "\n";
                str += "EnrollmentSpeechTime : " + DataProfiles[i].enrollmentSpeechTime + "\n";
                str += "RemainingEnrollmentSpeechTime : " + DataProfiles[i].remainingEnrollmentSpeechTime + "\n";
                str += "CreatedDateTime : " + DataProfiles[i].createdDateTime + "\n";
                str += "LastActionDateTime : " + DataProfiles[i].lastActionDateTime + "\n";
                str += "EnrollmentStatus : " + DataProfiles[i].enrollmentStatus + "\n";
                str += "*********************************************************************\n";
            }

            return str;
        }
    }
}
