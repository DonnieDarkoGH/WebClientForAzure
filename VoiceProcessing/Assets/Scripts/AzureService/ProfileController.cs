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

    [SerializeField] private Toggle     toggle;
    [SerializeField] private Image      background;
    [SerializeField] private InputField fieldName;
    [SerializeField] private Text       fieldId;

    private float timerStatus = 0.0f;

    private void Awake() {

        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();

        if (background == null)
            background = GetComponentsInChildren<Image>()[2];

        if (fieldId == null)
            fieldId = GetComponentsInChildren<Text>()[0];

        if (fieldName == null)
            fieldName = GetComponentsInChildren<InputField>()[0];
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
    }

    internal void SetIdFromJson(DataProfile dataFromJsonObject) {
        IdentificationProfileId = dataFromJsonObject.identificationProfileId;
    }


    internal void SetVisibleStatus(ProfileController.EStatus status) {

        if (status == EStatus.Identified)
            timerStatus = 5.0f;
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
