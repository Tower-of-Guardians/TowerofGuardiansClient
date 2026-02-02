using UnityEngine;
using System;

public class TemporaryCardAnimeRequest
{
    public BattleCardData[] CardDatas;

    public Vector3[] StartPositions;
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    public float Interval = 0.05f;

    public TemporaryCardSettings Settings = new();

    public Func<int, TemporaryCardSettings, TemporaryCardSettings> PerCardOverride;

    public Vector3 GetStartPosition(int index)
        => (StartPositions != null && index < StartPositions.Length) ? StartPositions[index] : StartPosition;

    public TemporaryCardSettings GetSettings(int index)
        => PerCardOverride != null ? PerCardOverride(index, Settings) : Settings;    
}
