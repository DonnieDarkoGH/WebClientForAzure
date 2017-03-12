﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStreamer : MonoBehaviour {

    internal static System.Action<byte[], VoiceRecord.EState> OnBufferSaved;

    public Queue<byte[]> AudioFiles = new Queue<byte[]>();

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
