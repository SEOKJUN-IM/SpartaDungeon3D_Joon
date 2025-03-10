using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetectRay : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;    
    public LayerMask playerLayerMask;

    public GameObject player;
    private Rigidbody playerRigidbody;
    public TextMeshProUGUI warningPromptText;

    void Start()
    {
        RemoveText();                
    }

    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray[] rays = {
                new Ray(transform.position + Vector3.up * 1, transform.right),
                new Ray(transform.position + Vector3.up * 5, transform.right),
                new Ray(transform.position + Vector3.up * 9, transform.right)
            };

            RaycastHit hit;

            for (int i = 0; i < rays.Length; i++)
            {
                if (Physics.Raycast(rays[i], out hit, 30, playerLayerMask))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerRigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
                        if (playerRigidbody.velocity.y > 0)
                        {
                            warningPromptText.gameObject.SetActive(true);
                            Invoke("RemoveText", 3f);
                        }
                        else RemoveText();
                    }                    
                }                
            }            
        }
    }

    void RemoveText()
    {
        warningPromptText.gameObject.SetActive(false);
    }
}
