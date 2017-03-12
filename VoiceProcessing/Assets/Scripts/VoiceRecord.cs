using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class VoiceRecord : MonoBehaviour {

    public static float  SAMPLE_RATE_IDENTIFY = 2f;
    public static float  SAMPLE_RATE_ENROLL   = 10f;
    public static string MICRONAME            = string.Empty;

    internal enum EState {None, Enrolling, Identifying, Recording, Stopped }

    private AudioSource _audioSourceRef         = null;

    private float       _recordingTimer         = 0.0f;
    private float       _totalSpeechDuration    = 0.0f;
    private int         _currentBufferIndex     = 0;

    [SerializeField] AudioStreamer _audioStreamerRef = null;

    [SerializeField] internal EState _state = EState.None;

    [SerializeField] Image      _startBtnImage  = null;
    [SerializeField] Text       _timerDisplay   = null;
    [SerializeField] Text       _totalSpeechDisplay  = null;

    [SerializeField] SwitchButton _enrollSwitchBtn   = null;

    [SerializeField]
    [Range(1f,10f)]   private float _identifyDuration  = 2f;

    [SerializeField]
    [Range(5f, 100f)] private float _enrollingDuration = 10f;

    [ExecuteInEditMode]
    public float IdentifyDuration {
        get {
            return _identifyDuration;
        }

        set {
            SAMPLE_RATE_IDENTIFY = value;
            _identifyDuration = value;
        }
    }

    [ExecuteInEditMode]
    public float EnrollingDuration {
        get {
            return _enrollingDuration;
        }

        set {
            SAMPLE_RATE_ENROLL = value;
            _enrollingDuration = value;
        }
    }

    // Use this for initialization
    void Awake () {

        //path = Application.dataPath + @"/Audio.dat";
        //Debug.Log(path);
        DebugHelper.Instance.HandleDebugInfo("Buffer path : " + Application.persistentDataPath, false, true);
        Debug.Log(AudioSettings.GetConfiguration().speakerMode);

        _audioSourceRef = GetComponent<AudioSource>();

        int minFreq = 0;
        int maxFreq = 0;

        foreach(var device in Microphone.devices)
        {
            Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
            Debug.Log("Name : " + device + " / " + minFreq +" to " + maxFreq);
            MICRONAME = device;
            DebugHelper.Instance.HandleDebugInfo("Micro : " + MICRONAME, true, true);
        }

        Debug.Log(AudioSettings.outputSampleRate + ", " + AudioSettings.speakerMode);

        SAMPLE_RATE_IDENTIFY = _identifyDuration;
        SAMPLE_RATE_ENROLL   = _enrollingDuration;
    }

    private void Update() {

        if (_state == EState.Stopped || _state == EState.None)
            return;

        if (!Microphone.IsRecording(MICRONAME))
        {
            Debug.Log("Start recording with : " + MICRONAME);
            _audioSourceRef.clip = Microphone.Start(MICRONAME, true, (int)_recordingTimer, 16000);
            _audioSourceRef.Play();
        }

        if (_state == EState.Identifying)
        {
            _totalSpeechDuration    += Time.deltaTime;
            _totalSpeechDisplay.text = String.Format("Total Speech :\n {0:#0.0} sec.", _totalSpeechDuration);
        }

        _recordingTimer     -= Time.deltaTime;
        _timerDisplay.text  = String.Format("{0:#0.00} sec.", _recordingTimer);

        if (_recordingTimer <= 0f)
        {
            DebugHelper.Instance.HandleDebugInfo("Saving Buffer in state : " + _state, true);
            _audioStreamerRef.SaveBuffer(_audioSourceRef, _currentBufferIndex, _state);

            _currentBufferIndex = _currentBufferIndex == 0 ? 1 : 0;

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

    /*private void SaveFile(AudioClip clip) {
        Debug.Log("<b>VoiceRecord</b> SaveFile " + clip.name);
        Debug.Log(File.Exists(_path));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(clip, UnityEditor.AssetDatabase.GetAssetPath(this) + @"Assets/buffer.wav");
        UnityEditor.AssetDatabase.SaveAssets();
#endif


        if (!File.Exists(_path))
        {
            // Create a file to write to.
            using (BinaryWriter bw = new BinaryWriter(File.Create(_path)))
            {
                bw.Write(clip);
            }
        }
        else
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(_path, FileMode.Create)))
            {

                bw.Write(clip);
            }
        }
    }*/

    //internal static void StreamAudio() {
    //    Debug.Log("<b>VoiceRecord</b> StreamAudio");

    //    string filePath = Application.dataPath + @"/Resources/buffer.wav";

    //    FileStream stream = File.OpenRead(filePath);
    //    //Debug.Log(stream.Length);
    //    fileBytes = new byte[stream.Length];

    //    stream.Read(fileBytes, 0, fileBytes.Length);
    //    stream.Close();
    //}

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

        Microphone.End(MICRONAME);

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

    public void ReplayLastSample() {
        Debug.Log("<b>VoiceRecord</b> ReplayLastSample");

        DebugHelper.Instance.HandleDebugInfo("Replaying last audio sample (" + SAMPLE_RATE_IDENTIFY + " sec.)");
        _audioSourceRef.PlayOneShot(_audioSourceRef.clip);
    }

}
