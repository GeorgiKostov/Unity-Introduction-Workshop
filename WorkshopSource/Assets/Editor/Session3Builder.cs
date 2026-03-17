using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;
using WorkshopBehaviours.Session2.Collectibles;
using WorkshopBehaviours.Session2.Triggers;
using WorkshopBehaviours.Session2.UI;
using WorkshopBehaviours.Session3.Movement;
using WorkshopBehaviours.Session3.Feedback;
using WorkshopBehaviours.Session3.Platforms;

/// <summary>
/// Builds the Session 3 showcase scene — a simple platformer that demonstrates
/// every concept from the Session 3 curriculum in sequence:
///   1. PlayerRespawner + HazardZone (fall recovery)
///   2. Tags and Layers (Ground layer, Player tag)
///   3. Trigger Zones: AudioTrigger (SoundTrigger), ColorTrigger (ColorChanger), ParticleTrigger (ParticleOnTrigger)
///   4. Collectible, ScoreManager, ScoreDisplay (three-script event chain)
///   5. Moving Platforms: Horizontal (X-axis), Vertical (Y-axis)
///
/// Run via Workshop > Build Session 3.
/// </summary>
public class Session3Builder
{
    [MenuItem("Workshop/Build Session 3")]
    public static void Build()
    {
        // ── Create and immediately save an empty scene ──────────────────────
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool saved = EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Session3.unity");
        if (!saved)
        {
            Debug.LogError("Session3Builder: Failed to save scene. Make sure Assets/Scenes/ exists.");
            return;
        }

        SetupTagsAndLayers();
        BuildLighting();
        BuildMaterials();
        BuildEnvironment();

        var spawnPoint = BuildSpawnPoint();

        BuildPlayer(spawnPoint);
        BuildTriggerSection();
        BuildPlatformSection();
        BuildCollectibles();
        BuildScoreSystemAndUI();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Session3.unity");
        Debug.Log("Session 3 built successfully! Remember to assign audio clips to the SoundTrigger zone in the Inspector.");
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
        sunGo.transform.rotation = Quaternion.Euler(48, -30, 0);
        light.color = new Color(1.0f, 0.95f, 0.85f);
        light.intensity = 1.35f;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.65f;
        RenderSettings.sun = light;

        // Slightly purple ambient to make the platform colors pop
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.10f, 0.08f, 0.18f);
        RenderSettings.ambientIntensity = 0.9f;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MATERIALS
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildMaterials()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Session3"))
            AssetDatabase.CreateFolder("Assets/Materials", "Session3");

        // Environment
        CreateMat("Mat_Floor",          new Color(0.15f, 0.15f, 0.18f), 0.0f, 0.55f);
        CreateMat("Mat_Wall",           new Color(0.10f, 0.08f, 0.18f), 0.0f, 0.2f);
        CreateMat("Mat_SpawnPad",       new Color(0.15f, 0.80f, 0.30f), 0.0f, 0.5f,
                                        new Color(0.05f, 0.5f, 0.1f) * 0.4f);

        // Platforms — three tiers, each a distinct colour
        CreateMat("Mat_Platform_A",     new Color(0.25f, 0.37f, 0.60f), 0.15f, 0.50f);
        CreateMat("Mat_Platform_B",     new Color(0.38f, 0.22f, 0.60f), 0.15f, 0.55f);
        CreateMat("Mat_Platform_C",     new Color(0.60f, 0.32f, 0.12f), 0.20f, 0.60f);

        // Moving platforms
        CreateMat("Mat_PlatformH",      new Color(0.15f, 0.70f, 0.50f), 0.20f, 0.65f,
                                        new Color(0.0f, 0.4f, 0.25f) * 0.25f);
        CreateMat("Mat_PlatformV",      new Color(0.70f, 0.45f, 0.10f), 0.20f, 0.65f,
                                        new Color(0.5f, 0.3f, 0.0f) * 0.25f);

        // Trigger zones — coloured pads, visible from above
        CreateMat("Mat_Zone_Audio",     new Color(0.20f, 0.75f, 0.20f), 0.0f, 0.6f,
                                        new Color(0.0f, 0.5f, 0.0f) * 0.35f);
        CreateMat("Mat_Zone_Color",     new Color(0.90f, 0.20f, 0.20f), 0.0f, 0.6f,
                                        new Color(0.6f, 0.0f, 0.0f) * 0.35f);
        CreateMat("Mat_Zone_Particle",  new Color(0.15f, 0.55f, 1.00f), 0.0f, 0.6f,
                                        new Color(0.0f, 0.3f, 0.8f) * 0.35f);

