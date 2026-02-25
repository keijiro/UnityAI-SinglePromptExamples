using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private PlayerController player;
    private VisualElement energyBar;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        energyBar = root.Q<VisualElement>("EnergyBar");
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (player != null && energyBar != null)
        {
            float normalizedEnergy = player.GetEnergyNormalized();
            energyBar.style.width = Length.Percent(normalizedEnergy * 100f);
            
            // Change color if barrier is active
            if (player.IsBarrierActive())
                energyBar.style.backgroundColor = new Color(1, 1, 0); // Yellow when active
            else
                energyBar.style.backgroundColor = new Color(0, 0.75f, 1); // Blue when idle
        }
    }
}
