using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator _anim;

    public event Action OnStartAnim = null;
    public event Action OnRunningAnim = null;
    public event Action OnEndAnim = null;
    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    #region 애니메이션에 연결하는 함수
    private void OnAnim_Start()
    {
        OnStartAnim?.Invoke();
    }
    private void OnAnim_Running()
    {
        OnRunningAnim?.Invoke();
    }
    private void OnAnim_End()
    {
        OnEndAnim?.Invoke();
    }
    #endregion

}