        // Hazard zone
        CreateMat("Mat_Hazard",         new Color(0.95f, 0.15f, 0.05f), 0.0f, 0.3f,
                                        new Color(0.8f, 0.1f, 0.0f) * 0.6f);

        // Collectibles
        CreateMat("Mat_Coin_Common",    new Color(1.00f, 0.85f, 0.00f), 0.85f, 1.0f,
                                        new Color(1.0f, 0.7f, 0.0f) * 0.6f);
        CreateMat("Mat_Coin_Rare",      new Color(0.00f, 0.85f, 1.00f), 0.90f, 1.0f,
                                        new Color(0.0f, 0.5f, 0.9f) * 0.6f);
        CreateMat("Mat_Coin_Bonus",     new Color(1.00f, 0.20f, 0.85f), 0.95f, 1.0f,
                                        new Color(0.8f, 0.0f, 0.6f) * 0.6f);

        // Player
        CreateMat("Mat_Player",         new Color(0.00f, 0.85f, 0.50f), 0.20f, 0.70f,
                                        new Color(0.0f, 0.4f, 0.2f) * 0.25f);

        // Physics material — zero friction on player capsule
        string pmPath = "Assets/Materials/Session3/PhysicsMat_Player.physicMaterial";
        if (AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(pmPath) == null)
        {
            var pm = new PhysicsMaterial("PhysicsMat_Player")
            {
                staticFriction  = 0f,
                dynamicFriction = 0f,
                bounciness      = 0f,
                frictionCombine = PhysicsMaterialCombine.Minimum,
                bounceCombine   = PhysicsMaterialCombine.Minimum
            };
            AssetDatabase.CreateAsset(pm, pmPath);
        }

