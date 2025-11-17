using UnityEngine;

public static class CardLayoutCalculator
{
    public static CardLayoutData CalculatedTransform(int index, 
                                                     int card_count,
                                                     float target_radius,
                                                     float arc_angle,
                                                     float depth_multiplier)
    {
        var step_count  =   card_count > 1 ? card_count - 1 : 1f;
        var start_angle =   card_count > 1 ? -arc_angle / 2f : 0f;
        var angle_step  =   card_count > 1 ? arc_angle / step_count : 0f;
        var radius      =   card_count * target_radius;  

        var angle = start_angle + index * angle_step;
        var rad = angle * Mathf.Deg2Rad;      

        var target_position = new Vector3(Mathf.Sin(rad) * radius,
                                            Mathf.Cos(rad) * radius - radius,
                                            -Mathf.Abs(angle) * depth_multiplier);
        var target_rotation = new Vector3(0f, 0f, -angle);
        var target_scale = Vector3.one;

        return new CardLayoutData(target_position,
                                  target_rotation,
                                  target_scale);
    }
}
