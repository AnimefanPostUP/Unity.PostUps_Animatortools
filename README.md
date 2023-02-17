# PostUpsTools
VR Chat Animator Toolkit

Project Ongoing, some Major issues still in code,
but basic functions and Layout were build.



Current Toolfunctions:

ResetUI: Function to Resolve UI Bugs that may currecntly Exist
Fast Rendering: Increases UI Update Rate of Unity to make it Fluig (Uses Cooroutine Loop for Repaint(); calls)

Choose Filedirectory: 
Allows for Saving Animations into a Choosen Folder
Path: has to be relative to the Project!

Custom Filename_OPT: 
Replaces the String for the Gameobjects Name to a Choosen one. Example:  MyCube_On.anim
Custom Filename: will show up when toggle is enabled

Controller Gameobject: 
Object that Holds the Controller, the Tool will work in this space! (Causes Errormessages (NullReference) can be ignored)
Target Object: Object where Animations will be applied on



Quick Animations:

Create Toggle Animations: 
Will create on and off toggle animations and add them with a new parameter to the Animator 

Create Linear Audio Volume: 
Creates Curve for Audio Volume that goes from 0 to 1

Create Hue Curve (Color+Emission): 
Uses RGB Values to create a Curve, the tool will be later modified so Parameter Type can be pasted into it

Animator Utils:
AddCondition: 
Debugging Function

Rebind: 
Will move the Destination of the current selected Transition to the next selected State.

Absolute Animations:
Tools for Creating Constant animations for: All Transforms, Transforms Individually, Volume, Particle Emission, SelfActive

Transitions Parser (Experimental):
Creates Transitions based on Text Input

Transistion Utilitys:
Can copy Transitions of a Selected Transitions into a Cache where transistions and conditions can be edited, 
no need for switchting the window all the time

Checkbox: Only Selected will be Pasted
Circle Button: Will Select the Transistion and Deselect all others
Paste: Pastes the Transition on top of the Current Selection
X: Deletes Transition from Cache

Condition:
First Button with Parameter: can be clicked for Larger parameter Selection
Type: Shows which type the Parameter is
Dropdown: Avaiable Modes for the Transition
Dropdown/Textbow: Shows the Threshhold for the Transition and allows for Edit, either Int: 2 or float 5,2
X: Removes Conditions
+: Adds Condition if no Parameters are existing a Dummy will be created

Add Transitions: if no Parameters are existing a Dummy will be created



Copy All Transitions: 
Will Copy all Transitions

Paste Selected Transistions: 
Will Paste all transitions with the Checkbox checked

Overwrite All Transitions:
Will Overwrite the Transitions of the Currently Selected Transistion will all included that are under it with the Cached Transitions
Will cause the UI to load a Moment, Click arround to fix issues



