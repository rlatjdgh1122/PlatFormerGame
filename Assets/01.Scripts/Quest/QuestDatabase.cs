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
    //ContextMenu로 해서 인스펙터에서 적용
    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestsBy<Quest>();
    }
    //ContextMenu로 해서 인스펙터에서 적용
    [ContextMenu("FindAchievements")]
    private void FindAchievement()
    {
        FindQuestsBy<Achievement>();
    }
    private void FindQuestsBy<T>() where T : Quest
    {
        _quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        //FindAssets는 Asset폴더에서 필터에 맞는 GUID를 가져옴
        //GUID는 유니티가 Asset을 관리하기 위해서 내부적으로 사용하는 ID

        //배열로하는 이유는 Achievement가 Quest를 상속받고 있기에
        //Quest객체와 Achievement객체를 다찾아오기 위해서임 

        foreach (var guid in guids) //다시 확인
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if(quest.GetType() == typeof(T))
                _quests.Add(quest);

            EditorUtility.SetDirty(this);
            //SetDirty는 Serialize(_quests가 SerializeField로 되어있음) 변수가 변화가 생겼으니
            //에셋을 저장할때 this를 반영하라는 뜻
            AssetDatabase.SaveAssets();
            //에셋 저장(안해주면 리스트에 추가해줘도 에디터를 껏다키면 초기화댐)
        }
        
    }
#endif
}
