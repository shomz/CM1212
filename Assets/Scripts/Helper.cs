using UnityEngine;

public class Helper
{
    public static Color GetColor(int id) {

        var colors = new Color[]
        {
            Color.green,
            Color.navyBlue,
            Color.yellow,
            Color.red
        };

        return colors[id];
    }
}
