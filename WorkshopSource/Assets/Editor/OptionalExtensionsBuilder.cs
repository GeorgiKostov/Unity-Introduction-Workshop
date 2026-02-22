using UnityEngine;
using UnityEditor;
using Workshop.Session2.Collectibles;
using Workshop.Session2.Camera;

public class OptionalExtensionsBuilder
{
    [MenuItem("Workshop/Build Optional Extensions")]
    public static void Build()
    {
        // Extension A - Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollower follower = mainCam.GetComponent<CameraFollower>();
            if (follower != null) Object.DestroyImmediate(follower);

            CameraOrbiter orbiter = mainCam.GetComponent<CameraOrbiter>();
            if (orbiter == null) orbiter = mainCam.gameObject.AddComponent<CameraOrbiter>();
            
            SerializedObject so = new SerializedObject(orbiter);
            so.FindProperty("m_target").objectReferenceValue = GameObject.Find("Player").transform;
            so.FindProperty("m_distance").floatValue = 8f;
            so.FindProperty("m_orbitSpeed").floatValue = 3f;
            so.FindProperty("m_verticalClamp").vector2Value = new Vector2(-20, 80);
            so.FindProperty("m_zoomSpeed").floatValue = 2f;
            so.FindProperty("m_zoomClamp").vector2Value = new Vector2(2, 15);
            so.ApplyModifiedProperties();
        }

        // Extension B - Score variation
        GameObject col10 = GameObject.Find("Collectible_10");
        if (col10 != null) SetPointValue(col10, 50);

        GameObject col8 = GameObject.Find("Collectible_08");
        if (col8 != null) SetPointValue(col8, 25);

        GameObject col9 = GameObject.Find("Collectible_09");
        if (col9 != null) SetPointValue(col9, 25);

        // Extension C - Second Scoring Ring
        GameObject colGroup = GameObject.Find("Collectibles");
        if (colGroup != null)
        {
            GameObject existingRing2 = GameObject.Find("Collectibles_Ring2");
            if (existingRing2 != null) Object.DestroyImmediate(existingRing2);

            GameObject newRing = Object.Instantiate(colGroup, colGroup.transform.parent);
            newRing.name = "Collectibles_Ring2";
            newRing.transform.position += new Vector3(0, 2, 0);

            // Create cyan material
            Material cyanMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Collectible_Cyan.mat");
            if (cyanMat == null)
            {
                cyanMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                cyanMat.SetColor("_BaseColor", new Color(0.0f, 0.8f, 1.0f));
                cyanMat.SetFloat("_Metallic", 0.8f);
                cyanMat.SetFloat("_Smoothness", 0.9f);
                AssetDatabase.CreateAsset(cyanMat, "Assets/Materials/Mat_Collectible_Cyan.mat");
            }

            foreach (Transform child in newRing.transform)
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = cyanMat;

                SetPointValue(child.gameObject, 20);
            }
        }
        
        Debug.Log("Optional Extensions Built!");
    }

    private static void SetPointValue(GameObject go, int value)
    {
        Collectible col = go.GetComponent<Collectible>();
        if (col != null)
        {
            SerializedObject so = new SerializedObject(col);
            so.FindProperty("m_pointValue").intValue = value;
            so.ApplyModifiedProperties();
        }
    }
}
