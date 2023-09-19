using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    public static CameraManager Instance;

    private Camera _mainCam;
    public Camera MainCam
    {
        get
        {
            if (_mainCam == null)
                _mainCam = Camera.main;
            return _mainCam;
        }
    }

}
