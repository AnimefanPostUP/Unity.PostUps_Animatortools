using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxdrawer 
{



/*
examples:
    myScript = FindObjectOfType<MySriptEditor>();
    blue = MakeRoundRectangle((int)ButtonWidth, (int)ButtonHeight, new Color(0.15f, 0.15f, 0.15f), 2f);
    lightblue = MakeRoundRectangle((int)ButtonWidth, (int)ButtonHeight, new Color(0.25f, 0.25f, 0.25f), 2f);
    */

public static Texture2D MakeOutlineTexture(int width, int height, Color color, float cornerRadius, int outlineSize)
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


public static Texture2D MakeQuad(int width, int height, Color color)
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

public static Texture2D MakeCircle(Color color, int width, int height)
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
public static Texture2D MakeRoundRectangle(int width, int height, Color color, float cornerRadius)
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







}
