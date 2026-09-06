using UnityEngine;

public class Glitzie : MonoBehaviour
{
    public enum GlitzieType
    {
        Gold,
        Ruby,
        Saphire,
        Emerald,
        Diamond
    }

    [SerializeField] private GlitzieType type;

    public GlitzieType Type => type;

    private bool isCaptured;

    public int Points
    {
        get
        {
            switch (type)
            {
                case GlitzieType.Gold:
                    return 100;

                case GlitzieType.Ruby:
                    return 200;

                case GlitzieType.Saphire:
                    return 300;

                case GlitzieType.Emerald:
                    return 500;

                case GlitzieType.Diamond:
                    return 1000;

                default:
                    return 0;
            }
        }
    }

    public void Capture(Transform catchPoint)
    {
        if (isCaptured)
            return;

        isCaptured = true;

        // Bewegung sofort stoppen
        GlitzieMovement[] movementScripts =
            GetComponentsInChildren<GlitzieMovement>(true);

        foreach (GlitzieMovement movement in movementScripts)
        {
            movement.enabled = false;
        }

        // Alle Collider deaktivieren
        Collider[] colliders =
            GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Falls ein Rigidbody vorhanden ist
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // An Luzz hängen
        transform.SetParent(catchPoint);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Deliver()
    {
        if (GlitzieManager.Instance)
        {
            GlitzieManager.Instance.AddGlitzie(
                type,
                Points
            );
        }

        Destroy(gameObject);
    }
}