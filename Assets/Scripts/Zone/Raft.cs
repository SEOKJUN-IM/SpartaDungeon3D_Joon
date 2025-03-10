using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour
{
    public float raftMoveSpeed;
    public float preMoveSpeed;
    public float raftWaitTime;
    public Vector3 moveVector;

    public Rigidbody raftRigidbody;   

    void Awake()
    {
        raftRigidbody = GetComponent<Rigidbody>();               
    }

    void FixedUpdate()
    {
        Invoke("Move", raftWaitTime);
    }

    void Move()
    {
        moveVector = -transform.forward * raftMoveSpeed * Time.deltaTime;
        raftRigidbody.MovePosition(transform.position + moveVector);
    }

    public Vector3 GetMoveVector()
    {
        return moveVector;
    }
}
