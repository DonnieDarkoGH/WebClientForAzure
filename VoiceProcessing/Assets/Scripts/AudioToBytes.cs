using System.IO;
using UnityEngine;

public class AudioToBytes : MonoBehaviour {

    public string BytesData = string.Empty;
    public byte[] bytes;

    public void EncodeFile() {

        bytes = File.ReadAllBytes(@"D:\VoiceProcessing\WebClientForAzure\VoiceProcessing\Assets\buffer.wav");
        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E))
            EncodeFile();
    }

}
