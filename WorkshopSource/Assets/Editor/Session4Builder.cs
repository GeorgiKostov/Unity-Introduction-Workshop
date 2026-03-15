using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using WorkshopBehaviours.Session2_New;
using WorkshopBehaviours.Session3.Platforms;

/// <summary>
/// Builds the Session 4 showcase scene: Rendering, Materials, Lighting and Baking.
/// Expands the Session 3 platformer layout with:
///   - Procedural skybox (warm key / cool ambient contrast)
///   - Global Volume: Bloom, Tonemapping ACES, ColorAdjustments, Vignette
///   - Material variety demo: Lit/Unlit/Transparent, metallic/smoothness grid
///   - Emissive trims and collectibles (Bloom teaching target)
///   - Static flags on environment for lightmap baking
///   - Light Probe group (8 probes at floor level)
///   - Reflection Probe (center of arena)
///   - Point lights as children of collectibles (no shadows)
///   - Particle burst prefabs for collection effect
///   - All Session 3 gameplay intact (player, score, platforms, hazards)
///
/// Run via Workshop > Build Session 4.
/// </summary>
public class Session4Builder
{
    [MenuItem("Workshop/Build Session 4")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        if (!EditorSceneManager.SaveScene(scene, "Assets/Scenes/Session4.unity"))
        {
            Debug.LogError("Session4Builder: Failed to save scene — ensure Assets/Scenes/ exists.");
            return;
        }

        SetupTagsAndLayers();
        BuildSkyboxAndLighting();
        BuildPostProcessing();
        BuildMaterials();
        BuildEnvironment();
        var spawnTf = BuildSpawnPoint();
        BuildPlayer(spawnTf);
        BuildPlatformSection();
        BuildHazardSection();
        BuildCollectibles();
        BuildLightProbes();
        BuildReflectionProbe();
        BuildMaterialShowcase();
        BuildScoreUI();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Session4.unity");
        Debug.Log("[Session4] Build complete. Assign audio clips to SoundTrigger zones. Bake lighting via Window > Rendering > Lighting > Generate Lighting.");
    }

    // ── TAGS & LAYERS ────────────────────────────────────────────────────────

    static void SetupTagsAndLayers()
    {
        var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tm.FindProperty("tags");
        EnsureTag(tags, "Player");
        var layers = tm.FindProperty("layers");
        EnsureLayer(layers, "Ground");
        tm.ApplyModifiedProperties();
    }
    static void EnsureTag(SerializedProperty tags, string t)
    {
        for (int i = 0; i < tags.arraySize; i++) if (tags.GetArrayElementAtIndex(i).stringValue == t) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = t;
    }
    static void EnsureLayer(SerializedProperty layers, string name)
    {
        for (int i = 8; i < layers.arraySize; i++)
        {
            var sp = layers.GetArrayElementAtIndex(i);
            if (sp.stringValue == name) return;
            if (sp.stringValue == "") { sp.stringValue = name; return; }
        }
    }

    // ── SKYBOX & DIRECTIONAL LIGHT ───────────────────────────────────────────
    // Session 4 teaching: warm key + cool ambient separation.

