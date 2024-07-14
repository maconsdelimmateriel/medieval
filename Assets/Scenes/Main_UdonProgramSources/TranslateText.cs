
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class TranslateText : UdonSharpBehaviour
{
    private bool _isTranslated = false; // Are the strings translated? False = French; True = English

    // Different strings with their translations.
    private string[][] translations = new string[][]
    {
        new string[] { "Bonjour", "Hello" },
        new string[] { "Bienvenue", "Welcome" }
    };

    // Different UI Texts to update.
    [SerializeField] private Text[] textElements;

    public void Translate()
    {
        // Update translation state
        _isTranslated = !_isTranslated;

        // Update all text elements with the appropriate translation
        for (int i = 0; i < textElements.Length; i++)
        {
            textElements[i].text = translations[i][_isTranslated ? 1 : 0];
        }
    }
}
