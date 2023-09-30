using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> _quests;

    public IReadOnlyList<Quest> Quests => _quests;

    public Quest FindQuesyBy(string codeName) => _quests.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR
    //ContextMenu�� �ؼ� �ν����Ϳ��� ����
    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestsBy<Quest>();
    }
    //ContextMenu�� �ؼ� �ν����Ϳ��� ����
    [ContextMenu("FindAchievements")]
    private void FindAchievement()
    {
        FindQuestsBy<Achievement>();
    }
    private void FindQuestsBy<T>() where T : Quest
    {
        _quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        //FindAssets�� Asset�������� ���Ϳ� �´� GUID�� ������
        //GUID�� ����Ƽ�� Asset�� �����ϱ� ���ؼ� ���������� ����ϴ� ID

        //�迭���ϴ� ������ Achievement�� Quest�� ��ӹް� �ֱ⿡
        //Quest��ü�� Achievement��ü�� ��ã�ƿ��� ���ؼ��� 

        foreach (var guid in guids) //�ٽ� Ȯ��
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if(quest.GetType() == typeof(T))
                _quests.Add(quest);

            EditorUtility.SetDirty(this);
            //SetDirty�� Serialize(_quests�� SerializeField�� �Ǿ�����) ������ ��ȭ�� ��������
            //������ �����Ҷ� this�� �ݿ��϶�� ��
            AssetDatabase.SaveAssets();
            //���� ����(�����ָ� ����Ʈ�� �߰����൵ �����͸� ����Ű�� �ʱ�ȭ��)
        }
        
    }
#endif
}
