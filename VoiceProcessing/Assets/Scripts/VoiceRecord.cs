using UnityEngine;
using UnityEngine.UI;
using System.IO;
using AzureServiceManagement;

public class VoiceRecord : MonoBehaviour {

    private enum EState {None, Enrolling, Identifying, Recording, Stopped }

    public static byte[] fileBytes;

    public float Duration = 35;

    AudioSource     audioSourceRef;
    AudioListener   audioListenerRef;
    string      microName;
    string      path = @"c:\temp\MyTest.dat";
    float       recordingTimer = 0.0f;

    [SerializeField] ProfilesManager _profilesManagerRef = null;

    [SerializeField] private EState  _state = EState.None;

    //[SerializeField] bool       _isRecording    = false;
    //[SerializeField] bool       _isIdentifying  = false;
    //[SerializeField] bool       _isEnrolling    = false;
    [SerializeField] AudioClip  _voiceClip      = null;
    [SerializeField] Image      _startBtnImage  = null;
    [SerializeField] Text       _timerDisplay   = null;

    // Use this for initialization
    void Awake () {

        path = Application.dataPath + @"/Audio.dat";
        Debug.Log(path);

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

        if (_state != EState.None && _state != EState.Stopped)
        {
            if (!Microphone.IsRecording(microName))
            {
                Debug.Log("Start recording with : " + microName);
                _voiceClip = Microphone.Start(microName, true, (int)Duration, 16000);
                audioSourceRef.clip = _voiceClip;
            }
            recordingTimer      -= Time.deltaTime;
            _timerDisplay.text  = recordingTimer + " sec";
        }
        else if (Microphone.IsRecording(microName))
        {
            Debug.Log("Stop MICRO : " + microName);
            Microphone.End(microName);
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

    }

    public void CreateEnrollment() {

        _startBtnImage.color = Color.green;
        //_isRecording    = true;
        //_isEnrolling    = true;
        //_isIdentifying  = false;

        Duration       = 10.0f;
        recordingTimer = Duration;

        _state = EState.Enrolling;
    }

    public void IdentifySpeaker() {

        _startBtnImage.color = Color.green;
        //_isRecording    = true;
        //_isEnrolling    = false;
        //_isIdentifying  = true;

        Duration        = 5.0f;
        recordingTimer  = Duration;

        _state = EState.Identifying;
    }


    public void StartRecord() {

        _startBtnImage.color = Color.green;
        //_isRecording         = true;

        Duration        = 5.0f;
        recordingTimer  = Duration;

        _state = EState.Recording;
    }

    public void StopRecord() {

        //_startBtnImage.color = Color.white;
        //_isRecording         = false;

        if (_state == EState.Enrolling)
        {
            //_isEnrolling     = false;
            string profileId = _profilesManagerRef.GetFirstProfileId();

            WebClientManager.Instance.CreateEnrollment(profileId);
        }

        if (_state == EState.Identifying)
        {
            //_isIdentifying = false;
            WebClientManager.Instance.Identification();
        }

        _startBtnImage.color = Color.white;
        _state               = EState.Stopped;

    }


}
