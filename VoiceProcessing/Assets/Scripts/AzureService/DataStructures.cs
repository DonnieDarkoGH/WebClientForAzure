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

        internal RequestConfig(string method, string url, byte[] data = null) {
            this.Method = method;
            this.Url    = url;
            this.Data   = data;
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

    [System.Serializable]
    internal class ProfileList {

        public List<ProfileController> Profiles = new List<ProfileController>();

        internal ProfileController GetProfileById(string id) {

            int len = Profiles.Count;

            for (int i = 0; i < len; i++)
            {
                if (Profiles[i].IdentificationProfileId == id)
                {
                    return Profiles[i];
                }
            }

            Debug.LogError("No profile correspond to " + id);

            return null;
        }

        internal ProfileController GetProfileByName(string name) {

            int len = Profiles.Count;

            for (int i = 0; i < len; i++)
            {
                if (Profiles[i].ProfileName == name)
                {
                    return Profiles[i];
                }
            }

            Debug.LogError("No profile correspond to " + name);

            return null;
        }
    }

    [System.Serializable]
    internal class DataProfile {
        public string identificationProfileId = string.Empty;
        public string locale                  = string.Empty;
        public float  enrollmentSpeechTime          = 0f;
        public float  remainingEnrollmentSpeechTime = 0f;
        public string createdDateTime       = string.Empty;
        public string lastActionDateTime    = string.Empty;
        public string enrollmentStatus      = string.Empty;

        public static DataProfile CreateFromJSON(string jsonString) {
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
    internal class JsonProfileInterface {

        public DataProfile[] DataProfiles;

        public static JsonProfileInterface CreateFromJSON(string jsonString) {

            string newJson = "{ \"DataProfiles\": " + jsonString + "}";
            Debug.Log(newJson);

            return JsonUtility.FromJson<JsonProfileInterface>(newJson);
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
