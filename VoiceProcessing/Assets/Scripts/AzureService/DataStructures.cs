using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzureServiceManagement {

    internal static class HttpMethod {

        private static string get      = "GET";
        private static string post     = "POST";
        private static string delete   = "DELETE";
        private static string head     = "HEAD";

        internal static string GET {
            get {
                return get;
            }
        }

        internal static string POST {
            get {
                return post;
            }
        }

        internal static string DELETE {
            get {
                return delete;
            }
        }

        internal static string HEAD {
            get {
                return head;
            }
        }
    }

    internal struct RequestConfig {

        internal string Method;
        internal string Url;
        internal byte[] Data;
        internal System.Type JsonToObjectType;

        internal RequestConfig(string method, string url, System.Type jsonToObjectType, byte[] data = null) {
            this.Method = method;
            this.Url    = url;
            this.Data   = data;
            this.JsonToObjectType = jsonToObjectType;
        }

        public override string ToString() {
            string returnValue = "RequestConfig " + Method + " : " + Url;

            if (Data != null)
            {
                returnValue += "\n" + Encoding.UTF8.GetString(Data);
            }

            return returnValue;
        }
    }

    internal abstract class JsonToObject : object {

        public static JsonToObject CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<JsonToObject>(jsonString);
        }
    }

    [System.Serializable]
    internal class DataProfile : JsonToObject {
        public string name                    = string.Empty;
        public string identificationProfileId = string.Empty;
        public string locale                  = string.Empty;
        public float  enrollmentSpeechTime          = 0f;
        public float  remainingEnrollmentSpeechTime = 0f;
        public string createdDateTime       = string.Empty;
        public string lastActionDateTime    = string.Empty;
        public string enrollmentStatus      = string.Empty;

        internal DataProfile(string profileName, string id) {
            name                    = profileName;
            identificationProfileId = id;
        }

        public static new DataProfile CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<DataProfile>(jsonString);
        }

        public override string ToString() {

            string str = string.Empty;

            str += "IdentificationProfileId : " + identificationProfileId + "\n";
            str += "Locale : "                  + locale + "\n";
            str += "EnrollmentSpeechTime : "    + enrollmentSpeechTime + "\n";
            str += "RemainingEnrollmentSpeechTime : " + remainingEnrollmentSpeechTime + "\n";
            str += "CreatedDateTime : "     + createdDateTime + "\n";
            str += "LastActionDateTime : "  + lastActionDateTime + "\n";
            str += "EnrollmentStatus : "    + enrollmentStatus + "\n";
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
                str += "Locale : "                  + DataProfiles[i].locale + "\n";
                str += "EnrollmentSpeechTime : "    + DataProfiles[i].enrollmentSpeechTime + "\n";
                str += "RemainingEnrollmentSpeechTime : " + DataProfiles[i].remainingEnrollmentSpeechTime + "\n";
                str += "CreatedDateTime : "         + DataProfiles[i].createdDateTime + "\n";
                str += "LastActionDateTime : "      + DataProfiles[i].lastActionDateTime + "\n";
                str += "EnrollmentStatus : "        + DataProfiles[i].enrollmentStatus + "\n";
                str += "*********************************************************************\n";
            }

            return str;
        }
    }

}
