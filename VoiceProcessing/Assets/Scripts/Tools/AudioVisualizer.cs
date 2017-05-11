using UnityEngine;


/// <summary>
/// This class is an attempt to display an audio spectrum when recording a voice, but it only works when reading an audio file with the Audio Source
/// </summary>
public class AudioVisualizer : MonoBehaviour {

    public const int SAMPLE_RATE = 256;

    [SerializeField]
    internal    bool         IsSpectrumVisible = false;

    [SerializeField]
    private     LineRenderer _lineRenderer = null;

    [SerializeField]
    private     AudioSource  _audioSource = null;

    private     float[]      _spectrum;
    private     Vector3[]    _positions;

    // Use this for initialization
    private void Awake () {

        _spectrum  = new float[SAMPLE_RATE];
        _positions = new Vector3[SAMPLE_RATE];

        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        _lineRenderer.numPositions = SAMPLE_RATE;

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        SetSpectrumPoints();

    }

    private void Update() {

        if (IsSpectrumVisible)
        {
            SetSpectrumPoints();
        }
    }

    internal void SetSpectrumPoints() {

        Vector3 position = new Vector3();

        AudioListener.GetSpectrumData(_spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < SAMPLE_RATE; i++)
        {
            position.x = (i - SAMPLE_RATE*0.5f) * 0.1f;
            position.y = _spectrum[i]*1000f;
            position.z = 0;

            _positions[i] = position;
        }

        _lineRenderer.SetPositions(_positions);

    }


}
