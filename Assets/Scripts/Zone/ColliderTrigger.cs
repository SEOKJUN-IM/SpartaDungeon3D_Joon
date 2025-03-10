using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    private Raft raft;
    private Rigidbody _rigidbody;
    private BoxCollider boxCollider;

    void Awake()
    {
        raft = GetComponentInParent<Raft>();
        _rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            raft.preMoveSpeed = raft.raftMoveSpeed;
            raft.raftMoveSpeed = 0f;            
            Invoke("Turn", raft.raftWaitTime);
        }
    }

    void Turn()
    {
        raft.raftMoveSpeed = -raft.preMoveSpeed;      
    }
}
