using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;

public class uisettings : MonoBehaviour
{
    public Button dronePreset;
    public Slider m_slide;
    public Slider d_slide;
    public Slider ad_slide;
    public Slider t_slide;
    public Slider y_slide;
    public Slider r_slide;
    public Slider p_slide;
    public Slider n_slide;
    public Slider g_slide;

    private void Start()
    {
        dronePreset.onClick.AddListener(DronePreset);
        Debug.Log("DronePreset method assigned to button onClick event.");
    }
    public void DronePreset()
    {
        Debug.Log("called dronepreset method");
        m_slide.value = 200;
        d_slide.value = 02;
        ad_slide.value = 10;
        t_slide.value = 50;
        y_slide.value = 260;
        r_slide.value = 400;
        p_slide.value = 400;
        n_slide.value = 30;
        g_slide.value = 1400;
    }
}
