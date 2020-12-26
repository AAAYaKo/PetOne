using UnityEngine;

[RequireComponent(typeof(Rigidbody)), ExecuteInEditMode]
public class PlayerTag : MonoBehaviour
{
    public Transform PlayerTransform;
    public Rigidbody PlayerRigidbody;
    public Transform ViewTransform;
    public Transform CameraTransform;

#if UNITY_EDITOR
    private void Awake()
    {
        PlayerTransform = GetComponent<Transform>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        ViewTransform = PlayerTransform.GetChild(0);
        CameraTransform = Camera.main.GetComponent<Transform>();
    }
#endif
}
