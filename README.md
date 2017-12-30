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

- SPInput - An input library that defines input devices as a IPlayerInputDevice interface. Can be integrated with built in UnityInput, as well with 3rd party.

- SPSerialization - a serialization library built on top of the .Net serialization interface. It supports various formats (json/binary included), and attempts to allow serializing GameObjects as asset id's that can be pulled from Resources/AssetBundles.

- SPTriggers - A feature of SPEvent that UnityEvent does not contain is an interface for simple visual programming by using GameObjects as trigger nodes in a chain of commands. SPTriggers is a collection of reusable commands such as 'T_OnStart', 'I_SetValue', 'I_Destroy', and many more. We have found these tools to be very useful for creating scenarios in game. Note, various other modules contain commands specific to their module. SPAnim has a 'I_PlayAnimation', SPTween has 'I_Tween', and SPWaypoint has 'I_MoveOnPath'.

- SPTween - a tween engine built on Spacepuppy

- SPWaypoint - a waypoint library with algorithms for bezier, catmull-rom, linear, as well as a UI to set up paths in your game. This is very useful for setting up camera paths and other animated events. (warning - this is not a pathfinding system, that is in development in another module)

# License
Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.