using System;

[System.Serializable]
public readonly struct ActionData
{
    private readonly int m_current_action_count;
    public int Current => m_current_action_count;

    private readonly int m_max_action_count;
    public int Max => m_max_action_count;

    public ActionData(int current_action_count,
                      int max_action_count)
    {
        m_current_action_count = current_action_count;
        m_max_action_count = max_action_count;
    }
}