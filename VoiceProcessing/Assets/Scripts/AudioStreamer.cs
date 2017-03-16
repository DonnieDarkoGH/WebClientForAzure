using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class AudioStreamer : MonoBehaviour {

    internal static System.Action<byte[], VoiceRecord.EState> OnBufferSaved;

    public Queue<byte[]> AudioFiles = new Queue<byte[]>();

    private void Update() {

        float[] spectrum = new float[256];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            //Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }

    internal void SaveBuffer(AudioSource source, int index, VoiceRecord.EState recordState) {
        Debug.Log("<b>AudioStreamer</b> SaveBuffer : " + index);
        DebugHelper.Instance.HandleDebugInfo("Buffer index : " + index, true);

        SavWav.Save("buffer" + index, source.clip);
        //SavWav.Save("Resources/buffer" + index, source.clip);

        string filePath = Application.persistentDataPath + @"/buffer" + index + ".wav";
        DebugHelper.Instance.HandleDebugInfo("filePath : " + filePath, true);
        //string filePath = Application.dataPath + @"/Resources/buffer" + index + ".wav";

        FileStream stream       = File.OpenRead(filePath);

        byte[]      fileBytes   = new byte[stream.Length];

        stream.Read(fileBytes, 0, fileBytes.Length);
        stream.Close();

        OnBufferSaved(fileBytes, recordState);
    }


}
