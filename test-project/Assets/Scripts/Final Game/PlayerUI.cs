using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text nameText;
    public Image lifeBar;
    public Image healBar;
    public Image shieldBar;
    public Image bulletsBar;
    public Text lifeText;

    public void UpdateName(string name)
    {
        nameText.text = name;
    }

    public void UpdateLifeUI(float currLife, float maxLife)
    {
        lifeText.text = currLife.ToString();
        lifeBar.fillAmount = currLife / maxLife;
    }

    public void UpdateBulletsUI(int currBullets, float rateTimer)
    {
        bulletsBar.fillAmount = (currBullets + rateTimer) / 3;
    }

    public void UpdateShieldUI(float cooldownTimer, float cooldown)
    {
        shieldBar.fillAmount = 1 - cooldownTimer / cooldown;
    }

    public void UpdateHealUI(float cooldownTimer, float cooldown)
    {
        healBar.fillAmount = 1 - cooldownTimer / cooldown;
    }
}
