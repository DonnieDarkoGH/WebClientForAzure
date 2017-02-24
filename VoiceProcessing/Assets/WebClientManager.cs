using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

public class WebClientManager : MonoBehaviour {

    public static WebClientManager Instance;

    void Awake() {
        Instance = GetComponent<WebClientManager>();
    }

    static private IEnumerator CheckURL() {
        bool foundURL;
        string checkThisURL = "http://www.example.com/index.html";
        WebAsync webAsync = new WebAsync();

        yield return Instance.StartCoroutine(webAsync.CheckForMissingURL(checkThisURL));
        Debug.Log("Does " + checkThisURL + " exist? " + webAsync.isURLmissing);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartCoroutine(CheckURL());
            //StartCoroutine(Check("http://www.example.com/index.html"));
            //StartCoroutine(AreWeConnectedToInternet());
            MakeRequestNew();
        }
    }

    private IEnumerator Check(string url) {
        WebAsync webAsync = new WebAsync(); StartCoroutine(webAsync.CheckForMissingURL(url));
        while (! webAsync.isURLcheckingCompleted) yield return null;
        bool result = webAsync.isURLmissing;
    }

    WebAsync webAsync = new WebAsync();

    private IEnumerator AreWeConnectedToInternet() {
        bool areWe;
        WebRequest requestAnyURL = HttpWebRequest.Create("http://www.example.com");
        requestAnyURL.Method = "HEAD";
        IEnumerator e = webAsync.GetResponse(requestAnyURL);
        while (e.MoveNext()) { yield return e.Current; }

        areWe = (webAsync.requestState.errorMessage == null);

        Debug.Log("Are we connected to the inter webs? " + areWe);
        Debug.Log(webAsync.requestState.webResponse.ResponseUri.PathAndQuery);
    }

    void MakeRequestNew(string ur = "") {

        var client = new System.Net.WebClient();
        var queryString = "8842b380852146a48496b709e0fbad2a";

        client.Headers.Add("Ocp-Apim-Subscription-Key", "8842b380852146a48496b709e0fbad2a");
        // Display the headers in the request
        Debug.Log("Resulting Request Headers: ");
        Debug.Log(client.Headers.ToString());

        var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/operations/{operationId}?" + queryString;

        StartCoroutine(Check(uri));
    }

    //static IEnumerator MakeRequest() {
    //    var client = new HttpClient();
    //    var queryString = HttpUtility.ParseQueryString(string.Empty);

    //    // Request headers
    //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

    //    var uri = "https://westus.api.cognitive.microsoft.com/spid/v1.0/operations/{operationId}?" + queryString;

    //    var response = await client.GetAsync(uri);
    //}
}

