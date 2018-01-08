# spacepuppy-unity-framwork-3.0
A modular framework of tools for use with the Unity game engine version 2017.3.

This framework starts with the base portion of SpaceuppyUnityFramework which contains all the general tools needed by each module. All subsequent modules require this dll to be included.

The base framework includes a number of types. Several of them are extensions or replacements of similar Unity types. Often I designed these tools years before Unity added their versions (a Coroutine object token, custom yield instructions, UnityEvent). Even though Unity has added/improved their versions of these objects, I still prefer my versions as they often have more features than the built-in Unity ones.

- SPComponent: a more robust version of MonoBehaviour
- RadicalCoroutine: an extension of the built in Unity Coroutine. It adds events/pausing/scheduling/custom yield instructions.
- RadicalTask: multi-threaded coroutines (async/await is planned to replace this feature)
- SPEvent: Similar to UnityEvent. This was designed prior to UnityEvent existing, and as a result has a different interface to it, and as a result can perform many tasks that UnityEvent can not.
- SPEntity: an entity structure to relate multiple GameObjects together as a single entity.
- MultiTag: add more than 1 tag to any given GameObject.
- VariantReference: a dynamic/variant data type that allows selecting the type & value through the inspector for a given serialized property
- Collections: several collection types like BinaryHeap, MultitonPool, ObjectCachePool, etc.

After adding the SpaceuppyUnityFramework you can pick and choose the modules to include with it for those tools sets.

- SPAnim - an extension of the Unity Legacy Animation system. If you don't like mecanim (like me), but find the legacy animation system a bit lacking. This perks it up a bit.

- SPCamera - a uniform interface and manager for cameras and their effects. One feature of this interface is that it facilitates treating a group of cameras as one. For example if you wanted to make a racing game and have multiple view modes for it. Your ICamera script can handle the swapping of actual cameras, but itself be accessed by all other scripts as if it were the camera regardless of which is active.

- SPInput - An input library that defines input devices as a IPlayerInputDevice interface. Can be integrated with built in UnityInput, as well with 3rd party.

- SPMotor - motor scripts to facilitate movement of player and enemies. This includes a generalized interface IMotor so you can handle Rigidbody and CharacterController as a uniform type. As well as a state machine for movement styles.

- SPPathfinding - currently is in early development, but is planned to house a robust pathfinding (A*) system for use in games. Currently only houses basic implementations of the algorithms as well as fundamental interface/contracts for use with AI agents.

- SPProject - A collection of classes intended to make project/asset management easier.

- SPScenes - an extension of SceneManager adopting the IService model.

- SPSensors - attach an 'aspect' to an object, and then your AI/Player can use a 'sensor' of various shapes/types to determine if said aspect can be seen.

- SPSerialization - a serialization library built on top of the .Net serialization interface. It supports various formats (json/binary included), and attempts to allow serializing GameObjects as asset id's that can be pulled from Resources/AssetBundles.

- SPSpawn - A Spawn Pool Library

- SPTriggers - (requires SPTween) A feature of SPEvent that UnityEvent does not contain is an interface for simple visual programming by using GameObjects as trigger nodes in a chain of commands. SPTriggers is a collection of reusable commands such as 'T_OnStart', 'I_SetValue', 'I_Destroy', and many more. We have found these tools to be very useful for creating scenarios in game. Note, various other modules contain commands specific to their module. SPAnim has a 'I_PlayAnimation', SPTween has 'I_Tween', and SPWaypoint has 'I_MoveOnPath'.

- SPTween - a tween engine built on Spacepuppy

- SPUtils - (requires SPTween) some useful utility classes. These used to be in SpacepuppyUnityFramework base dll, but was moved here to reduce the size of that dll since they aren't necessary for any module to work.

- SPWaypoint - (requires SPTween) a waypoint library with algorithms for bezier, catmull-rom, linear, as well as a UI to set up paths in your game. This is very useful for setting up camera paths and other animated events. (warning - this is not a pathfinding system, that is in development in another module)

# Quick Import

Download the latest build from the github project and unzip contents into your project's Asset folder.

# Quick Build

First open 'dobuild.release.bat' and make sure the path to MSBuild.exe matches where you have it installed on your computer.

Run 'dobuild.release.bat'.

A 'Builds' folder will be created with a 'SpaceuppyUnityFramework' inside of it.

Delete any module's dll's you don't want from inside the 'SpaceuppyUnityFramework' folder. You could also have commented out the xcopy lines for any modules you don't want in the bat file.

Copy the 'SpaceuppyUnityFramework' folder in there into your project wherever you'd like (this is the same thing you would find in the downloaded builds from the github page).

# Manual Build

Open the SpacepuppyUnityFramework.sln in Visual Studio (or other suitable IDE) and select Build like you would any project.

Traverse through each module's output directories (%module%/bin/Release) and copy the appropriate *.dll for that module into an appropriate folder in Assets. I prefer to name mine 'Assets/SpaceuppyUnityFramework'.

Traverse through each module's editor output directories (%module%editor/bin/Release) and copy the appropriate *.dll for that module editor into an appropriate Editor folder in Assets. I prefer to name mine 'Assets/SpacepuppyUnityFramework/Editor'.

Go into the 'Resources' folder.

If you use the SPSensors module, copy 'Shaders' folder into the previously created editor folder for the editor script dll's.

Make sure you have 'Visible Meta Files' enabled in 'Edit->Project Settings->Editor Settings'.

Copy 'SpacepuppyUnityFramework.dll.meta' to the same folder as 'SpacepuppyUnityFramework.dll', overwrite if Unity already created the meta file.

If you don't show meta files in your project, and would like to keep it that way, you may have to manually configure the execution order. In this case open the SpacepuppyUnityFramework.dll.meta and locate the 'executionOrder' line, in your project go to 'Edit->Project Settings->Script Execution Order' and in the appropriate screen drag in the matching scripts and set them to the corresponding values. It is advised to use the included meta file though as it's easier.

# License
Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.