﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
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
using static Layer_Functions;
using static Transition_Functions;
using static Parser_Functions;
using static Save_Functions;
using static AbsoluteAnimations;

using static Boxdrawer;

//TODO
//Animator Functions
//Preset Settings
//Emote Menu Adder+Parameters

/*
Particle Functions:
Buffer Particle Setup
Smooth Fading
Length

Fast Audio setup
Logic
Looped Statement placer
WD Off Statements
Parameter Driver Copyier

Bookmark and History Function
WD ON statements

//Edit Function

Discomfort Museum

*/
public class PostUP_UI : EditorWindow
{

    //Menus

    public PostUP_UI postUP_UI;
    public AbsoluteAnimations generator;
    public Folder_Dialog folder_dialog;
    public Quick_Animations quickAnimations;
    public Animator_Utils animator_Utils;
    public Copy_Tools copy_Tools;
    public Parser_Menu parser_Menu;
    public AnimatorInspector animatorInspector;
    //Basic Inputs ----------------------------------------------------------------------------------------------------------------------------
    public bool customName;
    public string usepath;
    private string animationName;
    private string scenepath;


    //Main Objects----------------------------------------------------------------------------------------------------------------------------

    public GameObject controllergameobject;
    public AnimatorController controller;
    public Animator animator;
    public GameObject target;



    //UI ----------------------------------------------------------------------------------------------------------------------------
    private Vector2 scrollPos;

    //Buffers----------------------------------------------------------------------------------------------------------------------------
    private GameObject controllergameobject_buffer;
    private GameObject target_buffer;


    //UI Refresher ----------------------------------------------------------------------------------------------------------------------------

    public int fluigserviceid = 0;
    public bool fluidping = false;
    public bool runFluidServices = true;


