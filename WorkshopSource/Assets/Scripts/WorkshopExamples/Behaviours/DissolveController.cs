using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolveController : MonoBehaviour
{
    [Header("Shader Settings")]
    [SerializeField] private string dissolvePropertyName = "_DissolveAmount";
    [SerializeField] private float speed = 1f;

    private Material _material;
    private static readonly int DissolveProperty = Shader.PropertyToID("_DissolveAmount");
    private int _propertyID;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _propertyID = Shader.PropertyToID(dissolvePropertyName);
    }

    void Update()
    {
        float dissolve = Mathf.PingPong(Time.time * speed, 1f);
        _material.SetFloat(_propertyID, dissolve);
    }

    void OnDestroy()
    {
        // Clean up the material instance created by accessing .material
        if (_material != null)
            Destroy(_material);
    }
}
