using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPUIManager : MonoBehaviour {
  [Header("XP Bar")]
  [SerializeField]
  private Slider xpBar;
  [SerializeField]
  private TextMeshProUGUI levelText;
  [SerializeField]
  private TextMeshProUGUI xpProgressText;


  [Header("Floating XP Text")]
  [SerializeField]
  private GameObject floatingXpTextPrefab;
  [SerializeField]
  private float floatingTextDuration = 1f;
  [SerializeField]
  private float floatingTextMoveSpeed = 1f;


  private PlayerStats _playerStats;
  private Canvas _mainCanvas;

  private void Awake() {
    _mainCanvas = GetComponentInParent<Canvas>();
    if (!_mainCanvas) {
      Debug.LogError("XPUIManager must be a child of a Canvas!");
    }

    if (!floatingXpTextPrefab) return;
    var tempText = floatingXpTextPrefab.GetComponent<TextMeshProUGUI>();

    if (tempText && !tempText.font) tempText.font = TMP_Settings.defaultFontAsset;

    if (levelText && !levelText.font) levelText.font = TMP_Settings.defaultFontAsset;
    
    if (xpProgressText && !xpProgressText.font) xpProgressText.font = TMP_Settings.defaultFontAsset;
  }

  private void Start() {
    _playerStats = FindFirstObjectByType<PlayerStats>();
    if (_playerStats) {
      _playerStats.onExperienceChange.AddListener(UpdateXpBar);
      _playerStats.onLevelUp.AddListener(UpdateLevelText);
      UpdateXpBar(_playerStats.CurrentXp);
      UpdateLevelText(_playerStats.CurrentLevel);
    }
  }

  private void UpdateXpBar(float currentXp) {
    if (xpBar) xpBar.value = currentXp / _playerStats.XpToNextLevel;
    if (xpProgressText) xpProgressText.text =  $"{Mathf.Floor(currentXp)} / {Mathf.Floor(_playerStats.XpToNextLevel)}";
  }

  private void UpdateLevelText(int level) {
    if (levelText) {
      levelText.text = $"{level}";
    }
  }

  public void ShowFloatingXpText(Vector3 worldPosition, float xpAmount) {
    if (floatingXpTextPrefab && _mainCanvas) {
      if (!Camera.main) return;
      Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
      GameObject floatingText = Instantiate(floatingXpTextPrefab, _mainCanvas.transform);

      RectTransform rectTransform = floatingText.GetComponent<RectTransform>();
      rectTransform.position = screenPosition;

      TextMeshProUGUI textComponent = floatingText.GetComponent<TextMeshProUGUI>();
      if (textComponent && textComponent.font) {
        textComponent.text = $"+{xpAmount} XP";
        StartCoroutine(AnimateFloatingText(floatingText));
      }
    }
  }

  private System.Collections.IEnumerator AnimateFloatingText(GameObject textObj) {
    TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
    RectTransform rectTransform = textObj.GetComponent<RectTransform>();
    float elapsedTime = 0f;
    Color startColor = text.color;
    Vector2 startPosition = rectTransform.position;

    while (elapsedTime < floatingTextDuration) {
      elapsedTime += Time.deltaTime;

      // Move upward in screen space
      rectTransform.position = startPosition + Vector2.up * (floatingTextMoveSpeed * elapsedTime * 100f);

      // Fade out
      float alpha = 1f - (elapsedTime / floatingTextDuration);
      text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

      yield return null;
    }

    Destroy(textObj);
  }
}