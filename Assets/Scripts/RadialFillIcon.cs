using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialFillIcon : MonoBehaviour {

    public Color iconColor = Color.white;

    private Image icon;
    private Image overlay;

    public bool available;

    public int numerator {
        get;
        private set;
    }

    public int denominator {
        get;
        private set;
    }

    // Use this for initialization
    void Awake() {
        icon = GetComponent<Image>();
        icon.color = iconColor;
        overlay = transform.GetChild(0).GetComponent<Image>();
        overlay.gameObject.SetActive(false);
        available = true;
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetFraction(int numerator, int denominator) {
        this.numerator = numerator;
        this.denominator = denominator;
        if (numerator == denominator) {
            overlay.gameObject.SetActive(false);
            available = true;
            icon.color = iconColor;
        } else {
            if (!overlay.gameObject.activeSelf) {
                overlay.gameObject.SetActive(true);
                available = false;
                HSBColor hsb = new HSBColor(iconColor);
                hsb.s *= .5f;
                hsb.b *= .8f;
                icon.color = hsb.ToColor();
            }
            overlay.fillAmount = 1f - (float)numerator / (float)denominator;
        }
    }
}
