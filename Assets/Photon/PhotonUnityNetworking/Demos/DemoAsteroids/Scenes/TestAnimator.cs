using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Parameters to control animations
    public bool isWalking;
    public bool isRunning;
    public bool isJumping;

    private void Start()
    {
    }

    private void Update()
    {
        // Set animator parameters based on the current state
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isJumping", isJumping);
        }

        // Example logic to avoid conflicting states
        if (isJumping)
        {
            isWalking = false;
            isRunning = false;
        }
        else if (isRunning)
        {
            isWalking = false;
        }
    }
}
