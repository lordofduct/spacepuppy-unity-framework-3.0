REM Replace with the location of MSBuild.exe
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"\MSBuild.exe .\SpacepuppyUnityFramework.sln /t:rebuild /verbosity:q /p:Configuration=Release && (
	echo SUCCESSFUL BUILD
) || (
	echo Failed to build, check to make sure that path to MSBuild.exe is correct.
	PAUSE
	EXIT
)

REM Make Directories to copy to
rmdir /S /Q .\Builds\Release
mkdir .\Builds\Release\
mkdir .\Builds\Release\Editor\
mkdir .\Builds\Release\Editor\Shaders\

REM Copy Runtime dll's
xcopy /Y /S .\SpacepuppyUnityFramework\bin\Release\SpacepuppyUnityFramework.dll .\Builds\Release\
xcopy /Y /S .\SpacepuppyUnityFramework\bin\Release\SpacepuppyUnityFramework.pdb .\Builds\Release\
xcopy /Y /S .\SPAnim\bin\Release\SPAnim.dll .\Builds\Release\
xcopy /Y /S .\SPAnim\bin\Release\SPAnim.pdb .\Builds\Release\
xcopy /Y /S .\SPCamera\bin\Release\SPCamera.dll .\Builds\Release\
xcopy /Y /S .\SPCamera\bin\Release\SPCamera.pdb .\Builds\Release\
xcopy /Y /S .\SPInput\bin\Release\SPInput.dll .\Builds\Release\
xcopy /Y /S .\SPInput\bin\Release\SPInput.pdb .\Builds\Release\
xcopy /Y /S .\SPMotor\bin\Release\SPMotor.dll .\Builds\Release\
xcopy /Y /S .\SPMotor\bin\Release\SPMotor.pdb .\Builds\Release\
xcopy /Y /S .\SPPathfinding\bin\Release\SPPathfinding.dll .\Builds\Release\
xcopy /Y /S .\SPPathfinding\bin\Release\SPPathfinding.pdb .\Builds\Release\
xcopy /Y /S .\SPScenes\bin\Release\SPScenes.dll .\Builds\Release\
xcopy /Y /S .\SPScenes\bin\Release\SPScenes.pdb .\Builds\Release\
xcopy /Y /S .\SPSensors\bin\Release\SPSensors.dll .\Builds\Release\
xcopy /Y /S .\SPSensors\bin\Release\SPSensors.pdb .\Builds\Release\
xcopy /Y /S .\SPSerialization\bin\Release\SPSerialization.dll .\Builds\Release\
xcopy /Y /S .\SPSerialization\bin\Release\SPSerialization.pdb .\Builds\Release\
xcopy /Y /S .\SPTriggers\bin\Release\SPTriggers.dll .\Builds\Release\
xcopy /Y /S .\SPTriggers\bin\Release\SPTriggers.pdb .\Builds\Release\
xcopy /Y /S .\SPTween\bin\Release\SPTween.dll .\Builds\Release\
xcopy /Y /S .\SPTween\bin\Release\SPTween.pdb .\Builds\Release\
xcopy /Y /S .\SPWaypoint\bin\Release\SPWaypoint.dll .\Builds\Release\
xcopy /Y /S .\SPWaypoint\bin\Release\SPWaypoint.pdb .\Builds\Release\

xcopy /Y /S .\Resources\SpacepuppyUnityFramework.dll.meta .\Builds\Release\

REM Copy Editor dll's
xcopy /Y /S .\SpacepuppyUnityFrameworkEditor\bin\Release\SpacepuppyUnityFrameworkEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SpacepuppyUnityFrameworkEditor\bin\Release\SpacepuppyUnityFrameworkEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPAnimEditor\bin\Release\SPAnimEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPAnimEditor\bin\Release\SPAnimEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPCameraEditor\bin\Release\SPCameraEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPCameraEditor\bin\Release\SPCameraEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPInputEditor\bin\Release\SPInputEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPInputEditor\bin\Release\SPInputEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPMotorEditor\bin\Release\SPMotorEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPMotorEditor\bin\Release\SPMotorEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPPathfindingEditor\bin\Release\SPPathfindingEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPPathfindingEditor\bin\Release\SPPathfindingEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPScenesEditor\bin\Release\SPScenesEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPScenesEditor\bin\Release\SPScenesEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPSensorsEditor\bin\Release\SPSensorsEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPSensorsEditor\bin\Release\SPSensorsEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPSerializationEditor\bin\Release\SPSerializationEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPSerializationEditor\bin\Release\SPSerializationEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPTriggersEditor\bin\Release\SPTriggersEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPTriggersEditor\bin\Release\SPTriggersEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPTweenEditor\bin\Release\SPTweenEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPTweenEditor\bin\Release\SPTweenEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\SPWaypointEditor\bin\Release\SPWaypointEditor.dll .\Builds\Release\Editor
xcopy /Y /S .\SPWaypointEditor\bin\Release\SPWaypointEditor.pdb .\Builds\Release\Editor
xcopy /Y /S .\Resources\Shaders .\Builds\Release\Editor\Shaders

PAUSE