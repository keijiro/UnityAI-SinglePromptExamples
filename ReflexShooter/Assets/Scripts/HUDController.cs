using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public RectTransform energyFill;
    private PlayerController player;

    void Start()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj) player = pObj.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (player != null && energyFill != null)
        {
            float ratio = player.currentEnergy / player.maxEnergy;
            energyFill.anchorMax = new Vector2(ratio, 1);
        }
    }
}
