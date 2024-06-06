using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Quality { Common, Uncommon, Rare, Epic }

public static class QualityColor
{
    private static Dictionary<Quality, string> color = new Dictionary<Quality, string>()
    {
        {Quality.Common, "#ffffffff" },
         {Quality.Uncommon, "#00ff00ff" },
          {Quality.Rare, "#E30EDE" },
           {Quality.Epic, "#C3454B" },

    };

    public static Dictionary<Quality, string> MyColors { get => color; }
}

