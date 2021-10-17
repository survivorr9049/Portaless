using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour {
	[SerializeField] private TMPro.TMP_Dropdown _qualityDropdown;
	[SerializeField] private TMPro.TMP_Dropdown _resolutionDropdown;
	[SerializeField] private Toggle _windowedToggle; 
	[SerializeField] private Toggle _vSyncToggle;
	[SerializeField] private Toggle _fpsShowToggle;

	private void Start() {
		InitQualityDropdown(_qualityDropdown);
		InitResolutionDropdown(_resolutionDropdown);
		InitWindowedToggle(_windowedToggle);
		InitVSyncToggle(_vSyncToggle);
		InitFPSShowToggle(_fpsShowToggle);
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
			Screen.SetResolution(r.width, r.height, !_windowedToggle.isOn, r.refreshRate);
			Debug.Log($"Changed resolution to {ResolutionToString(r)}");
		});
	}

	private void InitWindowedToggle(Toggle toggle) {
		toggle.isOn = !Screen.fullScreen;

		// Handle value change
		toggle.onValueChanged.AddListener(delegate {
			Screen.fullScreenMode = toggle.isOn ?
				FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
		});
	}

	private void InitVSyncToggle(Toggle toggle) {
		_vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

		// Handle value change
		toggle.onValueChanged.AddListener(delegate {
			QualitySettings.vSyncCount = toggle.isOn ? 1 : 0;
		});
	}

	private void InitFPSShowToggle(Toggle toggle) {
		// This is awful way to do this but hey it works!
		GameObject GetFPSCounterObject() =>
			FindObjectOfType<FpsUI>().gameObject.transform.GetChild(0).gameObject;

		_fpsShowToggle.isOn = GetFPSCounterObject().activeSelf;

		// Handle value change
		toggle.onValueChanged.AddListener(delegate {
			GetFPSCounterObject().SetActive(_fpsShowToggle.isOn);
		});
	}
}
