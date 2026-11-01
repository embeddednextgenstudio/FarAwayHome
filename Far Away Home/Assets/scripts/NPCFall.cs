
using UnityEngine;

public class NPCFall : MonoBehaviour
{
    public Animator animator;
    public AnimationClip fallAnimation;
    public float detectionDistance = 2f;

    private Transform player;
    private bool hasFallen = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("Player tag not found!");
    }

    void Update()
    {
        if (player == null || hasFallen) return;

        float distance = Vector3.Distance(
            player.position, transform.position);

        if (distance <= detectionDistance)
        {
            hasFallen = true;

            animator.Play(fallAnimation.name);
        }
    }
}
