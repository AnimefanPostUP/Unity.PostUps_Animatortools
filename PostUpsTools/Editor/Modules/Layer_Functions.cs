using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using UnityEngine.Animations;
using Unity.EditorCoroutines.Editor;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using static State_Functions;
using static Clip_Generator;
using static Parameter_Functions;



public class Layer_Functions
{

    public static bool HasLayer(AnimatorController controller, string layerName)
    {
        int layerCount = controller.layers.Length;
        for (int i = 0; i < layerCount; i++)
        {
            if (controller.layers[i].name == layerName)
            {
                return true;
            }
        }
        return false;
    }

    public static AnimatorControllerLayer GetLayer(AnimatorController controller, string layerName)
    {
        int layerCount = controller.layers.Length;
        for (int i = 0; i < layerCount; i++)
        {
            if (controller.layers[i].name == layerName)
            {
                return controller.layers[i];
            }
        }
        return null;
    }

    public static void AddLayerIfNotExists(AnimatorController controller, string name)
    {
        AnimatorControllerLayer layer = controller.layers.FirstOrDefault(l => l.name == name);

        if (layer == null)
        {
            layer = new AnimatorControllerLayer();
            layer.name = name;
            layer.stateMachine = new AnimatorStateMachine();

            AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);

            int layerCount = controller.layers.Length;
            AnimatorControllerLayer[] newLayers = new AnimatorControllerLayer[layerCount + 1];

            for (int i = 0; i < layerCount; i++)
            {
                newLayers[i] = controller.layers[i];
            }

            newLayers[layerCount] = layer;
            controller.layers = newLayers;

            EditorUtility.SetDirty(controller);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


}
