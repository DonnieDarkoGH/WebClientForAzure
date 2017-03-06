using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

public class ProfilesManager : MonoBehaviour {

    [SerializeField] private  ProfileController       profileControllerPrefab = null;

    [SerializeField] private  InputField              inputFieldRef           = null;

    [SerializeField] private  LocalApplicationData    _localApplicationData   = null;

    private string newProfileName = string.Empty;
    private string newProfileId   = string.Empty;


    // Use this for initialization
    void Awake () {

        WebClientManager.OnProfileListModified  += PopulateProfiles;
        WebClientManager.OnProfileCreated       += CreateNewProfile;
        WebClientManager.OnIdentificationDone   += DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed      += _localApplicationData.HandleProfileRenamed;

        _localApplicationData.ResetTimers();
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
            var profileController = Instantiate(profileControllerPrefab, transform);
            profileController.Init(profileArray.DataProfiles[i]);

            id   = profileController.IdentificationProfileId;

            name = _localApplicationData.GetProfileById(id).Name;
            profileController.SetName(name);
        }
    }

    private void CreateNewProfile(string id) {
        Debug.Log("<b>ServiceProfilesManager</b> CreateNewProfile with Id : " + id);

        newProfileId = id;

        var newDataProfile        =  _localApplicationData.AddNewProfile(id);
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
    }

    internal void DisplayNewNameInputField(bool isVisible) {
        inputFieldRef.gameObject.SetActive(isVisible);
    }

    public void RenameLastProfile() {

        var newDataProfile  = _localApplicationData.GetProfileById(newProfileId);
        newDataProfile.Name = inputFieldRef.text;

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

        string idToDelete = string.Empty;

        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].isOn)
            {
                idToDelete = children[i].GetComponent<ProfileController>().IdentificationProfileId;
                WebClientManager.Instance.DeleteProfile(idToDelete);

                _localApplicationData.DeleteProfile(idToDelete);
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

    private void RenameProfile(ProfileController profileController) {
        Debug.Log("<b>ProfilesManager</b> RenameProfile : " + profileController.ProfileName);

        var renamedProfile  = _localApplicationData.GetProfileById(profileController.IdentificationProfileId);

        renamedProfile.Name = profileController.ProfileName;

    }

    private void OnDisable() {

        WebClientManager.OnProfileListModified  -= PopulateProfiles;
        WebClientManager.OnProfileCreated       -= CreateNewProfile;
        WebClientManager.OnIdentificationDone   -= DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed      -= _localApplicationData.HandleProfileRenamed;
    }

    private void OnDestroy() {

        WebClientManager.OnProfileListModified -= PopulateProfiles;
        WebClientManager.OnProfileCreated     -= CreateNewProfile;
        WebClientManager.OnIdentificationDone -= DisplayIdentifiedSpeaker;

        ProfileController.OnProfileRenamed    -= _localApplicationData.HandleProfileRenamed;
    }
}
