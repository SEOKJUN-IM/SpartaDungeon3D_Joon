using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInformation : MonoBehaviour
{
    public GameObject informationWindow;

    private PlayerController controller;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;

        controller.information += Toggle;

        informationWindow.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen()) informationWindow.SetActive(false);
        else informationWindow.SetActive(true);
    }

    public bool IsOpen()
    {
        return informationWindow.activeInHierarchy;
    }
}
