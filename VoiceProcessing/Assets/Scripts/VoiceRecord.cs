using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class VoiceRecord : MonoBehaviour {

    internal enum EState {None, Enrolling, Identifying, Recording, Stopped }

    public static byte[] fileBytes;

    AudioSource     _audioSourceRef;
    string          microName;
    string          path = @"c:\temp\MyTest.dat";
    float           _recordingTimer = 0.0f;
    float           _totalSpeechDuration = 0.0f;

    [SerializeField] AudioStreamer   _audioStreamerRef   = null;

    [SerializeField] internal EState _state = EState.None;

    [SerializeField] AudioClip  _voiceClip      = null;
    [SerializeField] Image      _startBtnImage  = null;
    [SerializeField] Text       _timerDisplay   = null;
    [SerializeField] Text       _totalSpeechDisplay = null;

    [SerializeField] SwitchButton _enrollSwitchBtn   = null;

    private int currentBufferIndex = 0;

    public static float SAMPLE_RATE_IDENTIFY = 2f;
    public static float SAMPLE_RATE_ENROLL   = 10f;

    // Use this for initialization
    void Awake () {

        //path = Application.dataPath + @"/Audio.dat";
        //Debug.Log(path);

        Debug.Log(AudioSettings.GetConfiguration().speakerMode);

        _audioSourceRef = GetComponent<AudioSource>();

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

        if (_state == EState.Stopped || _state == EState.None)
            return;

        if (!Microphone.IsRecording(microName))
        {
            Debug.Log("Start recording with : " + microName);
            _voiceClip          = Microphone.Start(microName, true, (int)_recordingTimer, 16000);
            _audioSourceRef.clip = _voiceClip;
        }

        _recordingTimer     -= Time.deltaTime;
        if (_state == EState.Identifying)
        {
            _totalSpeechDuration += Time.deltaTime;
            _totalSpeechDisplay.text = String.Format("Total Speech :\n {0:#0.0} sec.", _totalSpeechDuration);
        }

        _timerDisplay.text  = String.Format("{0:#0.00} sec.", _recordingTimer);

        if (_recordingTimer <= 0f)
        {
            _audioStreamerRef.SaveBuffer(_audioSourceRef, currentBufferIndex, _state);
            currentBufferIndex = currentBufferIndex == 0 ? 1 : 0;

            if (_state == EState.Identifying)
            {
                _recordingTimer = SAMPLE_RATE_IDENTIFY;
            }

            if(_state == EState.Enrolling)
            {
                _recordingTimer = 0f;
                _state          = EState.Stopped;
                _enrollSwitchBtn.Switch();
            }
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
        //Debug.Log(stream.Length);
        fileBytes = new byte[stream.Length];

        stream.Read(fileBytes, 0, fileBytes.Length);
        stream.Close();
    }

    public void CreateEnrollment() {
        Debug.Log("<b>VoiceRecord</b> CreateEnrollment");

        if(_state == EState.Enrolling)
        {
            StopRecord();
        }
        else
        {
            _recordingTimer = SAMPLE_RATE_ENROLL;
            _state = EState.Enrolling;
        }

    }

    public void IdentifySpeaker() {
        Debug.Log("<b>VoiceRecord</b> IdentifySpeaker");

        if (_state == EState.Identifying)
        {
            StopRecord();
        }
        else
        {
            _recordingTimer = SAMPLE_RATE_IDENTIFY;
            _state = EState.Identifying;
        }

    }


    public void StartRecord() {
        Debug.Log("<b>VoiceRecord</b> StartRecord");

        _startBtnImage.color = Color.green;

        _recordingTimer      = SAMPLE_RATE_IDENTIFY;

        _state = EState.Recording;
    }

    public void StopRecord() {
        Debug.Log("<b>VoiceRecord</b> StopRecord");

        Microphone.End(microName);

        _state = EState.Stopped;

    }

    public void ClearSpeechDuration() {
        _totalSpeechDuration = 0.0f;
        _totalSpeechDisplay.text = String.Format("Total Speech :\n {0:#0.0} sec.", _totalSpeechDuration);
    }

    //public void StopRecord() {
    //    Debug.Log("<b>VoiceRecord</b> StopRecord");

    //    if (_state == EState.Enrolling)
    //    {
    //        string profileId = _profilesManagerRef.GetFirstProfileId();

    //        WebClientManager.Instance.CreateEnrollment(profileId);
    //    }

    //    if (_state == EState.Identifying)
    //    {
    //        WebClientManager.Instance.Identification();
    //    }

    //    _startBtnImage.color = Color.white;
    //    _state               = EState.Stopped;

    //}

    public void ExitApplication() {
        Debug.Log("<b>VoiceRecord</b> ExitApplication");

        Application.Quit();
    }

}
