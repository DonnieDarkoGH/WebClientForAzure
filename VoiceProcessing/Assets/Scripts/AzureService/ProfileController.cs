using System;
using UnityEngine;
using UnityEngine.UI;
using AzureServiceManagement;

[System.Serializable]
public class ProfileController : MonoBehaviour {

    public enum EStatus { None, Identified}

    [SerializeField]
    internal string IdentificationProfileId = string.Empty;

    [SerializeField]
    internal string ProfileName             = string.Empty;

    [SerializeField] private Toggle toggle;
    [SerializeField] private Image  background;
    [SerializeField] private Text   profileName;
    [SerializeField] private Text   profileId;

    private float timerStatus = 0.0f;

    private void Awake() {

        if (toggle == null)
            toggle = GetComponentInChildren<Toggle>();

        if (background == null)
            background = GetComponentsInChildren<Image>()[2];

        if (profileId == null)
            profileId = GetComponentsInChildren<Text>()[0];

        if (profileName == null)
            profileName = GetComponentsInChildren<Text>()[2];
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
        profileId.text          = IdentificationProfileId;

    }

    internal void SetIdFromJson(DataProfile dataFromJsonObject) {
        IdentificationProfileId = dataFromJsonObject.identificationProfileId;
    }

    internal void SetVisibleStatus(ProfileController.EStatus status) {

        if (status == EStatus.Identified)
            timerStatus = 5.0f;

    }
}
