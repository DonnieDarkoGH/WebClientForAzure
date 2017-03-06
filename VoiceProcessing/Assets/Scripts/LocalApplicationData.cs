using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalApplicationData : ScriptableObject {

    [System.Serializable]
    internal class Profile {

        [SerializeField] internal string Name = string.Empty;
        [SerializeField] internal string Id   = string.Empty;
        [SerializeField] internal float  SpeechTime = 0f;

        internal Profile(string id, string name) {
            Name = name;
            Id   = id;
        }

        internal Profile(string id) : this(id, string.Empty) {}

        internal Profile() : this(string.Empty, string.Empty) {}
    }

    [SerializeField]
    internal List<Profile> ListOfProfiles = new List<Profile>();

    internal Profile GetProfileById(string id) {
        Debug.Log("<b>LocalApplicationData</b> GetProfileById " + id);

        int len = ListOfProfiles.Count;
        for (int i = 0; i < len; i++)
        {
            if(ListOfProfiles[i].Id.CompareTo(id) == 0)
            {
                return ListOfProfiles[i];
            }
        }

        Debug.Log("No Profile has the Id " + id);
        return null;
    }

    internal Profile GetProfileByName(string name) {
        Debug.Log("<b>LocalApplicationData</b> GetProfileByName " + name);

        int len = ListOfProfiles.Count;
        for (int i = 0; i < len; i++)
        {
            if (ListOfProfiles[i].Name.CompareTo(name) == 0)
            {
                return ListOfProfiles[i];
            }
        }

        Debug.Log("No Profile has the name " + name);
        return null;
    }

    internal Profile AddNewProfile(string id, string name = "") {

        Profile newProfile = new Profile(id, name);

        ListOfProfiles.Add(newProfile);

        return newProfile;
    }

    internal bool DeleteProfile(string id) {

        int len = ListOfProfiles.Count;
        for (int i = 0; i < len; i++)
        {
            if (ListOfProfiles[i].Id.CompareTo(id) == 0)
            {
                ListOfProfiles.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    internal void ResetTimers() {

        int len = ListOfProfiles.Count;
        for (int i = 0; i < len; i++)
        {
            ListOfProfiles[i].SpeechTime = 0f;
        }
    }

    internal void SetSpeechTime(string SpeakerId, float duration, bool isAdditive = false) {

        Profile speakerProfile = GetProfileById(SpeakerId);

        if (speakerProfile != null)
        {
            if (isAdditive)
            {
                speakerProfile.SpeechTime += duration;
            }
            else
            {
                speakerProfile.SpeechTime = duration;
            }
        }
        else
        {
            Debug.LogError("Cannot find a speaker whose Id is " + SpeakerId);
        }

    }

    internal void HandleProfileRenamed(ProfileController profileController) {
        Debug.Log("<b>ProfileController</b> HandleProfileRenamed");

        Profile renamedProfile  = GetProfileById(profileController.IdentificationProfileId);

        renamedProfile.Name     = profileController.ProfileName;

    }

}
