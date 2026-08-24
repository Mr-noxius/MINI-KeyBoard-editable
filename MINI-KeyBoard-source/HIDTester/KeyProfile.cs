using System;

namespace HIDTester;

/// <summary>
/// Represents a single selectable profile in the Profile Switcher popup.
/// A profile can map either to a firmware layer (1-3, matching LayerFun.cs)
/// or to an application-specific mapping that the app applies purely in software
/// (re-labelling / re-binding the on-screen keys) since the firmware itself only
/// exposes 3 physical layers.
/// </summary>
public sealed class KeyProfile
{
	public enum ProfileKind
	{
		FirmwareLayer,
		AppProfile
	}

	public string Name { get; }

	public string Description { get; }

	public ProfileKind Kind { get; }

	/// <summary>1-3 when Kind == FirmwareLayer, otherwise unused.</summary>
	public byte LayerNumber { get; }

	/// <summary>Accent dot color shown in the switcher list, ARGB.</summary>
	public System.Drawing.Color Indicator { get; }

	public KeyProfile(string name, string description, ProfileKind kind, byte layerNumber, System.Drawing.Color indicator)
	{
		Name = name;
		Description = description;
		Kind = kind;
		LayerNumber = layerNumber;
		Indicator = indicator;
	}

	public override string ToString()
	{
		return Name;
	}
}
