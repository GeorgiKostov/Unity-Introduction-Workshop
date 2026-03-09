using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Workshop.Session2_New;

/// <summary>
/// Builds the Session 2 showcase scene from scratch.
/// Run via Workshop > Build Session 2 to create, populate, and save the scene.
/// Modelled after the Session 4 Builder pattern.
/// </summary>
public class Session2Builder
{
    [MenuItem("Workshop/Build Session 2")]
    public static void Build()
    {
        // Create and immediately save an empty scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Session2.unity");
        if (!saved)
        {
            Debug.LogError("Session2Builder: Failed to save scene. Make sure Assets/Scenes/ exists.");
            return;
        }

        SetupTagsAndLayers();
        BuildLighting();
        BuildMaterials();
        BuildEnvironment();

        var spawnPoint = BuildSpawnPoint();

        BuildMovementShowcases();
        BuildTriggerShowcases();
        BuildPlayer(spawnPoint);
        BuildScoreSystemAndUI();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Session2.unity");
        Debug.Log("Session 2 built successfully!");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TAGS & LAYERS
    // ─────────────────────────────────────────────────────────────────────────

    private static void SetupTagsAndLayers()
    {
        var tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        var tags = tagManager.FindProperty("tags");
        AddTag(tags, "Player");
        AddTag(tags, "Destructible");

        var layers = tagManager.FindProperty("layers");
        AddLayer(layers, "Ground");

        tagManager.ApplyModifiedProperties();
    }

    private static void AddTag(SerializedProperty tags, string tag)
    {
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
    }

    private static void AddLayer(SerializedProperty layers, string layerName)
    {
        for (int i = 8; i < layers.arraySize; i++)
        {
            var sp = layers.GetArrayElementAtIndex(i);
            if (sp.stringValue == layerName) return;
            if (sp.stringValue == string.Empty) { sp.stringValue = layerName; return; }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LIGHTING
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildLighting()
    {
        var sunGo = new GameObject("Directional Light");
        var light = sunGo.AddComponent<Light>();
        light.type = LightType.Directional;
        sunGo.transform.rotation = Quaternion.Euler(52, -30, 0);
        light.color = new Color(1.0f, 0.95f, 0.85f);
        light.intensity = 1.4f;
        light.shadows = LightShadows.Soft;
        RenderSettings.sun = light;
        RenderSettings.ambientIntensity = 0.8f;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MATERIALS
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildMaterials()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Session2"))
            AssetDatabase.CreateFolder("Assets/Materials", "Session2");

        // Environment
        CreateMat("Mat_Floor",          new Color(0.18f, 0.18f, 0.20f), 0.0f, 0.6f);
        CreateMat("Mat_Wall",           new Color(0.12f, 0.10f, 0.20f), 0.1f, 0.3f);
        CreateMat("Mat_Platform",       new Color(0.25f, 0.35f, 0.55f), 0.2f, 0.5f);
        CreateMat("Mat_SpawnPad",       new Color(0.2f,  0.8f,  0.3f),  0.0f, 0.5f, new Color(0.05f, 0.6f, 0.1f) * 0.5f);

        // Movement Showcases
        CreateMat("Mat_Oscillator",     new Color(0.2f,  0.6f,  1.0f),  0.1f, 0.7f, new Color(0.0f, 0.3f, 0.8f) * 0.4f);
        CreateMat("Mat_Rotator",        new Color(1.0f,  0.5f,  0.1f),  0.2f, 0.8f, new Color(0.8f, 0.3f, 0.0f) * 0.4f);
        CreateMat("Mat_Orbiter",        new Color(0.6f,  0.2f,  1.0f),  0.3f, 0.9f, new Color(0.4f, 0.0f, 0.8f) * 0.5f);
        CreateMat("Mat_OrbiterPivot",   new Color(0.3f,  0.3f,  0.35f), 0.6f, 0.3f);

        // Trigger Zones (opaque floor pads, coloured by type)
        CreateMat("Mat_Zone_Color",     new Color(0.9f,  0.2f,  0.2f),  0.0f, 0.6f, new Color(0.6f, 0.0f, 0.0f) * 0.4f);
        CreateMat("Mat_Zone_Sound",     new Color(0.2f,  0.8f,  0.2f),  0.0f, 0.6f, new Color(0.0f, 0.5f, 0.0f) * 0.4f);
        CreateMat("Mat_Zone_Push",      new Color(0.9f,  0.8f,  0.1f),  0.0f, 0.6f, new Color(0.6f, 0.5f, 0.0f) * 0.4f);
        CreateMat("Mat_Zone_Destruct",  new Color(0.8f,  0.2f,  0.9f),  0.0f, 0.6f, new Color(0.5f, 0.0f, 0.6f) * 0.4f);
        CreateMat("Mat_Zone_Hazard",    new Color(1.0f,  0.15f, 0.0f),  0.0f, 0.6f, new Color(0.8f, 0.1f, 0.0f) * 0.8f);
        CreateMat("Mat_Destructible",   new Color(0.5f,  0.25f, 0.0f),  0.4f, 0.5f);

        // Player
        CreateMat("Mat_Player",         new Color(0.0f,  0.85f, 0.5f),  0.2f, 0.7f, new Color(0.0f, 0.4f, 0.2f) * 0.3f);

        // Collectible
        CreateMat("Mat_Collectible",    new Color(1.0f,  0.85f, 0.0f),  1.0f, 1.0f, new Color(1.0f, 0.7f, 0.0f) * 0.8f);

        // Physics Material (no friction)
        var pMat = new PhysicsMaterial("PhysicsMat_Player")
        {
            staticFriction  = 0f,
            dynamicFriction = 0f,
            bounciness      = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounceCombine   = PhysicsMaterialCombine.Minimum
        };
        AssetDatabase.CreateAsset(pMat, "Assets/Materials/Session2/PhysicsMat_Player.physicMaterial");

        AssetDatabase.SaveAssets();
    }

    private static Material CreateMat(string name, Color baseColor, float metallic, float smoothness, Color? emission = null)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Metallic",   metallic);
        mat.SetFloat("_Smoothness", smoothness);
        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission.Value);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        AssetDatabase.CreateAsset(mat, $"Assets/Materials/Session2/{name}.mat");
        return mat;
    }

    private static Material GetMat(string name)
        => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/Session2/{name}.mat");

    // ─────────────────────────────────────────────────────────────────────────
    // ENVIRONMENT
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildEnvironment()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        var arena = new GameObject("Arena");

        // Main floor
        var floor = CreateCube("Floor", Vector3.zero, new Vector3(5, 0.5f, 5), arena, GetMat("Mat_Floor"));
        floor.layer = groundLayer;
        floor.transform.position = new Vector3(0, -0.25f, 0);

        // Walls
        var walls = new GameObject("Walls"); walls.transform.SetParent(arena.transform);
        MakeWall("Wall_North", new Vector3(0,  2.5f,  25), new Vector3(50, 5, 1), groundLayer, walls);
        MakeWall("Wall_South", new Vector3(0,  2.5f, -25), new Vector3(50, 5, 1), groundLayer, walls);
        MakeWall("Wall_East",  new Vector3(25, 2.5f,   0), new Vector3(1,  5, 50), groundLayer, walls);
        MakeWall("Wall_West",  new Vector3(-25,2.5f,   0), new Vector3(1,  5, 50), groundLayer, walls);

        // Section divider platforms (to visually separate showcases)
        var platforms = new GameObject("Section_Platforms"); platforms.transform.SetParent(arena.transform);
        var divA = CreateCube("Divider_Movement_Triggers",  new Vector3(0, 0, 5),    new Vector3(50, 0.3f, 0.5f), platforms, GetMat("Mat_Wall"));
        divA.layer = groundLayer;
    }

