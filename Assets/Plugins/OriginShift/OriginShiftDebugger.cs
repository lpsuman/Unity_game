using UnityEngine;
using twoloop;

public class OriginShiftDebugger : MonoBehaviour
{
    public void OnGUI()
    {
        if (!OriginShift.singleton.focus)
        {
            return;
        }
        var g = new GUIStyle();
        g.normal.textColor = Color.white;
        g.fontSize = 42;
        GUI.Label(new Rect(10, 370, 600, 40), $"Pos: {OriginShift.singleton.focus.transform.position}", g);

        if (OriginShift.singleton)
        {
            GUI.Label(new Rect(10, 430, 1000, 40), $"Offset: {OriginShift.LocalOffset.ToVector3()}", g);
        }
    }
}