        AssetDatabase.SaveAssets();
    }

    private static Material CreateMat(string name, Color baseColor, float metallic, float smoothness, Color? emission = null)
    {
        string path = $"Assets/Materials/Session3/{name}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Metallic",   metallic);
            mat.SetFloat("_Smoothness", smoothness);
            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission.Value);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    private static Material GetMat(string name)
        => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/Session3/{name}.mat");

    // ─────────────────────────────────────────────────────────────────────────
    // ENVIRONMENT  —  simple platformer layout
    //
    //  Z layout (bird's-eye):
    //   +Z  Spawn pad at (0, 0, -24)  —  players start here
    //   ~0  Trigger Zone row:  Audio (-12,0,0)  Color (0,0,0)  Particle (12,0,0)
    //  -Z   Platform tier 1 at z≈-6 to -10
    //  -Z   Platform tier 2 at z≈-14 to -18  (elevated)
    //  -Z   Platform tier 3 at z≈-22 to -26  (highest, near back wall)
    //       Moving platforms bridge the gaps between tiers
    //       Hazard zone (invisible void catcher) at y = -10
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildEnvironment()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        var arena = new GameObject("Arena");

        // ── Main floor ──────────────────────────────────────────────────────
        // A wide base plane that the player walks on near spawn
        var floor = CreateCube("Floor", new Vector3(0, -0.25f, 0), new Vector3(6, 0.5f, 6), arena, GetMat("Mat_Floor"));
        floor.layer = groundLayer;

        // ── Outer walls ─────────────────────────────────────────────────────
        var walls = new GameObject("Walls"); walls.transform.SetParent(arena.transform);
        MakeWall("Wall_North", new Vector3(0, 3, -32), new Vector3(56, 6, 1), groundLayer, walls);
        MakeWall("Wall_South", new Vector3(0, 3,  28), new Vector3(56, 6, 1), groundLayer, walls);
        MakeWall("Wall_East",  new Vector3(28, 3,  -2), new Vector3(1, 6, 62), groundLayer, walls);
        MakeWall("Wall_West",  new Vector3(-28,3,  -2), new Vector3(1, 6, 62), groundLayer, walls);

        // ── Tier 1 platforms  (z ≈ -4 to -10, y = 0) ───────────────────────
        var tier1 = new GameObject("Tier1_Platforms"); tier1.transform.SetParent(arena.transform);
        MakePlatform("T1_Left",   new Vector3(-10, 0.5f, -6),  new Vector3(7, 1, 5),  groundLayer, tier1, "Mat_Platform_A");
        MakePlatform("T1_Center", new Vector3(  0, 0.5f, -8),  new Vector3(8, 1, 6),  groundLayer, tier1, "Mat_Platform_A");
        MakePlatform("T1_Right",  new Vector3( 10, 0.5f, -6),  new Vector3(7, 1, 5),  groundLayer, tier1, "Mat_Platform_A");

        // Small gap-jump stepping stones across Tier 1
        MakePlatform("T1_Step_L", new Vector3(-5, 0.5f, -10),  new Vector3(3, 1, 3),  groundLayer, tier1, "Mat_Platform_A");
        MakePlatform("T1_Step_R", new Vector3( 5, 0.5f, -10),  new Vector3(3, 1, 3),  groundLayer, tier1, "Mat_Platform_A");

        // ── Tier 2 platforms  (z ≈ -14 to -18, y = 3) ──────────────────────
        var tier2 = new GameObject("Tier2_Platforms"); tier2.transform.SetParent(arena.transform);
        MakePlatform("T2_Left",   new Vector3(-12, 3.5f, -15), new Vector3(6, 1, 5),  groundLayer, tier2, "Mat_Platform_B");
        MakePlatform("T2_Center", new Vector3(  0, 3.5f, -17), new Vector3(7, 1, 6),  groundLayer, tier2, "Mat_Platform_B");
        MakePlatform("T2_Right",  new Vector3( 12, 3.5f, -15), new Vector3(6, 1, 5),  groundLayer, tier2, "Mat_Platform_B");

        // ── Tier 3 platforms  (z ≈ -22 to -28, y = 7) ──────────────────────
        var tier3 = new GameObject("Tier3_Platforms"); tier3.transform.SetParent(arena.transform);
        MakePlatform("T3_Left",   new Vector3(-8,  7.5f, -23), new Vector3(5, 1, 4),  groundLayer, tier3, "Mat_Platform_C");
        MakePlatform("T3_Center", new Vector3( 0,  7.5f, -26), new Vector3(8, 1, 5),  groundLayer, tier3, "Mat_Platform_C");
        MakePlatform("T3_Right",  new Vector3( 8,  7.5f, -23), new Vector3(5, 1, 4),  groundLayer, tier3, "Mat_Platform_C");
    }

    private static void MakeWall(string name, Vector3 pos, Vector3 scale, int layer, GameObject parent)
    {
        var w = CreateCube(name, pos, scale, parent, GetMat("Mat_Wall"));
        w.layer = layer;
    }

    private static GameObject MakePlatform(string name, Vector3 pos, Vector3 scale, int layer, GameObject parent, string matName)
    {
        var go = CreateCube(name, pos, scale, parent, GetMat(matName));
        go.layer = layer;
        return go;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SPAWN POINT
    // ─────────────────────────────────────────────────────────────────────────

    private static Transform BuildSpawnPoint()
    {
        // Visual spawn pad — bright green so students can see it clearly
        var pad = CreateCube("SpawnPad", new Vector3(0, 0.15f, 22), new Vector3(4, 0.3f, 4), null, GetMat("Mat_SpawnPad"));
        pad.layer = LayerMask.NameToLayer("Ground");

        var sp = new GameObject("SpawnPoint");
        sp.transform.position = new Vector3(0, 1.5f, 22);
        return sp.transform;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PLAYER
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildPlayer(Transform spawnPoint)
    {
        // Camera first — PlayerMover needs the reference
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.AddComponent<Camera>();
        camGo.AddComponent<AudioListener>();
        camGo.transform.position  = new Vector3(0, 10, 30);
        camGo.transform.rotation  = Quaternion.Euler(18, 180, 0);

        // Player capsule
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag  = "Player";
        player.transform.position = spawnPoint.position;
        player.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Player");

        // Rigidbody — all rotations frozen so capsule stays upright
        var rb            = player.AddComponent<Rigidbody>();
        rb.mass           = 1f;
        rb.useGravity     = true;
        rb.constraints    = RigidbodyConstraints.FreezeRotation;

        // Physics material — zero friction so the capsule doesn't stick to walls
        var cc       = player.GetComponent<CapsuleCollider>();
        cc.material  = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(
                           "Assets/Materials/Session3/PhysicsMat_Player.physicMaterial");

        // PlayerOrbitalMover — camera-relative, reads Camera.main, no serialized camera reference
        var mover = player.AddComponent<PlayerOrbitalMover>();
        var soM   = new SerializedObject(mover);
        soM.FindProperty("m_moveSpeed").floatValue = 6f;
        soM.ApplyModifiedProperties();

        // PlayerJumper — ground layer assigned so SphereCast can detect Ground
        var jumper = player.AddComponent<WorkshopBehaviours.Session2.Movement.PlayerJumper>();
        var soJ    = new SerializedObject(jumper);
        soJ.FindProperty("m_jumpForce").floatValue           = 7f;
        soJ.FindProperty("m_groundCheckDistance").floatValue = 1.15f;
        soJ.FindProperty("m_groundCheckRadius").floatValue   = 0.45f;
        soJ.FindProperty("m_groundLayer").intValue           = LayerMask.GetMask("Ground");
        soJ.ApplyModifiedProperties();

        // PlayerRespawner — triggers Respawn() if player falls to m_rigidbody.isKinematic threshold
        //   Public Respawn() also called by HazardZone scripts
        var respawner = player.AddComponent<WorkshopBehaviours.Session2.Movement.PlayerRespawner>();
        var soR       = new SerializedObject(respawner);
        soR.FindProperty("m_spawnPoint").objectReferenceValue = spawnPoint;
        soR.ApplyModifiedProperties();

        // Invisible void HazardZone — catches the player before the auto-respawn threshold
        // Demonstrates HazardZone calling GetComponent<PlayerRespawner>() and Respawn()
        var void3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        void3.name = "HazardZone_Void";
        void3.transform.position   = new Vector3(0, -9, -2);
        void3.transform.localScale = new Vector3(200, 1, 200);
        void3.GetComponent<Collider>().isTrigger = true;
        Object.DestroyImmediate(void3.GetComponent<MeshRenderer>());
        void3.AddComponent<HazardZone>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TRIGGER SECTION  —  three coloured pads laid out in the centre arena
    //
    //  AUDIO  zone   at (-12, 0.05, 0)  — green pad   — plays a sound on enter
    //  COLOR  zone   at (  0, 0.05, 0)  — red pad     — changes player colour
    //  PARTICLE zone at ( 12, 0.05, 0)  — blue pad    — spawns burst at player
    //
    //  Each pad has a label sign above it to help students identify it at a glance.
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildTriggerSection()
    {
        var group = new GameObject("TriggerZones");

        // ── AUDIO TRIGGER ────────────────────────────────────────────────────
        // SoundTrigger: plays audioClip once per enter, resets on exit.
        // Students assign the AudioClip from kenney.nl in the Inspector.
        var audioZone = CreateZonePad("TriggerZone_Audio", new Vector3(-12, 0.05f, 0),
                                      new Vector3(7, 0.1f, 7), "Mat_Zone_Audio", group);
        var audioSrc  = audioZone.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioZone.AddComponent<SoundTrigger>();
        // ↑ audioClip field intentionally left unassigned — students drag it in from kenney.nl assets

        // ── COLOR TRIGGER ─────────────────────────────────────────────────────
        // ColorChanger: GetComponent<Renderer> on other.gameObject (the player),
        //               changes material color, resets after resetDelay via Coroutine.
        var colorZone  = CreateZonePad("TriggerZone_Color", new Vector3(0, 0.05f, 0),
                                       new Vector3(7, 0.1f, 7), "Mat_Zone_Color", group);
        var changer    = colorZone.AddComponent<ColorChanger>();
        var soCh       = new SerializedObject(changer);
        soCh.FindProperty("m_targetColor").colorValue = new Color(1.0f, 0.2f, 0.8f, 1f); // vivid magenta
        soCh.FindProperty("m_resetDelay").floatValue  = 1.5f;
        soCh.ApplyModifiedProperties();

        // ── PARTICLE TRIGGER ──────────────────────────────────────────────────
        // ParticleOnTrigger: Instantiate the burst prefab at the player's world position.
        // effectPrefab intentionally left empty — students must set in Inspector
        // after building their FX_Burst prefab following the session walkthrough.
        var particleZone = CreateZonePad("TriggerZone_Particle", new Vector3(12, 0.05f, 0),
                                         new Vector3(7, 0.1f, 7), "Mat_Zone_Particle", group);
        var ptrig        = particleZone.AddComponent<ParticleOnTrigger>();
        var soPt         = new SerializedObject(ptrig);
        soPt.FindProperty("m_shouldSpawnAtPlayer").boolValue = true;
        soPt.FindProperty("m_isSingleTrigger").boolValue     = false;
        soPt.ApplyModifiedProperties();

        // ── HAZARD ZONES — lava pits between platforms ───────────────────────
        // Demonstrate HazardZone calling playerRespawner.Respawn() directly.
        var hazards = new GameObject("HazardZones"); hazards.transform.SetParent(group.transform);

        CreateHazardPad("Hazard_Lava_A", new Vector3(0, 0.05f, -12),    new Vector3(28, 0.1f, 4), hazards);
        CreateHazardPad("Hazard_Lava_B", new Vector3(0, 3.55f, -11),    new Vector3(28, 0.1f, 4), hazards);
        CreateHazardPad("Hazard_Lava_C", new Vector3(0, 3.55f, -20),    new Vector3(28, 0.1f, 4), hazards);
    }

    private static GameObject CreateZonePad(string name, Vector3 pos, Vector3 scale, string matName, GameObject parent)
    {
        var go = CreateCube(name, pos, scale, parent, GetMat(matName));
        go.GetComponent<Collider>().isTrigger = true;
        return go;
    }

    private static void CreateHazardPad(string name, Vector3 pos, Vector3 scale, GameObject parent)
    {
        var go = CreateCube(name, pos, scale, parent, GetMat("Mat_Hazard"));
        var bc = go.GetComponent<BoxCollider>();
        bc.isTrigger = true;
        go.AddComponent<HazardZone>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PLATFORM SECTION — Moving Platforms bridging the three tiers
    //
    //  Horizontal (X-axis):  bridges between Tier1 left / right  → Tier2
    //  Vertical (Y-axis):    lifts player from Tier2 center       → Tier3
    //
    //  Both use MovingPlatform (Workshop.Session3.Hazards) with a Kinematic
    //  Rigidbody so objects standing on them are carried correctly.
    //  The key teaching point: transform.position teleports; MovePosition moves.
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildPlatformSection()
    {
        var group = new GameObject("MovingPlatforms");

        // ── HORIZONTAL PLATFORMS — PlatformMoverHorizontal (X-axis Mathf.Sin + MovePosition) ──
        // Bridges the Tier 1 → Tier 2 gaps on left and right.
        // Teaching point: Kinematic Rigidbody + MovePosition carries the player;
        //                 transform.position teleports and the player slides off.
        CreateHPlat("MovPlat_H_Left",
            pos:      new Vector3(-10, 1.5f, -13),
            scale:    new Vector3(4, 0.5f, 4),
            distance: 4f,
            speed:    1.2f,
            parent:   group);

        CreateHPlat("MovPlat_H_Right",
            pos:      new Vector3(10, 1.5f, -13),
            scale:    new Vector3(4, 0.5f, 4),
            distance: 4f,
            speed:    1.4f,
            parent:   group);

        // Extra horizontal at the top tier — wide sweep, interesting to jump across
        CreateHPlat("MovPlat_H_Top",
            pos:      new Vector3(0, 8f, -25),
            scale:    new Vector3(4, 0.5f, 4),
            distance: 10f,
            speed:    1.6f,
            parent:   group);

        // ── VERTICAL PLATFORM — PlatformMoverVertical (Y-axis Mathf.Sin + MovePosition) ───
        // Lifts the player from Tier 2 Center up to Tier 3.
        // Teaching point: Same pattern as horizontal, different axis.
        //   - What if moveHeight = 0?   Platform does not move.
        //   - What if moveSpeed < 0?    Direction reverses (starts downward).
        CreateVPlat("MovPlat_V_Center",
            pos:    new Vector3(0, 4f, -19),
            scale:  new Vector3(4, 0.5f, 4),
            height: 4f,
            speed:  0.8f,
            parent: group);
    }

    /// <summary>Creates a horizontal (X-axis) moving platform with PlatformMoverHorizontal.</summary>
    private static void CreateHPlat(string name, Vector3 pos, Vector3 scale,
                                     float distance, float speed, GameObject parent)
    {
        var go         = CreateCube(name, pos, scale, parent, GetMat("Mat_PlatformH"));
        // [RequireComponent] on PlatformMoverHorizontal ensures the Rigidbody is present,
        // but we add it first and set IsKinematic so it is configured correctly from the start.
        var rb         = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        var mh         = go.AddComponent<PlatformMoverHorizontal>();
        var so         = new SerializedObject(mh);
        so.FindProperty("moveDistance").floatValue = distance;
        so.FindProperty("moveSpeed").floatValue    = speed;
        so.ApplyModifiedProperties();
    }

    /// <summary>Creates a vertical (Y-axis) moving platform with PlatformMoverVertical.</summary>
    private static void CreateVPlat(string name, Vector3 pos, Vector3 scale,
                                     float height, float speed, GameObject parent)
    {
        var go         = CreateCube(name, pos, scale, parent, GetMat("Mat_PlatformV"));
        var rb         = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        var mv         = go.AddComponent<PlatformMoverVertical>();
        var so         = new SerializedObject(mv);
        so.FindProperty("moveHeight").floatValue = height;
        so.FindProperty("moveSpeed").floatValue  = speed;
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COLLECTIBLES
    //
    //  Three tiers of collectibles match the three platform tiers.
    //  Common (10 pts)  — ground level and Tier 1
    //  Rare   (25 pts)  — Tier 2
    //  Bonus  (50 pts)  — Tier 3 (hardest to reach)
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildCollectibles()
    {
        var group = new GameObject("Collectibles");

        // ── Common  — ground / Tier 1 ────────────────────────────────────────
        var commonGrp = new GameObject("Common"); commonGrp.transform.SetParent(group.transform);
        Vector3[] commonPos =
        {
            // Near spawn, easy tutorial pickups
            new Vector3(-3, 1, 18), new Vector3(0, 1, 18), new Vector3(3, 1, 18),
            // Trigger section (walk through zones and collect)
            new Vector3(-12, 1, 2), new Vector3(0, 1, 2), new Vector3(12, 1, 2),
            // Tier 1 platforms
            new Vector3(-10, 2, -6), new Vector3(0, 2, -8), new Vector3(10, 2, -6),
        };
        for (int i = 0; i < commonPos.Length; i++)
            SpawnCollectible($"Coin_C{i + 1:D2}", commonPos[i], 0.5f, "Mat_Coin_Common", 10, commonGrp);

        // ── Rare — Tier 2 ────────────────────────────────────────────────────
        var rareGrp = new GameObject("Rare"); rareGrp.transform.SetParent(group.transform);
        Vector3[] rarePos =
        {
            new Vector3(-12, 5, -15), new Vector3(-10, 5, -16),
            new Vector3(  0, 5, -17), new Vector3(  2, 5, -17),
            new Vector3( 12, 5, -15), new Vector3( 10, 5, -16),
        };
        for (int i = 0; i < rarePos.Length; i++)
            SpawnCollectible($"Coin_R{i + 1:D2}", rarePos[i], 0.55f, "Mat_Coin_Rare", 25, rareGrp);

        // ── Bonus — Tier 3 ───────────────────────────────────────────────────
        var bonusGrp = new GameObject("Bonus"); bonusGrp.transform.SetParent(group.transform);
        Vector3[] bonusPos =
        {
            new Vector3(-8, 9, -23), new Vector3(0, 9.5f, -26), new Vector3(8, 9, -23),
        };
        for (int i = 0; i < bonusPos.Length; i++)
            SpawnCollectible($"Coin_B{i + 1:D2}", bonusPos[i], 0.65f, "Mat_Coin_Bonus", 50, bonusGrp);
    }

    private static void SpawnCollectible(string name, Vector3 pos, float size, string matName, int points, GameObject parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position   = pos;
        go.transform.localScale = new Vector3(size, size, size);
        go.GetComponent<MeshRenderer>().sharedMaterial = GetMat(matName);
        go.GetComponent<Collider>().isTrigger = true;
        go.transform.SetParent(parent.transform);

        var col = go.AddComponent<Collectible>();
        var so  = new SerializedObject(col);
        so.FindProperty("m_pointValue").intValue = points;
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SCORE SYSTEM + UI  (ScoreManager → UnityEvent → ScoreDisplay)
    //
    //  Teaching sequence from the curriculum:
    //    Collectible.cs       → calls ScoreManager.Instance.AddScore(pointValue)
    //    ScoreManager.cs      → fires OnScoreChanged(int newScore)
    //    ScoreDisplay.cs      → receives newScore, updates TextMeshProUGUI
    //
    //  Wiring: Inspector wire from ScoreManager.OnScoreChanged → UpdateText
    //  Teaching point: ScoreManager does not import ScoreDisplay. Neither knows
    //  the other exists. The Inspector is the wire.
    // ─────────────────────────────────────────────────────────────────────────

    private static void BuildScoreSystemAndUI()
    {
        // ScoreManager — singleton, any script can call ScoreManager.Instance
        var smGo = new GameObject("ScoreManager");
        var sm   = smGo.AddComponent<ScoreManager>();

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // HUD top bar
        var hud = MakePanel("HUD_Panel", canvasGo,
            new Color(0f, 0f, 0f, 0.55f),
            anchorMin:  new Vector2(0, 1),
            anchorMax:  new Vector2(1, 1),
            pivot:      new Vector2(0.5f, 1),
            sizeDelta:  new Vector2(0, 72));

        // Score text — upper-left
        var scoreTmp = MakeText("ScoreText", hud, "Score: 0", 40,
            new Color(1f, 0.90f, 0.15f),
            sizeDelta:      new Vector2(350, 60),
            anchoredPos:    new Vector2(20, -36),
            anchorMin:      new Vector2(0, 0.5f),
            anchorMax:      new Vector2(0, 0.5f),
            alignment:      TextAlignmentOptions.Left);

        // Hint text — upper-right
        MakeText("HintText", hud,
            "WASD  Move   |   Space  Jump   |   Collect all coins!",
            22, new Color(0.85f, 0.85f, 0.85f, 0.75f),
            sizeDelta:   new Vector2(850, 60),
            anchoredPos: new Vector2(-20, -36),
            anchorMin:   new Vector2(1, 0.5f),
            anchorMax:   new Vector2(1, 0.5f),
            alignment:   TextAlignmentOptions.Right);

        // Zone labels — bottom bar to remind students which pad does what
        var zoneBar = MakePanel("ZoneBar", canvasGo,
            new Color(0f, 0f, 0f, 0.40f),
            anchorMin:  new Vector2(0, 0),
            anchorMax:  new Vector2(1, 0),
            pivot:      new Vector2(0.5f, 0),
            sizeDelta:  new Vector2(0, 48));

        MakeText("ZoneLabel", zoneBar,
            "GREEN  = Audio Trigger   |   RED  = Color Trigger   |   BLUE  = Particle Trigger",
            22, new Color(0.9f, 0.9f, 0.9f, 0.85f),
            sizeDelta:   new Vector2(1200, 40),
            anchoredPos: new Vector2(0, 24),
            anchorMin:   new Vector2(0.5f, 0),
            anchorMax:   new Vector2(0.5f, 0),
            alignment:   TextAlignmentOptions.Center);

        // ScoreDisplay — uses RequireComponent(TextMeshProUGUI), must be on same GameObject as TMP
        var sd = scoreTmp.gameObject.AddComponent<ScoreDisplay>();

        // Wire OnScoreChanged → ScoreDisplay.UpdateScoreText
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(sm.OnScoreChanged, sd.UpdateScoreText);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UI HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    private static GameObject MakePanel(string name, GameObject parent, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta,
        Vector2? anchoredPos = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin       = anchorMin;
        rt.anchorMax       = anchorMax;
        rt.pivot           = pivot;
        rt.sizeDelta       = sizeDelta;
        rt.anchoredPosition = anchoredPos ?? Vector2.zero;
        return go;
    }

    private static TextMeshProUGUI MakeText(string name, GameObject parent, string text,
        int size, Color color, Vector2 sizeDelta, Vector2 anchoredPos,
        Vector2? anchorMin = null, Vector2? anchorMax = null,
        TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.alignment = alignment;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin       = anchorMin ?? new Vector2(0, 1);
        rt.anchorMax       = anchorMax ?? new Vector2(0, 1);
        rt.pivot           = new Vector2(0, 0.5f);
        rt.sizeDelta       = sizeDelta;
        rt.anchoredPosition = anchoredPos;
        return tmp;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PRIMITIVE HELPERS
    // ─────────────────────────────────────────────────────────────────────────

    private static GameObject CreateCube(string name, Vector3 pos, Vector3 scale,
                                          GameObject parent, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name             = name;
        go.transform.position   = pos;
        go.transform.localScale = scale;
        if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        if (parent != null) go.transform.SetParent(parent.transform);
        return go;
    }
}
