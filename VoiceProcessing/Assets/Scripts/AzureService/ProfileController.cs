using System;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

/// <summary>
/// This class manages the UI fields that display the Profiles data (name, Id, changing color when profile is recognized...)
/// </summary>
[System.Serializable]
public class ProfileController : MonoBehaviour {

    public enum EStatus { None, Identified}

    // Send an event when named is changed in order to update PlayerPrefs
    internal static System.Action<ProfileController> OnProfileRenamed;

    // The GUID used by Azure service
    [SerializeField]
    internal string IdentificationProfileId = string.Empty;

    // The real name entered by the user
    [SerializeField]
    internal string ProfileName             = string.Empty;

    // Speech duration for THIS user
    private float  _totalSpeechDuration = 0.0f;

    public float TotalSpeechDuration {
        get {
            return _totalSpeechDuration;
        }
    }

    // UI components used for displaying info
    [SerializeField] private Toggle     toggle;
    [SerializeField] private Image      background;
    [SerializeField] private InputField fieldName;
    [SerializeField] private Text       fieldId;
    [SerializeField] private Text       fieldDuration;

    // Timer used for turning on the green color of the text field during a few seconds
    private float timerStatus = 0.0f;

    // USe this for initialization
    private void Awake() {

        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();

        if (background == null)
            background = GetComponentsInChildren<Image>()[2];

        if (fieldId == null)
            fieldId = GetComponentsInChildren<Text>()[0];

        if (fieldName == null)
            fieldName = GetComponentsInChildren<InputField>()[0];

        if (fieldDuration == null)
            fieldDuration = GetComponentsInChildren<Text>()[3];
    }

    // Only used for managing green color when the Profile is recognize during the idenfication process
    private void Update() {

        if (timerStatus > 0.0f)
        {
            background.color = Color.green;
            timerStatus     -= Time.deltaTime;
        }
        else
        {
            timerStatus = 0.0f;
            background.color = Color.white;
        }
    }

    // Set the data by parsing the JSON values returned by a request to the server
    internal void Init(DataProfile dataFromJsonObject) {

        if (dataFromJsonObject == null)
            dataFromJsonObject = new DataProfile();

        IdentificationProfileId = dataFromJsonObject.identificationProfileId;
        fieldId.text            = IdentificationProfileId;

        if (PlayerPrefs.HasKey(IdentificationProfileId))
        {
            SetName(PlayerPrefs.GetString(IdentificationProfileId));
        }
    }

    internal void SetIdFromJson(DataProfile dataFromJsonObject) {
        IdentificationProfileId = dataFromJsonObject.identificationProfileId;
    }

    // Update the speech duration when the profile is identified and turn the color of the text field to green during 5 seconds
    internal void SetVisibleStatus(ProfileController.EStatus status) {

        _totalSpeechDuration += VoiceRecord.SAMPLE_RATE_IDENTIFY;

        fieldDuration.text = String.Format("{0:#0} sec.", TotalSpeechDuration);

        if (status == EStatus.Identified)
            timerStatus = 5.0f;
    }

    // Reset all values
    internal void ResetSpeechTimer() {
        _totalSpeechDuration = 0;
        fieldDuration.text   = String.Format("{0:#0} sec.", TotalSpeechDuration);
    }

    // Change the real user name
    internal void SetName(string name) {
        Debug.Log("<b>ProfileController</b> SetName to " + name);

        ProfileName    = name;
        fieldName.text = name;
    }

    // Called by the InputField when Edit is done
    public void RenameProfile() {
        Debug.Log("<b>ProfileController</b> RenameProfile to " + fieldName.text);

        ProfileName = fieldName.text;
        OnProfileRenamed(this);
    }

}
