using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //15Ка
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
    private bool useAutoComplete;
    private int currentTaskGroupIndex;

    public Category Category => _category;
    public Sprite Icon => _icon;
    public string CodeName => _codeName;
    public string Description => _description;
    public string DisplayName => _displayName;

    public QuestState State { get; private set; }
    public TaskGroup CurrentTaskGroup => _taskGroups[currentTaskGroupIndex];
    public IReadOnlyList<TaskGroup> TaskGroups => _taskGroups;

    public bool IsRegistered => State != QuestState.InActive;

    public bool IsComplatable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;


}
