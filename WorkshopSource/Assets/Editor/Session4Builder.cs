using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;

using Workshop.Session2.Movement;
using Workshop.Session2.Camera;
using Workshop.Session2.Collectibles;

using Workshop.Session3.Hazards;
using Workshop.Session3.GameFlow;
using Workshop.Session2.UI;
using Workshop.Session3.Feedback;

using Workshop.Session4.Polish;
using Workshop.Session4.Advanced;
using Workshop.Session4.Spawning;

public class Session4Builder
{
    [MenuItem("Workshop/Build Session 4")]
    public static void Build()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        bool success = EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Session4.unity");
        if (!success)
        {
            Debug.LogError("Failed to save Assets/Scenes/Session4.unity. Ensure the folder exists.");
            return;
        }

        BuildSkyboxAndLighting();
        BuildPostProcessing();
        BuildMaterials();
        BuildEnvironment();
        BuildPlayerAndCamera();
        BuildHazards();
        BuildCollectibles();
        BuildManagersAndUI();
        BuildAudio();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Session4.unity");
        Debug.Log("Session 4 built successfully!");
    }

    private static void BuildSkyboxAndLighting()
    {
        Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));
        skyboxMat.SetColor("_SkyTint", new Color(0.4f, 0.6f, 0.9f));
        skyboxMat.SetFloat("_AtmosphereThickness", 1.2f);
        skyboxMat.SetColor("_GroundColor", new Color(0.15f, 0.12f, 0.10f));
        skyboxMat.SetFloat("_Exposure", 1.1f);
        RenderSettings.skybox = skyboxMat;

        GameObject sun = new GameObject("Sun");
        Light light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        sun.transform.rotation = Quaternion.Euler(52, -35, 0);
        light.color = new Color(1.0f, 0.92f, 0.78f);
        light.intensity = 1.6f;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.75f;
        light.shadowResolution = LightShadowResolution.High;
        RenderSettings.sun = light;
    }

    private static void BuildPostProcessing()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Settings"))
        {
            AssetDatabase.CreateFolder("Assets", "Settings");
        }

        GameObject ppVol = new GameObject("PostProcessing_Volume");
        ppVol.transform.position = Vector3.zero;
        Volume vol = ppVol.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.priority = 1;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, "Assets/Settings/S4_PostProcessProfile.asset");

        Bloom bloom = profile.Add<Bloom>();
        bloom.active = true;
        bloom.threshold.Override(0.9f);
        bloom.intensity.Override(0.8f);
        bloom.scatter.Override(0.7f);
        bloom.tint.Override(new Color(1.0f, 0.95f, 0.85f));

        ColorAdjustments ca = profile.Add<ColorAdjustments>();
        ca.active = true;
        ca.postExposure.Override(0.2f);
        ca.contrast.Override(12f);
        ca.colorFilter.Override(new Color(1.0f, 0.97f, 0.93f));
        ca.saturation.Override(15f);

        Vignette vignette = profile.Add<Vignette>();
        vignette.active = true;
        vignette.color.Override(Color.black);
        vignette.intensity.Override(0.28f);
        vignette.smoothness.Override(0.4f);
        vignette.rounded.Override(true);

        DepthOfField dof = profile.Add<DepthOfField>();
        dof.active = true;
        dof.mode.Override(DepthOfFieldMode.Bokeh);
        dof.focusDistance.Override(12f);
        dof.aperture.Override(5.6f);
        dof.focalLength.Override(50f);

        vol.profile = profile;
        AssetDatabase.SaveAssets();
    }

    private static void BuildMaterials()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Session4")) AssetDatabase.CreateFolder("Assets/Materials", "Session4");

        CreateMat("Mat_Floor", new Color(0.14f, 0.14f, 0.16f), 0.05f, 0.6f);
        CreateMat("Mat_Floor_Grid", new Color(0.10f, 0.10f, 0.12f), 0f, 0f, new Color(0.0f, 0.3f, 0.6f) * 0.4f);
        CreateMat("Mat_Wall", new Color(0.08f, 0.07f, 0.15f), 0.1f, 0.2f);
        CreateMat("Mat_Wall_Trim", new Color(0.2f, 0.15f, 0.4f), 0f, 0f, new Color(0.3f, 0.1f, 0.8f) * 0.6f);
        CreateMat("Mat_Platform_Low", new Color(0.2f, 0.3f, 0.5f), 0.2f, 0.45f);
        CreateMat("Mat_Platform_Mid", new Color(0.3f, 0.2f, 0.5f), 0.25f, 0.55f, new Color(0.2f, 0.05f, 0.5f) * 0.3f);
        CreateMat("Mat_Platform_High", new Color(0.5f, 0.3f, 0.1f), 0.5f, 0.7f, new Color(0.8f, 0.4f, 0.0f) * 0.4f);
        CreateMat("Mat_Ramp", new Color(0.25f, 0.25f, 0.28f), 0.1f, 0.25f);
        CreateMat("Mat_MovingPlatform", new Color(0.1f, 0.6f, 0.45f), 0.3f, 0.6f, new Color(0.0f, 0.8f, 0.5f) * 0.5f);
        CreateMat("Mat_Hazard", new Color(0.8f, 0.15f, 0.0f), 0f, 0f, new Color(1.0f, 0.3f, 0.0f) * 1.2f);
        CreateMat("Mat_SwingBar", new Color(0.7f, 0.4f, 0.0f), 0.6f, 0.7f);
        CreateMat("Mat_Player", new Color(0.0f, 0.9f, 0.5f), 0.2f, 0.7f, new Color(0.0f, 0.5f, 0.3f) * 0.3f);
        CreateMat("Mat_Collectible_Common", new Color(1.0f, 0.85f, 0.0f), 1.0f, 1.0f, new Color(1.0f, 0.7f, 0.0f) * 0.8f);
        CreateMat("Mat_Collectible_Rare", new Color(0.0f, 0.8f, 1.0f), 1.0f, 1.0f, new Color(0.0f, 0.6f, 1.0f) * 1.0f);
        CreateMat("Mat_Collectible_Bonus", new Color(1.0f, 0.2f, 0.9f), 1.0f, 1.0f, new Color(1.0f, 0.0f, 0.8f) * 1.2f);
        CreateMat("Mat_Pillar", new Color(0.18f, 0.15f, 0.3f), 0.3f, 0.4f);
        
        // Slow zone material
        Material slowMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        slowMat.SetFloat("_Surface", 1); // Transparent
        slowMat.SetOverrideTag("RenderType", "Transparent");
        slowMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        slowMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        slowMat.SetInt("_ZWrite", 0);
        slowMat.DisableKeyword("_ALPHATEST_ON");
        slowMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        slowMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        slowMat.SetColor("_BaseColor", new Color(0.3f, 0.3f, 1.0f, 0.25f));
        AssetDatabase.CreateAsset(slowMat, "Assets/Materials/Session4/Mat_SlowZone.mat");

        AssetDatabase.SaveAssets();
    }

    private static Material CreateMat(string name, Color baseColor, float metallic, float smoothness, Color? emission = null)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);
        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission.Value);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        AssetDatabase.CreateAsset(mat, $"Assets/Materials/Session4/{name}.mat");
        return mat;
    }

    private static Material GetMat(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/Session4/{name}.mat");
    }

    private static void BuildEnvironment()
    {
        GameObject arena = new GameObject("Arena");

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(6, 1, 6);
        floor.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Floor");
        floor.transform.SetParent(arena.transform);

        GameObject floorGrid = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floorGrid.name = "Floor_Grid";
        floorGrid.transform.position = new Vector3(0, 0.01f, 0);
        floorGrid.transform.localScale = new Vector3(6, 1, 6);
        floorGrid.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Floor_Grid");
        Object.DestroyImmediate(floorGrid.GetComponent<Collider>());
        floorGrid.transform.SetParent(arena.transform);

        // Walls
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(arena.transform);
        CreateCube("Wall_North", new Vector3(0, 5, 30), new Vector3(60, 10, 1), walls, GetMat("Mat_Wall"));
        CreateCube("Wall_South", new Vector3(0, 5, -30), new Vector3(60, 10, 1), walls, GetMat("Mat_Wall"));
        CreateCube("Wall_East", new Vector3(30, 5, 0), new Vector3(1, 10, 60), walls, GetMat("Mat_Wall"));
        CreateCube("Wall_West", new Vector3(-30, 5, 0), new Vector3(1, 10, 60), walls, GetMat("Mat_Wall"));

        // Trims
        GameObject trims = new GameObject("Trims");
        trims.transform.SetParent(walls.transform);
        CreateTrim("Trim_North", new Vector3(0, 10.1f, 30), new Vector3(60, 0.2f, 1.1f), trims);
        CreateTrim("Trim_South", new Vector3(0, 10.1f, -30), new Vector3(60, 0.2f, 1.1f), trims);
        CreateTrim("Trim_East", new Vector3(30, 10.1f, 0), new Vector3(1.1f, 0.2f, 60), trims);
        CreateTrim("Trim_West", new Vector3(-30, 10.1f, 0), new Vector3(1.1f, 0.2f, 60), trims);

        // Tiers
        GameObject tier1 = new GameObject("Tier1_Platforms");
        tier1.transform.SetParent(arena.transform);
        CreateCube("Plat_L1_A", new Vector3(10, 0.5f, 10), new Vector3(8, 1, 8), tier1, GetMat("Mat_Platform_Low"));
        CreateCube("Plat_L1_B", new Vector3(-12, 0.5f, 8), new Vector3(6, 1, 10), tier1, GetMat("Mat_Platform_Low"));
        CreateCube("Plat_L1_C", new Vector3(0, 0.5f, -15), new Vector3(10, 1, 6), tier1, GetMat("Mat_Platform_Low"));
        CreateCube("Plat_L1_D", new Vector3(16, 0.5f, -8), new Vector3(5, 1, 5), tier1, GetMat("Mat_Platform_Low"));

        GameObject tier2 = new GameObject("Tier2_Platforms");
        tier2.transform.SetParent(arena.transform);
        CreateCube("Plat_L2_A", new Vector3(-10, 4.5f, -4), new Vector3(8, 1, 8), tier2, GetMat("Mat_Platform_Mid"));
        CreateCube("Plat_L2_B", new Vector3(6, 4.5f, -6), new Vector3(7, 1, 7), tier2, GetMat("Mat_Platform_Mid"));
        CreateCube("Plat_L2_C", new Vector3(-18, 4.5f, -10), new Vector3(5, 1, 5), tier2, GetMat("Mat_Platform_Mid"));
        CreateCube("Plat_L2_D", new Vector3(-12, 4.5f, -8), new Vector3(3, 1, 3), tier2, GetMat("Mat_Platform_Mid"));

        GameObject tier3 = new GameObject("Tier3_Platforms");
        tier3.transform.SetParent(arena.transform);
        CreateCube("Plat_L3_A", new Vector3(0, 9.5f, -20), new Vector3(10, 1, 8), tier3, GetMat("Mat_Platform_High"));
        CreateCube("Plat_L3_B", new Vector3(12, 9.5f, -18), new Vector3(5, 1, 5), tier3, GetMat("Mat_Platform_High"));
        CreateCube("Plat_L3_C", new Vector3(-12, 9.5f, -18), new Vector3(5, 1, 5), tier3, GetMat("Mat_Platform_High"));

        // Ramps
        GameObject ramps = new GameObject("Ramps");
        ramps.transform.SetParent(arena.transform);
        CreateCube("Ramp_A", new Vector3(-8, 2, 2), new Vector3(4, 0.5f, 8), ramps, GetMat("Mat_Ramp")).transform.rotation = Quaternion.Euler(25, 0, 0);
        CreateCube("Ramp_B", new Vector3(14, 2.5f, 0), new Vector3(4, 0.5f, 10), ramps, GetMat("Mat_Ramp")).transform.rotation = Quaternion.Euler(25, 0, 0);
        CreateCube("Ramp_C", new Vector3(0, 6, -10), new Vector3(5, 0.5f, 8), ramps, GetMat("Mat_Ramp")).transform.rotation = Quaternion.Euler(30, 0, 0);

        // Pillars
        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(arena.transform);
        CreatePillar("Pillar_01", new Vector3(6, 2, 6), pillars);
        CreatePillar("Pillar_02", new Vector3(-6, 2, 6), pillars);
        CreatePillar("Pillar_03", new Vector3(6, 2, -2), pillars);
        CreatePillar("Pillar_04", new Vector3(-6, 2, -2), pillars);
        CreatePillar("Pillar_05", new Vector3(14, 2, 12), pillars);
        CreatePillar("Pillar_06", new Vector3(-14, 2, 12), pillars);

        // Extra B - Welcome Gate
        GameObject gate = new GameObject("Gate");
        gate.transform.position = new Vector3(0, 0, 24);
        gate.transform.SetParent(arena.transform);

        CreateCube("Gate_PillarLeft", new Vector3(-3, 3, 24), new Vector3(1, 6, 1), gate, GetMat("Mat_Pillar"));
        CreateCube("Gate_PillarRight", new Vector3(3, 3, 24), new Vector3(1, 6, 1), gate, GetMat("Mat_Pillar"));

        GameObject gateBeam = CreateCube("Gate_Beam", new Vector3(0, 6.5f, 24), new Vector3(7, 0.5f, 1), gate, GetMat("Mat_Wall_Trim"));
        ColorPulse beamPulse = gateBeam.AddComponent<ColorPulse>();
        SerializedObject soBeamPulse = new SerializedObject(beamPulse);
        soBeamPulse.FindProperty("m_colorA").colorValue = new Color(0.3f, 0.1f, 0.8f);
        soBeamPulse.FindProperty("m_colorB").colorValue = new Color(0.0f, 0.6f, 1.0f);
        soBeamPulse.FindProperty("m_pulseSpeed").floatValue = 1.5f;
        soBeamPulse.ApplyModifiedProperties();

        // Extra D - Checkpoint
        Material cpMat = CreateMat("Mat_Checkpoint", new Color(0.3f, 1.0f, 0.3f), 0.2f, 0.5f, new Color(0.1f, 0.8f, 0.1f) * 0.5f);
        GameObject cp = CreateCube("Checkpoint_Tier2", new Vector3(-8, 3, -1), new Vector3(3, 6, 3), GameObject.Find("Tier2_Platforms"), cpMat);
        cp.GetComponent<Collider>().isTrigger = true;
        cp.AddComponent<Workshop.Session4.Advanced.CheckpointZone>();
    }

    private static void CreateTrim(string name, Vector3 pos, Vector3 scale, GameObject parent)
    {
        GameObject go = CreateCube(name, pos, scale, parent, GetMat("Mat_Wall_Trim"));
        Object.DestroyImmediate(go.GetComponent<Collider>());
    }

    private static void CreatePillar(string name, Vector3 pos, GameObject parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(0.6f, 4f, 0.6f);
        go.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Pillar");
        go.GetComponent<Collider>().isTrigger = false;
        go.transform.SetParent(parent.transform);
    }

    private static GameObject CreateCube(string name, Vector3 pos, Vector3 scale, GameObject parent, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        go.transform.SetParent(parent.transform);
        return go;
    }

    private static void BuildPlayerAndCamera()
    {
        GameObject spawn = new GameObject("SpawnPoint");
        spawn.transform.position = new Vector3(0, 1.5f, 22);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 1.5f, 22);
        player.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Player");

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

        Workshop.Session3.Movement.PlayerOrbitalMover mover = player.AddComponent<Workshop.Session3.Movement.PlayerOrbitalMover>();
        mover.MoveSpeed = 7f;

        PlayerJumper jumper = player.AddComponent<PlayerJumper>();
        SerializedObject soJumper = new SerializedObject(jumper);
        soJumper.FindProperty("m_jumpForce").floatValue = 8f;
        soJumper.FindProperty("m_groundCheckDistance").floatValue = 1.15f;
        soJumper.FindProperty("m_groundLayer").intValue = LayerMask.GetMask("Default");
        soJumper.ApplyModifiedProperties();

        PlayerSlider slider = player.AddComponent<PlayerSlider>();
        SerializedObject soSlider = new SerializedObject(slider);
        soSlider.FindProperty("m_slideSpeedMultiplier").floatValue = 2.2f;
        soSlider.FindProperty("m_slideDuration").floatValue = 0.7f;
        soSlider.FindProperty("m_slideCooldown").floatValue = 1.2f;
        soSlider.ApplyModifiedProperties();

        PlayerRespawner respawner = player.AddComponent<PlayerRespawner>();
        SerializedObject soRespawner = new SerializedObject(respawner);
        soRespawner.FindProperty("m_spawnPoint").objectReferenceValue = spawn.transform;
        soRespawner.FindProperty("m_respawnDelay").floatValue = 0.4f;
        soRespawner.ApplyModifiedProperties();

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
        }
        mainCam.transform.position = new Vector3(0, 6, 18);
        mainCam.transform.rotation = Quaternion.Euler(15, 180, 0);
        UniversalAdditionalCameraData camData = mainCam.gameObject.GetComponent<UniversalAdditionalCameraData>();
        if (camData == null) camData = mainCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        camData.renderPostProcessing = true;

        CameraFollower follower = mainCam.gameObject.AddComponent<CameraFollower>();
        SerializedObject soFollower = new SerializedObject(follower);
        soFollower.FindProperty("m_target").objectReferenceValue = player.transform;
        soFollower.FindProperty("m_offset").vector3Value = new Vector3(0, 6, -10);
        soFollower.FindProperty("m_smoothSpeed").floatValue = 0.10f;
        soFollower.ApplyModifiedProperties();
        follower.enabled = false;

        ScreenShake shake = mainCam.gameObject.AddComponent<ScreenShake>();
        SerializedObject soShake = new SerializedObject(shake);
        soShake.FindProperty("m_defaultDuration").floatValue = 0.3f;
        soShake.FindProperty("m_defaultMagnitude").floatValue = 0.2f;
        soShake.ApplyModifiedProperties();

        CameraOrbiter orbiter = mainCam.gameObject.AddComponent<CameraOrbiter>();
        SerializedObject soOrbiter = new SerializedObject(orbiter);
        soOrbiter.FindProperty("m_target").objectReferenceValue = player.transform;
        soOrbiter.FindProperty("m_distance").floatValue = 8f;
        soOrbiter.FindProperty("m_orbitSpeed").floatValue = 3f;
        soOrbiter.FindProperty("m_verticalClamp").vector2Value = new Vector2(-20f, 80f);
        soOrbiter.FindProperty("m_zoomSpeed").floatValue = 2f;
        soOrbiter.FindProperty("m_zoomClamp").vector2Value = new Vector2(2f, 15f);
        soOrbiter.ApplyModifiedProperties();
        orbiter.enabled = true;
    }

    private static void BuildHazards()
    {
        GameObject hazards = new GameObject("Hazards");

        // Lava
        GameObject lavaZones = new GameObject("LavaZones");
        lavaZones.transform.SetParent(hazards.transform);

        CreateHazard("HazardZone_Lava_A", new Vector3(0, 0.05f, 5), new Vector3(8, 0.1f, 6), GetMat("Mat_Hazard"), lavaZones);
        CreateHazard("HazardZone_Lava_B", new Vector3(-5, 0.05f, -8), new Vector3(6, 0.1f, 5), GetMat("Mat_Hazard"), lavaZones);
        CreateHazard("HazardZone_Lava_C", new Vector3(-14, 4.55f, -6), new Vector3(3, 0.1f, 4), GetMat("Mat_Hazard"), lavaZones);

        GameObject hzVoid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hzVoid.name = "HazardZone_Void";
        hzVoid.transform.position = new Vector3(0, -5, 0);
        hzVoid.transform.localScale = new Vector3(100, 1, 100);
        hzVoid.GetComponent<Collider>().isTrigger = true;
        Object.DestroyImmediate(hzVoid.GetComponent<MeshRenderer>());
        hzVoid.AddComponent<HazardZone>();
        hzVoid.transform.SetParent(lavaZones.transform);

        // Moving Platforms
        GameObject movPlats = new GameObject("Platforms");
        movPlats.transform.SetParent(hazards.transform);
        CreateMovingPlatform("MovPlat_A", new Vector3(0, 1.5f, 0), new Vector3(3, 0.5f, 3), new Vector3(0, 1.5f, -10), 1.5f, movPlats);
        CreateMovingPlatform("MovPlat_B", new Vector3(-6, 5, -6), new Vector3(3, 0.5f, 3), new Vector3(-16, 5, -6), 2.0f, movPlats);
        CreateMovingPlatform("MovPlat_C", new Vector3(6, 5, -12), new Vector3(4, 0.5f, 4), new Vector3(6, 10, -12), 1.0f, movPlats);

        // Swinging Bars
        GameObject swingBars = new GameObject("SwingBars");
        swingBars.transform.SetParent(hazards.transform);
        CreateSwingBar("SwingBar_A_Pivot", new Vector3(6, 7, -6), 70, 0.8f, 0, new Vector3(0, 1, 0), "SwingBar_A_Bar", new Vector3(3, 0, 0), new Vector3(6, 0.3f, 0.3f), swingBars);
        CreateSwingBar("SwingBar_B_Pivot", new Vector3(0, 14, -18), 50, 1.1f, 1.57f, new Vector3(0, 0, 1), "SwingBar_B_Bar", new Vector3(0, -3, 0), new Vector3(5, 0.3f, 0.3f), swingBars);

        // Slow Zones
        GameObject slowZones = new GameObject("SlowZones");
        slowZones.transform.SetParent(hazards.transform);
        CreateSlowZone("SlowZone_A", new Vector3(4, 3, -4), new Vector3(4, 4, 4), 0.35f, 4.0f, slowZones);
        CreateSlowZone("SlowZone_B", new Vector3(0, 10, -17), new Vector3(10, 4, 3), 0.4f, 3.0f, slowZones);
    }

    private static void CreateSlowZone(string name, Vector3 pos, Vector3 scale, float slowScale, float transition, GameObject parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_SlowZone");
        go.GetComponent<Collider>().isTrigger = true;
        SlowMotionZone zone = go.AddComponent<SlowMotionZone>();
        SerializedObject so = new SerializedObject(zone);
        so.FindProperty("m_slowScale").floatValue = slowScale;
        so.FindProperty("m_transitionSpeed").floatValue = transition;
        so.ApplyModifiedProperties();
        go.transform.SetParent(parent.transform);
    }

    private static void CreateHazard(string name, Vector3 pos, Vector3 scale, Material mat, GameObject parent)
    {
        GameObject go = CreateCube(name, pos, scale, parent, mat);
        go.GetComponent<Collider>().isTrigger = true;
        go.AddComponent<HazardZone>();

        if (name.Contains("Lava"))
        {
            ColorPulse cp = go.AddComponent<ColorPulse>();
            SerializedObject soCp = new SerializedObject(cp);
            soCp.FindProperty("m_colorA").colorValue = new Color(0.9f, 0.1f, 0.0f);
            soCp.FindProperty("m_colorB").colorValue = new Color(1.0f, 0.6f, 0.0f);
            soCp.FindProperty("m_pulseSpeed").floatValue = 3.0f;
            soCp.ApplyModifiedProperties();
        }
    }

    private static void CreateMovingPlatform(string name, Vector3 pos, Vector3 scale, Vector3 ptB, float speed, GameObject parent)
    {
        GameObject go = CreateCube(name, pos, scale, parent, GetMat("Mat_MovingPlatform"));
        MovingPlatform mp = go.AddComponent<MovingPlatform>();
        SerializedObject so = new SerializedObject(mp);
        so.FindProperty("m_pointA").vector3Value = pos;
        so.FindProperty("m_pointB").vector3Value = ptB;
        so.FindProperty("m_speed").floatValue = speed;
        so.FindProperty("m_isSmoothPingPong").boolValue = true;
        so.ApplyModifiedProperties();
    }

    private static void CreateSwingBar(string pivotName, Vector3 pivotPos, float angle, float speed, float offset, Vector3 axis, string barName, Vector3 barLocalPos, Vector3 barScale, GameObject parent)
    {
        GameObject pivot = new GameObject(pivotName);
        pivot.transform.position = pivotPos;
        pivot.transform.SetParent(parent.transform);
        SwingingBar sb = pivot.AddComponent<SwingingBar>();
        SerializedObject so = new SerializedObject(sb);
        so.FindProperty("m_swingAngle").floatValue = angle;
        so.FindProperty("m_swingSpeed").floatValue = speed;
        so.FindProperty("m_phaseOffset").floatValue = offset;
        so.FindProperty("m_swingAxis").vector3Value = axis;
        so.ApplyModifiedProperties();

        GameObject bar = CreateCube(barName, Vector3.zero, barScale, pivot, GetMat("Mat_SwingBar"));
        bar.transform.localPosition = barLocalPos;
        Rigidbody rb = bar.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private static void BuildCollectibles()
    {
        GameObject colls = new GameObject("Collectibles");
        GameObject commonGrp = new GameObject("Common"); commonGrp.transform.SetParent(colls.transform);
        GameObject rareGrp = new GameObject("Rare"); rareGrp.transform.SetParent(colls.transform);
        GameObject bonusGrp = new GameObject("Bonus"); bonusGrp.transform.SetParent(colls.transform);
        GameObject spawnersGrp = new GameObject("Spawners");

        CreateParticlePrefab("FX_Collect_Burst", new Color(1.0f, 0.85f, 0.0f));
        CreateParticlePrefab("FX_Collect_Rare", new Color(0.0f, 0.8f, 1.0f));
        CreateParticlePrefab("FX_Collect_Bonus", new Color(1.0f, 0.2f, 0.9f));

        // Common
        float[] cPhases = { 0.0f, 0.8f, 1.6f, 2.4f, 3.2f, 4.0f, 4.8f, 5.6f };
        Vector3[] cPositions = {
            new Vector3(3, 1, 18), new Vector3(-3, 1, 18), new Vector3(10, 1.5f, 10), new Vector3(-12, 1.5f, 8),
            new Vector3(0, 1, -15), new Vector3(16, 1.5f, -8), new Vector3(-5, 1, -5), new Vector3(5, 1, -5)
        };
        for (int i = 0; i < 8; i++)
        {
            GameObject c = CreateCollectible($"Col_C{i + 1:00}", cPositions[i], 0.5f, GetMat("Mat_Collectible_Common"), 10, 0.25f, 1.5f, 90, null, 0, cPhases[i], commonGrp);
            
            // Generate Prefab from the first common collectible for spawners
            if (i == 0)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
                GameObject prefabObj = Object.Instantiate(c);
                TimedDestroyer td = prefabObj.AddComponent<TimedDestroyer>();
                SerializedObject soTd = new SerializedObject(td);
                soTd.FindProperty("m_lifetime").floatValue = 20f;
                soTd.ApplyModifiedProperties();
                PrefabUtility.SaveAsPrefabAsset(prefabObj, "Assets/Prefabs/Collectible_Common_Prefab.prefab");
                Object.DestroyImmediate(prefabObj);
            }
        }

        // Rare
        float[] rPhases = { 1.0f, 2.5f, 4.0f, 5.5f };
        Vector3[] rPositions = {
            new Vector3(-10, 5.5f, -4), new Vector3(6, 5.5f, -6), new Vector3(-18, 5.5f, -10), new Vector3(-12, 5.5f, -8)
        };
        for (int i = 0; i < 4; i++)
        {
            CreateCollectible($"Col_R{i + 1:00}", rPositions[i], 0.6f, GetMat("Mat_Collectible_Rare"), 25, 0.3f, 1.8f, 120, new Color(0, 0.8f, 1f), 2.0f, rPhases[i], rareGrp);
        }

        // Bonus
        float[] bPhases = { 0.5f, 2.0f, 3.5f };
        Vector3[] bPositions = {
            new Vector3(0, 10.5f, -20), new Vector3(12, 10.5f, -18), new Vector3(-12, 10.5f, -18)
        };
        for (int i = 0; i < 3; i++)
        {
            CreateCollectible($"Col_B{i + 1:00}", bPositions[i], 0.75f, GetMat("Mat_Collectible_Bonus"), 50, 0.4f, 2.0f, 180, new Color(1f, 0.2f, 0.9f), 3.0f, bPhases[i], bonusGrp);
        }

        // Spawners
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Collectible_Common_Prefab.prefab");
        CreateSpawner("Spawner_A", new Vector3(0, 1.5f, 12), prefab, 8, 3, new Vector3(4, 0, 4), spawnersGrp);
        CreateSpawner("Spawner_B", new Vector3(-8, 5, -4), prefab, 10, 2, new Vector3(2, 0, 2), spawnersGrp);
    }

    private static void CreateSpawner(string name, Vector3 pos, GameObject prefab, float interval, int max, Vector3 offset, GameObject parent)
    {
        GameObject sp = new GameObject(name);
        sp.transform.position = pos;
        sp.transform.SetParent(parent.transform);
        ObjectSpawner os = sp.AddComponent<ObjectSpawner>();
        SerializedObject so = new SerializedObject(os);
        so.FindProperty("m_prefabToSpawn").objectReferenceValue = prefab;
        so.FindProperty("m_spawnInterval").floatValue = interval;
        so.FindProperty("m_maxActive").intValue = max;
        so.FindProperty("m_randomOffset").vector3Value = offset;
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateCollectible(string name, Vector3 pos, float scale, Material mat, int points, float bobH, float bobS, float rotS, Color? pulseA, float pulseSpeed, float phase, GameObject parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        go.GetComponent<Collider>().isTrigger = true;
        go.transform.SetParent(parent.transform);

        Collectible c = go.AddComponent<Collectible>();
        SerializedObject soC = new SerializedObject(c);
        soC.FindProperty("m_pointValue").intValue = points;
        string pName = points == 50 ? "FX_Collect_Bonus" : (points == 25 ? "FX_Collect_Rare" : "FX_Collect_Burst");
        GameObject pFx = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{pName}.prefab");
        if (pFx != null) soC.FindProperty("m_collectEffectPrefab").objectReferenceValue = pFx;
        soC.ApplyModifiedProperties();

        ObjectBobber ob = go.AddComponent<ObjectBobber>();
        SerializedObject soOb = new SerializedObject(ob);
        soOb.FindProperty("m_bobHeight").floatValue = bobH;
        soOb.FindProperty("m_bobSpeed").floatValue = bobS;
        soOb.FindProperty("m_phaseOffset").floatValue = phase;
        soOb.ApplyModifiedProperties();

        ObjectRotator or = go.AddComponent<ObjectRotator>();
        SerializedObject soOr = new SerializedObject(or);
        soOr.FindProperty("m_rotationSpeed").vector3Value = new Vector3(0, rotS, 0); // Assuming Y rotation based on previous instructions slightly generalized
        soOr.ApplyModifiedProperties();

        if (pulseA.HasValue)
        {
            ColorPulse cp = go.AddComponent<ColorPulse>();
            SerializedObject soCp = new SerializedObject(cp);
            soCp.FindProperty("m_colorA").colorValue = pulseA.Value;
            soCp.FindProperty("m_colorB").colorValue = Color.white;
            if (points == 50) soCp.FindProperty("m_colorB").colorValue = new Color(1, 1, 0.5f);
            soCp.FindProperty("m_pulseSpeed").floatValue = pulseSpeed;
            soCp.ApplyModifiedProperties();
        }

        return go;
    }

    private static void BuildManagersAndUI()
    {
        GameObject scoreMgr = new GameObject("ScoreManager");
        ScoreManager sm = scoreMgr.AddComponent<ScoreManager>();

        GameObject gameMgr = new GameObject("GameManager");
        CountdownTimer timer = gameMgr.AddComponent<CountdownTimer>();
        SerializedObject soTimer = new SerializedObject(timer);
        soTimer.FindProperty("m_startTime").intValue = 90;
        soTimer.ApplyModifiedProperties();

        WinCondition win = gameMgr.AddComponent<WinCondition>();
        SerializedObject soWin = new SerializedObject(win);
        soWin.FindProperty("m_timerToStop").objectReferenceValue = timer;
        soWin.ApplyModifiedProperties();

        SceneLoader loader = gameMgr.AddComponent<SceneLoader>();

        // UI
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject hud = new GameObject("HUD_Panel");
        hud.transform.SetParent(canvasGo.transform, false);
        Image hudImg = hud.AddComponent<Image>();
        hudImg.color = new Color(0, 0, 0, 0.45f);
        RectTransform rtHud = hud.GetComponent<RectTransform>();
        rtHud.anchorMin = new Vector2(0, 1);
        rtHud.anchorMax = new Vector2(1, 1);
        rtHud.pivot = new Vector2(0.5f, 1);
        rtHud.sizeDelta = new Vector2(0, 80);
        rtHud.anchoredPosition = Vector2.zero;

        // ScoreText
        GameObject scoreTxtGo = new GameObject("ScoreText");
        scoreTxtGo.transform.SetParent(hud.transform, false);
        TextMeshProUGUI scoreText = scoreTxtGo.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 38;
        scoreText.color = new Color(1.0f, 0.9f, 0.2f, 1.0f);
        RectTransform rtScore = scoreTxtGo.GetComponent<RectTransform>();
        rtScore.anchorMin = new Vector2(0, 1);
        rtScore.anchorMax = new Vector2(0, 1);
        rtScore.pivot = new Vector2(0, 1);
        rtScore.sizeDelta = new Vector2(320, 55);
        rtScore.anchoredPosition = new Vector2(140, -40);
        ScoreDisplay sd = scoreTxtGo.AddComponent<ScoreDisplay>();
        SerializedObject soSd = new SerializedObject(sd);
        soSd.FindProperty("m_prefix").stringValue = "Score: ";
        soSd.ApplyModifiedProperties();

        // TimerText
        GameObject timerTxtGo = new GameObject("TimerText");
        timerTxtGo.transform.SetParent(hud.transform, false);
        TextMeshProUGUI timerText = timerTxtGo.AddComponent<TextMeshProUGUI>();
        timerText.text = "Time: 90";
        timerText.fontSize = 42;
        timerText.color = Color.white;
        RectTransform rtTimer = timerTxtGo.GetComponent<RectTransform>();
        rtTimer.anchorMin = new Vector2(0.5f, 1);
        rtTimer.anchorMax = new Vector2(0.5f, 1);
        rtTimer.pivot = new Vector2(0.5f, 1);
        rtTimer.sizeDelta = new Vector2(220, 55);
        rtTimer.anchoredPosition = new Vector2(0, -40);
        TimerDisplay td = timerTxtGo.AddComponent<TimerDisplay>();
        SerializedObject soTd = new SerializedObject(td);
        soTd.FindProperty("m_prefix").stringValue = "Time: ";
        soTd.FindProperty("m_warningThreshold").intValue = 15;
        soTd.FindProperty("m_normalColor").colorValue = Color.white;
        soTd.FindProperty("m_warningColor").colorValue = new Color(1, 0.1f, 0.1f);
        soTd.ApplyModifiedProperties();

        // TipText
        GameObject tipTxtGo = new GameObject("TipText");
        tipTxtGo.transform.SetParent(canvasGo.transform, false);
        TextMeshProUGUI tipText = tipTxtGo.AddComponent<TextMeshProUGUI>();
        tipText.text = "WASD Move   Space Jump   Shift Slide   Collect all orbs to win";
        tipText.fontSize = 24;
        tipText.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        tipText.alignment = TextAlignmentOptions.Center;
        RectTransform rtTip = tipTxtGo.GetComponent<RectTransform>();
        rtTip.anchorMin = new Vector2(0.5f, 0);
        rtTip.anchorMax = new Vector2(0.5f, 0);
        rtTip.pivot = new Vector2(0.5f, 0);
        rtTip.sizeDelta = new Vector2(800, 40);
        rtTip.anchoredPosition = new Vector2(0, 30);

        // GameOverPanel
        GameObject gop = CreatePanel("GameOverPanel", canvasGo, new Color(0, 0, 0, 0.8f));
        CreateText("GameOverTitle", gop, "TIME'S UP", 80, new Color(1f, 0.2f, 0.2f), new Vector2(700, 120), new Vector2(0, 100));
        CreateText("FinalScoreLabel", gop, "Score: 0", 44, new Color(1f, 0.85f, 0f), new Vector2(400, 70), new Vector2(0, 0));
        Button goRestart = CreateButton("RestartButton", gop, "PLAY AGAIN", new Color(0.2f, 0.2f, 0.5f), new Vector2(300, 70), new Vector2(0, -100));
        Button goMenu = CreateButton("MenuButton", gop, "MAIN MENU", new Color(0.15f, 0.15f, 0.15f), new Vector2(300, 70), new Vector2(0, -190));
        gop.SetActive(false);

        // WinPanel
        GameObject wp = CreatePanel("WinPanel", canvasGo, new Color(0, 0, 0, 0.8f));
        CreateText("WinTitle", wp, "YOU WIN!", 80, new Color(0f, 1f, 0.5f), new Vector2(700, 120), new Vector2(0, 120));
        CreateText("WinSubtitle", wp, "All orbs collected!", 40, Color.white, new Vector2(600, 70), new Vector2(0, 30));
        CreateText("WinScore", wp, "Score: 0", 48, new Color(1f, 0.85f, 0f), new Vector2(500, 70), new Vector2(0, -70));
        Button wRestart = CreateButton("WinRestartButton", wp, "PLAY AGAIN", Color.black, new Vector2(300, 70), new Vector2(0, -170));
        Button wMenu = CreateButton("WinMenuButton", wp, "MAIN MENU", Color.black, new Vector2(300, 70), new Vector2(0, -260));
        wp.SetActive(false);

        // Wiring Events
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(sm.ScoreChanged, sd.UpdateScoreText); // Will fix method name later
        UnityEditor.Events.UnityEventTools.AddPersistentListener(timer.TimerExpired, delegate { gop.SetActive(true); });
        UnityEditor.Events.UnityEventTools.AddPersistentListener<int>(timer.TimerTicked, td.UpdateTimerDisplay);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(win.AllCollected, delegate { wp.SetActive(true); });

        UnityEditor.Events.UnityEventTools.AddPersistentListener(goRestart.onClick, loader.ReloadCurrentScene);
        UnityEditor.Events.UnityEventTools.AddIntPersistentListener(goMenu.onClick, loader.LoadSceneByIndex, 0);

        UnityEditor.Events.UnityEventTools.AddPersistentListener(wRestart.onClick, loader.ReloadCurrentScene);
        UnityEditor.Events.UnityEventTools.AddIntPersistentListener(wMenu.onClick, loader.LoadSceneByIndex, 0);
    }

    private static GameObject CreatePanel(string name, GameObject parent, Color color)
    {
        GameObject p = new GameObject(name);
        p.transform.SetParent(parent.transform, false);
        Image img = p.AddComponent<Image>();
        img.color = color;
        RectTransform rt = p.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        return p;
    }

    private static TextMeshProUGUI CreateText(string name, GameObject parent, string text, int size, Color color, Vector2 sizeDelta, Vector2 pos)
    {
        GameObject t = new GameObject(name);
        t.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = t.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = text.Contains("TIME") || text.Contains("WIN") ? FontStyles.Bold : FontStyles.Normal;
        RectTransform rt = t.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = pos;
        return tmp;
    }

    private static Button CreateButton(string name, GameObject parent, string text, Color btnColor, Vector2 sizeDelta, Vector2 pos)
    {
        GameObject b = new GameObject(name);
        b.transform.SetParent(parent.transform, false);
        Image img = b.AddComponent<Image>();
        img.color = btnColor;
        Button btn = b.AddComponent<Button>();
        RectTransform rt = b.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = pos;

        CreateText(name + "_Text", b, text, 36, Color.white, sizeDelta, Vector2.zero).fontStyle = FontStyles.Bold;
        return btn;
    }

    private static void BuildAudio()
    {
        GameObject az = new GameObject("AudioZones");

        CreateAudioZone("AudioZone_Tier2", new Vector3(-10, 3, -2), new Vector3(12, 6, 4), 0.9f, true, az);
        CreateAudioZone("AudioZone_Tier3", new Vector3(0, 8, -16), new Vector3(16, 6, 4), 1.0f, true, az);
        CreateAudioZone("AudioZone_SlowZone", new Vector3(4, 3, -4), new Vector3(4, 4, 4), 0.6f, false, az);
    }

    private static void CreateAudioZone(string name, Vector3 pos, Vector3 bounds, float volume, bool playOnce, GameObject parent)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        go.transform.SetParent(parent.transform);
        BoxCollider bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = bounds;

        AudioOnTrigger aot = go.AddComponent<AudioOnTrigger>();
        SerializedObject so = new SerializedObject(aot);
        so.FindProperty("m_volume").floatValue = volume;
        so.FindProperty("m_isSinglePlay").boolValue = playOnce;
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateParticlePrefab(string name, Color color)
    {
        string path = $"Assets/Prefabs/{name}.prefab";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject go = new GameObject(name);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = 4f;
        main.startSize = 0.15f;
        main.startColor = color;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        ps.GetComponent<ParticleSystemRenderer>().sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-ParticleSystem.mat") ?? new Material(Shader.Find("Hidden/InternalErrorShader"));

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }
}
