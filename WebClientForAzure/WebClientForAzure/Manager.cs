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

    public static class Manager {

        static HttpResponseMessage response;

        public static async void MakeRequest(string subscriptionKey) {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters
            queryString["shortAudio"] = "{boolean}";
            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles/{identificationProfileId}/enroll?" + queryString;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            Console.WriteLine(GetResponseAsString());

        }

        public static async void GetAllProfiles(string subscriptionKey) {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/identificationProfiles?" + queryString;

            response = await client.GetAsync(uri);

            Console.WriteLine(GetResponseAsString());
        }

        public static string GetResponseAsString() {

            string result = string.Empty;
            result  = "StatusCode : " + response.StatusCode.ToString() + "\n";
            result += response.Content.ReadAsStringAsync() + "\n";
            result += response.Content.Headers.ToString();

            return result;
        }
    }
}
