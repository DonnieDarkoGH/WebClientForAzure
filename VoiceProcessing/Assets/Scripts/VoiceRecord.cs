using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// This class is a first try to record voice audio data and to send them to Azure WebService (Voice Recognition)
/// Most of the code below should be refactored because it results from iterative tries and researches
/// </summary>
public class VoiceRecord : MonoBehaviour {

    // Microname is more easily accessed as a static field but there's no more reason in the present state of the project
    public static string MICRONAME = string.Empty;

    // Sample rates are recording duration values (a small duration when proceeding to identification and a bigger one for enrolling)
    public static float  SAMPLE_RATE_IDENTIFY = 2f;
    public static float  SAMPLE_RATE_ENROLL   = 10f;

    // These rates can be modified in the inspector thanks to the following serialized fields and their related properties
    [SerializeField]
    [Range(1f, 10f)]
    private float _identifyDuration = 2f;

    [SerializeField]
    [Range(5f, 100f)]
    private float _enrollingDuration = 10f;

    // Processing state may be useful in general
    internal enum EState {None, Enrolling, Identifying, Recording, Stopped }
    [SerializeField] internal EState _state = EState.None;

    // Reference fields to some useful components
    [SerializeField]
    private AudioStreamer   _audioStreamer = null;
    private AudioSource     _audioSource = null;
    private AudioVisualizer _vizualizer     = null; // Caution ! This does not work to vizualise spectrum when recording (it only works when reading an audio file)

    // Timer value used for sampling audio files
    private float       _recordingTimer         = 0.0f;

    // This private value is only used for displaying total speech duration in screen
    private float       _totalSpeechDuration    = 0.0f;

    // The buffer index is used for switching between 2 indexes
    private int         _currentBufferIndex     = 0;

    // These UI components should have been managed in a dedicated class but there was not enough for doing the job correctly
    [SerializeField] Image      _startBtnImage  = null; // CAUTION ! This button is currently disable
    [SerializeField] Text       _timerDisplay   = null;
    [SerializeField] Text       _totalSpeechDisplay  = null;

    [SerializeField] SwitchButton _enrollSwitchBtn   = null;


    // This property is used for updating a static field through the inspector
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

    // This property is used for updating a static field through the inspector
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

    // Just in case we need to access the value from outside the class
    public float TotalSpeechDuration {
        get {
            return _totalSpeechDuration;
        }
    }

    // Use this for initialization
    void Awake () {

        DebugHelper.Instance.HandleDebugInfo("Buffer path : " + Application.persistentDataPath, false, true);
        Debug.Log(AudioSettings.GetConfiguration().speakerMode);

        //AudioSettings.outputSampleRate = 256;

        // Set the audio source and vizualer reference
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_vizualizer == null)
            _vizualizer = GetComponent<AudioVisualizer>();


        int minFreq = 0;
        int maxFreq = 0;

        // Find the micro name for later use, get some information about the device and display that on debug panel
        foreach(var device in Microphone.devices)
        {
            Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
            Debug.Log("Name : " + device + " / " + minFreq +" to " + maxFreq);
            MICRONAME = device;
            DebugHelper.Instance.HandleDebugInfo("Micro : " + MICRONAME, true, true);
        }

        Debug.Log(AudioSettings.outputSampleRate + ", " + AudioSettings.speakerMode);

        // Set the sampling values
        SAMPLE_RATE_IDENTIFY = _identifyDuration;
        SAMPLE_RATE_ENROLL   = _enrollingDuration;

    }

    private void Update() {

        // No need to go further if not recording
        if (_state == EState.Stopped || _state == EState.None)
            return;

        
        // Start the microphone, but only once
        if (!Microphone.IsRecording(MICRONAME))
        {
            Debug.Log("Start recording with : " + MICRONAME);
            _audioSource.clip = Microphone.Start(MICRONAME, true, (int)_recordingTimer, 16000);
            _audioSource.Play();
        }

        // Update global speech time when processing Identification
        if (_state == EState.Identifying)
        {
            _totalSpeechDuration    += Time.deltaTime;
            _totalSpeechDisplay.text = String.Format("Total Speech :\n {0:#0.0} sec.", _totalSpeechDuration);
        }

        _recordingTimer     -= Time.deltaTime;
        _timerDisplay.text  = String.Format("{0:#0.00} sec.", _recordingTimer);

        // recordingTimer value decrease from the sampling rate value to zero, then data are streamed to the server and we restart the process
        if (_recordingTimer <= 0f)
        {
            DebugHelper.Instance.HandleDebugInfo("Saving Buffer in state : " + _state, true);
            _audioStreamer.SaveBuffer(_audioSource, _currentBufferIndex, _state);

            // Switch between 2 buffers in order to properly chain speech process
            _currentBufferIndex = _currentBufferIndex == 0 ? 1 : 0;

            if (_state == EState.Identifying)
            {
                _recordingTimer = SAMPLE_RATE_IDENTIFY;
            }

            // Enrooling process is done only once, so we stop recording when it's over
            if(_state == EState.Enrolling)
            {
                _recordingTimer = 0f;
                _state          = EState.Stopped;
                _enrollSwitchBtn.Switch();
            }
        }

    }

    // This method is called by a UI button that has a toggle comportment
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

    // This one too
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


    // No longer used because the button is disabled 
    public void StartRecord() {
        Debug.Log("<b>VoiceRecord</b> StartRecord");

        _startBtnImage.color = Color.green;

        _recordingTimer      = SAMPLE_RATE_IDENTIFY;

        _state = EState.Recording;
    }

    // Explicit
    public void StopRecord() {
        Debug.Log("<b>VoiceRecord</b> StopRecord");

        Microphone.End(MICRONAME);

        _state = EState.Stopped;

    }

    // Called by the reset button
    public void ClearSpeechDuration() {
        _totalSpeechDuration = 0.0f;
        _totalSpeechDisplay.text = String.Format("Total Speech :\n {0:#0.0} sec.", _totalSpeechDuration);
    }
      
    // Called by the Exit button
    public void ExitApplication() {
        Debug.Log("<b>VoiceRecord</b> ExitApplication");

        Application.Quit();
    }

    // Not used anymore
    public void ReplayLastSample() {
        Debug.Log("<b>VoiceRecord</b> ReplayLastSample");

        DebugHelper.Instance.HandleDebugInfo("Replaying last audio sample (" + SAMPLE_RATE_IDENTIFY + " sec.)");
        _audioSource.PlayOneShot(_audioSource.clip);
    }


}
