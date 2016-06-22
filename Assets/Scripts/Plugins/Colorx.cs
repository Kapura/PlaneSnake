using UnityEngine;

public class Colorx {
    public static Color Slerp(Color a, Color b, float t) {
        return (HSBColor.Lerp(HSBColor.FromColor(a), HSBColor.FromColor(b), t)).ToColor();
    }
}
