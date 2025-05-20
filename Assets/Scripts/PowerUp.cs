using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, SlowDown, StrengthBoost }
    public PowerUpType type;
    public float effectDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {

    }
}
