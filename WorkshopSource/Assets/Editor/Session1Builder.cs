// ─────────────────────────────────────────────────────────────────────────────
// Session 1 Builder
//
// What this builds
// ────────────────
// A self-contained "teaching scene" that demonstrates every concept from the
// Session 1 instruction sheet:
//
//   • Folder structure   – creates the recommended Assets sub-folders
//   • Primitives         – Cube, Sphere, Capsule placed and labelled
//   • Parent-child       – PARENT_CHILD_DEMO group shows local vs. world transform
//   • Materials          – six named URP/Lit materials with varied colour/metallic/smoothness
//   • PhysicMaterial     – Bouncy (bounciness=0.85) and Icy (friction=0) physics materials
//   • Rigidbody physics  – physics tower, ramp, kinematic floor
//   • Camera             – Perspective, FOV 60, dark background
//   • Directional light  – warm key light with soft shadows
//   • UI                 – "MAIN MENU" button wired to SceneLoader
//
// Usage: Workshop ▸ Build Session 1  (runs in the Editor, no Play needed)
// ─────────────────────────────────────────────────────────────────────────────

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorkshopBehaviours.Session4.Advanced;

public class Session1Builder
{
    // ── Menu entry ────────────────────────────────────────────────────────────
    [MenuItem("Workshop/Build Session 1")]
    public static void Build()
    {
        // 1. Create a fresh empty scene and save it immediately so all asset
        //    references have a valid scene path to attach to.
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        const string scenePath = "Assets/Scenes/Session1.unity";
        bool saved = EditorSceneManager.SaveScene(scene, scenePath);
        if (!saved)
        {
            Debug.LogError($"[Session1Builder] Could not save scene to {scenePath}. " +
                           "Make sure Assets/Scenes/ exists.");
            return;
        }

        // 2. Recommended folder structure from the instruction sheet
        CreateFolderStructure();

        // 3. Assets (materials + physic materials)
        CreateMaterials();
        CreatePhysicMaterials();

        // 4. Scene contents
        BuildLighting();
        BuildCamera();
        BuildArena();              // floor + walls (kinematic, no Rigidbody fall-through)
        BuildPrimitivesShowcase(); // Cube / Sphere / Capsule demo row
        BuildParentChildDemo();    // parent-child relationship example
        BuildMaterialsShowcase();  // row of spheres showing material properties
        BuildPhysicsTower();       // stacked boxes + bouncy sphere + ramp
        BuildUI();                 // "MAIN MENU" screen-space button

        // 5. Final save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[Session1Builder] Session 1 scene built successfully → " + scenePath);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // FOLDER STRUCTURE  (Session 1, Part 5)
    // ═════════════════════════════════════════════════════════════════════════
    private static void CreateFolderStructure()
    {
        // Assets/_Scenes  (underscore sorts it to the top)
        EnsureFolder("Assets", "_Scenes");

        // Assets/Art/Models, Assets/Art/Textures
        EnsureFolder("Assets", "Art");
        EnsureFolder("Assets/Art", "Models");
        EnsureFolder("Assets/Art", "Textures");

        // Assets/Audio, Assets/Scripts, Assets/Prefabs
        EnsureFolder("Assets", "Audio");
        EnsureFolder("Assets", "Scripts");
        EnsureFolder("Assets", "Prefabs");

        // Assets/Materials (shared folder for builder outputs)
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets/Materials", "Session1");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // MATERIALS  (Session 1, Part 2 – Mesh Renderer / Material)
    // ═════════════════════════════════════════════════════════════════════════
    private static void CreateMaterials()
    {
        // Floor / structural
        MakeMat("Mat_Floor",    "#2b2b40", 0.00f, 0.55f);
        MakeMat("Mat_Wall",     "#3c3c58", 0.00f, 0.25f);
        MakeMat("Mat_Ramp",     "#7a5cc4", 0.00f, 0.35f);

        // Physics objects
        MakeMat("Mat_Box",      "#e07050", 0.00f, 0.20f); // heavy box – warm terracotta
        MakeMat("Mat_Sphere",   "#50c8e0", 0.30f, 0.90f); // bouncy sphere – shiny cyan
        MakeMat("Mat_Capsule",  "#78e060", 0.10f, 0.55f); // capsule – matte green

        // Material showcase row (demonstrates the Metallic / Smoothness sliders)
        MakeMat("Mat_Show_Rough",    "#d4a04a", 0.00f, 0.00f); // gold-ish, fully rough
        MakeMat("Mat_Show_MidGloss", "#d4a04a", 0.00f, 0.50f); // gold-ish, half gloss
        MakeMat("Mat_Show_Smooth",   "#d4a04a", 0.00f, 1.00f); // gold-ish, fully smooth
        MakeMat("Mat_Show_Metal",    "#c0c0c8", 1.00f, 0.85f); // metallic silver
        MakeMat("Mat_Show_Plastic",  "#e83264", 0.00f, 0.70f); // plastic pink
        MakeMat("Mat_Show_Matte",    "#32e884", 0.00f, 0.05f); // matte green

        // Parent-child demo
        MakeMat("Mat_Parent", "#f0c040", 0.10f, 0.60f); // parent body – yellow
        MakeMat("Mat_Child",  "#f04060", 0.00f, 0.30f); // child limbs – red

        AssetDatabase.SaveAssets();
    }

    private static Material MakeMat(string name, string hex, float metallic, float smoothness)
    {
        string path = $"Assets/Materials/Session1/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (mat == null)
        {
            Shader urp = Shader.Find("Universal Render Pipeline/Lit");
            mat = new Material(urp != null ? urp : Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }

        Color col;
        ColorUtility.TryParseHtmlString(hex, out col);
        mat.SetColor("_BaseColor", col);
        mat.color = col; // covers Standard fallback
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static Material GetMat(string name) =>
        AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/Session1/{name}.mat");

    // ═════════════════════════════════════════════════════════════════════════
    // PHYSIC MATERIALS  (Session 1, Part 4 – Physic Material)
    // ═════════════════════════════════════════════════════════════════════════
    private static void CreatePhysicMaterials()
    {
        // Bouncy – demonstrates bounciness = 1 extreme
        MakePhysicMat("Bouncy",
            dynamicFriction: 0.1f, staticFriction: 0.1f,
            bounciness: 0.85f,
            frictionCombine: PhysicsMaterialCombine.Minimum,
            bounceCombine: PhysicsMaterialCombine.Maximum);

        // Icy – demonstrates friction = 0 (ice)
        MakePhysicMat("Icy",
            dynamicFriction: 0.0f, staticFriction: 0.0f,
            bounciness: 0.0f,
            frictionCombine: PhysicsMaterialCombine.Minimum,
            bounceCombine: PhysicsMaterialCombine.Minimum);

        AssetDatabase.SaveAssets();
    }

    private static PhysicsMaterial MakePhysicMat(string name,
        float dynamicFriction, float staticFriction, float bounciness,
        PhysicsMaterialCombine frictionCombine, PhysicsMaterialCombine bounceCombine)
    {
        string path = $"Assets/Materials/Session1/{name}.physicMaterial";
        PhysicsMaterial mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
        if (mat == null)
        {
            mat = new PhysicsMaterial(name);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.dynamicFriction = dynamicFriction;
        mat.staticFriction  = staticFriction;
        mat.bounciness      = bounciness;
        mat.frictionCombine = frictionCombine;
        mat.bounceCombine   = bounceCombine;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static PhysicsMaterial GetPhysicMat(string name) =>
        AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(
            $"Assets/Materials/Session1/{name}.physicMaterial");

    // ═════════════════════════════════════════════════════════════════════════
    // LIGHTING
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildLighting()
    {
        GameObject go = new GameObject("Directional Light");
        go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        Light l = go.AddComponent<Light>();
        l.type      = LightType.Directional;
        l.intensity = 1.2f;
        l.shadows   = LightShadows.Soft;
        ColorUtility.TryParseHtmlString("#fff4e0", out Color col);
        l.color = col;

        RenderSettings.ambientMode  = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.06f, 0.06f, 0.14f);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // CAMERA  (Session 1, Part 3)
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildCamera()
    {
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(0f, 8f, -18f);
        go.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

        Camera cam = go.AddComponent<Camera>();
        cam.orthographic = false;          // Perspective (see Inspector → Projection)
        cam.fieldOfView  = 60f;            // FOV – students change this in session
        cam.clearFlags   = CameraClearFlags.SolidColor;
        ColorUtility.TryParseHtmlString("#1a1a2e", out Color bg);
        cam.backgroundColor = bg;

        go.AddComponent<AudioListener>();
    }

    // ═════════════════════════════════════════════════════════════════════════
    // ARENA – floor + three walls
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildArena()
    {
        GameObject arena = new GameObject("ARENA");

        // Floor  (kinematic so it never falls – demonstrates Is Kinematic)
        GameObject floor = MakeCube("Floor", new Vector3(0f, -0.15f, 2f),
                                    new Vector3(22f, 0.3f, 20f), GetMat("Mat_Floor"), arena.transform);
        MakeKinematic(floor);

        // Back wall
        MakeCube("Wall_Back",  new Vector3(0f, 2f,  12f), new Vector3(22f, 4.3f, 0.3f), GetMat("Mat_Wall"), arena.transform);
        // Left wall
        MakeCube("Wall_Left",  new Vector3(-11f, 2f, 2f), new Vector3(0.3f, 4.3f, 20f), GetMat("Mat_Wall"), arena.transform);
        // Right wall
        MakeCube("Wall_Right", new Vector3( 11f, 2f, 2f), new Vector3(0.3f, 4.3f, 20f), GetMat("Mat_Wall"), arena.transform);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // PRIMITIVES SHOWCASE  (Session 1, Part 2 – Creating Objects)
    // Shows Cube / Sphere / Capsule in a labelled row
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildPrimitivesShowcase()
    {
        GameObject grp = new GameObject("PRIMITIVES_SHOWCASE");
        grp.transform.position = new Vector3(-6f, 0f, -4f);

        // Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Cube_Demo";
        cube.transform.SetParent(grp.transform);
        cube.transform.position    = new Vector3(-6f, 0.5f, -4f);
        cube.transform.localScale  = Vector3.one;
        cube.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Box");

        // Sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Sphere_Demo";
        sphere.transform.SetParent(grp.transform);
        sphere.transform.position    = new Vector3(0f, 0.5f, -4f);
        sphere.transform.localScale  = Vector3.one;
        sphere.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Sphere");

        // Capsule
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = "Capsule_Demo";
        capsule.transform.SetParent(grp.transform);
        capsule.transform.position    = new Vector3(6f, 1f, -4f);
        capsule.transform.localScale  = Vector3.one;
        capsule.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Capsule");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // PARENT-CHILD DEMO  (Session 1, Part 2 – Parent-Child Relationships)
    // A "body" cube with three "limb" children so students can see how
    // moving the parent moves all children.
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildParentChildDemo()
    {
        // The parent (body) – note the name prefix so it reads clearly in the
        // Hierarchy during the teaching moment.
        GameObject parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parent.name = "PARENT_Body";
        parent.transform.position   = new Vector3(-6f, 0.5f, 4f);
        parent.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        parent.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Parent");

        // Child: left arm
        AddChild(parent.transform, "Child_ArmLeft",
            localPos: new Vector3(-1.5f, 0f, 0f),
            localScale: new Vector3(0.8f, 0.35f, 0.35f));

        // Child: right arm
        AddChild(parent.transform, "Child_ArmRight",
            localPos: new Vector3( 1.5f, 0f, 0f),
            localScale: new Vector3(0.8f, 0.35f, 0.35f));

        // Child: head (sits on top)
        AddChild(parent.transform, "Child_Head",
            localPos: new Vector3(0f, 1.25f, 0f),
            localScale: new Vector3(0.9f, 0.9f, 0.9f));
    }

    private static void AddChild(Transform parent, string childName,
                                  Vector3 localPos, Vector3 localScale)
    {
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        child.name = childName;
        child.transform.SetParent(parent, worldPositionStays: false);
        child.transform.localPosition = localPos;
        child.transform.localScale    = localScale;
        child.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Child");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // MATERIALS SHOWCASE  (Session 1, Part 2 – Mesh Renderer / Material)
    // Six spheres in a row demonstrating the Metallic and Smoothness sliders.
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildMaterialsShowcase()
    {
        GameObject grp = new GameObject("MATERIALS_SHOWCASE");

        string[] matNames = {
            "Mat_Show_Rough",
            "Mat_Show_MidGloss",
            "Mat_Show_Smooth",
            "Mat_Show_Metal",
            "Mat_Show_Plastic",
            "Mat_Show_Matte",
        };

        for (int i = 0; i < matNames.Length; i++)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = matNames[i].Replace("Mat_Show_", "Sphere_");
            s.transform.SetParent(grp.transform);
            s.transform.position   = new Vector3(-5f + i * 2f, 0.5f, 8f);
            s.transform.localScale = Vector3.one;
            s.GetComponent<MeshRenderer>().sharedMaterial = GetMat(matNames[i]);
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // PHYSICS TOWER  (Session 1, Part 4 – Rigidbody)
    // A stacked tower of boxes (vary mass/drag), a bouncy sphere, an icy
    // ramp, and a kinematic floor-platform to demonstrate colliders.
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildPhysicsTower()
    {
        GameObject grp = new GameObject("PHYSICS_DEMO");

        // ── Stacked tower (press Play and watch it wobble / fall) ─────────
        // Each box has a slightly different mass so the tower is unstable.
        AddDynamicBox(grp.transform, "Box_Heavy",   new Vector3(4f, 0.5f,  2f), mass: 5f,  drag: 0.1f);
        AddDynamicBox(grp.transform, "Box_Medium",  new Vector3(4f, 1.5f,  2f), mass: 2f,  drag: 0.05f);
        AddDynamicBox(grp.transform, "Box_Light",   new Vector3(4f, 2.5f,  2f), mass: 0.5f,drag: 0.02f);
        AddDynamicBox(grp.transform, "Box_Topmost", new Vector3(4f, 3.5f,  2f), mass: 0.2f,drag: 0.0f);

        // ── Bouncy sphere – has Bouncy PhysicMaterial on its collider ─────
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Sphere_Bouncy";
        sphere.transform.SetParent(grp.transform);
        sphere.transform.position   = new Vector3(7f, 6f, 2f);
        sphere.transform.localScale = Vector3.one;
        sphere.GetComponent<MeshRenderer>().sharedMaterial = GetMat("Mat_Sphere");
        SphereCollider sc = sphere.GetComponent<SphereCollider>();
        sc.sharedMaterial = GetPhysicMat("Bouncy");
        Rigidbody rbSphere = sphere.AddComponent<Rigidbody>();
        rbSphere.mass = 1f;
        rbSphere.useGravity = true;
        rbSphere.linearDamping = 0f;

        // ── Ramp (kinematic) so the bouncy sphere slides off it ───────────
        GameObject ramp = MakeCube("Ramp_Icy", new Vector3(5.5f, 0.8f, 5f),
            new Vector3(3f, 0.3f, 4f), GetMat("Mat_Ramp"), grp.transform);
        ramp.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        ramp.GetComponent<BoxCollider>().sharedMaterial = GetPhysicMat("Icy");
        MakeKinematic(ramp);

        // ── One extra kinematic platform (Is Kinematic checkbox demo) ─────
        GameObject platform = MakeCube("Platform_Kinematic",
            new Vector3(-4f, 0.15f, 4f), new Vector3(6f, 0.3f, 4f),
            GetMat("Mat_Floor"), grp.transform);
        MakeKinematic(platform);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // UI  (Session 1 – "Exit to Main Menu" button)
    // ═════════════════════════════════════════════════════════════════════════
    private static void BuildUI()
    {
        // GameManager – hosts the SceneLoader used by the button
        GameObject gm = new GameObject("GameManager");
        SceneLoader loader =
            gm.AddComponent<SceneLoader>();

        // Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        UnityEngine.UI.CanvasScaler scaler =
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode        = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // ── "MAIN MENU" button (top-left) ────────────────────────────────
        GameObject btn = new GameObject("ExitButton");
        btn.transform.SetParent(canvasGo.transform, false);

        UnityEngine.UI.Image img = btn.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.75f, 0.15f, 0.25f);

        UnityEngine.UI.Button button = btn.AddComponent<UnityEngine.UI.Button>();

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(0f, 1f);
        rt.pivot            = new Vector2(0f, 1f);
        rt.sizeDelta        = new Vector2(220f, 60f);
        rt.anchoredPosition = new Vector2(20f, -20f);

        // Button label
        GameObject label = new GameObject("Text");
        label.transform.SetParent(btn.transform, false);
        TMPro.TextMeshProUGUI tmp = label.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text      = "MAIN MENU";
        tmp.fontSize  = 24f;
        tmp.color     = Color.white;
        tmp.fontStyle = TMPro.FontStyles.Bold;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        RectTransform rtLabel = label.GetComponent<RectTransform>();
        rtLabel.anchorMin        = Vector2.zero;
        rtLabel.anchorMax        = Vector2.one;
        rtLabel.sizeDelta        = Vector2.zero;
        rtLabel.anchoredPosition = Vector2.zero;

        // Wire onClick → SceneLoader.LoadSceneByName("MainMenu")
        UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
            button.onClick, loader.LoadSceneByName, "MainMenu");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>Creates a Cube primitive and parents/positions/scales it.</summary>
    private static GameObject MakeCube(string name, Vector3 pos, Vector3 scale,
                                        Material mat, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, worldPositionStays: true);
        go.transform.position   = pos;
        go.transform.localScale = scale;
        if (mat != null)
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        return go;
    }

    /// <summary>Adds a Rigidbody to an existing GameObject (dynamic physics).</summary>
    private static void AddDynamicBox(Transform parent, string name, Vector3 pos,
                                       float mass, float drag)
    {
        GameObject go = MakeCube(name, pos, Vector3.one, GetMat("Mat_Box"), parent);
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.mass          = mass;
        rb.linearDamping = drag;
        rb.useGravity    = true;
        rb.isKinematic   = false;
    }

    /// <summary>
    /// Adds or finds a Rigidbody and sets isKinematic = true.
    /// A kinematic Rigidbody participates in collision but is not moved by physics.
    /// Students set this on the floor/walls so they act as solid ground.
    /// </summary>
    private static void MakeKinematic(GameObject go)
    {
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb == null)
            rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
    }
}
