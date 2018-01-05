REM Replace with the location of MSBuild.exe
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"\MSBuild.exe .\SpacepuppyUnityFramework.sln /t:rebuild /verbosity:q /p:Configuration=Release && (
	echo SUCCESSFUL BUILD
) || (
	echo Failed to build, check to make sure that path to MSBuild.exe is correct.
	PAUSE
	EXIT
)

REM Make Directories to copy to
rmdir /S /Q .\Builds\SpacepuppyUnityFramework
mkdir .\Builds\SpacepuppyUnityFramework\
mkdir .\Builds\SpacepuppyUnityFramework\Editor\
mkdir .\Builds\SpacepuppyUnityFramework\Editor\Shaders\

REM Copy Runtime dll's
xcopy /Y /S .\SpacepuppyUnityFramework\bin\Release\SpacepuppyUnityFramework.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SpacepuppyUnityFramework\bin\Release\SpacepuppyUnityFramework.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPAI\bin\Release\SPAI.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPAI\bin\Release\SPAI.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPAnim\bin\Release\SPAnim.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPAnim\bin\Release\SPAnim.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPCamera\bin\Release\SPCamera.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPCamera\bin\Release\SPCamera.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPInput\bin\Release\SPInput.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPInput\bin\Release\SPInput.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPMotor\bin\Release\SPMotor.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPMotor\bin\Release\SPMotor.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPPathfinding\bin\Release\SPPathfinding.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPPathfinding\bin\Release\SPPathfinding.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPProject\bin\Release\SPProject.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPProject\bin\Release\SPProject.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPScenes\bin\Release\SPScenes.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPScenes\bin\Release\SPScenes.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPSensors\bin\Release\SPSensors.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPSensors\bin\Release\SPSensors.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPSerialization\bin\Release\SPSerialization.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPSerialization\bin\Release\SPSerialization.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPTriggers\bin\Release\SPTriggers.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPTriggers\bin\Release\SPTriggers.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPTween\bin\Release\SPTween.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPTween\bin\Release\SPTween.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPUtils\bin\Release\SPUtils.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPUtils\bin\Release\SPUtils.pdb .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPWaypoint\bin\Release\SPWaypoint.dll .\Builds\SpacepuppyUnityFramework\
xcopy /Y /S .\SPWaypoint\bin\Release\SPWaypoint.pdb .\Builds\SpacepuppyUnityFramework\

xcopy /Y /S .\Resources\SpacepuppyUnityFramework.dll.meta .\Builds\SpacepuppyUnityFramework\

REM Copy Editor dll's
xcopy /Y /S .\SpacepuppyUnityFrameworkEditor\bin\Release\SpacepuppyUnityFrameworkEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SpacepuppyUnityFrameworkEditor\bin\Release\SpacepuppyUnityFrameworkEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPAIEditor\bin\Release\SPAIEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPAIEditor\bin\Release\SPAIEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPAnimEditor\bin\Release\SPAnimEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPAnimEditor\bin\Release\SPAnimEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPCameraEditor\bin\Release\SPCameraEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPCameraEditor\bin\Release\SPCameraEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPInputEditor\bin\Release\SPInputEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPInputEditor\bin\Release\SPInputEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPMotorEditor\bin\Release\SPMotorEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPMotorEditor\bin\Release\SPMotorEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPPathfindingEditor\bin\Release\SPPathfindingEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPPathfindingEditor\bin\Release\SPPathfindingEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPProjectEditor\bin\Release\SPProjectEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPProjectEditor\bin\Release\SPProjectEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPScenesEditor\bin\Release\SPScenesEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPScenesEditor\bin\Release\SPScenesEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPSensorsEditor\bin\Release\SPSensorsEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPSensorsEditor\bin\Release\SPSensorsEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPSerializationEditor\bin\Release\SPSerializationEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPSerializationEditor\bin\Release\SPSerializationEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPTriggersEditor\bin\Release\SPTriggersEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPTriggersEditor\bin\Release\SPTriggersEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPTweenEditor\bin\Release\SPTweenEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPTweenEditor\bin\Release\SPTweenEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPUtilsEditor\bin\Release\SPUtilsEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPUtilsEditor\bin\Release\SPUtilsEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPWaypointEditor\bin\Release\SPWaypointEditor.dll .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\SPWaypointEditor\bin\Release\SPWaypointEditor.pdb .\Builds\SpacepuppyUnityFramework\Editor
xcopy /Y /S .\Resources\Shaders .\Builds\SpacepuppyUnityFramework\Editor\Shaders

PAUSE