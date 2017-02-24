using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoiceRecord : MonoBehaviour {

    AudioSource     audioSourceRef;
    AudioListener   audioListenerRef;
    string      microName;
    string      path = @"c:\temp\MyTest.dat";

    [SerializeField] bool       isRecording = false;
    [SerializeField] AudioClip  VoiceClip   = null;
    [SerializeField]
    Object testObj = null;

    // Use this for initialization
    void Awake () {

        path = Application.dataPath + @"/Audio.dat";
        Debug.Log(path);

#if UNITY_EDITOR
        Debug.Log(UnityEditor.AssetDatabase.GetAssetPath(testObj));
#endif

        audioListenerRef = GetComponent<AudioListener>();
        Debug.Log(AudioSettings.GetConfiguration().speakerMode);


        audioSourceRef = GetComponent<AudioSource>();

        int minFreq = 0;
        int maxFreq = 0;

        foreach(var device in Microphone.devices)
        {
            Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
            Debug.Log("Name : " + device + " / " + minFreq +" to " + maxFreq);
            microName = device;
        }

        Debug.Log(AudioSettings.outputSampleRate + ", " + AudioSettings.speakerMode);


    }

    private void Update() {
        isRecording = Microphone.IsRecording(microName);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!Microphone.IsRecording(microName))
            {
                VoiceClip = Microphone.Start(microName, true, 5, 16000);
                audioSourceRef.clip = VoiceClip;

            }
            else
            {
                Microphone.End(microName);
                //SaveFile(audioSourceRef.clip);
                SavWav.Save("Buffer", audioSourceRef.clip);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            audioSourceRef.Play();
        }
    }

    private void SaveFile(AudioClip clip) {
        Debug.Log("<b>VoiceRecord</b> SaveFile " + clip.name);
        Debug.Log(File.Exists(path));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(clip, UnityEditor.AssetDatabase.GetAssetPath(this) + @"Assets/buffer.wav");
#endif
        UnityEditor.AssetDatabase.SaveAssets();

        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (BinaryWriter bw = new BinaryWriter(File.Create(path)))
            {
                bw.Write(clip);
            }
        }
        else
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {

                bw.Write(clip);
            }
        }
    }


}
