using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour {
	[SerializeField] private TMPro.TMP_Dropdown qualityDropdown;
	[SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
	[SerializeField] private bool windowed;

	void Start() {
		InitQualityDropdown(qualityDropdown);
		InitResolutionDropdown(resolutionDropdown);
		windowed = Screen.fullScreen;
	}

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
			int level = dropdown.value;
			QualitySettings.SetQualityLevel(level);
			Debug.Log($"Changed quality settings to {QualitySettings.names[level]}");
		});
	}

	private void InitResolutionDropdown(TMPro.TMP_Dropdown dropdown) {
		Dictionary<int, string> resolutions = new Dictionary<int, string>();

		// Create string from Resolution struct
		string ResolutionToString(Resolution res) =>
			$"{res.width}x{res.height} {res.refreshRate} Hz";

		// Add available resolutions and their indexes
		foreach (var r in Screen.resolutions.Select((val, i) => (val, i)))
			resolutions[r.i] = ResolutionToString(r.val);

		// Add options to the list
		dropdown.AddOptions(resolutions.Values.ToList());
		// Find current resolution
		dropdown.value = resolutions.FirstOrDefault(
			r => r.Value == ResolutionToString(Screen.currentResolution)
		).Key;

		// Handle value change
		dropdown.onValueChanged.AddListener(delegate {
			Resolution r = Screen.resolutions[dropdown.value];
			Screen.SetResolution(r.width, r.height, windowed, r.refreshRate);
			Debug.Log($"Changed resolution to {ResolutionToString(r)}");
		});
	}
}
