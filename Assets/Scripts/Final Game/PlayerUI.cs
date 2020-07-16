using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text nameText;
    public Image lifeBar;
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

    public void UpdateBulletsUI(int currBullets, int maxBullets, float rateTimer)
    {
        bulletsBar.fillAmount = (currBullets + rateTimer) / maxBullets;
    }
}
