using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

public class ProfilesManager : MonoBehaviour {

    [SerializeField]
    private ProfileController profileControllerPrefab = null;

    [SerializeField]
    private InputField inputFieldRef = null;

    [SerializeField]
    internal List<ProfileController> Profiles = new List<ProfileController>(0);

    private string newProfileName = string.Empty;
    private string newProfileId   = string.Empty;


    // Use this for initialization
    void Awake () {

        WebClientManager.OnProfileListModified  += PopulateProfiles;
        WebClientManager.OnProfileCreated       += CreateNewProfile;

        WebClientManager.OnIdentificationDone   += DisplayIdentifiedSpeaker;

    }

    private void ClearChildren() {

        ProfileController[] children = GetComponentsInChildren<ProfileController>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            Destroy(children[i].gameObject);
        }

    }

    private void PopulateProfiles(string json) {
        Debug.Log("<b>ServiceProfilesManager</b> PopulateProfiles with Json : " + json );

        ClearChildren();

        DataProfileArray profileArray = DataProfileArray.CreateFromJSON(json);
        Debug.Log(profileArray.ToString());

        int len = profileArray.DataProfiles.Length;
        //Profiles = new List<ProfileController>(len);
        Debug.Log(Profiles.Count);

        for (int i = 0; i < len; i++)
        {
            Profiles[i] = Instantiate(profileControllerPrefab, transform);
            Profiles[i].Init(profileArray.DataProfiles[i]);
            Profiles[i].name = "Toto";
        }
    }

    private void CreateNewProfile(RequestConfig requestConfig, string json) {
        Debug.Log("<b>ServiceProfilesManager</b> CreateNewProfile with request :\n" + requestConfig.ToString() + "\n and Json : " + json);

        DataProfile profile = DataProfile.CreateFromJSON(json);
        WebClientManager.Instance._profiles.Add(profile);

    }

    internal ProfileController GetProfileById(string id) {

        ProfileController[] children = GetComponentsInChildren<ProfileController>();
        int len = children.Length;

        for (int i = 0; i < len; i++)
        {
            if (children[i].IdentificationProfileId == id)
            {
                return children[i];
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

    internal void DisplayNewNameInputField(bool isVisible) {
        inputFieldRef.gameObject.SetActive(isVisible);
    }

    public void RenameLastProfile() {

        int len = WebClientManager.Instance._profiles.Count;

        WebClientManager.Instance._profiles[len - 1].name = inputFieldRef.text;

    }

    public void GetProfile() {

        Toggle[] children = GetComponentsInChildren<Toggle>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                string id = children[i].GetComponent<ProfileController>().IdentificationProfileId;
                WebClientManager.Instance.GetProfile(id);
            }
        }
    }

    public void DeleteProfiles() {

        Toggle[] children = GetComponentsInChildren<Toggle>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                string id = children[i].GetComponent<ProfileController>().IdentificationProfileId;
                WebClientManager.Instance.DeleteProfile(id);
            }
        }
    }

    internal string GetCSVProfiles() {

        string csvList = string.Empty;

        Toggle[] children = GetComponentsInChildren<Toggle>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                if (csvList != string.Empty)
                    csvList += ",";

                csvList += children[i].GetComponent<ProfileController>().IdentificationProfileId;
            }
        }

        return csvList;
    }

    internal string GetFirstProfileId() {

        Toggle[] children = GetComponentsInChildren<Toggle>();
        int len = children.Length;

        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                return children[i].GetComponent<ProfileController>().IdentificationProfileId;
            }
        }

        return string.Empty;
    }

    private void DisplayIdentifiedSpeaker(string speakerId) {

        GetProfileById(speakerId).SetVisibleStatus(ProfileController.EStatus.Identified);
    }
}
