

using UnityEngine;
using System.Collections;
using UnityEditor;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.
[CustomEditor(typeof(AnimatorStateExtension))]
public class AnimatorStateExtension : Editor
{
    /*
    public override VisualElement CreateInspectorGUI()
    {
        return new Label("This is a Label in a Custom Editor");
    }
    */
}




/*

//code for testing for custom editor


[CustomEditor(typeof(AnimatorStateTransition))]
public class AnimatorStateExtension2 : Editor
{
    public override void OnInspectorGUI()
    {
         
        if (GUILayout.Button("Click me!"))
        {
            Debug.Log("Button clicked!");
        }

        //DrawDefaultInspector();

    }
}

[CustomEditor(typeof(GameObject))]
public class AnimatorStateExtension3 : Editor
{
    public override void OnInspectorGUI()
    {
          base.OnInspectorGUI(); // Call the base method first to draw the default inspector UI

        if (GUILayout.Button("Click me!Gameobject"))
        {
            Debug.Log("Button clicked!");
        }
    }
}
*/
/*

//Custom Editor for Animator
[CustomEditor(typeof(AnimatorController))]
public class AnimatorExtension2 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorController"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(AnimatorStateTransition))]
public class AnimatorExtension3 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorStateTransition"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(GameObject))]
public class AnimatorExtension4 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("GameObject"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(AnimatorControllerLayer))]
public class AnimatorExtension5 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorControllerLayer"))
        {
            Debug.Log("Button clicked!");
        }
    }
}





//Custom Editor for Animator
[CustomEditor(typeof(UnityEngine.Object))]
public class AnimatorExtension6 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("UnityEngine.Object"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(MonoBehaviour))]
public class AnimatorExtension7 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("MonoBehaviour"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(ScriptableObject))]
public class AnimatorExtension8 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ScriptableObject"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(AnimatorController))]
public class AnimatorExtension9 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorController"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(Animator))]
public class AnimatorExtension10 : Editor
{
        public override void OnInspectorGUI()
    {
    base.OnInspectorGUI();

        if (GUILayout.Button("Animator"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(StateMachineBehaviour))]
public class AnimatorExtension11 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("StateMachineBehaviour"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(Behaviour))]
public class AnimatorExtension12 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Behaviour"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(Transform))]
public class AnimatorExtension13 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Transform"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(AnimatorState))]
public class AnimatorExtension14 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorState"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(AnimatorOverrideController))]
public class AnimatorExtension15 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorOverrideController"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(AnimatorStateMachine))]
public class AnimatorExtension16 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorStateMachine"))
        {
            Debug.Log("Button clicked!");
        }
    }
}



//Custom Editor for Animator
[CustomEditor(typeof(AnimatorState))]
public class AnimatorExtension17 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorState"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(AnimatorTransitionBase))]
public class AnimatorExtension18 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorTransitionBase"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(AnimatorCondition))]
public class AnimatorExtension19 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorCondition"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(AnimatorStateInfo))]
public class AnimatorExtension20 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorStateInfo"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(AnimatorTransitionInfo))]
public class AnimatorExtension21 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorTransitionInfo"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(AnimatorOverrideController))]
public class AnimatorExtension22 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorOverrideController"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(ChildAnimatorStateMachine))]
public class AnimatorExtension23 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ChildAnimatorStateMachine"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(ChildMotion))]
public class AnimatorExtension24 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ChildMotion"))
        {
            Debug.Log("Button clicked!");
        }
    }
}





//Custom Editor for Animator
[CustomEditor(typeof(AnimatorUtility))]
public class AnimatorExtension25 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorUtility"))
        {
            Debug.Log("Button clicked!");
        }
    }
}


//Custom Editor for Animator
[CustomEditor(typeof(AnimationEvent))]
public class AnimatorExtension26 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimationEvent"))
        {
            Debug.Log("Button clicked!");
        }
    }
}




//Custom Editor for Animator
[CustomEditor(typeof(AnimatorClipInfo))]
public class AnimatorExtension27 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimationInfo"))
        {
            Debug.Log("Button clicked!");
        }
    }
}

//Custom Editor for Animator
[CustomEditor(typeof(AnimatorStateTransition))]
public class AnimatorExtension28 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorStateTransition"))
        {
            Debug.Log("Button clicked!");
        }
    }
}

[CustomEditor(typeof(AnimatorState))]
public class AnimatorExtension29 : Editor
{
        public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("AnimatorState"))
        {
            Debug.Log("Button clicked!");
        }
    }
}















*/


