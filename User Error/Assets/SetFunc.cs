using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public GameObject controlsPanel;
    public Image[] controlImages;

    Resolution[] resolutions;

    void Start()
    {
        if (controlsPanel == null)
        {
            Transform child = transform.Find("ControlsPanel");
            if (child != null)
            {
                controlsPanel = child.gameObject;
                Debug.Log("Found ControlsPanel child: " + controlsPanel.name);
            }
        }
            
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings(int currentResolutionIndex)
    { 
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;
    }

    public void OpenControlsPanel()
    {
        if (controlsPanel == null)
        {
            Transform child = transform.Find("ControlsPanel");
            if (child != null)
                controlsPanel = child.gameObject;
        }
        
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            Debug.Log("ControlsPanel opened!");
        }
        else
        {
            Debug.LogError("ControlsPanel not found! Add a child object named 'ControlsPanel' to SettingMenu.");
        }
    }

    public void CloseControlsPanel()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void ToggleControlsPanel()
    {
        if (controlsPanel == null)
        {
            Transform child = transform.Find("ControlsPanel");
            if (child != null)
                controlsPanel = child.gameObject;
        }
        
        if (controlsPanel != null)
            controlsPanel.SetActive(!controlsPanel.activeSelf);
    }
}
