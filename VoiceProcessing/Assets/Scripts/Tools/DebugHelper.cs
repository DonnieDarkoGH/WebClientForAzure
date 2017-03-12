using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHelper : MonoBehaviour {

    private static DebugHelper instance;

    public  static DebugHelper Instance {
        get {
            return instance;
        }
    }

    private List<string> PersistentDebugInfo = new List<string>();

    [SerializeField] private Text _resultPanel = null;

    // Use this for initialization
    void Awake () {

        if (instance == null)
            instance = this;

        if (_resultPanel == null)
            _resultPanel = GameObject.FindGameObjectWithTag("DebugPanel").GetComponent<Text>();

        if (_resultPanel == null)
        {
            Debug.LogError("Cannot find DebugPanel object : Debug info will not be displayed on screen...");
        }

    }

    internal void HandleDebugInfo(string textToShow, bool isAdditive = true, bool isPersistent = false) {
        //Debug.Log("<b>DebugHelper</b> HandleDebugInfo : " + textToShow);

        if (_resultPanel == null)
            return;

        if (isAdditive)
        {
            _resultPanel.text += "\n" + textToShow;
        }
        else
        {
            _resultPanel.text = textToShow;
        }

        if (isPersistent)
        {
            PersistentDebugInfo.Add(textToShow);
        }
    }

    internal void ShowPersistentDebugInfo() {
        //Debug.Log("<b>DebugHelper</b> ShowPersistentDebugInfo");

        int len = PersistentDebugInfo.Count;
        for(int i = 0; i < len; i++)
        {
            HandleDebugInfo(PersistentDebugInfo[i], true, false);
        }

    }

    private void OnDestroy() {

    }
}
