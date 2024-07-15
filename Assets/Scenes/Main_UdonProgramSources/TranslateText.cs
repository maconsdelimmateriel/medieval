
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class TranslateText : UdonSharpBehaviour
{
    private bool _isTranslated = false; // Are the strings translated? False = French; True = English

    // Different strings with their translations.
    private string[][] translations = new string[][]
    {
        new string[] { "Translate to English", "Traduire en Français" },
        new string[] { "En construction", "Work in progress" },
        new string[] { "Dernière modification : 11/07/2011", "Last update: 2024/07/11" },
        new string[] { "Les arcs et arbalètes s'utilisent avec une seule main : attrapez les et tirez avec le même contrôlleur.", "The bows and arrows are used with both hands: grab them and shoot with the same controller." },
        new string[] { "A propos de l'association et de ses membres WIP", "About the association and its members WIP" },
        new string[] { "Crédit des assers WIP", "Assets credits WIP" }
    };

    // Different UI Texts to update.
    [SerializeField] private TextMeshPro[] textElements;

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
