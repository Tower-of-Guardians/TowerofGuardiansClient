using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reinforcement DataBase", menuName = "SO/DB/Reinforcement DataBase")]
public class ReinforcementDataBase : ScriptableObject, IReinforcementDataBase
{
    [Header("강화 데이터 목록")]
    [SerializeField] private List<ReinforcementData> m_data_list;

    private Dictionary<int, ReinforcementData> m_data_dict;

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
            m_data_dict[data.Stage] = data;
    }

    public ReinforcementData GetReinforcementData(int stage)
    {
        if(m_data_dict == null)
            Initialize();

        return m_data_dict.TryGetValue(stage, out var data) ? data : null;
    }
}
