using UnityEngine;
using UnityEngine.UI;
using System.IO;
using AzureServiceManagement;

public class VoiceRecord : MonoBehaviour {

    public static byte[] fileBytes;

    public float Duration = 35;

    AudioSource     audioSourceRef;
    AudioListener   audioListenerRef;
    string      microName;
    string      path = @"c:\temp\MyTest.dat";
    float       recordingTimer = 0.0f;

    [SerializeField] ProfilesManager profilesManagerRef = null;

    [SerializeField] bool       isRecording = false;
    [SerializeField] bool       isEnrolling = false;
    [SerializeField] AudioClip  VoiceClip   = null;
    [SerializeField] Image      StartBtnImage = null;
    [SerializeField] Text       TimerDisplay = null;

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

        if(recordingTimer <= 0)
        {
            StopRecord();
        }

        if (isRecording)
        {
            if (!Microphone.IsRecording(microName))
            {
                Debug.Log("Start recording with : " + microName);
                VoiceClip = Microphone.Start(microName, true, (int)Duration, 16000);
                audioSourceRef.clip = VoiceClip;
            }
            recordingTimer -= Time.deltaTime;
            TimerDisplay.text = recordingTimer + " sec";
        }
        else if (Microphone.IsRecording(microName))
        {
            Debug.Log("Stop MICRO : " + microName);
            Microphone.End(microName);
            //SaveFile(audioSourceRef.clip);
            SavWav.Save("Resources/Buffer", audioSourceRef.clip);

            StreamAudio();
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
        UnityEditor.AssetDatabase.SaveAssets();
#endif


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

    internal static void StreamAudio() {
        Debug.Log("<b>VoiceRecord</b> StreamAudio");

        string filePath = Application.dataPath + @"/Resources/buffer.wav";

        FileStream stream = File.OpenRead(filePath);
        Debug.Log(stream.Length);
        fileBytes = new byte[stream.Length];

        stream.Read(fileBytes, 0, fileBytes.Length);
        stream.Close();

        //Debug.Log(fileBytes.Length);

        //for(int i = 0; i <100; i++)
        //{
        //    Debug.Log(fileBytes[i].ToString() + "\n");
        //}
        //Debug.Log(System.Text.Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length));

    }

    public void CreateEnrollment() {

        StartBtnImage.color = Color.green;
        isRecording    = true;
        isEnrolling    = true;

        Duration = 35.0f;
        recordingTimer = Duration;
    }


    public void StartRecord() {

        StartBtnImage.color = Color.green;
        isRecording = true;
        Duration = 5.0f;
        recordingTimer = Duration;
    }

    public void StopRecord() {

        StartBtnImage.color = Color.white;
        isRecording = false;
        if (isEnrolling)
        {
            isEnrolling = false;
            string profileId = profilesManagerRef.GetFirstProfileId();

            WebClientManager.Instance.CreateEnrollment(profileId);
        }
        
    }


}