    private static void MakeWall(string name, Vector3 pos, Vector3 scale, int layer, GameObject parent)
    {
        var w = CreateCube(name, pos, scale, parent, GetMat("Mat_Wall"));
        w.layer = layer;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SPAWN POINT
    // ─────────────────────────────────────────────────────────────────────────

    private static Transform BuildSpawnPoint()
    {
        // Visual spawn pad
        var padGo = CreateCube("SpawnPad", new Vector3(0, 0.15f, -20), new Vector3(3, 0.3f, 3), null, GetMat("Mat_SpawnPad"));
        padGo.GetComponent<Collider>().enabled = true;
        padGo.layer = LayerMask.NameToLayer("Ground");

        var sp = new GameObject("SpawnPoint");
        sp.transform.position = new Vector3(0, 1.5f, -20);
        return sp.transform;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PLAYER
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildPlayer(Transform spawnPoint)
    {
        // Camera first (PlayerMover needs camera ref)
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        camGo.transform.position = new Vector3(0, 12, -32);
        camGo.transform.rotation = Quaternion.Euler(20, 0, 0);

        // Player capsule
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag  = "Player";
        player.transform.position = spawnPoint.position;
        player.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Player");

        // Rigidbody
        var rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Physics material
        var cc = player.GetComponent<CapsuleCollider>();
        cc.material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Materials/Session2/PhysicsMat_Player.physicMaterial");

        // PlayerMover
        var mover = player.AddComponent<PlayerMover>();
        var soMover = new SerializedObject(mover);
        soMover.FindProperty("cameraTransform").objectReferenceValue = camGo.transform;
        soMover.FindProperty("moveSpeed").floatValue = 6f;
        soMover.ApplyModifiedProperties();

        // PlayerJumper
        var jumper = player.AddComponent<PlayerJumper>();
        var soJumper = new SerializedObject(jumper);
        soJumper.FindProperty("jumpForce").floatValue = 7f;
        soJumper.FindProperty("groundCheckDistance").floatValue = 1.15f;
        soJumper.FindProperty("groundCheckRadius").floatValue = 0.45f;
        soJumper.FindProperty("groundLayer").intValue = LayerMask.GetMask("Ground");
        soJumper.ApplyModifiedProperties();

        // PlayerRespawner
        var respawner = player.AddComponent<PlayerRespawner>();
        var soResp = new SerializedObject(respawner);
        soResp.FindProperty("spawnPoint").objectReferenceValue = spawnPoint;
        soResp.FindProperty("fallThreshold").floatValue = -8f;
        soResp.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MOVEMENT SHOWCASES  (back half of arena Z: -18 to +3)
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildMovementShowcases()
    {
        var group = new GameObject("Movement_Showcases");

        // ── OSCILLATORS ──────────────────────────────────────────────────────
        var oscGroup = new GameObject("Oscillators"); oscGroup.transform.SetParent(group.transform);

        // 1. Horizontal (X)
        var oscH = CreateCube("Oscillator_Horizontal", new Vector3(-18, 1, -10), Vector3.one, oscGroup, GetMat("Mat_Oscillator"));
        SetProps(oscH.AddComponent<Oscillator>(), ("amplitude", 3f), ("frequency", 1f));

        // 2. Vertical (Y) – repurposed: we oscillate along default X; rotate 90° to look vertical
        var oscV = CreateCube("Oscillator_Vertical", new Vector3(-12, 1, -10), Vector3.one, oscGroup, GetMat("Mat_Oscillator"));
        oscV.transform.rotation = Quaternion.Euler(0, 0, 90);
        SetProps(oscV.AddComponent<Oscillator>(), ("amplitude", 3f), ("frequency", 1.5f));

        // 3. Fast small amplitude
        var oscF = CreateCube("Oscillator_Fast", new Vector3(-6, 1, -10), Vector3.one, oscGroup, GetMat("Mat_Oscillator"));
        SetProps(oscF.AddComponent<Oscillator>(), ("amplitude", 1f), ("frequency", 4f));

        // ── ROTATORS ──────────────────────────────────────────────────────────
        var rotGroup = new GameObject("Rotators"); rotGroup.transform.SetParent(group.transform);

        // 1. Y-axis spin
        var rotY = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rotY.name = "Rotator_Y_Axis"; rotY.transform.position = new Vector3(0, 1, -10);
        rotY.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Rotator");
        rotY.transform.SetParent(rotGroup.transform);
        SetProps(rotY.AddComponent<Rotator>(), ("rotationSpeed", new Vector3(0, 90, 0)));

        // 2. Z-axis spin
        var rotZ = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rotZ.name = "Rotator_Z_Axis"; rotZ.transform.position = new Vector3(6, 1, -10);
        rotZ.transform.rotation = Quaternion.Euler(90, 0, 0); // lay flat
        rotZ.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Rotator");
        rotZ.transform.SetParent(rotGroup.transform);
        SetProps(rotZ.AddComponent<Rotator>(), ("rotationSpeed", new Vector3(0, 0, 180)));

        // 3. Multi-axis
        var rotAll = CreateCube("Rotator_Chaotic", new Vector3(12, 1.5f, -10), new Vector3(1.5f, 1.5f, 1.5f), rotGroup, GetMat("Mat_Rotator"));
        SetProps(rotAll.AddComponent<Rotator>(), ("rotationSpeed", new Vector3(45, 90, 30)));

        // ── ORBITERS ─────────────────────────────────────────────────────────
        var orbGroup = new GameObject("Orbiters"); orbGroup.transform.SetParent(group.transform);

        CreateOrbiterSetup("Orbiter_CloseFast", new Vector3(20, 1, -10), 2f, 180f, orbGroup, GetMat("Mat_Orbiter"), GetMat("Mat_OrbiterPivot"));
        CreateOrbiterSetup("Orbiter_FarSlow",   new Vector3(20, 1,  -4), 4f,  40f, orbGroup, GetMat("Mat_Orbiter"), GetMat("Mat_OrbiterPivot"));
    }

    private static void CreateOrbiterSetup(string name, Vector3 pivotPos, float radius, float speed, GameObject parent, Material orbMat, Material pivotMat)
    {
        // Pivot marker
        var pivotGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pivotGo.name = name + "_Pivot";
        pivotGo.transform.position = pivotPos;
        pivotGo.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        pivotGo.GetComponent<MeshRenderer>().sharedMaterial = pivotMat;
        Object.DestroyImmediate(pivotGo.GetComponent<SphereCollider>());
        pivotGo.transform.SetParent(parent.transform);

        // Orbiting sphere
        var orbGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orbGo.name = name;
        orbGo.transform.position = pivotPos + new Vector3(radius, 0, 0);
        orbGo.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        orbGo.GetComponent<MeshRenderer>().sharedMaterial = orbMat;
        orbGo.transform.SetParent(parent.transform);

        var orb = orbGo.AddComponent<ObjectOrbiter>();
        var so = new SerializedObject(orb);
        so.FindProperty("target").objectReferenceValue     = pivotGo.transform;
        so.FindProperty("orbitRadius").floatValue          = radius;
        so.FindProperty("orbitSpeed").floatValue           = speed;
        so.FindProperty("orbitAxis").vector3Value          = Vector3.up;
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TRIGGER SHOWCASES  (front half of arena Z: +5 to +22)
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildTriggerShowcases()
    {
        var group = new GameObject("Trigger_Showcases");

        // ── COLOR CHANGER ────────────────────────────────────────────────────
        // A flat coloured pad. Walk onto it to turn red; resets after delay.
        var cc = CreateZonePad("ColorChanger_Zone", new Vector3(-18, 0.05f, 12), new Vector3(8, 0.1f, 8), GetMat("Mat_Zone_Color"), group);
        var changer = cc.AddComponent<ColorChanger>();
        var soCc = new SerializedObject(changer);
        soCc.FindProperty("targetColor").colorValue = Color.red;
        soCc.FindProperty("resetDelay").floatValue  = 2f;
        soCc.ApplyModifiedProperties();

        // ── SOUND TRIGGER ────────────────────────────────────────────────────
        // Walk onto the green pad – plays AudioClip (assign in Inspector).
        var st = CreateZonePad("SoundTrigger_Zone", new Vector3(-6, 0.05f, 12), new Vector3(8, 0.1f, 8), GetMat("Mat_Zone_Sound"), group);
        var audioSrc = st.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        st.AddComponent<SoundTrigger>();
        // [audioClip]: assign your AudioClip from the Project window in the Inspector.

        // ── PUSH ZONE ────────────────────────────────────────────────────────
        // Step on the yellow pad to be launched upward.
        var pz = CreateZonePad("PushZone_Launchpad", new Vector3(6, 0.05f, 12), new Vector3(8, 0.1f, 8), GetMat("Mat_Zone_Push"), group);
        var pusher = pz.AddComponent<PushZone>();
        var soPz = new SerializedObject(pusher);
        soPz.FindProperty("pushDirection").vector3Value = Vector3.up;
        soPz.FindProperty("pushForce").floatValue       = 25f;
        soPz.ApplyModifiedProperties();

        // ── DESTRUCTION ZONE + TARGET ─────────────────────────────────────────
        // Push the brown box off of its little shelf into the purple pit.
        var dzGroup = new GameObject("DestructionSetup"); dzGroup.transform.SetParent(group.transform);

        // Shelf the box sits on
        int groundLayer = LayerMask.NameToLayer("Ground");
        var shelf = CreateCube("Shelf", new Vector3(18, 1.75f, 10), new Vector3(6, 0.5f, 6), dzGroup, GetMat("Mat_Platform"));
        shelf.layer = groundLayer;

        // Destructible box on the shelf
        var box = CreateCube("Destructible_Box", new Vector3(18, 2.75f, 10), new Vector3(1.5f, 1.5f, 1.5f), dzGroup, GetMat("Mat_Destructible"));
        box.tag = "Destructible";
        box.AddComponent<Rigidbody>();

        // Destruction zone – a glowing pit in front of the shelf
        var dz = CreateZonePad("DestructionZone_Pit", new Vector3(18, 0.05f, 16), new Vector3(8, 0.1f, 6), GetMat("Mat_Zone_Destruct"), dzGroup);
        dz.AddComponent<DestructionZone>();

        // ── HAZARD ZONE (death pit) ───────────────────────────────────────────
        var hz = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hz.name = "HazardZone_Void";
        hz.transform.position = new Vector3(0, -9, 0);
        hz.transform.localScale = new Vector3(200, 1, 200);
        hz.GetComponent<Collider>().isTrigger = true;
        Object.DestroyImmediate(hz.GetComponent<MeshRenderer>());
        hz.AddComponent<HazardZone>();
        hz.transform.SetParent(group.transform);
    }

    /// <summary>Creates a flat trigger pad with the given material.</summary>
    private static GameObject CreateZonePad(string name, Vector3 pos, Vector3 scale, Material mat, GameObject parent)
    {
        var go = CreateCube(name, pos, scale, parent, mat);
        go.GetComponent<Collider>().isTrigger = true;
        return go;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SCORE SYSTEM + UI
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildScoreSystemAndUI()
    {
        // ScoreManager
        var smGo  = new GameObject("ScoreManager");
        var sm    = smGo.AddComponent<ScoreManager>();

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode       = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // HUD strip at top
        var hud = MakePanel("HUD_Panel", canvasGo, new Color(0, 0, 0, 0.5f),
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(1, 1),
            pivot: new Vector2(0.5f, 1), sizeDelta: new Vector2(0, 70));

        // Score text
        var scoreTmp = MakeText("ScoreText", hud, "Score: 0", 38, new Color(1f, 0.9f, 0.2f),
            new Vector2(300, 55), new Vector2(-200, -35),
            anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f));

        // Tip text at bottom
        MakeText("TipText", canvasGo, "WASD  Move  |  Space  Jump  |  Collect all orbs!", 22,
            new Color(0.8f, 0.8f, 0.8f, 0.7f), new Vector2(800, 36), new Vector2(0, 30),
            anchorMin: new Vector2(0.5f, 0), anchorMax: new Vector2(0.5f, 0),
            alignment: TextAlignmentOptions.Center);

        // ScoreDisplay component – wired to the text
        var sdGo = new GameObject("ScoreDisplay");
        var sd   = sdGo.AddComponent<ScoreDisplay>();
        var soSd = new SerializedObject(sd);
        soSd.FindProperty("scoreText").objectReferenceValue = scoreTmp;
        soSd.ApplyModifiedProperties();

        // Wire OnScoreChanged -> ScoreDisplay.UpdateText
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(sm.OnScoreChanged, sd.UpdateText);

        // Collectibles
        var colGroup = new GameObject("Collectibles");
        Vector3[] positions = {
            new Vector3(-18, 1, -18), new Vector3(-10, 1, -18), new Vector3(0, 1, -18),
            new Vector3( 10, 1, -18), new Vector3( 18, 1, -18), new Vector3(-8, 1, -5),
            new Vector3(  8, 1,  -5)
        };
        for (int i = 0; i < positions.Length; i++)
        {
            var col = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            col.name = $"Collectible_{i + 1:00}";
            col.transform.position   = positions[i];
            col.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            col.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Collectible");
            col.GetComponent<Collider>().isTrigger = true;
            col.transform.SetParent(colGroup.transform);

            var c  = col.AddComponent<Collectible>();
            var so = new SerializedObject(c);
            so.FindProperty("pointValue").intValue = 10;
            so.ApplyModifiedProperties();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UI HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    private static GameObject MakePanel(string name, GameObject parent, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2? anchoredPos = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos ?? Vector2.zero;
        return go;
    }

    private static TextMeshProUGUI MakeText(string name, GameObject parent, string text, int size, Color color,
        Vector2 sizeDelta, Vector2 anchoredPos,
        Vector2? anchorMin = null, Vector2? anchorMax = null,
        TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = size; tmp.color = color; tmp.alignment = alignment;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin ?? new Vector2(0, 1);
        rt.anchorMax = anchorMax ?? new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;
        return tmp;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PRIMITIVE HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    private static GameObject CreateCube(string name, Vector3 pos, Vector3 scale, GameObject parent, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position   = pos;
        go.transform.localScale = scale;
        if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        if (parent != null) go.transform.SetParent(parent.transform);
        return go;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SERIALIZED-PROPERTY SHORTHAND
    // ─────────────────────────────────────────────────────────────────────────

    private static void SetProps(Object obj, params (string name, object value)[] props)
    {
        var so = new SerializedObject(obj);
        foreach (var (name, value) in props)
        {
            var prop = so.FindProperty(name);
            if (prop == null) { Debug.LogWarning($"Session2Builder: property '{name}' not found on {obj.GetType().Name}"); continue; }
            switch (value)
            {
                case float   f: prop.floatValue        = f; break;
                case int     i: prop.intValue          = i; break;
                case bool    b: prop.boolValue         = b; break;
                case Vector3 v: prop.vector3Value      = v; break;
                case Color   c: prop.colorValue        = c; break;
                case Object  o: prop.objectReferenceValue = o; break;
                case string  s: prop.stringValue       = s; break;
            }
        }
        so.ApplyModifiedProperties();
    }
}
