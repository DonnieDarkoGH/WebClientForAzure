using System.Collections.Generic;
using UnityEngine;

// CAUTION : As long as ScriptableObject are not saved between sessions, this class is no longer used
// and the storing in PlayerPrefs is now done in ProfileController

/// <summary>
/// This class is a helper to manage profile data that are not saved on the server side
/// In fact, this is just a mapping between profile Id stored on the server and the real name of users
/// </summary>
public class LocalApplicationData : ScriptableObject {

    // Base class for defining a Profile
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

    // Dynamic list of profiles : It is populated when parsing the JSON result of a GetAllProfiles request
    [SerializeField]
    internal List<Profile> ListOfProfiles = new List<Profile>();

    // Getter
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

    // Getter
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

    // Add a profile and save it in PlayerPrefs
    internal Profile AddNewProfile(string id, string name = "") {

        Profile newProfile = new Profile(id, name);

        ListOfProfiles.Add(newProfile);

        PlayerPrefs.SetString(id, name);
        PlayerPrefs.Save();

        return newProfile;
    }

    // Delete a profile and remove it from PlayerPrefs
    internal bool DeleteProfile(string id) {

        if (PlayerPrefs.HasKey(id))
        {
            PlayerPrefs.DeleteKey(id);
            PlayerPrefs.Save();
        }

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

    // ProfileController is a script attached to each UI component that display data for a single user
    // When changing the name on screen, it is important to update PlayerPrefs
    internal void HandleProfileRenamed(ProfileController profileController) {
        Debug.Log("<b>ProfileController</b> HandleProfileRenamed");

        string id               = profileController.IdentificationProfileId;

        Profile renamedProfile  = GetProfileById(id);

        if(renamedProfile != null)
            renamedProfile.Name     = profileController.ProfileName;

        if (PlayerPrefs.HasKey(id))
        {
            PlayerPrefs.SetString(id, profileController.ProfileName);
            PlayerPrefs.Save();
        }

    }

}
