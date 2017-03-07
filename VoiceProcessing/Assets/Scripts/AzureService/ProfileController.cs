using System;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

[System.Serializable]
public class ProfileController : MonoBehaviour {

    public enum EStatus { None, Identified}

    internal static System.Action<ProfileController> OnProfileRenamed;

    [SerializeField]
    internal string IdentificationProfileId = string.Empty;

    [SerializeField]
    internal string ProfileName             = string.Empty;

    internal float  TotalSpeechDuration = 0.0f;

    [SerializeField] private Toggle     toggle;
    [SerializeField] private Image      background;
    [SerializeField] private InputField fieldName;
    [SerializeField] private Text       fieldId;
    [SerializeField] private Text       fieldDuration;

    private float timerStatus         = 0.0f;

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

    internal void Init(DataProfile dataFromJsonObject) {
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


    internal void SetVisibleStatus(ProfileController.EStatus status) {

        TotalSpeechDuration += VoiceRecord.SAMPLE_RATE_IDENTIFY;

        fieldDuration.text = String.Format("{0:#0} sec.", TotalSpeechDuration);

        if (status == EStatus.Identified)
            timerStatus = 5.0f;
    }

    internal void ResetSpeechTimer() {
        TotalSpeechDuration = 0;
        fieldDuration.text = String.Format("{0:#0} sec.", TotalSpeechDuration);
    }

    internal void SetName(string name) {
        Debug.Log("<b>ProfileController</b> SetName to " + name);

        ProfileName    = name;
        fieldName.text = name;
    }

    public void RenameProfile() {
        Debug.Log("<b>ProfileController</b> RenameProfile to " + fieldName.text);

        ProfileName = fieldName.text;
        OnProfileRenamed(this);
    }

}
