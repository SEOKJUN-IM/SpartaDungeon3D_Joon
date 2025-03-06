using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public float jumpZoneForce;

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager.Instance.Player.controller._rigidbody.AddForce(Vector2.up * jumpZoneForce, ForceMode.Impulse);
    }
}
