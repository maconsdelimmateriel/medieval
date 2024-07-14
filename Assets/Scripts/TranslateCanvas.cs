using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslateCanvas : MonoBehaviour
{
    private bool _isTranslated = false;
    private string[] _hello = new string[] { "Bonjour", "Hello" };
    private string _helloTranslation;

    [SerializeField]
    private Text _helloText;

    public void Translate()
    {
        Debug.Log("yes");
        if (!_isTranslated)
        {
            _helloTranslation = _hello[1];
            _isTranslated = true;
        }
        else
        {
            _helloTranslation = _hello[0];
            _isTranslated = false;
        }

        _helloText.text = _helloTranslation;
    }
}
