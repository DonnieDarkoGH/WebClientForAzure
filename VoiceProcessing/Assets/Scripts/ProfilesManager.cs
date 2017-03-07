using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

public class ProfilesManager : MonoBehaviour {

    [SerializeField] private  ProfileController       _profileControllerPrefab = null;

    [SerializeField] private  InputField              _inputFieldRef           = null;

    [SerializeField] private  GameObject              _displayRatioPanel      = null;

    private string newProfileId   = string.Empty;


    // Use this for initialization
    void Awake () {

        WebClientManager.OnProfileListModified  += PopulateProfiles;
        WebClientManager.OnProfileCreated       += CreateNewProfile;
        WebClientManager.OnIdentificationDone   += DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed      += RenameProfile;
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
        Debug.Log("<b>ProfilesManager</b> PopulateProfiles with Json : " + json );

        ClearChildren();

        DataProfileArray profileArray = DataProfileArray.CreateFromJSON(json);
        //Debug.Log(profileArray.ToString());

        int len = profileArray.DataProfiles.Length;
        string id;
        string name;

        for (int i = 0; i < len; i++)
        {
            var profileController = Instantiate(_profileControllerPrefab, transform);
            profileController.Init(profileArray.DataProfiles[i]);

            id   = profileController.IdentificationProfileId;

            name = GetProfileName(id);

            profileController.SetName(name);
        }
    }

    private void CreateNewProfile(string id) {
        Debug.Log("<b>ServiceProfilesManager</b> CreateNewProfile with Id : " + id);

        newProfileId = id;

        if(PlayerPrefs.HasKey(id))
        {
            Debug.Log("Id " + id + " already exists ! Profile will not be created.");
        }
        else
        {
            PlayerPrefs.SetString(id, "");
            PlayerPrefs.Save();
        }
        
    }

    internal ProfileController GetProfileById(string id) {
        Debug.Log("<b>ProfilesManager</b> GetProfileById " + id);

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

    /*internal ProfileController GetProfileByName(string name) {

        ProfileController[] children = GetComponentsInChildren<ProfileController>();
        int len = children.Length;

        for (int i = 0; i < len; i++)
        {
            if (children[i].ProfileName == name)
            {
                return children[i];
            }
        }

        Debug.LogError("No profile correspond to " + name);

        return null;
    }*/

    public void GetProfile() {
        Debug.Log("<b>ProfilesManager</b> GetProfile");

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
        Debug.Log("<b>ProfilesManager</b> DeleteProfiles");

        Toggle[] children = GetComponentsInChildren<Toggle>();

        string idToDelete = string.Empty;

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                idToDelete = children[i].GetComponent<ProfileController>().IdentificationProfileId;
                WebClientManager.Instance.DeleteProfile(idToDelete);

                if (PlayerPrefs.HasKey(idToDelete))
                {
                    PlayerPrefs.DeleteKey(idToDelete);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    public void DeleteAllNames() {
        Debug.Log("<b>ProfilesManager</b> DeleteAllNames");

        PlayerPrefs.DeleteAll();
    }

    public void ResetAllTimers() {

        ProfileController[] children = GetComponentsInChildren<ProfileController>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            children[i].ResetSpeechTimer();
        }
    }

    public void RenameLastProfile() {
        Debug.Log("<b>ProfilesManager</b> RenameLastProfile");

        var lastCreatedProfile = GetProfileById(newProfileId);

        lastCreatedProfile.ProfileName = _inputFieldRef.text;

        RenameProfile(lastCreatedProfile);
    }

    internal string GetCSVProfiles() {
        Debug.Log("<b>ProfilesManager</b> GetCSVProfiles");

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

    public void DisplaySpeechRatio() {

        _displayRatioPanel.SetActive(!_displayRatioPanel.activeSelf);

        string result = "SPEECH RATIO :\n";
        ProfileController[] children = GetComponentsInChildren<ProfileController>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            result += System.String.Format("\n- {0} : {1:0.0%} \n", children[i].ProfileName, GetSpeechRatio(children[i]));
        }


        _displayRatioPanel.GetComponentInChildren<Text>().text = result;
    }

    internal string GetFirstProfileId() {
        Debug.Log("<b>ProfilesManager</b> GetFirstProfileId");

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
        Debug.Log("<b>ProfilesManager</b> DisplayIdentifiedSpeaker : " + speakerId);

        ProfileController profile = GetProfileById(speakerId);

        if (profile != null)
        {
            profile.SetVisibleStatus(ProfileController.EStatus.Identified);
        }
        else
        {
            Debug.Log("Cannot find any Speaker with Id " + speakerId);
        }
    }

    private string GetProfileName(string id) {
        Debug.Log("<b>ProfilesManager</b> GetProfileName : " + id);

        string name = string.Empty;

        if (PlayerPrefs.HasKey(id))
        {
            name = PlayerPrefs.GetString(id);
        }

        return name;
    }

    private void RenameProfile(ProfileController profileController) {
        Debug.Log("<b>ProfilesManager</b> RenameProfile : " + profileController.ProfileName);

        if (profileController == null)
        {
            Debug.LogError("Cannot rename a null ProfileController !");
        }

        string idToRename = profileController.IdentificationProfileId;
        Debug.Log(idToRename);

        PlayerPrefs.SetString(idToRename, profileController.ProfileName);
        PlayerPrefs.Save();

    }

    private void OnDisable() {

        WebClientManager.OnProfileListModified  -= PopulateProfiles;
        WebClientManager.OnProfileCreated       -= CreateNewProfile;
        WebClientManager.OnIdentificationDone   -= DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed      -= RenameProfile;
    }

    private void OnDestroy() {

        WebClientManager.OnProfileListModified -= PopulateProfiles;
        WebClientManager.OnProfileCreated     -= CreateNewProfile;
        WebClientManager.OnIdentificationDone -= DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed    -= RenameProfile;
    }

    private float GetSpeechRatio(ProfileController profile) {

        float speechTime = profile.TotalSpeechDuration;
        float totalSpeechTime = 0f;

        ProfileController[] children = GetComponentsInChildren<ProfileController>();

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            totalSpeechTime += children[i].TotalSpeechDuration;
        }

        if(totalSpeechTime <= Mathf.Epsilon)
        {
            return 0f;
        }

        return speechTime / totalSpeechTime;
    }
}
