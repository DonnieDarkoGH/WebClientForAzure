using System.Text;

namespace AzureServiceManagement {

    internal enum EServerOperation {
        CreateProfile       = 0,
        CreateEnrollment    = 1,
        GetAllProfiles      = 2,
        GetProfile          = 3,
        DeleteProfile       = 4,
        ResetEnrollments    = 5,
        GetOperationStatus  = 6,
        Identification      = 7,
        Verification        = 8
    }

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
        internal EServerOperation ServerOperation;

        internal RequestConfig(string method, string url, EServerOperation serverOperation, byte[] data = null) {
            this.Method = method;
            this.Url    = url;
            this.Data   = data;
            this.ServerOperation = serverOperation;
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

}
