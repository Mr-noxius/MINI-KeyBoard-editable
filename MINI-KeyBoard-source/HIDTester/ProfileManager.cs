using System;
using System.Collections.Generic;
using System.Drawing;

namespace HIDTester;

/// <summary>
/// Central registry of profiles the user can switch between via the Profile Switcher
/// popup, plus the currently active profile. Kept as a static singleton so both
/// FormMain and the Hid callback (running on the async read thread) can read/set it.
/// </summary>
internal static class ProfileManager
{
	public static event EventHandler<KeyProfile> ActiveProfileChanged;

	private static readonly List<KeyProfile> profiles = new List<KeyProfile>
	{
		new KeyProfile("Layer 1 — Default", "Standaard toetsen (fabrieksinstelling).", KeyProfile.ProfileKind.FirmwareLayer, 1, Color.FromArgb(67, 208, 173)),
		new KeyProfile("Layer 2 — Gaming", "Alternatieve toewijzingen voor games.", KeyProfile.ProfileKind.FirmwareLayer, 2, Color.FromArgb(255, 138, 76)),
		new KeyProfile("Layer 3 — Media", "Media- en volumebediening.", KeyProfile.ProfileKind.FirmwareLayer, 3, Color.FromArgb(94, 156, 255)),
		new KeyProfile("App Profile — Photoshop", "Software-only snelkoppelingen voor Photoshop.", KeyProfile.ProfileKind.AppProfile, 0, Color.FromArgb(214, 92, 214))
	};

	public static IReadOnlyList<KeyProfile> Profiles => profiles;

	public static KeyProfile ActiveProfile { get; private set; } = profiles[0];

	public static void SetActiveProfile(KeyProfile profile)
	{
		if (profile == null || profile == ActiveProfile)
		{
			return;
		}
		ActiveProfile = profile;
		ActiveProfileChanged?.Invoke(null, profile);
	}

	public static void AddAppProfile(string name, string description)
	{
		profiles.Add(new KeyProfile(name, description, KeyProfile.ProfileKind.AppProfile, 0, Color.FromArgb(214, 92, 214)));
	}
}
