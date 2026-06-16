using System;

[Serializable]
public class ResultData
{
    private readonly int _gold;
    public int Gold => _gold;

    private readonly int _exp;
    public int EXP => _exp;

    private readonly bool _isLevelUp;
    public bool IsLevelUp => _isLevelUp;

    public ResultData(int gold, int exp, bool isLevelUp = false)
    {
        _gold = gold;
        _exp = exp;
        _isLevelUp = isLevelUp;
    }

    public ResultData(ResultData resultData)
    {
        _gold = resultData.Gold;
        _exp = resultData.EXP;
        _isLevelUp = resultData.IsLevelUp;
    }
}