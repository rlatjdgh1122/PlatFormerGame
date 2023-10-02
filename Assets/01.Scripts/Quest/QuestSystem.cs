using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    #region Save Path
    private const string KSaveRootPath = "questsystem";
    private const string KActiveQuestsSavePath = "activeQuests";
    private const string KCompletedQuestsSavePath = "completedQuests";
    private const string KActiveAchievementsSavePath = "activeAchievements";
    private const string KCompletedAchievementsSavePath = "completedAchievements";
    #endregion
    #region Events
    public delegate void QuestRegisterHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion
    private static QuestSystem instance;
    private static bool isApplicationQuitting = false;

    public static QuestSystem Instance
    {
        get
        {
            if (isApplicationQuitting == false && instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if (instance == null)
                {
                    instance = new GameObject("QuestSystem").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private List<Quest> activeQuests = new();
    private List<Quest> completedQuests = new();

    private List<Quest> activeAchievements = new();
    private List<Quest> completedAchievements = new();

    private QuestDatabase questDatabase;
    private QuestDatabase achievementDatabase;

    public event QuestRegisterHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    public event QuestRegisterHandler onAchievementRegistered;
    public event QuestCompletedHandler onAchievementCompleted;
    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;
    public IReadOnlyList<Quest> ActiveAchievements => activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => completedAchievements;

    private void Awake()
    {
        questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase");

        if (Load() == false)
        {
            foreach (var achievement in achievementDatabase.Quests)
                Register(achievement);
        }
    }
    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        Save();
    }
    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();

        if (newQuest is Achievement)
        {
            newQuest.onCompleted += OnAchievementCompleted;

            activeAchievements.Add(newQuest);

            newQuest.OnRegister();
            onAchievementRegistered?.Invoke(newQuest);
        }
        else
        {
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            activeQuests.Add(newQuest);

            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }
        return newQuest;
    }

    #region ReceiveReport
    //외부
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(activeQuests, category, target, successCount);
        ReceiveReport(activeAchievements, category, target, successCount);
    }

    //내부
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach (var quest in quests.ToArray())
            quest.ReceiveReport(category, target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Value, successCount);

    #endregion

    public bool ContainsInActiveQuests(Quest quest)
       => activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedQuests(Quest quest)
        => completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievements(Quest quest)
        => activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedAchievements(Quest quest)
        => completedAchievements.Any(x => x.CodeName == quest.CodeName);

    private void Save()
    {
        var root = new JObject();

        root.Add(KActiveQuestsSavePath, CreateSaveDatas(activeQuests));
        root.Add(KCompletedQuestsSavePath, CreateSaveDatas(completedQuests));
        root.Add(KActiveAchievementsSavePath, CreateSaveDatas(activeAchievements));
        root.Add(KCompletedAchievementsSavePath, CreateSaveDatas(completedAchievements));

        PlayerPrefs.SetString(KSaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    private bool Load()
    {
        if (PlayerPrefs.HasKey(KSaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(KSaveRootPath));

            LoadSaveDatas(root[KActiveQuestsSavePath], questDatabase, LoadActiveQuest);
            LoadSaveDatas(root[KCompletedQuestsSavePath], questDatabase, LoadCompletedQuest);

            LoadSaveDatas(root[KActiveAchievementsSavePath], achievementDatabase, LoadActiveQuest);
            LoadSaveDatas(root[KCompletedAchievementsSavePath], achievementDatabase, LoadCompletedQuest);

            return true;
        }
        else
            return false;
    }

    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            //quest 데이터들을 JArray에 넣어줌
            if (quest.IsSavable)
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));
        }
        return saveDatas;
    }

    //JToken : 위 함수 CreateSaveDatas에서 만들어진 saveDatas가 저장되었다가 Load시 이 클래스로 들어옴
    private void LoadSaveDatas(JToken datasToken, QuestDatabase database, Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuesyBy(saveData.codeName);
            onSuccess.Invoke(saveData, quest);
        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if (newQuest is Achievement)
            completedAchievements.Add(newQuest);
        else
            completedQuests.Add(newQuest);
    }
    #region Callback
    private void OnQuestCompleted(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    private void OnAchievementCompleted(Quest achievement)
    {
        activeAchievements.Remove(achievement);
        completedAchievements.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }
    #endregion
}
