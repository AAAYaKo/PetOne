using UnityEngine;

[RequireComponent(typeof(Rigidbody)), ExecuteInEditMode]
public class PlayerTag : MonoBehaviour
{
    public Transform PlayerTransform;
    public Rigidbody PlayerRigidbody;
    public Transform ViewTransform;
    public Animator ViewAnimator;
    public Transform CameraTransform;

#if UNITY_EDITOR
    private void Awake()
    {
        PlayerTransform = GetComponent<Transform>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        ViewAnimator = GetComponentInChildren<Animator>();
        ViewTransform = ViewAnimator.GetComponent<Transform>();
        CameraTransform = Camera.main.GetComponent<Transform>();
    }
#endif
}
