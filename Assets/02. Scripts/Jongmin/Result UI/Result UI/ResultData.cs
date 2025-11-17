[System.Serializable]
public class ResultData
{
    private readonly BattleResultType m_result_type;
    public BattleResultType Type => m_result_type;

    private readonly int m_gold;
    public int Gold => m_gold;

    private readonly int m_exp;
    public int EXP => m_exp;

    public ResultData(BattleResultType result_type,
                      int gold,
                      int exp)
    {
        m_result_type = result_type;
        m_gold = gold;
        m_exp = exp;
    }

    public ResultData(ResultData result_data)
    {
        m_result_type = result_data.Type;
        m_gold = result_data.Gold;
        m_exp = result_data.EXP;
    }
}