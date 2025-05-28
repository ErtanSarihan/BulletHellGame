using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player {
  public class PlayerStats : MonoBehaviour {
    [Header("Leveling")]
    [SerializeField]
    private int currentLevel = 1;
    [SerializeField]
    private float currentXp = 0f;
    [SerializeField]
    private float xpToNextLevel = 100f;
    [SerializeField]
    private float xpMultiplier = 1.5f;

    public UnityEvent<int> onLevelUp = new UnityEvent<int>();
    public UnityEvent<float> onExperienceChange = new UnityEvent<float>();

    public int CurrentLevel => currentLevel;
    public float CurrentXp => currentXp;
    public float XpToNextLevel => xpToNextLevel;

    public void AddXp(float amount) {
      currentXp += amount;
      onExperienceChange?.Invoke(currentXp);

      // Check for level up
      while (currentXp >= xpToNextLevel) {
        LevelUp();
      }
    }

    private void LevelUp() {
      currentLevel++;
      currentXp -= xpToNextLevel;
      xpToNextLevel *= xpMultiplier;

      onLevelUp?.Invoke(currentLevel);
      onExperienceChange?.Invoke(currentXp);

      Debug.Log("LevelUp" + currentLevel + "-----" + currentXp + "/" + xpToNextLevel);
    }
  }
}