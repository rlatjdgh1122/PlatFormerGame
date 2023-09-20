using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager
{
    public static CameraManager Instance;

    private Camera _mainCam;
    private CinemachineVirtualCamera _Vcam;

    public Camera Maincam
    {
        get
        {
            if (_mainCam == null)
                _mainCam = Camera.main;

            return _mainCam;
        }
    }
    public CameraManager(Transform parentTrm, CinemachineVirtualCamera Vcam)
    {
        _Vcam = Vcam;
        _Vcam.m_Follow = parentTrm;
    }
    public void Shake()
    {

    }
}
