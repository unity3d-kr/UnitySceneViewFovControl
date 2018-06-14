using UnityEngine;
using UnityEditor;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {

class Status {
    float fov = 0.0f;
    bool reset = false;
    double showResetButtonTime = 0.0;

    public void OnScene(SceneView sceneView) {
        if(sceneView == null || sceneView.camera == null) {
            return;
        }

        if(sceneView.in2DMode) {
            return;
        }

        Camera camera = sceneView.camera;
        if(fov == 0.0f || reset) {
            fov = camera.fieldOfView;
            reset = false;
        }

        var ev = Event.current;
        var settings = Settings.Data;
        float deltaFov = 0.0f;

        if(ev.modifiers == settings.ModifiersNormal || ev.modifiers == settings.ModifiersQuick) {
            if(ev.type == EventType.ScrollWheel) {
                // todo : Check compatibility of Event.delta.y.
                //        e.g. Platform, mice, etc.
                // note : In MacOS, ev.delta becomes zero when "Shift" pressed.  I don't know the reason.
                deltaFov = ev.delta.y;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == settings.KeyCodeDecreaseFov) {
                deltaFov = -1.0f;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == settings.KeyCodeDecreaseFov) {
                deltaFov = -1.0f;
                ev.Use();
            }
        }

        if(deltaFov != 0.0f) {
            deltaFov *= settings.FovSpeed;
            if(ev.modifiers == settings.ModifiersQuick) {
                deltaFov *= settings.FovQuickMultiplier;
            }
            fov += deltaFov;
            fov = Mathf.Clamp(fov, settings.MinFov, settings.MaxFov);
            showResetButtonTime = EditorApplication.timeSinceStartup + settings.ButtonShowingDurationInSeconds;
        }

        camera.fieldOfView = fov;
    }

    public void OnSceneGUI(SceneView sceneView) {
        var settings = Settings.Data;
        if(EditorApplication.timeSinceStartup < showResetButtonTime || settings.AlwaysShowResetButton) {
            Handles.BeginGUI();
			Rect r = new Rect( 10, 10, 60, 16 );
			string fovstr;
			if( r.Contains( Event.current.mousePosition ) )
			{
				fovstr = "Reset";
				showResetButtonTime = EditorApplication.timeSinceStartup + settings.ButtonShowingDurationInSeconds;
			}
			else
				fovstr = string.Format( fov >= 100 ? "Fov {0:0}" : "Fov {0:0.0}", fov );

            if (GUI.Button(r, fovstr)) {
                reset = true;
            }
            Handles.EndGUI();
        }
    }
}

} // namespace
