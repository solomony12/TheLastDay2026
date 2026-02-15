using UnityEngine;

public class CureSelf : MonoBehaviour
{
    private PlayerInputHandler inputHandler;

    private float cureTimer = 0f;
    private float cureCooldown = 0.5f;

    private void Awake()
    {
        inputHandler = PlayerInputHandler.Instance;
    }
    private void Update()
    {
        if (cureTimer >= 0)
            cureTimer -= Time.deltaTime;

        if (inputHandler.SelfCureTriggered && cureTimer <= 0)
        {
            cureTimer = cureCooldown;
            string selfHealResultText = CureSystem.Instance.CurePlayer();
            if (selfHealResultText != null)
            {
                Captions.Instance.TimedShowCaptions(selfHealResultText, 2f);
            }
        }
    }
}
