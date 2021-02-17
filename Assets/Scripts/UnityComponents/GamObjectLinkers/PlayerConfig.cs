using UnityEngine;

[RequireComponent(typeof(Rigidbody)), ExecuteInEditMode]
public class PlayerConfig : MonoBehaviour
{
    public Transform SelfTransform;
    public Rigidbody SelfRigidbody;
    public Transform ViewTransform;
    public Animator ViewAnimator;


#if UNITY_EDITOR
    private void Awake()
    {
        SelfTransform = GetComponent<Transform>();
        SelfRigidbody = GetComponent<Rigidbody>();
        ViewAnimator = GetComponentInChildren<Animator>();
        ViewTransform = ViewAnimator.GetComponent<Transform>();
    }
#endif
}
