using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Portrait DataBase", menuName = "SO/DB/Portrait DataBase")]
public class PortraitDataBase : ScriptableObject
{
    [Header("데이터 목록")]
    [SerializeField] private List<PortraitData> m_data_list;
    private Dictionary<CharacterCode, Sprite> m_data_dict;

#if UNITY_EDITOR
    private void OnEnable()
        => Initialize();
#endif

    private void Initialize()
    {
        if(m_data_list == null || m_data_list.Count == 0)
            return;

        m_data_dict = new();

        foreach(var data in m_data_list)
            m_data_dict.TryAdd(data.Code, data.Sprite);
    }

    public Sprite GetPortrait(CharacterCode code)
    {
        if(m_data_dict == null)
            Initialize();

        return m_data_dict.TryGetValue(code, out var portrait) ? portrait 
                                                               : null;
    } 
}