    static void BuildSkyboxAndLighting()
    {
        // Procedural skybox — students will swap for HDRI from polyhaven.com
        var skyMat = new Material(Shader.Find("Skybox/Procedural"));
        skyMat.SetColor("_SkyTint",   new Color(0.38f, 0.52f, 0.80f));
        skyMat.SetColor("_GroundColor", new Color(0.14f, 0.12f, 0.10f));
        skyMat.SetFloat("_AtmosphereThickness", 1.15f);
        skyMat.SetFloat("_SunSize", 0.04f);
        skyMat.SetFloat("_Exposure", 1.05f);
        AssetDatabase.CreateAsset(skyMat, EnsureFolder("Assets/Materials/Session4") + "/Mat_Skybox_Procedural.mat");
        RenderSettings.skybox = skyMat;

        // Directional light — warm, Mixed mode for baking demo
        var sunGo = new GameObject("Sun_Directional");
        var sun   = sunGo.AddComponent<Light>();
        sun.type       = LightType.Directional;
        sun.lightmapBakeType = LightmapBakeType.Mixed;
        sunGo.transform.rotation = Quaternion.Euler(50, -30, 0);
        sun.color      = new Color(1.0f, 0.95f, 0.82f);   // warm yellow
        sun.intensity  = 1.3f;
        sun.shadows    = LightShadows.Soft;
        sun.shadowStrength    = 0.70f;
        sun.shadowResolution  = LightShadowResolution.Medium;
        RenderSettings.sun    = sun;

        // Cool blue-grey ambient — contrast with warm key is the teaching moment
        RenderSettings.ambientMode  = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.10f, 0.12f, 0.22f);
    }

    // ── POST PROCESSING ──────────────────────────────────────────────────────
    // Bloom needs HDR on in the URP Asset AND Emission Intensity > 1 on materials.

    static void BuildPostProcessing()
    {
        EnsureFolder("Assets/Settings");

        var ppGo = new GameObject("GlobalVolume_PostProcess");
        var vol  = ppGo.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.priority = 10;

        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, "Assets/Settings/S4_PostProcessProfile.asset");

        // Bloom — threshold 1.0 so only HDR emissive pixels glow
        var bloom = profile.Add<Bloom>();
        bloom.active = true;
        bloom.threshold.Override(1.0f);
        bloom.intensity.Override(0.6f);
        bloom.scatter.Override(0.70f);
        bloom.tint.Override(new Color(1.0f, 0.96f, 0.88f));

        // Tonemapping ACES — maps HDR to display range with film-like curve
        var tm = profile.Add<Tonemapping>();
        tm.active = true;
        tm.mode.Override(TonemappingMode.ACES);

        // Color grading — slight warmth, +10 contrast, +15 saturation
        var ca = profile.Add<ColorAdjustments>();
        ca.active = true;
        ca.postExposure.Override(0.2f);
        ca.contrast.Override(10f);
        ca.colorFilter.Override(new Color(1.0f, 0.97f, 0.94f));
        ca.saturation.Override(15f);

        // Vignette — draws eye to centre of arena
        var vig = profile.Add<Vignette>();
        vig.active = true;
        vig.color.Override(Color.black);
        vig.intensity.Override(0.25f);
        vig.smoothness.Override(0.40f);
        vig.rounded.Override(true);

        vol.profile = profile;
        AssetDatabase.SaveAssets();

        // Camera: enable post processing
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.AddComponent<Camera>();
        camGo.AddComponent<AudioListener>();
        camGo.transform.position = new Vector3(0, 10, 30);
        camGo.transform.rotation = Quaternion.Euler(18, 180, 0);
        var camData = camGo.AddComponent<UniversalAdditionalCameraData>();
        camData.renderPostProcessing = true;
        // Player will be added later; store ref for camera follow wiring
        _cameraGo = camGo;
    }
    static GameObject _cameraGo;

    // ── MATERIALS ────────────────────────────────────────────────────────────

    static void BuildMaterials()
    {
        var f = EnsureFolder("Assets/Materials/Session4");

        // Environment — Lit, PBR
        CreateMat("Mat_Floor",       new Color(0.16f, 0.16f, 0.18f), 0.05f, 0.55f);
        CreateMat("Mat_Wall",        new Color(0.10f, 0.09f, 0.20f), 0.10f, 0.20f);
        CreateMat("Mat_SpawnPad",    new Color(0.15f, 0.80f, 0.30f), 0.00f, 0.50f,
                                     new Color(0.04f, 0.50f, 0.10f) * 0.40f);

        // Emissive trims — teaching target for Bloom
        CreateMat("Mat_Trim_Purple", new Color(0.20f, 0.10f, 0.50f), 0.10f, 0.30f,
                                     new Color(0.40f, 0.10f, 1.00f) * 1.5f);
        CreateMat("Mat_Trim_Cyan",   new Color(0.05f, 0.40f, 0.60f), 0.10f, 0.30f,
                                     new Color(0.00f, 0.80f, 1.00f) * 1.5f);

        // Platforms — three tiers
        CreateMat("Mat_Platform_A",  new Color(0.22f, 0.33f, 0.58f), 0.15f, 0.50f);
        CreateMat("Mat_Platform_B",  new Color(0.36f, 0.20f, 0.58f), 0.20f, 0.55f,
                                     new Color(0.20f, 0.05f, 0.50f) * 0.25f);
        CreateMat("Mat_Platform_C",  new Color(0.58f, 0.30f, 0.10f), 0.30f, 0.65f,
                                     new Color(0.80f, 0.40f, 0.00f) * 0.30f);

        // Moving platforms
        CreateMat("Mat_PlatformH",   new Color(0.12f, 0.68f, 0.48f), 0.20f, 0.60f,
                                     new Color(0.00f, 0.40f, 0.25f) * 0.20f);
        CreateMat("Mat_PlatformV",   new Color(0.68f, 0.42f, 0.08f), 0.20f, 0.60f,
                                     new Color(0.50f, 0.28f, 0.00f) * 0.20f);

        // Hazards — red emissive, visible even if Directional Light off
        CreateMat("Mat_Hazard",      new Color(0.92f, 0.14f, 0.04f), 0.00f, 0.10f,
                                     new Color(1.00f, 0.30f, 0.00f) * 1.2f);

        // Collectibles — Unlit + Emission; these are the Bloom demo targets
        CreateMat("Mat_Coin_Common", new Color(1.00f, 0.85f, 0.00f), 0.90f, 1.00f,
                                     new Color(1.00f, 0.72f, 0.00f) * 1.4f);
        CreateMat("Mat_Coin_Rare",   new Color(0.00f, 0.85f, 1.00f), 0.92f, 1.00f,
                                     new Color(0.00f, 0.60f, 1.00f) * 1.4f);
        CreateMat("Mat_Coin_Bonus",  new Color(1.00f, 0.18f, 0.85f), 0.95f, 1.00f,
                                     new Color(1.00f, 0.00f, 0.70f) * 1.6f);

        // Player
        CreateMat("Mat_Player",      new Color(0.00f, 0.86f, 0.50f), 0.20f, 0.70f,
                                     new Color(0.00f, 0.40f, 0.20f) * 0.20f);

        // Physics material — zero friction
        string pmPath = f + "/PhysicsMat_Player.physicMaterial";
        if (AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(pmPath) == null)
        {
            var pm = new PhysicsMaterial("PhysicsMat_Player")
            { staticFriction = 0, dynamicFriction = 0, bounciness = 0,
              frictionCombine = PhysicsMaterialCombine.Minimum,
              bounceCombine   = PhysicsMaterialCombine.Minimum };
            AssetDatabase.CreateAsset(pm, pmPath);
        }
        AssetDatabase.SaveAssets();
    }

    static Material CreateMat(string name, Color col, float met, float smo, Color? emi = null)
    {
        string path = $"Assets/Materials/Session4/{name}.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", col);
        m.SetFloat("_Metallic",   met);
        m.SetFloat("_Smoothness", smo);
        if (emi.HasValue)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", emi.Value);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        AssetDatabase.CreateAsset(m, path);
        return m;
    }
    static Material GetMat(string name) =>
        AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/Session4/{name}.mat");

    // ── ENVIRONMENT ──────────────────────────────────────────────────────────
    // Static flags on floor/walls/platforms so students can bake lightmaps.

    static void BuildEnvironment()
    {
        int gLayer = LayerMask.NameToLayer("Ground");
        var arena  = new GameObject("Arena");

        // Floor — Static for GI baking
        var floor = Cube("Floor", new Vector3(0, -0.25f, 0), new Vector3(6, 0.5f, 6), arena, GetMat("Mat_Floor"));
        floor.layer = gLayer;
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);

        // Walls
        var walls = Child("Walls", arena);
        foreach (var (n, p, s) in new[]{
            ("Wall_North", new Vector3(0,3,-32), new Vector3(56,6,1)),
            ("Wall_South", new Vector3(0,3, 28), new Vector3(56,6,1)),
            ("Wall_East",  new Vector3(28,3,-2), new Vector3(1,6,62)),
            ("Wall_West",  new Vector3(-28,3,-2),new Vector3(1,6,62)) })
        {
            var w = Cube(n, p, s, walls, GetMat("Mat_Wall"));
            w.layer = gLayer;
            GameObjectUtility.SetStaticEditorFlags(w, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);
        }

        // Emissive wall trims — Bloom teaching targets (not Static so they show realtime emission)
        var trims = Child("Trims", walls);
        TrimStrip("Trim_N", new Vector3(0, 6.1f,-32), new Vector3(56,0.3f,1.2f), "Mat_Trim_Purple", trims);
        TrimStrip("Trim_S", new Vector3(0, 6.1f, 28), new Vector3(56,0.3f,1.2f), "Mat_Trim_Purple", trims);
        TrimStrip("Trim_E", new Vector3(28,6.1f,-2 ), new Vector3(1.2f,0.3f,62), "Mat_Trim_Cyan",   trims);
        TrimStrip("Trim_W", new Vector3(-28,6.1f,-2), new Vector3(1.2f,0.3f,62), "Mat_Trim_Cyan",   trims);

        // Platforms — Static for baking
        var t1 = Child("Tier1_Platforms", arena);
        StaticPlatform("T1_Left",   new Vector3(-10,0.5f,-6), new Vector3(7,1,5), "Mat_Platform_A", gLayer, t1);
        StaticPlatform("T1_Center", new Vector3(  0,0.5f,-8), new Vector3(8,1,6), "Mat_Platform_A", gLayer, t1);
        StaticPlatform("T1_Right",  new Vector3( 10,0.5f,-6), new Vector3(7,1,5), "Mat_Platform_A", gLayer, t1);
        StaticPlatform("T1_StepL",  new Vector3( -5,0.5f,-10),new Vector3(3,1,3), "Mat_Platform_A", gLayer, t1);
        StaticPlatform("T1_StepR",  new Vector3(  5,0.5f,-10),new Vector3(3,1,3), "Mat_Platform_A", gLayer, t1);

        var t2 = Child("Tier2_Platforms", arena);
        StaticPlatform("T2_Left",   new Vector3(-12,3.5f,-15),new Vector3(6,1,5), "Mat_Platform_B", gLayer, t2);
        StaticPlatform("T2_Center", new Vector3(  0,3.5f,-17),new Vector3(7,1,6), "Mat_Platform_B", gLayer, t2);
        StaticPlatform("T2_Right",  new Vector3( 12,3.5f,-15),new Vector3(6,1,5), "Mat_Platform_B", gLayer, t2);

        var t3 = Child("Tier3_Platforms", arena);
        StaticPlatform("T3_Left",   new Vector3( -8,7.5f,-23),new Vector3(5,1,4), "Mat_Platform_C", gLayer, t3);
        StaticPlatform("T3_Center", new Vector3(  0,7.5f,-26),new Vector3(8,1,5), "Mat_Platform_C", gLayer, t3);
        StaticPlatform("T3_Right",  new Vector3(  8,7.5f,-23),new Vector3(5,1,4), "Mat_Platform_C", gLayer, t3);
    }

    static void TrimStrip(string name, Vector3 pos, Vector3 scale, string mat, GameObject parent)
    {
        var go = Cube(name, pos, scale, parent, GetMat(mat));
        Object.DestroyImmediate(go.GetComponent<Collider>());
    }

    static void StaticPlatform(string name, Vector3 pos, Vector3 scale, string mat, int layer, GameObject parent)
    {
        var go = Cube(name, pos, scale, parent, GetMat(mat));
        go.layer = layer;
        GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);
    }

    // ── SPAWN POINT ──────────────────────────────────────────────────────────

    static Transform BuildSpawnPoint()
    {
        var pad = Cube("SpawnPad", new Vector3(0, 0.15f, 22), new Vector3(4, 0.3f, 4), null, GetMat("Mat_SpawnPad"));
        pad.layer = LayerMask.NameToLayer("Ground");
        GameObjectUtility.SetStaticEditorFlags(pad, StaticEditorFlags.ContributeGI);
        var sp = new GameObject("SpawnPoint");
        sp.transform.position = new Vector3(0, 1.5f, 22);
        return sp.transform;
    }

    // ── PLAYER ───────────────────────────────────────────────────────────────

    static void BuildPlayer(Transform spawnTf)
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag  = "Player";
        player.transform.position = spawnTf.position;
        player.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Player");

        var rb = player.AddComponent<Rigidbody>();
        rb.useGravity  = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        var cc = player.GetComponent<CapsuleCollider>();
        cc.material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Materials/Session4/PhysicsMat_Player.physicMaterial");

        var mover = player.AddComponent<PlayerMover>();
        Sp(mover, ("cameraTransform", (Object)_cameraGo.transform), ("moveSpeed", 6f));

        var jumper = player.AddComponent<PlayerJumper>();
        Sp(jumper, ("jumpForce", 7f), ("groundCheckDistance", 1.15f),
                   ("groundCheckRadius", 0.45f), ("groundLayer", (object)LayerMask.GetMask("Ground")));

        var resp = player.AddComponent<PlayerRespawner>();
        Sp(resp, ("spawnPoint", (Object)spawnTf), ("fallThreshold", -10f));

        // Session 4 keeps the camera at a fixed angle overlooking the arena.
        // Camera scripting was covered in Session 3 — the focus here is lighting
        // and rendering, not movement systems. Students wanting orbital camera
        // can copy CameraOrbiter from their Session 3 scene.
        _cameraGo.transform.position = new Vector3(0, 14, 32);
        _cameraGo.transform.rotation = Quaternion.Euler(22, 180, 0);

        // HazardZone void catcher
        var voidGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        voidGo.name = "HazardZone_Void";
        voidGo.transform.position   = new Vector3(0, -10, -2);
        voidGo.transform.localScale = new Vector3(200, 1, 200);
        voidGo.GetComponent<Collider>().isTrigger = true;
        Object.DestroyImmediate(voidGo.GetComponent<MeshRenderer>());
        voidGo.AddComponent<HazardZone>();
    }

    // ── MOVING PLATFORMS ─────────────────────────────────────────────────────

    static void BuildPlatformSection()
    {
        var grp = new GameObject("MovingPlatforms");
        HPlat("MovPlat_H_Left",  new Vector3(-10,1.5f,-13), new Vector3(4,.5f,4), 4f, 1.2f, grp);
        HPlat("MovPlat_H_Right", new Vector3( 10,1.5f,-13), new Vector3(4,.5f,4), 4f, 1.4f, grp);
        HPlat("MovPlat_H_Top",   new Vector3(  0,8f,  -25), new Vector3(4,.5f,4), 10f,1.6f, grp);
        VPlat("MovPlat_V_Center",new Vector3(  0,4f,  -19), new Vector3(4,.5f,4), 4f, 0.8f, grp);
    }

    static void HPlat(string n, Vector3 pos, Vector3 s, float dist, float spd, GameObject p)
    {
        var go = Cube(n, pos, s, p, GetMat("Mat_PlatformH"));
        var rb = go.AddComponent<Rigidbody>(); rb.isKinematic = true;
        var mp = go.AddComponent<PlatformMoverHorizontal>();
        Sp(mp, ("moveDistance", dist), ("moveSpeed", spd));
    }
    static void VPlat(string n, Vector3 pos, Vector3 s, float h, float spd, GameObject p)
    {
        var go = Cube(n, pos, s, p, GetMat("Mat_PlatformV"));
        var rb = go.AddComponent<Rigidbody>(); rb.isKinematic = true;
        var mp = go.AddComponent<PlatformMoverVertical>();
        Sp(mp, ("moveHeight", h), ("moveSpeed", spd));
    }

    // ── HAZARD SECTION ───────────────────────────────────────────────────────

    static void BuildHazardSection()
    {
        var grp = Child("HazardZones", null);
        HazardPad("Hazard_Lava_A", new Vector3( 0, 0.05f,-12), new Vector3(28,.1f,4),  grp);
        HazardPad("Hazard_Lava_B", new Vector3( 0, 3.55f,-11), new Vector3(28,.1f,4),  grp);
        HazardPad("Hazard_Lava_C", new Vector3( 0, 3.55f,-20), new Vector3(28,.1f,4),  grp);
    }

    static void HazardPad(string name, Vector3 pos, Vector3 scale, GameObject parent)
    {
        var go = Cube(name, pos, scale, parent, GetMat("Mat_Hazard"));
        go.GetComponent<Collider>().isTrigger = true;
        go.AddComponent<HazardZone>();
    }

    // ── COLLECTIBLES ─────────────────────────────────────────────────────────
    // Point lights as children — shadows disabled; emissive + local light is the teaching moment.

    static void BuildCollectibles()
    {
        EnsureFolder("Assets/Prefabs");
        var burstPrefab = MakeParticlePrefab("FX_Collect_Burst", new Color(1f, 0.85f, 0f));

        var grp     = new GameObject("Collectibles");
        var common  = Child("Common", grp);
        var rare    = Child("Rare",   grp);
        var bonus   = Child("Bonus",  grp);

        var commonPos = new Vector3[]
        {
            new(-3,1,18), new(3,1,18), new(0,1,18),     // spawn area
            new(-12,1,2), new(0,1,2), new(12,1,2),       // trigger zone row
            new(-10,2,-6), new(0,2,-8), new(10,2,-6),    // tier 1
        };
        for (int i = 0; i < commonPos.Length; i++)
            SpawnCoin($"Coin_C{i+1:D2}", commonPos[i], .50f, "Mat_Coin_Common", 10, burstPrefab, common);

        var rarePos = new Vector3[]
        { new(-12,5,-15), new(0,5,-17), new(12,5,-15), new(-10,5,-16), new(10,5,-16) };
        for (int i = 0; i < rarePos.Length; i++)
            SpawnCoin($"Coin_R{i+1:D2}", rarePos[i], .55f, "Mat_Coin_Rare", 25, burstPrefab, rare);

        var bonusPos = new Vector3[]
        { new(-8,9,-23), new(0,9.5f,-26), new(8,9,-23) };
        for (int i = 0; i < bonusPos.Length; i++)
            SpawnCoin($"Coin_B{i+1:D2}", bonusPos[i], .65f, "Mat_Coin_Bonus", 50, burstPrefab, bonus);
    }

    static void SpawnCoin(string name, Vector3 pos, float sz, string mat, int pts, GameObject burst, GameObject parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position   = pos;
        go.transform.localScale = Vector3.one * sz;
        go.GetComponent<MeshRenderer>().sharedMaterial = GetMat(mat);
        go.GetComponent<Collider>().isTrigger = true;
        go.transform.SetParent(parent.transform);

        var col = go.AddComponent<Collectible>();
        Sp(col, ("pointValue", pts));

        // Point light — yellow/tinted, range 3, no shadows.
        // Teaching: Emission drives the material; Point Light drives the scene.
        var lightGo = new GameObject("PointLight");
        lightGo.transform.SetParent(go.transform, false);
        var pl = lightGo.AddComponent<Light>();
        pl.type      = LightType.Point;
        pl.range     = 3f;
        pl.intensity = 1.8f;
        pl.color     = mat == "Mat_Coin_Common" ? new Color(1f,0.9f,0.2f)
                     : mat == "Mat_Coin_Rare"   ? new Color(0.2f,0.8f,1f)
                                                : new Color(1f,0.2f,0.9f);
        pl.shadows   = LightShadows.None; // shadow-casting point lights are expensive
        pl.lightmapBakeType = LightmapBakeType.Realtime; // coins are destroyed at runtime
    }

    // ── LIGHT PROBES ─────────────────────────────────────────────────────────
    // 8 probes in a low grid — dynamic objects (player, coins) sample them
    // to receive believable ambient colour from the baked environment.

    static void BuildLightProbes()
    {
        var lpGo = new GameObject("LightProbeGroup");
        var lpg  = lpGo.AddComponent<LightProbeGroup>();

        lpg.probePositions = new Vector3[]
        {
            // Floor level ring — just above ground
            new(-20,1.5f, 15), new(20,1.5f, 15),
            new(-20,1.5f,-10), new(20,1.5f,-10),
            new(-20,1.5f,-25), new(20,1.5f,-25),
            // Mid-height probes for elevated platforms
            new(-12,5f,-15), new(12,5f,-15),
            // Tier 3
            new(0,9f,-26),
        };
    }

    // ── REFLECTION PROBE ─────────────────────────────────────────────────────
    // One baked probe at arena centre. Increase Mat_Floor smoothness > 0.5
    // to see it reflect the baked scene instead of the default skybox.

    static void BuildReflectionProbe()
    {
        var rpGo = new GameObject("ReflectionProbe_Centre");
        rpGo.transform.position = new Vector3(0, 3, -5);
        var rp = rpGo.AddComponent<ReflectionProbe>();
        rp.mode       = ReflectionProbeMode.Baked;
        rp.size       = new Vector3(60, 20, 70);
        rp.boxProjection = true;
        rp.resolution = 128;
        rp.importance = 1;
    }

    // ── MATERIAL SHOWCASE ────────────────────────────────────────────────────
    // A row of spheres demonstrating the Metallic × Smoothness grid.
    // Placed near the spawn pad so students see them immediately.

    static void BuildMaterialShowcase()
    {
        var grp = new GameObject("MaterialShowcase");
        grp.transform.position = new Vector3(-14, 0, 20);

        // Sign label parent (text label not possible via editor script without canvas;
        // naming the GameObjects descriptively is sufficient for the demo)
        var configs = new (string name, float met, float smo)[]
        {
            ("Chalk_M0_S0",    0.0f, 0.0f),
            ("Plastic_M0_S1",  0.0f, 1.0f),
            ("BrushedSteel_M1_S0", 1.0f, 0.0f),
            ("Chrome_M1_S1",   1.0f, 1.0f),
            ("MidRange_M05_S05", 0.5f, 0.5f),
        };

        for (int i = 0; i < configs.Length; i++)
        {
            var (n, met, smo) = configs[i];
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = n;
            sphere.transform.SetParent(grp.transform);
            sphere.transform.localPosition = new Vector3(i * 2.5f, 1, 0);
            sphere.transform.localScale    = Vector3.one * 0.9f;

            // Each sphere gets a unique material for real-time inspector editing
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetColor("_BaseColor",  new Color(0.7f, 0.7f, 0.7f));
            mat.SetFloat("_Metallic",   met);
            mat.SetFloat("_Smoothness", smo);
            AssetDatabase.CreateAsset(mat, $"Assets/Materials/Session4/Mat_Demo_{n}.mat");
            sphere.GetComponent<MeshRenderer>().sharedMaterial = mat;
            GameObjectUtility.SetStaticEditorFlags(sphere, StaticEditorFlags.ContributeGI);
        }

        // Small label cubes behind each sphere so students can read the names in Scene view
        AssetDatabase.SaveAssets();
    }

    // ── SCORE UI ─────────────────────────────────────────────────────────────

    static void BuildScoreUI()
    {
        var smGo = new GameObject("ScoreManager");
        var sm   = smGo.AddComponent<ScoreManager>();

        var canvasGo = new GameObject("Canvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // HUD top bar
        var hud = MakePanel("HUD", canvasGo, new Color(0,0,0,.5f),
            new Vector2(0,1), new Vector2(1,1), new Vector2(.5f,1), new Vector2(0,72));

        var scoreTmp = MakeTMP("ScoreText", hud, "Score: 0", 40, new Color(1f,.9f,.15f),
            new Vector2(350,60), new Vector2(20,-36),
            new Vector2(0,.5f), new Vector2(0,.5f), TextAlignmentOptions.Left);

        MakeTMP("HintText", hud,
            "WASD Move  |  Space Jump  |  Collect all coins!",
            22, new Color(.85f,.85f,.85f,.75f),
            new Vector2(800,60), new Vector2(-20,-36),
            new Vector2(1,.5f), new Vector2(1,.5f), TextAlignmentOptions.Right);

        // Bottom info bar — reminds students about the baking workflow
        var bottomBar = MakePanel("InfoBar", canvasGo, new Color(0,0,0,.35f),
            new Vector2(0,0), new Vector2(1,0), new Vector2(.5f,0), new Vector2(0,44));
        MakeTMP("InfoLabel", bottomBar,
            "Session 4 — Window > Rendering > Lighting > Generate Lighting to bake  |  Light Probes & Reflection Probe present",
            20, new Color(.9f,.9f,.9f,.80f),
            new Vector2(1400,36), new Vector2(0,22),
            new Vector2(.5f,0), new Vector2(.5f,0), TextAlignmentOptions.Center);

        var sdGo = new GameObject("ScoreDisplay");
        sdGo.transform.SetParent(canvasGo.transform, false);
        var sd = sdGo.AddComponent<ScoreDisplay>();
        Sp(sd, ("scoreText", (Object)scoreTmp));

        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(sm.OnScoreChanged, sd.UpdateText);
    }

    // ── PARTICLE PREFAB ──────────────────────────────────────────────────────

    static GameObject MakeParticlePrefab(string name, Color color)
    {
        string path = $"Assets/Prefabs/{name}.prefab";
        EnsureFolder("Assets/Prefabs");
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = new GameObject(name);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration      = 0.5f;
        main.loop          = false;
        main.startLifetime = 0.4f;
        main.startSpeed    = 5f;
        main.startSize     = 0.12f;
        main.startColor    = color;
        main.stopAction    = ParticleSystemStopAction.Destroy;
        var em = ps.emission;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, 24) });
        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Sphere;
        sh.radius    = 0.1f;

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ── PRIMITIVE & UI HELPERS ───────────────────────────────────────────────

    static GameObject Cube(string name, Vector3 pos, Vector3 scale, GameObject parent, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position   = pos;
        go.transform.localScale = scale;
        if (mat != null)    go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        if (parent != null) go.transform.SetParent(parent.transform);
        return go;
    }

    static GameObject Child(string name, GameObject parent)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent.transform);
        return go;
    }

    static string EnsureFolder(string path)
    {
        string[] parts = path.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
        return cur;
    }

    static GameObject MakePanel(string name, GameObject parent, Color color,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>(); img.color = color;
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin; rt.anchorMax = ancMax;
        rt.pivot = pivot; rt.sizeDelta = size;
        return go;
    }

    static TextMeshProUGUI MakeTMP(string name, GameObject parent, string text, int size,
        Color color, Vector2 sd, Vector2 apos, Vector2? aMin = null, Vector2? aMax = null,
        TextAlignmentOptions align = TextAlignmentOptions.Left)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = size; tmp.color = color; tmp.alignment = align;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin ?? new Vector2(0,1); rt.anchorMax = aMax ?? new Vector2(0,1);
        rt.pivot = new Vector2(0,.5f); rt.sizeDelta = sd; rt.anchoredPosition = apos;
        return tmp;
    }

    // Generic SerializedObject property setter — supports float, int, bool, Vector3, Color, Object
    static void Sp(Object obj, params (string n, object v)[] props)
    {
        var so = new SerializedObject(obj);
        foreach (var (n, v) in props)
        {
            var p = so.FindProperty(n);
            if (p == null) { Debug.LogWarning($"[S4Builder] property '{n}' not found on {obj.GetType().Name}"); continue; }
            switch (v)
            {
                case float   f: p.floatValue = f; break;
                case int     i: p.intValue   = i; break;
                case bool    b: p.boolValue  = b; break;
                case Vector3 u: p.vector3Value = u; break;
                case Color   c: p.colorValue   = c; break;
                case Object  o: p.objectReferenceValue = o; break;
            }
        }
        so.ApplyModifiedProperties();
    }
}
