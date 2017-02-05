using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {
    static class Settings {
        static SettingsData data = new SettingsData();

        public const string MenuItemName = "Edit/Scene View FoV Settings";
        public const string EditorPrefsKey = "UTJ.UnityEditor.Extension.SceneViewFovControl";

        public const float FovSpeedMin = 0.01f;
        public const float FovSpeedMax = 10.0f;
        public const float FovQuickMultiplierMin = 0.1f;
        public const float FovQuickMultiplierMax = 20.0f;
        public const float MinFovMin = 1.0f;
        public const float MinFovMax = 160.0f;
        public const float MaxFovMin = 1.0f;
        public const float MaxFovMax = 160.0f;

        public static SettingsData Data {
            get {
                return data;
            }

            set {
                data = value;
            }
        }

        static Settings() {
            Reset();

            if(EditorPrefs.HasKey(EditorPrefsKey)) {
                Data = Load(EditorPrefsKey);
            }
        }

        public static void Reset() {
            Data.Reset();
        }

        public static void Save() {
            Store(EditorPrefsKey, Data);
        }

        public static SettingsData Load(string key) {
            var jsonString = EditorPrefs.GetString(key);
            return JsonUtility.FromJson<SettingsData>(jsonString);
        }

        public static void Store(string key, SettingsData settingsData) {
            var jsonString = JsonUtility.ToJson(settingsData);
            EditorPrefs.SetString(key, jsonString);
        }
    }

    [Serializable]
    public class SettingsData {
        public EventModifiers ModifiersNormal;
        public EventModifiers ModifiersQuick;

        public KeyCode KeyCodeIncreaseFov;
        public KeyCode KeyCodeDecreaseFov;

        public float FovSpeed;
        public float FovQuickMultiplier;
        public float MinFov;
        public float MaxFov;

        public void Reset() {
            ModifiersNormal = EventModifiers.Alt | EventModifiers.Control;
            ModifiersQuick  = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift;

            KeyCodeIncreaseFov = KeyCode.O;
            KeyCodeDecreaseFov = KeyCode.P;

            FovSpeed = 0.15f;
            FovQuickMultiplier = 5.0f;
            MinFov = 2.0f;
            MaxFov = 160.0f;
        }
    }


    class SettingsGui : EditorWindow {
        static SettingsGui settingGui;

        [MenuItem(Settings.MenuItemName)]
        static void Open() {
            if(settingGui == null) {
                settingGui = CreateInstance<SettingsGui>();
            }
            settingGui.ShowUtility();
        }

        void OnGUI() {
            var d = Settings.Data;

            GUILayout.Space(4);

            GUILayout.Label("<<< Scene View FoV Control Settings >>>");

            GUILayout.Space(8);

            GUILayout.Label("FoV Speed:" + d.FovSpeed);
            d.FovSpeed = GUILayout.HorizontalSlider(d.FovSpeed, Settings.FovSpeedMin, Settings.FovSpeedMax);

            GUILayout.Space(8);

            GUILayout.Label("FoV <Shift> Modifier Multiplier:" + d.FovQuickMultiplier);
            d.FovQuickMultiplier = GUILayout.HorizontalSlider(d.FovQuickMultiplier, Settings.FovQuickMultiplierMin, Settings.FovQuickMultiplierMax);

            GUILayout.Space(8);

            GUILayout.Label("Min FoV:" + d.MinFov);
            d.MinFov = GUILayout.HorizontalSlider(d.MinFov, Settings.MinFovMin, Settings.MinFovMax);

            if(d.MinFov > d.MaxFov) {
                d.MaxFov = d.MinFov;
            }

            GUILayout.Space(8);

            GUILayout.Label("Max FoV:" + d.MaxFov);
            d.MaxFov = GUILayout.HorizontalSlider(d.MaxFov, Settings.MaxFovMin, Settings.MaxFovMax);

            if(d.MaxFov < d.MinFov) {
                d.MinFov = d.MaxFov;
            }

            GUILayout.Space(20);

            if(SceneViewFovControl.EnableFlag) {
                if(GUILayout.Button("Disable")) {
                    SceneViewFovControl.Enable(false);
                }
            } else {
                if(GUILayout.Button("Enable")) {
                    SceneViewFovControl.Enable(true);
                }
            }

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Save")) {
                    Settings.Save();
                }

                GUILayout.Space(20);

                if(GUILayout.Button("Default")) {
                    d.Reset();
                }

                GUILayout.Space(20);

                if(GUILayout.Button("Close")) {
                    this.Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
