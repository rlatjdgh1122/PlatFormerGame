using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quest/Acievement", fileName = "Achievement_")]
public class Achievement : Quest
{
    public override bool IsCanelable => false;
    public override bool IsSavable => true;
    public override void Cancel()
    {   
        base.Cancel();
        Debug.LogAssertion("Achievement can`t be canceled");
    }
}
