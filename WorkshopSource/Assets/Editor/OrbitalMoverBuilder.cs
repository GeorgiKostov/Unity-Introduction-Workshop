using UnityEditor;
using UnityEngine;
using Workshop.Session2.Camera;
using Workshop.Session3.Movement;
using Workshop.Session2.Movement;
using UnityEditor.SceneManagement;

public class OrbitalMoverBuilder
{
    [MenuItem("Workshop/Apply Orbital Mover")]
    public static void Build()
    {
        bool changed = false;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerMover oldMover = player.GetComponent<PlayerMover>();
            if (oldMover != null)
            {
                oldMover.enabled = false;
                changed = true;
            }

            PlayerOrbitalMover newMover = player.GetComponent<PlayerOrbitalMover>();
            if (newMover == null)
            {
                player.AddComponent<PlayerOrbitalMover>();
                changed = true;
            }
        }

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollower follower = mainCam.GetComponent<CameraFollower>();
            if (follower != null)
            {
                Object.DestroyImmediate(follower);
                changed = true;
            }

            CameraOrbiter orbiter = mainCam.GetComponent<CameraOrbiter>();
            if (orbiter == null)
            {
                orbiter = mainCam.gameObject.AddComponent<CameraOrbiter>();
                changed = true;
            }

            SerializedObject so = new SerializedObject(orbiter);
            if (player != null) so.FindProperty("m_target").objectReferenceValue = player.transform;
            so.FindProperty("m_distance").floatValue = 10f;
            so.FindProperty("m_orbitSpeed").floatValue = 3f;
            so.FindProperty("m_verticalClamp").vector2Value = new Vector2(-20, 80);
            so.FindProperty("m_zoomSpeed").floatValue = 2f;
            so.FindProperty("m_zoomClamp").vector2Value = new Vector2(2, 15);
            so.ApplyModifiedProperties();
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("Orbital Mover components applied to Player and Camera.");
        }
        else
        {
            Debug.Log("Orbital Mover already applied.");
        }
    }
}
