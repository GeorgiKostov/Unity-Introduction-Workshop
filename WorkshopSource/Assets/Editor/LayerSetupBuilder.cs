using UnityEditor;
using UnityEngine;
using Workshop.Session2.Movement;
using UnityEditor.SceneManagement;

public class LayerSetupBuilder
{
    [MenuItem("Workshop/Apply Physics Layers")]
    public static void ApplyLayers()
    {
        bool changed = false;

        // 1. Assign Player layer
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            if (playerLayer != -1 && player.layer != playerLayer)
            {
                SetLayerRecursively(player, playerLayer);
                changed = true;
            }

            // Update PlayerJumper LayerMask
            PlayerJumper jumper = player.GetComponent<PlayerJumper>();
            if (jumper != null)
            {
                SerializedObject so = new SerializedObject(jumper);
                int groundLayerIndex = LayerMask.NameToLayer("Ground");
                int obstacleLayerIndex = LayerMask.NameToLayer("Obstacle");
                
                if (groundLayerIndex != -1 && obstacleLayerIndex != -1)
                {
                    // Create mask for both Ground and Obstacle layers
                    int newMask = (1 << groundLayerIndex) | (1 << obstacleLayerIndex);
                    SerializedProperty layerMaskProp = so.FindProperty("m_groundLayer");
                    if (layerMaskProp != null && layerMaskProp.intValue != newMask)
                    {
                        layerMaskProp.intValue = newMask;
                        so.ApplyModifiedProperties();
                        changed = true;
                    }
                }
            }
        }

        // 2. Assign Ground layer to Arena
        GameObject arena = GameObject.Find("Arena");
        if (arena != null)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer != -1)
            {
                SetLayerRecursively(arena, groundLayer);
                changed = true;
            }
        }

        // 3. Assign Obstacle layer to Hazards
        GameObject hazards = GameObject.Find("Hazards");
        if (hazards != null)
        {
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            if (obstacleLayer != -1)
            {
                SetLayerRecursively(hazards, obstacleLayer);
                changed = true;
            }
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("Physics layers successfully applied to Player, Arena, Hazards, and PlayerJumper component.");
        }
        else
        {
            Debug.Log("Physics layers already applied.");
        }
    }

    private static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
       
        obj.layer = newLayer;
       
        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
