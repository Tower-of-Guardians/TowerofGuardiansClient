using System;
using UnityEngine;

public class ThrowCardEventBundle
{
    public Action OnBeginDrag;
    public Action<Vector2> OnDrag;
    public Action OnEndDrag;
}
