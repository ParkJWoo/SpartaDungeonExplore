using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpPower;
    public float coolDown;
    public Vector3 jumpDirection = Vector3.up;
    private bool isOnCoolDown = false;

    public bool useForceModeImpulse = true;

    private void OnCollisionEnter(Collision other)
    {
        if(isOnCoolDown)
        {
            return;
        }

        if(other.gameObject.CompareTag("Player"))
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();

            if(rigidbody != null)
            {
                Vector3 velocity = rigidbody.velocity;
                velocity.y = 0f;
                rigidbody.velocity = velocity;

                rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

                StartCoroutine(JumpCoolDown());
            }
        }
    }

    private System.Collections.IEnumerator JumpCoolDown()
    {
        isOnCoolDown = true;
        yield return new WaitForSeconds(coolDown);
        isOnCoolDown = false;
    }
}
