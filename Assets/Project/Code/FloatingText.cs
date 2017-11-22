using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {
    private GUIContent _content;
    private static readonly GUISkin skin = Resources.Load<GUISkin>("GameSkin");
    private IFloatingTextPositioner _positioner;
    public GUIStyle Style { get; set; }

    public static void Show(string text, string style, IFloatingTextPositioner positioner)
    {
        var go = new GameObject("FloatingText");
        var floatingText = go.AddComponent<FloatingText>();
        floatingText._content = new GUIContent(text);
        floatingText.Style = skin.GetStyle(style);
        floatingText._positioner = positioner;
    }
    public void OnGUI()
    {

        var position = new Vector2();
        var contentSize = Style.CalcSize(_content);
        if(!_positioner.GetPosition(ref position, _content, contentSize))
        {
            Destroy(gameObject);
            return;
        }
        
        GUI.Label (new Rect(position.x, position.y, contentSize.x,contentSize.y), _content, Style);
    }
}
