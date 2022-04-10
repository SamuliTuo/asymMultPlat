using UnityEngine;

public class ArmorColors : MonoBehaviour {

    public static ArmorColors current;

    [ColorUsage(true, true)]
    public Color armorColorRed;
    [ColorUsage(true, true)]
    public Color armorColorBlue;
    [ColorUsage(true, true)]
    public Color armorColorYellow;
    [ColorUsage(true, true)]
    public Color armorColorOrange;
    

    void Awake() {
        current = this;   
    }

    public Color GetArmorColor(DamageTypes type) {
        Color c;
        switch (type) {
            case DamageTypes.BLUE:
                c = armorColorBlue;
                break;
            case DamageTypes.ORANGE:
                c = armorColorOrange;
                break;
            case DamageTypes.YELLOW:
                c = armorColorYellow;
                break;
            case DamageTypes.RED:
                c = armorColorRed;
                break;
            default:
                c = Color.grey;
                break;
        }
        return c;
    }
}
