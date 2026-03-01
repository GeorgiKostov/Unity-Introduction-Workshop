using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Workshop.Session2.Collectibles;
using Workshop.Session2.UI;
using Workshop.Session2.Movement;
using Workshop.Session2.Camera;
using Workshop.Session3.Hazards;
using Workshop.Session3.GameFlow;
using Workshop.Session3.Feedback;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Workshop.Session4.Advanced;

public class Session3Builder
{
    [MenuItem("Workshop/Build Session 3")]
    public static void Build()
    {
        // Create and save the scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        newScene.name = "Session3";
        EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Session3.unity");

        // 1. LIGHTING
        GameObject dirLightObj = new GameObject("Directional Light");
        Light dirLight = dirLightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLightObj.transform.rotation = Quaternion.Euler(45, -60, 0);
        dirLight.color = new Color(1.0f, 0.9f, 0.75f);
        dirLight.intensity = 1.4f;
        dirLight.shadows = LightShadows.Soft;
        dirLight.shadowStrength = 0.7f;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.18f);

        // MATERIALS
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Session3"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.CreateFolder("Assets/Materials", "Session3");
        }

        Material matFloor = CreateMat("Mat_Floor", new Color(0.18f, 0.18f, 0.18f), 0, 0.15f);
        Material matWall = CreateMat("Mat_Wall", new Color(0.12f, 0.10f, 0.22f), 0, 0.05f);
        Material matPlatLow = CreateMat("Mat_Platform_Low", new Color(0.25f, 0.35f, 0.55f), 0.1f, 0.3f);
        Material matPlatMid = CreateMat("Mat_Platform_Mid", new Color(0.35f, 0.25f, 0.55f), 0.15f, 0.4f);
        Material matPlatHigh = CreateMat("Mat_Platform_High", new Color(0.55f, 0.35f, 0.20f), 0.3f, 0.6f);
        Material matRamp = CreateMat("Mat_Ramp", new Color(0.30f, 0.30f, 0.30f), 0, 0.1f);
        Material matMoving = CreateMat("Mat_MovingPlatform", new Color(0.2f, 0.7f, 0.5f), 0.2f, 0.5f);
        Material matHazard = CreateMat("Mat_Hazard", new Color(0.9f, 0.2f, 0.05f), 0, 0.05f);
        Material matSwing = CreateMat("Mat_SwingBar", new Color(0.8f, 0.5f, 0.0f), 0.4f, 0.6f);
        Material matPlayer = CreateMat("Mat_Player", new Color(0.0f, 0.85f, 0.45f), 0.1f, 0.6f);
        Material matColCommon = CreateMat("Mat_Collectible_Common", new Color(1.0f, 0.85f, 0.0f), 0.8f, 0.9f);
        Material matColRare = CreateMat("Mat_Collectible_Rare", new Color(0.0f, 0.8f, 1.0f), 0.9f, 1.0f);
        Material matColBonus = CreateMat("Mat_Collectible_Bonus", new Color(1.0f, 0.2f, 0.8f), 1.0f, 1.0f);

        // ENVIRONMENT
        GameObject arena = new GameObject("Arena");

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(6, 1, 6);
        floor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;
        floor.transform.parent = arena.transform;

        GameObject walls = new GameObject("Walls");
        walls.transform.parent = arena.transform;
        CreateOuterWall("Wall_North", new Vector3(0, 5, 30), new Vector3(60, 10, 1), matWall, walls.transform);
        CreateOuterWall("Wall_South", new Vector3(0, 5, -30), new Vector3(60, 10, 1), matWall, walls.transform);
        CreateOuterWall("Wall_East", new Vector3(30, 5, 0), new Vector3(1, 10, 60), matWall, walls.transform);
        CreateOuterWall("Wall_West", new Vector3(-30, 5, 0), new Vector3(1, 10, 60), matWall, walls.transform);

        GameObject tier1 = new GameObject("Tier1_Platforms");
        tier1.transform.parent = arena.transform;
        CreatePlatform("Plat_L1_A", new Vector3(10, 0.5f, 10), new Vector3(8, 1, 8), matPlatLow, tier1.transform);
        CreatePlatform("Plat_L1_B", new Vector3(-12, 0.5f, 8), new Vector3(6, 1, 10), matPlatLow, tier1.transform);
        CreatePlatform("Plat_L1_C", new Vector3(0, 0.5f, -15), new Vector3(10, 1, 6), matPlatLow, tier1.transform);
        CreatePlatform("Plat_L1_D", new Vector3(16, 0.5f, -8), new Vector3(5, 1, 5), matPlatLow, tier1.transform);

        GameObject ramps = new GameObject("Ramps");
        ramps.transform.parent = arena.transform;
        CreateRamp("Ramp_A", new Vector3(-8, 2, 2), new Vector3(25, 0, 0), new Vector3(4, 0.5f, 8), matRamp, ramps.transform);
        CreateRamp("Ramp_B", new Vector3(14, 2.5f, 0), new Vector3(25, 0, 0), new Vector3(4, 0.5f, 10), matRamp, ramps.transform);
        CreateRamp("Ramp_C", new Vector3(0, 6, -10), new Vector3(30, 0, 0), new Vector3(5, 0.5f, 8), matRamp, ramps.transform);

        GameObject tier2 = new GameObject("Tier2_Platforms");
        tier2.transform.parent = arena.transform;
        CreatePlatform("Plat_L2_A", new Vector3(-10, 4.5f, -4), new Vector3(8, 1, 8), matPlatMid, tier2.transform);
        CreatePlatform("Plat_L2_B", new Vector3(6, 4.5f, -6), new Vector3(7, 1, 7), matPlatMid, tier2.transform);
        CreatePlatform("Plat_L2_C", new Vector3(-18, 4.5f, -10), new Vector3(5, 1, 5), matPlatMid, tier2.transform);
        CreatePlatform("Plat_L2_D", new Vector3(-12, 4.5f, -8), new Vector3(3, 1, 3), matPlatMid, tier2.transform);

        GameObject tier3 = new GameObject("Tier3_Platforms");
        tier3.transform.parent = arena.transform;
        CreatePlatform("Plat_L3_A", new Vector3(0, 9.5f, -20), new Vector3(10, 1, 8), matPlatHigh, tier3.transform);
        CreatePlatform("Plat_L3_B", new Vector3(12, 9.5f, -18), new Vector3(5, 1, 5), matPlatHigh, tier3.transform);
        CreatePlatform("Plat_L3_C", new Vector3(-12, 9.5f, -18), new Vector3(5, 1, 5), matPlatHigh, tier3.transform);

        // 2. PLAYER & SPAWN
        GameObject spawnPoint = new GameObject("SpawnPoint");
        spawnPoint.transform.position = new Vector3(0, 1.5f, 20);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1.5f, 20);
        player.GetComponent<MeshRenderer>().sharedMaterial = matPlayer;
        player.tag = "Player";

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        CapsuleCollider cc = player.GetComponent<CapsuleCollider>();
        cc.isTrigger = false;
        cc.radius = 0.5f;
        cc.height = 2f;
        cc.direction = 1;

        PlayerMover mover = player.AddComponent<PlayerMover>();
        SetSerializedValue(mover, "m_moveSpeed", 7f);

        PlayerJumper jumper = player.AddComponent<PlayerJumper>();
        SetSerializedValue(jumper, "m_jumpForce", 8f);
        SetSerializedValue(jumper, "m_groundCheckDistance", 1.15f);

        PlayerSlider slider = player.AddComponent<PlayerSlider>();
        SetSerializedValue(slider, "m_slideSpeedMultiplier", 2.2f);
        SetSerializedValue(slider, "m_slideDuration", 0.7f);
        SetSerializedValue(slider, "m_slideCooldown", 1.2f);

        PlayerRespawner respawner = player.AddComponent<PlayerRespawner>();
        SetSerializedValue(respawner, "m_spawnPoint", spawnPoint.transform);
        SetSerializedValue(respawner, "m_respawnDelay", 0.4f);

        // 3. CAMERA
        GameObject mainCamObj = new GameObject("Main Camera");
        mainCamObj.tag = "MainCamera";
        Camera mainCam = mainCamObj.AddComponent<Camera>();
        mainCamObj.AddComponent<AudioListener>();
        mainCamObj.transform.position = new Vector3(0, 6, 16);
        mainCamObj.transform.rotation = Quaternion.Euler(15, 180, 0);

        CameraFollower camFollow = mainCamObj.AddComponent<CameraFollower>();
        SetSerializedValue(camFollow, "m_target", player.transform);
        SetSerializedValue(camFollow, "m_offset", new Vector3(0, 6, -10));
        SetSerializedValue(camFollow, "m_smoothSpeed", 0.10f);

        // 4. HAZARDS
        GameObject hazards = new GameObject("Hazards");

        CreateHazardZone("HazardZone_Lava_A", new Vector3(0, 0.05f, 5), new Vector3(8, 0.1f, 6), matHazard, hazards.transform);
        CreateHazardZone("HazardZone_Lava_B", new Vector3(-5, 0.05f, -8), new Vector3(6, 0.1f, 5), matHazard, hazards.transform);
        CreateHazardZone("HazardZone_Lava_C", new Vector3(-14, 4.55f, -6), new Vector3(3, 0.1f, 4), matHazard, hazards.transform);
        
        GameObject voidHz = CreateHazardZone("HazardZone_Void", new Vector3(0, -5, 0), new Vector3(100, 1, 100), matHazard, hazards.transform);
        voidHz.GetComponent<MeshRenderer>().enabled = false;

        CreateMovingPlatform("MovPlat_A", new Vector3(0, 1.5f, 0), new Vector3(0, 1.5f, -10), new Vector3(3, 0.5f, 3), 1.5f, matMoving, hazards.transform);
        CreateMovingPlatform("MovPlat_B", new Vector3(-6, 5f, -6), new Vector3(-16, 5f, -6), new Vector3(3, 0.5f, 3), 2.0f, matMoving, hazards.transform);
        CreateMovingPlatform("MovPlat_C", new Vector3(6, 5f, -12), new Vector3(6, 10f, -12), new Vector3(4, 0.5f, 4), 1.0f, matMoving, hazards.transform);

        CreateSwingingBar("SwingBar_A", new Vector3(6, 7, -6), new Vector3(3, 0, 0), new Vector3(6, 0.3f, 0.3f), matSwing, hazards.transform, 70f, 0.8f, 0f, new Vector3(0, 1, 0));
        CreateSwingingBar("SwingBar_B", new Vector3(0, 14, -18), new Vector3(0, -3, 0), new Vector3(5, 0.3f, 0.3f), matSwing, hazards.transform, 50f, 1.1f, 1.57f, new Vector3(0, 0, 1));

        // 5. COLLECTIBLES
        GameObject collectiblesAll = new GameObject("Collectibles");
        GameObject colCommonGrp = new GameObject("Common"); colCommonGrp.transform.parent = collectiblesAll.transform;
        GameObject colRareGrp = new GameObject("Rare"); colRareGrp.transform.parent = collectiblesAll.transform;
        GameObject colBonusGrp = new GameObject("Bonus"); colBonusGrp.transform.parent = collectiblesAll.transform;

        Vector3[] commonPos = { new Vector3(3, 1, 18), new Vector3(-3, 1, 18), new Vector3(10, 1.5f, 10), new Vector3(-12, 1.5f, 8), new Vector3(0, 1, -15), new Vector3(16, 1.5f, -8), new Vector3(-5, 1, -5), new Vector3(5, 1, -5) };
        for (int i=0; i<commonPos.Length; i++) CreateCollectible($"Col_C{i+1:D2}", commonPos[i], 0.5f, matColCommon, 10, colCommonGrp.transform);

        Vector3[] rarePos = { new Vector3(-10, 5.5f, -4), new Vector3(6, 5.5f, -6), new Vector3(-18, 5.5f, -10), new Vector3(-12, 5.5f, -8) };
        for (int i=0; i<rarePos.Length; i++) CreateCollectible($"Col_R{i+1:D2}", rarePos[i], 0.6f, matColRare, 25, colRareGrp.transform);

        Vector3[] bonusPos = { new Vector3(0, 10.5f, -20), new Vector3(12, 10.5f, -18), new Vector3(-12, 10.5f, -18) };
        for (int i=0; i<bonusPos.Length; i++) CreateCollectible($"Col_B{i+1:D2}", bonusPos[i], 0.7f, matColBonus, 50, colBonusGrp.transform);

        // 6. SCORE & GAME MGR
        GameObject scoreMgrObj = new GameObject("ScoreManager");
        scoreMgrObj.transform.position = Vector3.zero;
        ScoreManager scoreMgr = scoreMgrObj.AddComponent<ScoreManager>();

        GameObject gameMgrObj = new GameObject("GameManager");
        gameMgrObj.transform.position = Vector3.zero;
        CountdownTimer timer = gameMgrObj.AddComponent<CountdownTimer>();
        SetSerializedValue(timer, "m_startTime", 90f);

        WinCondition winCond = gameMgrObj.AddComponent<WinCondition>();
        SetSerializedValue(winCond, "m_timerToStop", timer);

        // 7. UI
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject scoreTextObj = CreateTextObj("ScoreText", canvasObj.transform, new Vector2(0, 1), new Vector2(120, -45), new Vector2(300, 60), 38, Color.white, "Score: 0");
        ScoreDisplay scoreDisplay = scoreTextObj.AddComponent<ScoreDisplay>();
        SetSerializedValue(scoreDisplay, "m_prefix", "Score: ");

        GameObject timerTextObj = CreateTextObj("TimerText", canvasObj.transform, new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(200, 60), 42, Color.white, "Time: 90");
        TimerDisplay timerDisplay = timerTextObj.AddComponent<TimerDisplay>();
        SetSerializedValue(timerDisplay, "m_prefix", "Time: ");
        SetSerializedValue(timerDisplay, "m_warningThreshold", 15f);
        SetSerializedValue(timerDisplay, "m_normalColor", Color.white);
        SetSerializedValue(timerDisplay, "m_warningColor", new Color(1.0f, 0.15f, 0.15f, 1.0f));

        GameObject gameOverPnl = CreatePanelObj("GameOverPanel", canvasObj.transform, new Color(0,0,0,0.75f));
        CreateTextObj("GameOverTitle", gameOverPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(600, 120), 72, new Color(1f, 0.2f, 0.2f, 1f), "TIME'S UP", FontStyles.Bold);
        CreateTextObj("FinalScoreText", gameOverPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(500, 80), 48, Color.white, "Final Score: 0");
        CreateTextObj("SubtitleText", gameOverPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, -120), new Vector2(600, 60), 32, new Color(0.8f, 0.8f, 0.8f, 1f), "Press Play again to retry");
        gameOverPnl.SetActive(false);

        GameObject winPnl = CreatePanelObj("WinPanel", canvasObj.transform, new Color(0,0,0,0.75f));
        CreateTextObj("WinTitle", winPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(700, 120), 72, new Color(0f, 1f, 0.5f, 1f), "YOU WIN!", FontStyles.Bold);
        CreateTextObj("WinSubtitle", winPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(600, 80), 40, Color.white, "All collectibles found!");
        CreateTextObj("WinScore", winPnl.transform, new Vector2(0.5f, 0.5f), new Vector2(0, -120), new Vector2(500, 60), 36, new Color(1f, 0.85f, 0f, 1f), "Score: 0");
        winPnl.SetActive(false);

        // Exit Button
        GameObject bQuit = CreatePanelObj("ExitButton", canvasObj.transform, new Color(0.75f, 0.15f, 0.25f, 1f));
        // Reset RectTransform so it's a clear button in the top left instead of a full panel stretch
        RectTransform rtQuit = bQuit.GetComponent<RectTransform>();
        rtQuit.anchorMin = new Vector2(0f, 1f);
        rtQuit.anchorMax = new Vector2(0f, 1f);
        rtQuit.pivot = new Vector2(0f, 1f);
        rtQuit.sizeDelta = new Vector2(250, 70);
        rtQuit.anchoredPosition = new Vector2(20, -20);
        rtQuit.offsetMin = new Vector2(20, rtQuit.offsetMin.y);
        rtQuit.offsetMax = new Vector2(rtQuit.offsetMax.x, -20);
        bQuit.AddComponent<Button>();
        CreateTextObj("Text", bQuit.transform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250, 70), 32, Color.white, "MAIN MENU", FontStyles.Bold);

        // Wire Events
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(bQuit.GetComponent<Button>().onClick, gameMgrObj.AddComponent<SceneLoader>().LoadSceneByName, "MainMenu");
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(scoreMgr.ScoreChanged, scoreDisplay.UpdateScoreText);
        
        // Use generic AddPersistentListener with reflection approach for SetActive since method has 1 implicit param
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(timer.TimerExpired, new UnityEngine.Events.UnityAction<bool>(gameOverPnl.SetActive), true);
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(timer.TimerTicked, timerDisplay.UpdateTimerDisplay);

        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(winCond.AllCollected, new UnityEngine.Events.UnityAction<bool>(winPnl.SetActive), true);

        // 8. AUDIO ZONES
        GameObject audioZones = new GameObject("AudioZones");
        CreateTriggerZone<AudioOnTrigger>("AudioZone_TierUp_A", new Vector3(-10, 3, -2), new Vector3(12, 6, 4), audioZones.transform, (c) => {
            SetSerializedValue(c, "m_volume", 0.8f);
            SetSerializedValue(c, "m_isSinglePlay", true);
        });
        CreateTriggerZone<AudioOnTrigger>("AudioZone_TierUp_B", new Vector3(0, 8, -16), new Vector3(16, 6, 4), audioZones.transform, (c) => {
            SetSerializedValue(c, "m_volume", 1.0f);
            SetSerializedValue(c, "m_isSinglePlay", true);
        });

        // 9. PARTICLE ZONES
        GameObject partZones = new GameObject("ParticleZones");
        CreateTriggerZone<ParticleOnTrigger>("ParticleZone_Win", new Vector3(0, 8, -18), new Vector3(10, 4, 3), partZones.transform, (c) => {
            SetSerializedValue(c, "m_shouldSpawnAtPlayer", true);
            SetSerializedValue(c, "m_isSingleTrigger", false);
        });

        EditorSceneManager.MarkSceneDirty(newScene);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Session 3 scene generation complete!");
    }

    private static Material CreateMat(string name, Color color, float metallic, float smoothness)
    {
        string path = $"Assets/Materials/Session3/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    private static void CreateOuterWall(string n, Vector3 pos, Vector3 scale, Material m, Transform p)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = n;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        go.transform.parent = p;
    }

    private static void CreatePlatform(string n, Vector3 pos, Vector3 scale, Material m, Transform p)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = n;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        go.transform.parent = p;
    }

    private static void CreateRamp(string n, Vector3 pos, Vector3 rot, Vector3 scale, Material m, Transform p)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = n;
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(rot);
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        go.transform.parent = p;
    }

    private static GameObject CreateHazardZone(string name, Vector3 pos, Vector3 scale, Material m, Transform parent)
    {
        GameObject hz = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hz.name = name;
        hz.transform.position = pos;
        hz.transform.localScale = scale;
        hz.GetComponent<MeshRenderer>().sharedMaterial = m;
        BoxCollider bc = hz.GetComponent<BoxCollider>();
        bc.isTrigger = true;
        hz.AddComponent<HazardZone>();
        hz.transform.parent = parent;
        return hz;
    }

    private static void CreateMovingPlatform(string name, Vector3 posA, Vector3 posB, Vector3 scale, float speed, Material m, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = posA;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        go.GetComponent<BoxCollider>().isTrigger = false;
        MovingPlatform mp = go.AddComponent<MovingPlatform>();
        SetSerializedValue(mp, "m_pointA", posA);
        SetSerializedValue(mp, "m_pointB", posB);
        SetSerializedValue(mp, "m_speed", speed);
        SetSerializedValue(mp, "m_smoothPingPong", true);
        go.transform.parent = parent;
    }

    private static void CreateSwingingBar(string name, Vector3 pivotPos, Vector3 loadPos, Vector3 scale, Material m, Transform parent, float angle, float speed, float phase, Vector3 axis)
    {
        GameObject pivot = new GameObject(name + "_Pivot");
        pivot.transform.position = pivotPos;
        pivot.transform.parent = parent;

        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name + "_Bar";
        bar.transform.parent = pivot.transform;
        bar.transform.localPosition = loadPos;
        bar.transform.localScale = scale;
        bar.GetComponent<MeshRenderer>().sharedMaterial = m;
        bar.GetComponent<BoxCollider>().isTrigger = false;
        
        Rigidbody rb = bar.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        SwingingBar sb = pivot.AddComponent<SwingingBar>();
        SetSerializedValue(sb, "m_swingAngle", angle);
        SetSerializedValue(sb, "m_swingSpeed", speed);
        SetSerializedValue(sb, "m_phaseOffset", phase);
        SetSerializedValue(sb, "m_swingAxis", axis);
    }

    private static void CreateCollectible(string name, Vector3 pos, float scale, Material m, int points, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale, scale, scale);
        go.GetComponent<MeshRenderer>().sharedMaterial = m;
        SphereCollider sc = go.GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.5f;
        Collectible col = go.AddComponent<Collectible>();
        SetSerializedValue(col, "m_pointValue", points);
        go.transform.parent = parent;
    }

    private static GameObject CreateTextObj(string name, Transform parent, Vector2 anchorPivot, Vector2 anchoredPos, Vector2 sizeDelta, float fontSize, Color color, string text, FontStyles style = FontStyles.Normal)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorPivot;
        rt.anchorMax = anchorPivot;
        rt.pivot = anchorPivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        return go;
    }

    private static GameObject CreatePanelObj(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateTriggerZone<T>(string name, Vector3 pos, Vector3 scale, Transform parent, System.Action<T> configAction) where T : MonoBehaviour
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().enabled = false;
        go.transform.parent = parent;
        T comp = go.AddComponent<T>();
        configAction?.Invoke(comp);
        return go;
    }

    private static void SetSerializedValue(Object obj, string propName, object value)
    {
        SerializedObject so = new SerializedObject(obj);
        SerializedProperty sp = so.FindProperty(propName);
        if (sp != null)
        {
            if (value is float f) sp.floatValue = f;
            else if (value is int i) sp.intValue = i;
            else if (value is bool b) sp.boolValue = b;
            else if (value is string s) sp.stringValue = s;
            else if (value is Vector2 v2) sp.vector2Value = v2;
            else if (value is Vector3 v3) sp.vector3Value = v3;
            else if (value is Color c) sp.colorValue = c;
            else if (value is Object o) sp.objectReferenceValue = o;
            so.ApplyModifiedProperties();
        }
    }
}
