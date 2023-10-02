using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSaveSystemTest : MonoBehaviour
{
    [SerializeField]
    private Quest _quest;
    [SerializeField]
    private Category _category;
    [SerializeField]
    private TaskTarget _target;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

       if(questSystem.ActiveQuests.Count == 0)
        {
            Debug.Log("Register");
            var newQuest = questSystem.Register(_quest);
        }
        else
        {
            questSystem.onQuestCompleted += (quest) =>
            {
                Debug.Log("Complete");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            };
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            QuestSystem.Instance.ReceiveReport(_category, _target, 1);
    }
}
