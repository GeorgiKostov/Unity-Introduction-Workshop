using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{
    public class ScriptToInspectorExample : MonoBehaviour
    {
        // Serializable basic types
        public int intValue = 42;
        public float floatValue = 3.14f;
        public string stringValue = "Hello, Unity!";
        public bool boolValue = true;

        // Arrays and Lists are also serializable
        public int[] intArray = { 1, 2, 3, 4, 5 };
        public List<string> stringList = new List<string>() { "Apple", "Banana", "Cherry" };

        // Unity-specific types
        public Vector3 vectorValue = new Vector3(1.0f, 2.0f, 3.0f);
        public Color colorValue = Color.red;
        public GameObject gameObjectReference;

        // Custom Serializable Class
        [System.Serializable]
        public class CustomData
        {
            public int customInt;
            public string customString;
        }

        public CustomData customData;

        // Inspector-related expressions
        [Header("Inspector Header")]
        public float headerValue;

        [Tooltip("This is a tooltip for the inspector.")]
        public string tooltipValue = "Hover over me!";

        // Example of using space in the inspector
        [Space(20)]
        public bool separateSection;

        // Example of a read-only field in the inspector
        [Header("Read-Only Field")]
        [SerializeField]
        private int readOnlyField = 100;

        // Example of a Range slider in the inspector
        [Header("Range Slider")]
        [Range(0, 1)]
        public float rangeValue = 0.5f;

        // Example of using a LayerMask in the inspector
        [Header("Layer Mask")]
        public LayerMask layerMaskValue;
    }
}