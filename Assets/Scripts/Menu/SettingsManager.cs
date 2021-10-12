using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsManager : MonoBehaviour {
	[SerializeField] private TMPro.TMP_Dropdown qualityDropdown;

	private void Start() {
		InitQualityDropdown(qualityDropdown);
	}

	#region Quality Settings
	private void InitQualityDropdown(TMPro.TMP_Dropdown dropdown) {
		// Get list of quality names
		List<string> qualityLevels = new List<string>(QualitySettings.names)
			// Add "Quality" to every string
			.Select(s => s += " Quality").ToList();

		// Add options to the list
		dropdown.AddOptions(new List<string>(qualityLevels));
		dropdown.value = QualitySettings.GetQualityLevel();

		// Handle value change
		dropdown.onValueChanged.AddListener(delegate {
			SetQualityLevel(dropdown.value);
		});
	}

	public void SetQualityLevel(int level) {
		QualitySettings.SetQualityLevel(level);
		Debug.Log($"Changed quality settings to {QualitySettings.names[level]}");
	}
	#endregion
}
