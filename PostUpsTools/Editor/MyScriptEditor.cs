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

      [MenuItem("Window/My Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<MySriptEditor>("My Window");
        }

        private void OnEnable()
        {
        }

    private void OnGUI()
    {
        if (GUILayout.Button("mybutton"))
        {

            //Call debug
            Debug.Log("Button clicked!");

            //for loop to create 1000 buttons
            for (int i = 0; i < 2; i++)
            {
                if (GUILayout.Button("mybutton"))
                {
                    Debug.Log("Button clicked!");
                }
            }

        }

    }


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

private Texture2D MakeQuad(int width, int height, Color color)
{
    Color[] pix = new Color[width * height];
    for (int i = 0; i < pix.Length; ++i)
    {
        pix[i] = color;
    }

    Texture2D result = new Texture2D(width, height);
    result.SetPixels(pix);
    result.Apply();
    return result;
}

private Texture2D MakeCircle(Color color, int width, int height)
{
    Color[] pix = new Color[width * height];

    float radius = Mathf.Min(width, height) * 0.5f;
    Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Vector2 position = new Vector2(x, y);
            float distance = Vector2.Distance(position, center);

            if (distance <= radius)
            {
                pix[y * width + x] = color;
            }
        }
    }

    Texture2D result = new Texture2D(width, height);
    result.SetPixels(pix);
    result.Apply();
    return result;
}

// Method for making a rectangle with rounded corners
private Texture2D MakeRoundRectangle(int width, int height, Color color, float cornerRadius)
{


    //crete a new texture
    Texture2D texture = new Texture2D(width, height);

    //create a new 2d array of colors
    Color[,] colors = new Color[width, height];



    //clamp the corner radius to the smallest dimension
    cornerRadius = Mathf.Min(cornerRadius, Mathf.Min((float)width, (float)height) * 0.5f);

    //loop through the array and set the color
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {

            //detect if we are in the corner
            if (x < cornerRadius && y < cornerRadius)
            {
                //top left corner
                if (!(Vector2.Distance(new Vector2(x, y), new Vector2(cornerRadius, cornerRadius)) <= cornerRadius))
                {
                    colors[x, y] = Color.clear;
                }
                else
                {
                    colors[x, y] = color;
                }
            }
            else if (x >= width - cornerRadius && y < cornerRadius)
            {
                //top right corner
                if (!(Vector2.Distance(new Vector2(x, y), new Vector2(width - cornerRadius - 1, cornerRadius)) <= cornerRadius))
                {
                    colors[x, y] = Color.clear;
                }
                else
                {
                    colors[x, y] = color;
                }
            }
            else if (x < cornerRadius && y >= height - cornerRadius)
            {
                //bottom left corner
                if (!(Vector2.Distance(new Vector2(x, y), new Vector2(cornerRadius, height - cornerRadius - 1)) <= cornerRadius))
                {
                    colors[x, y] = Color.clear;
                }
                else
                {
                    colors[x, y] = color;
                }
            }
            else if (x >= width - cornerRadius && y >= height - cornerRadius)
            {
                //bottom right corner
                if (!(Vector2.Distance(new Vector2(x, y), new Vector2(width - cornerRadius - 1, height - cornerRadius - 1)) <= cornerRadius))
                {
                    colors[x, y] = Color.clear;
                }
                else
                {
                    colors[x, y] = color;
                }
            }
            else
            {
                colors[x, y] = color;
            }


        }

    }

    //create a new 1d array of colors
    Color[] color1d = new Color[width * height];

    //loop through the 2d array and copy the colors to the 1d array
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            color1d[y * width + x] = colors[x, y];
        }
    }

    //set the texture colors
    texture.SetPixels(color1d);

    //apply the texture
    texture.Apply();

    return texture;

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


public Texture2D MakeOutlineTexture(int width, int height, Color color, float cornerRadius, int outlineSize)
{
    // Create a texture to hold the outline.
    Texture2D outlineTexture = new Texture2D(width, height);

    // Make the first rectangle with a color slightly darker than the final color.
    Color outlineColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
    Texture2D firstRectangle = MakeRoundRectangle(width, height, outlineColor, cornerRadius);

    // Make the second rectangle with the final color.
    Texture2D secondRectangle = MakeRoundRectangle(width - outlineSize * 2, height - outlineSize * 2, color, cornerRadius);

    // Apply the first rectangle to the outline texture.
    outlineTexture.SetPixels(0, 0, width, height, firstRectangle.GetPixels());

    // Apply the second rectangle to the outline texture, offset by the outline size.
    outlineTexture.SetPixels(outlineSize, outlineSize, width - outlineSize * 2, height - outlineSize * 2, secondRectangle.GetPixels());

    // Apply the changes to the texture.
    outlineTexture.Apply();

    // Return the outline texture.
    return outlineTexture;
}
//Method for custom checkbox

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