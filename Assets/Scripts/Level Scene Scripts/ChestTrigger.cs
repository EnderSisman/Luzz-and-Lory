using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    [Header("Chest")]
    [SerializeField] private Animator chestAnimator;

    [Header("Glitzie")]
    [SerializeField] private GlitzieMovement glitzieMovement;

    private bool opened;

    private void Start()
    {
        if (chestAnimator)
            chestAnimator.enabled = false;

        if (glitzieMovement)
            glitzieMovement.enabled = false;
    }

    public void HitByLuzz()
    {
        if (opened)
            return;

        opened = true;

        if (chestAnimator)
        {
            chestAnimator.enabled = true;
            chestAnimator.Play("Open", 0, 0f);
        }

        if (glitzieMovement)
            glitzieMovement.enabled = true;
    }
}