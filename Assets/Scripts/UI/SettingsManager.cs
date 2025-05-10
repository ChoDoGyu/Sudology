using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Settings 버튼 클릭 시 호출
    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
