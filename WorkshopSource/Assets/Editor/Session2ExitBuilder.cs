using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Session2ExitBuilder
{
    [MenuItem("Workshop/Add Session 2 Exit Button")]
    public static void AddExitButton()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/Session2.unity");

        // UI Canvas
        GameObject canvasGo = new GameObject("Session2_ExitCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Put it on top
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Exit Button
        GameObject bQuit = new GameObject("ExitButton");
        bQuit.transform.SetParent(canvasGo.transform, false);
        Image img = bQuit.AddComponent<Image>();
        img.color = new Color(0.75f, 0.15f, 0.25f);
        Button btnQuit = bQuit.AddComponent<Button>();
        RectTransform rt = bQuit.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.sizeDelta = new Vector2(250, 70);
        rt.anchoredPosition = new Vector2(145, -55);

        GameObject t = new GameObject("Text");
        t.transform.SetParent(bQuit.transform, false);
        TextMeshProUGUI tmp = t.AddComponent<TextMeshProUGUI>();
        tmp.text = "MAIN MENU";
        tmp.fontSize = 28;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform rtTxt = t.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;
        rtTxt.anchoredPosition = Vector2.zero;

        // Managers
        GameObject gm = GameObject.Find("GameManager") ?? new GameObject("GameManager");
        Workshop.Session4.Advanced.SceneLoader loader = gm.GetComponent<Workshop.Session4.Advanced.SceneLoader>();
        if (loader == null) loader = gm.AddComponent<Workshop.Session4.Advanced.SceneLoader>();

        // Wire Event
        UnityEditor.Events.UnityEventTools.AddIntPersistentListener(btnQuit.onClick, loader.LoadSceneByIndex, 0);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Session2.unity");
        Debug.Log("Session 2 Exit Button added successfully!");
    }
}
