using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider)), AddComponentMenu("CustomGravity/NGravitySourceTag"), ExecuteInEditMode]
public class NGravitySourceConfig : MonoBehaviour
{
    public float GravityFactor = 9.81f;
    public int Id;
    public float3 Position;

#if UNITY_EDITOR
    public int Layer = 7;

    private void Awake()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        gameObject.layer = 7;
        Id = GetComponent<Collider>().GetInstanceID();
        Position = transform.position;
    }
#endif
}