    [MenuItem("Animtools/PostUP_UI")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PostUP_UI));

    }


    //Reload UI
    public void reloadUI()
    {

        postUP_UI = ScriptableObject.CreateInstance<PostUP_UI>();

        generator = new AbsoluteAnimations();

        folder_dialog = new Folder_Dialog();

        quickAnimations = new Quick_Animations();

        copy_Tools = new Copy_Tools();

        animator_Utils = new Animator_Utils();

        parser_Menu = new Parser_Menu();

        animatorInspector = new AnimatorInspector();
    }

    private void Start()
    {
        postUP_UI = ScriptableObject.CreateInstance<PostUP_UI>();
    }

    private void OnGUI()
    {


        GUIStyle background = new GUIStyle(GUI.skin.box);
        background.normal.background = MakeRoundRectangle((int)100, (int)100, new Color(0.19f, 0.19f, 0.19f), 2f);

        //UI Refresher ----------------------------------------------------------------------------------------------------------------------------

        // generate a random int von 0 to 100000
        fluigserviceid = UnityEngine.Random.Range(0, 100000);
        EditorCoroutineUtility.StartCoroutine(UIRefresher(fluigserviceid), this);

        // Set the horizontal margin for the scroll view
        int horizontalMargin = 7;
        RectOffset marginOffset = new RectOffset(horizontalMargin, horizontalMargin, 0, 0);

        // Set the width of the scroll view
        int scrollViewWidth = (int)EditorGUIUtility.currentViewWidth - horizontalMargin * 2;

        // Center the scroll view horizontally
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(horizontalMargin);
        Rect scrollViewRect = GUILayoutUtility.GetRect(0, 1000);
        scrollViewRect.height = Screen.height - scrollViewRect.y - 40;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);


        //Horizontal Layout
        GUILayout.BeginHorizontal(background);

        if (GUILayout.Button("Reset UI"))
        {
            runFluidServices = true;
            reloadUI();
            EditorCoroutineUtility.StartCoroutine(Reloadfinisher(), this);

        }


        if (GUILayout.Button("Fast Rendering " + runFluidServices))
        {
            runFluidServices = !runFluidServices;
        }

        //end
        GUILayout.EndHorizontal();


        //Update controller if New Controller is selected
        if (controllergameobject_buffer != controllergameobject)
        {
            controllergameobject = controllergameobject_buffer;

            if (controllergameobject != null)
            {
                animator = controllergameobject.GetComponent<Animator>();

                if (animator != null)
                {
                    controller = animator.runtimeAnimatorController as AnimatorController;
                    if (controller == null) { Debug.LogError("No Controller"); }
                }
            }
        }

        //Initialize GUI Element Objects

        if (postUP_UI == null)
            postUP_UI = ScriptableObject.CreateInstance<PostUP_UI>();

        if (generator == null)
            generator = new AbsoluteAnimations();

        if (folder_dialog == null)
            folder_dialog = new Folder_Dialog();

        if (quickAnimations == null)
            quickAnimations = new Quick_Animations();

        if (copy_Tools == null)
            copy_Tools = new Copy_Tools();

        if (animator_Utils == null)
            animator_Utils = new Animator_Utils();

        if (parser_Menu == null)
            parser_Menu = new Parser_Menu();

        if (animatorInspector == null)
            animatorInspector = new AnimatorInspector();


        //Inputs ------------------------------------------------------------------------------------------------------------------

        //Folder Selection
        usepath = folder_dialog.FolderDialogGUI();
        EditorGUILayout.LabelField("Selected Folder:" + usepath, EditorStyles.boldLabel);
        GUILayout.Space(20);

        //Show options if path is valid
        if (usepath == null)
        {
            EditorGUILayout.LabelField("Select Save Folder for Animations Please!");
        }
        else
        {


             GUILayout.BeginVertical(background);
            GUILayout.Space(10);

            //Check for Custom Name Checkbox and show Textfield
            customName = EditorGUILayout.Toggle("Custom Filename_OPT", customName);
            if (customName) animationName = EditorGUILayout.TextField("Animation Name:", animationName);

            //Set Gameobjet for Controller
            controllergameobject_buffer = (GameObject)EditorGUILayout.ObjectField("Controller Object / AV:", controllergameobject_buffer, typeof(GameObject), true, GUILayout.Width(Screen.width * 0.75f));

            //Check if Controller Exists and show Options
            if (controllergameobject == null)
            {
                EditorGUILayout.LabelField("Select a Object for the Controller for Showing Options (A Controller will be created called Generator in PostUpsTools Folder)");
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }
            else if (animator == null)
            {
                EditorGUILayout.LabelField("No Animator on the Controller!");
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }
            else
            {



                //Set Target Object
                target_buffer = (GameObject)EditorGUILayout.ObjectField("Target Object:", target_buffer, typeof(GameObject), true, GUILayout.Width(Screen.width * 0.75f));
                GUILayout.Space(10);
                GUILayout.EndVertical();

                //if Target is not the same as the last frame, set it to the new one
                if (target_buffer != target)
                    target = target_buffer;

                //Show Options if Target exists
                if (target == null)
                {
                    EditorGUILayout.LabelField("Select a Target Object for Animation Options");
                    GUILayout.Space(20);
                }
                else
                {

                    //write Path
                    scenepath = GetRelativePathFromTo(controllergameobject_buffer, target);
                    if (scenepath.EndsWith("/"))
                    {
                        scenepath = scenepath.Substring(0, scenepath.Length - 1);
                    }

                    //Quickgen ------------------------------------------------------------------------------------------------------------------


                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("Animation Tools:", EditorStyles.boldLabel);
                    if (!customName) animationName = target.name;

                    GUILayout.Space(10);

                    GUILayout.BeginVertical(background);
                    quickAnimations.Menu(animationName, target, animator, controller, usepath, scenepath);
                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical(background);
                    generator.GenerateSelected(animationName, target, animator, controller, usepath, scenepath);
                    GUILayout.EndVertical();

                    GUILayout.Space(20);
                }

                EditorGUILayout.LabelField("Animator Utils:", EditorStyles.boldLabel);

                GUILayout.Space(10);

                GUILayout.BeginVertical(background);
                animator_Utils.Menu(controller);
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical(background);
                parser_Menu.Menu(controller);
                GUILayout.EndVertical();

                GUILayout.Space(10);

            }


            
            GUILayout.BeginVertical(background);
            copy_Tools.Menu(controller);
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(background);
            animatorInspector.Menu(controller);
            GUILayout.EndVertical();
        }



        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);
        GUILayout.EndHorizontal(); // end horizontal group
        GUILayout.Space(10);
        GUILayout.EndVertical(); // end vertical group
                                 // End ScrollView area
    }

    //Method to deselect all objects in the unity editor animator window

    private void ForceUpdates()
    {
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.QueuePlayerLoopUpdate();
    }


    private void UpdateAnimator(AnimatorController controller)
    {
        //Force Unity to update the Animator
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //State Functions----------------------------------------------------------------------------------------------------------------------------


    IEnumerator UIRefresher(int id)
    {

        yield return new WaitForSeconds(0.05f);

        if (id == fluigserviceid && runFluidServices == true)
        {
            //Debug.Log("Refresher Service Running");
            Repaint();
            fluidping = false;
            EditorCoroutineUtility.StartCoroutine(UIRefresher(id), this);
        }

    }



    public IEnumerator Reloadfinisher()
    {
        yield return new WaitForSeconds(0.5f);
        runFluidServices = true;
    }



}


