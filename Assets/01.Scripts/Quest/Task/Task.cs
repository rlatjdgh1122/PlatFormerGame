using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState
{
    InActive,
    Running,
    Complete,
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category category;
    [Header("Text")]
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private string _description;

    [Header("Action")]
    [SerializeField]
    private TaskAction _action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] _targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue _initialSuccessValue;
    [SerializeField]
    private int _needSuccessToCompelete;

    private int _currentSuccess;
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToCompelete => _needSuccessToCompelete;
    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int preSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, NeedSuccessToCompelete);

            if (_currentSuccess != preSuccess)
            {
                State = _currentSuccess == NeedSuccessToCompelete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, _currentSuccess, preSuccess);
            }
        }
    }

    [SerializeField]
    private bool canReceiveReportsDuringCompletion;
    //TASK가 완료되었어도 계속 성공 횟 수를 보고할 것이냐

    private TaskState state;

    public StateChangedHandler onStateChanged;
    public SuccessChangedHandler onSuccessChanged;
    public Category Category => category;
    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }

    public bool IsComplete => State == TaskState.Complete;
    public Quest Owner { get; private set; }

    public void SetUp(Quest owner)
    {
        Owner = owner;
    }
    public void Start()
    {
        State = TaskState.Running;

        if (_initialSuccessValue)
            CurrentSuccess = _initialSuccessValue.GetValue(this);
    }
    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;

    }
    public void ReceieveReport(int successCount) //액션에 보고
    {
        CurrentSuccess = _action.Run(this, CurrentSuccess, successCount);
    }
    public void Complete() //즉시 성공
    {
        CurrentSuccess = NeedSuccessToCompelete;
    }
    public bool IsTarget(string category, object target) //타겟인가
        =>
        _targets.Any(x => x.IsEqual(target))
        && Category == category
        && (IsComplete == false || (IsComplete && canReceiveReportsDuringCompletion));
}
