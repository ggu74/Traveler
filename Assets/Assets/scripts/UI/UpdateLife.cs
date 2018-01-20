using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateLife : MonoBehaviour
{
    public float HealthTransitionSpeed;
    private Damageable damageable;
    private Image healthImage;
    // Use this for initialization

    void Start()
    {
        damageable = GetComponentInParent<Damageable>();
        healthImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float healthAmount = damageable.Life / damageable.MaxLife;
        if (healthImage.fillAmount != healthAmount)
        {
            healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, healthAmount, HealthTransitionSpeed * Time.deltaTime * (healthImage.fillAmount- healthAmount));
        }
    }
}
