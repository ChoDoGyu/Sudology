using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Settings ��ư Ŭ�� �� ȣ��
    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
