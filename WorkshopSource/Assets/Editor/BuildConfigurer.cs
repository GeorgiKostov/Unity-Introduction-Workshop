using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class BuildConfigurer
{
    [MenuItem("Workshop/Configure Project for Export")]
    public static void ConfigureProject()
    {
        // Add Scenes to Build
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Session4.unity", true)
        };
        EditorBuildSettings.scenes = scenes;

        // Player Settings
        PlayerSettings.companyName = "Workshop";
        PlayerSettings.productName = "Collect-A-Thon";
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;

        // Note regarding Quality settings -> The user said "Edit > Project Settings > Quality, Set active level to High for the build".
        // This is tricky from script without knowing the exact index of "High" in the QualitySettings names, but usually High is index 2 or 3 in default URP projects.
        // I will attempt setting it to the name "High" if it exists.
        string[] names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == "High")
            {
                QualitySettings.SetQualityLevel(i, true);
                break;
            }
        }
        
        Debug.Log("Build Settings & Player Settings Configured Successfully!");
    }
}
