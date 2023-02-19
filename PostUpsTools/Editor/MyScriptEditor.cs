using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using Unity.EditorCoroutines.Editor;

using System.Collections;

public class MySriptEditor : EditorWindow
{
    
    /*  private const float ButtonWidth = 110f;
        private const float ButtonHeight = 20f;
        private const float CornerRadius = 2f;

        private Color normalColor = Color.blue;
        private Color hoverColor = new Color(0f, 0.5f, 1f);
        private bool isHovering;
        private GUIStyle buttonRoundStyle;
        private MySriptEditor myScript;

        private Texture2D blue;

        private Texture2D lightblue;
        private bool Reload = false;





        // other objects

        private bool toggleValue = false;
        private string textFieldValue = "";
        private string textAreaValue = "";
        private Vector2 scrollPosition = Vector2.zero;

      

        //on repaint

    */


    /*

    //Reloading Loop, UIRefresher calls repaint,  calls OnGUI, so it's a loop
    EditorCoroutineUtility.StartCoroutine(UIRefresher(100), this);

    //Debug.Log("OnGUI called");

    //initialize the GUIStyle if it's null
    if (buttonRoundStyle == null)
    {
        buttonRoundStyle = new GUIStyle(GUI.skin.button);

        reloadUI();
        Debug.Log("UI reloaded");

    }


    if (GUILayout.Button("Reset Addon UI", buttonRoundStyle, GUILayout.Width(ButtonWidth), GUILayout.Height(ButtonHeight)))
    {
        Reload = true;
        reloadUI();
        EditorCoroutineUtility.StartCoroutine(Reloadfinisher(), this);

    }


    if (GUILayout.Button("Status: " + runFluidServices, buttonRoundStyle, GUILayout.Width(ButtonWidth), GUILayout.Height(ButtonHeight)))
    {
        runFluidServices = !runFluidServices;
    }


    GUILayout.Label("This is a label.");

    if (GUILayout.Button("This is a button"))
    {
        Debug.Log("Button clicked!");
    }

    GUILayout.Box("This is a box.");

    textFieldValue = GUILayout.TextField(textFieldValue);

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    textAreaValue = GUILayout.TextArea(textAreaValue);
    GUILayout.EndScrollView();

    toggleValue = GUILayout.Toggle(toggleValue, "This is a toggle");

    GUILayout.BeginArea(new Rect(10, 100, 200, 200));
    GUILayout.Label("This is an area.");
    GUILayout.EndArea();

}





public IEnumerator Reloadfinisher()
{
    yield return new WaitForSeconds(0.5f);
    Reload = false;
}


public void reloadUI()
{
    myScript = FindObjectOfType<MySriptEditor>();
    blue = MakeRoundRectangle((int)ButtonWidth, (int)ButtonHeight, new Color(0.15f, 0.15f, 0.15f), CornerRadius);
    lightblue = MakeRoundRectangle((int)ButtonWidth, (int)ButtonHeight, new Color(0.25f, 0.25f, 0.25f), CornerRadius);

    //initialize the GUIStyle

    buttonRoundStyle.normal.textColor = Color.white;
    buttonRoundStyle.normal.background = blue;
    buttonRoundStyle.hover.background = lightblue;

    runningServices = 0;
    recursionRunning = false;
}




/*
Sure, here is a list of all the available GUILayout objects in Unity:

GUILayout.Button - Creates a clickable button.
GUILayout.Box - Creates a box with a border and optional title.
GUILayout.Label - Creates a text label.
GUILayout.TextField - Creates a text input field.
GUILayout.TextArea - Creates a multi-line text input field.
GUILayout.Toggle - Creates a toggle button.
GUILayout.HorizontalSlider - Creates a horizontal slider.
GUILayout.VerticalSlider - Creates a vertical slider.
GUILayout.HorizontalScrollbar - Creates a horizontal scrollbar.
GUILayout.VerticalScrollbar - Creates a vertical scrollbar.
GUILayout.Space - Adds empty space to the layout.
GUILayout.FlexibleSpace - Adds flexible empty space to the layout.
GUILayout.BeginArea - Begins a new layout area.
GUILayout.EndArea - Ends a layout area.
GUILayout.BeginScrollView - Begins a scroll view.
GUILayout.EndScrollView - Ends a scroll view.
GUILayout.BeginHorizontal - Begins a horizontal group of controls.
GUILayout.EndHorizontal - Ends a horizontal group of controls.
GUILayout.BeginVertical - Begins a vertical group of controls.
GUILayout.EndVertical - Ends a vertical group of controls.
GUILayout.BeginHorizontal - Begins a horizontal group of controls with a flexible space at the end.
GUILayout.EndHorizontal - Ends a horizontal group of controls with a flexible space at the end.
GUILayout.BeginVertical - Begins a vertical group of controls with a flexible space at the end.
GUILayout.EndVertical - Ends a vertical group of controls with a flexible space at the end.
GUILayout.BeginHorizontal - Begins a horizontal group of controls with a fixed size.
GUILayout.EndHorizontal - Ends a horizontal group of controls with a fixed size.
GUILayout.BeginVertical - Begins a vertical group of controls with a fixed size.
GUILayout.EndVertical - Ends a vertical group of controls with a fixed size.

*/

}