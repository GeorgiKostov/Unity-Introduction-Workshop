using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Workshop.Session4.Advanced;

public class MainMenuBuilder
{
    [MenuItem("Workshop/Build Main Menu")]
    public static void Build()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool success = EditorSceneManager.SaveScene(newScene, "Assets/Scenes/MainMenu.unity");
        if (!success)
        {
            Debug.LogError("Failed to save MainMenu.unity. Ensure the folder exists.");
            return;
        }

        // Camera
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
        cam.orthographic = true;

        // Managers
        GameObject gm = new GameObject("GameManager");
        SceneLoader loader = gm.AddComponent<SceneLoader>();

        // UI Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGo.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.08f, 0.05f, 0.15f, 1f);
        RectTransform rtBg = bg.GetComponent<RectTransform>();
        rtBg.anchorMin = Vector2.zero;
        rtBg.anchorMax = Vector2.one;
        rtBg.sizeDelta = Vector2.zero;
        rtBg.anchoredPosition = Vector2.zero;

        // Title
        GameObject titleGo = new GameObject("TitleText");
        titleGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI title = titleGo.AddComponent<TextMeshProUGUI>();
        title.text = "COLLECT-A-THON 3D";
        title.fontSize = 120;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.9f, 0.8f, 0.2f);
        RectTransform rtTitle = titleGo.GetComponent<RectTransform>();
        rtTitle.anchorMin = new Vector2(0.5f, 0.5f);
        rtTitle.anchorMax = new Vector2(0.5f, 0.5f);
        rtTitle.sizeDelta = new Vector2(1200, 200);
        rtTitle.anchoredPosition = new Vector2(0, 250);

        // Subtitle
        GameObject subGo = new GameObject("SubtitleText");
        subGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI subtitle = subGo.AddComponent<TextMeshProUGUI>();
        subtitle.text = "Session 4 Polish Update";
        subtitle.fontSize = 48;
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.color = new Color(0.8f, 0.8f, 0.8f);
        RectTransform rtSub = subGo.GetComponent<RectTransform>();
        rtSub.anchorMin = new Vector2(0.5f, 0.5f);
        rtSub.anchorMax = new Vector2(0.5f, 0.5f);
        rtSub.sizeDelta = new Vector2(800, 80);
        rtSub.anchoredPosition = new Vector2(0, 120);

        // Session 1 Button
        GameObject bSes1 = CreateButton("Session1Button", canvasGo, "SESSION 1", new Color(0.15f, 0.4f, 0.6f), new Vector2(400, 80), new Vector2(-250, -20));
        Button btnSes1 = bSes1.GetComponent<Button>();

        // Session 2 Button
        GameObject bSes2 = CreateButton("Session2Button", canvasGo, "SESSION 2", new Color(0.15f, 0.5f, 0.5f), new Vector2(400, 80), new Vector2(250, -20));
        Button btnSes2 = bSes2.GetComponent<Button>();

        // Session 3 Button
        GameObject bSes3 = CreateButton("Session3Button", canvasGo, "SESSION 3", new Color(0.5f, 0.4f, 0.2f), new Vector2(400, 80), new Vector2(-250, -120));
        Button btnSes3 = bSes3.GetComponent<Button>();

        // Session 4 Button
        GameObject bSes4 = CreateButton("Session4Button", canvasGo, "SESSION 4 (FINAL)", new Color(0.15f, 0.55f, 0.35f), new Vector2(400, 80), new Vector2(250, -120));
        Button btnSes4 = bSes4.GetComponent<Button>();

        // Quit Button
        GameObject bQuit = CreateButton("QuitButton", canvasGo, "QUIT GAME", new Color(0.75f, 0.15f, 0.25f), new Vector2(400, 80), new Vector2(0, -240));
        Button btnQuit = bQuit.GetComponent<Button>();

        // Wire Events
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btnSes1.onClick, loader.LoadSceneByName, "Session1");
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btnSes2.onClick, loader.LoadSceneByName, "Session2");
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btnSes3.onClick, loader.LoadSceneByName, "Session3");
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btnSes4.onClick, loader.LoadSceneByName, "Session4");
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnQuit.onClick, loader.QuitGame);

        EditorSceneManager.SaveScene(newScene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("Main Menu built successfully with all 4 sessions!");
    }

    private static GameObject CreateButton(string name, GameObject parent, string text, Color btnColor, Vector2 sizeDelta, Vector2 pos)
    {
        GameObject b = new GameObject(name);
        b.transform.SetParent(parent.transform, false);
        Image img = b.AddComponent<Image>();
        img.color = btnColor;
        Button btn = b.AddComponent<Button>();
        RectTransform rt = b.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = pos;

        GameObject t = new GameObject("Text");
        t.transform.SetParent(b.transform, false);
        TextMeshProUGUI tmp = t.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 45;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform rtTxt = t.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;
        rtTxt.anchoredPosition = Vector2.zero;

        return b;
    }
}
