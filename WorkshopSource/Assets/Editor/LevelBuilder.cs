using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Workshop.Session2.Collectibles;
using Workshop.Session2.UI;

public class LevelBuilder
{
    [MenuItem("Workshop/Build Rest Of Level")]
    public static void Build()
    {
        // 4. ScoreManager
        GameObject scoreMgrObj = new GameObject("ScoreManager");
        scoreMgrObj.transform.position = Vector3.zero;
        ScoreManager scoreMgr = scoreMgrObj.AddComponent<ScoreManager>();

        // 5. UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 5. ScoreText
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI tmp = scoreTextObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Score: 0";
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        
        RectTransform rt = scoreTextObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(120, -40);
        rt.sizeDelta = new Vector2(300, 60);

        ScoreDisplay scoreDisplay = scoreTextObj.AddComponent<ScoreDisplay>();
        
        // Wire the event via serialized objects since UnityEventTools needs it
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(scoreMgr.ScoreChanged, scoreDisplay.UpdateScoreText);

        // 6. Collectibles
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");

        Material colMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Collectible.mat");
        if (colMat == null) {
            colMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            colMat.SetColor("_BaseColor", new Color(1.0f, 0.85f, 0.0f));
            colMat.SetFloat("_Metallic", 0.8f);
            colMat.SetFloat("_Smoothness", 0.9f);
            AssetDatabase.CreateAsset(colMat, "Assets/Materials/Mat_Collectible.mat");
        }

        GameObject masterCol = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        masterCol.name = "Collectible";
        masterCol.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        masterCol.GetComponent<MeshRenderer>().sharedMaterial = colMat;
        
        SphereCollider sc = masterCol.GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.5f;

        Collectible colScript = masterCol.AddComponent<Collectible>();
        SerializedObject so = new SerializedObject(colScript);
        so.FindProperty("m_pointValue").intValue = 10;
        so.ApplyModifiedProperties();

        GameObject colGroup = new GameObject("Collectibles");

        Vector3[] positions = new Vector3[] {
            new Vector3(3, 0.5f, 3), new Vector3(-3, 0.5f, 3),
            new Vector3(3, 0.5f, -3), new Vector3(-3, 0.5f, -3),
            new Vector3(0, 0.5f, 8), new Vector3(8, 2.5f, 8),
            new Vector3(6, 2.5f, 10), new Vector3(-10, 4.5f, -6),
            new Vector3(-12, 4.5f, -4), new Vector3(0, 6.5f, -14)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject instance = Object.Instantiate(masterCol, positions[i], Quaternion.identity);
            instance.name = $"Collectible_{(i+1):D2}";
            instance.transform.SetParent(colGroup.transform);
        }
        Object.DestroyImmediate(masterCol);

        // 7. Lighting
        Light dirLight = Object.FindFirstObjectByType<Light>();
        if (dirLight != null && dirLight.type == LightType.Directional)
        {
            dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            dirLight.color = new Color(1.0f, 0.95f, 0.85f);
            dirLight.intensity = 1.2f;
            dirLight.shadows = LightShadows.Soft;
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.15f, 0.15f, 0.25f);
        
        Debug.Log("Level Build Complete!");
    }
}
