using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum QuestState
{
    InActive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion,
}


[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest_")]
public class Quest : ScriptableObject
{
    #region events
    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currnetSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);

    //15분
    #endregion


    [SerializeField]
    private Category _category;
    [SerializeField]
    private Sprite _icon;

    [Header("Text")]
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private string _displayName;
    [SerializeField, TextArea]
    private string _description;

    [Header("Task")]
    [SerializeField]
    private TaskGroup[] _taskGroups;

    [Header("Option")]
    [SerializeField]
    private bool _useAutoComplete;
    [SerializeField]
    private bool _isCancelable;
    [SerializeField]
    private bool _isSavable;

    [Header("Condition")]
    [SerializeField]
    private Condition[] _acceptionConditions;
    [SerializeField]
    private Condition[] _cancelConditions;

    [Header("Reward")]
    [SerializeField]
    private Reward[] _rewards;

    private int _currentTaskGroupIndex;

    public Category Category => _category;
    public Sprite Icon => _icon;
    public string CodeName => _codeName;
    public string Description => _description;
    public string DisplayName => _displayName;

    public QuestState State { get; private set; }
    public TaskGroup CurrentTaskGroup => _taskGroups[_currentTaskGroupIndex];
    public IReadOnlyList<TaskGroup> TaskGroups => _taskGroups;
    public IReadOnlyList<Condition> Conditions => _acceptionConditions;
    public IReadOnlyList<Reward> Rewards => _rewards;

    public bool IsRegistered => State != QuestState.InActive;
    public bool IsComplatable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCanelable => _isCancelable && _cancelConditions.All(x => x.IsPass(this));
    public bool IsAcceptable => _acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => _isSavable;


    public TaskSuccessChangedHandler onTaskSuccessChanged;
    public CompletedHandler onCompleted;
    public CanceledHandler onCanceled;
    public NewTaskGroupHandler onNewTaskGroup;

    public void OnRegister()
    {
        Debug.Assert(IsRegistered == false, "This quest has already been registered.");

        foreach (var taskGroup in _taskGroups)
        {
            taskGroup.SetUp(this);
            foreach (var task in taskGroup.Tasks)
            {
                task.onSuccessChanged += OnSuccessChanged;
            }
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }
    public void ReceiveReport(string category, object target, int successCount)
    {
        Debug.Assert(IsRegistered == true, "This quest has already been registered.");
        Debug.Assert(IsCancel == false, "This quest has been canceled.");

        if (IsComplete) return; //성공해도 확인하는 경우도 있기에 리턴으로 해줌

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (_currentTaskGroupIndex + 1 == _taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;

                if (_useAutoComplete)
                    Complete();
            }
            else
            {
                var prevTaskGroup = _taskGroups[_currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
            State = QuestState.Running; //Task가 완료가 되었어도 계속 보고받는 옵션때문에 설정
    }
    public void Complete()
    {
        CheckIsRunning();

        foreach (var taskGroup in _taskGroups)
            taskGroup.Complete();

        State = QuestState.Complete;

        foreach (var reward in _rewards)
            reward.Give(this);

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewTaskGroup = null;
    }
    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCanelable == true, "This quest can`t be canceled");


        State = QuestState.Cancel;
        onCanceled?.Invoke(this);


    }
    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone._taskGroups = _taskGroups.Select(x => new TaskGroup(x)).ToArray();
        return clone;
    }
    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            codeName = CodeName,
            state = State,
            taskGroupIndex = _currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    }
    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        _currentTaskGroupIndex = saveData.taskGroupIndex;
        for (int i = 0; i < _currentTaskGroupIndex; i++)
        {
            var taskGroup = TaskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }
        for (int i = 0; i < saveData.taskSuccessCounts.Length; i++)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }

    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
            => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(IsRegistered == true, "This quest has already been registered.");
        Debug.Assert(IsCancel == false, "This quest has been canceled.");
        Debug.Assert(IsComplete == false, "This quest has already been Completed.");
    }
}
