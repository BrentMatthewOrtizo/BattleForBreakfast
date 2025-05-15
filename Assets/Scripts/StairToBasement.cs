using System;
using UnityEngine;

public class StairToBasement : MonoBehaviour, IInteractable
{

    public GameObject player;
    
    private Vector2 teleportLocation = new Vector2(0.78f, -7.46f);

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        player.transform.position = teleportLocation;
    }

    public bool CanInteract()
    {
        return true;
    }
}
