using System;
using UnityEngine;

namespace AzureServiceManagement {

    public class ProfileController : MonoBehaviour {

        [SerializeField]
        internal string IdentificationProfileId = string.Empty;

        [SerializeField]
        internal string ProfileName = string.Empty;

        internal void SetIdFromJson(DataProfile dataFromJsonObject) {
            IdentificationProfileId = dataFromJsonObject.identificationProfileId;
        }
    }
}
