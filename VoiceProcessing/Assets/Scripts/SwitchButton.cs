using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour {

    public Color InactiveColor = Color.white;
    public Color ActiveColor   = Color.yellow;

    public string BaseText   = string.Empty;
    public string ActiveText = string.Empty;

    private Image _background;
    private Text  _label;
    private bool  _isOn;

    // Use this for initialization
    void Awake () {

        _background = GetComponent<Image>();
        _label      = GetComponentInChildren<Text>();

        InactiveColor = _background.color;
    }
	
    public void Switch() {

        if (_isOn)
        {
            _background.color = InactiveColor;
            _label.text       = BaseText;
            _isOn = false;
        }
        else
        {
            _background.color = ActiveColor;
            _label.text = ActiveText;
            _isOn = true;
        }
    }
    

}
