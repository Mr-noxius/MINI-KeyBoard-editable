using System.Windows.Forms;
using HidLibrary;

namespace HIDTester;

/// <summary>
/// Wires the rotary "hold + turn" gesture to the Profile Switcher popup for a given
/// FormMain instance, and applies the chosen profile back onto the running app.
/// Kept out of FormMain.cs itself to avoid touching that large generated-code file;
/// call HookProfileSwitcher(this) once from FormMain's constructor (after
/// InitializeComponent) or from Program.cs right after `new FormMain()`.
/// </summary>
internal static class ProfileSwitcherIntegration
{
	private static ProfileSwitcherDialog activeDialog;

	public static void HookProfileSwitcher(FormMain form, HidLib hidLib)
	{
		RotaryTriggerWatcher.Attach(hidLib);
		RotaryTriggerWatcher.PopupRequested += keyId => ShowPopup(form, keyId);
		RotaryTriggerWatcher.RotationStep += direction => activeDialog?.Invoke((System.Action)(() => activeDialog.PreviewStep(direction)));
		RotaryTriggerWatcher.PopupDismissRequested += () => activeDialog?.Invoke((System.Action)(() => activeDialog.Close()));

		ProfileManager.ActiveProfileChanged += (sender, profile) => form.Invoke((System.Action)(() => ApplyProfile(form, profile)));
	}

	private static void ShowPopup(FormMain form, byte keyId)
	{
		form.Invoke((System.Action)delegate
		{
			if (activeDialog != null)
			{
				return;
			}
			string keyName = "KEY" + keyId;
			activeDialog = new ProfileSwitcherDialog(keyName);
			RotaryTriggerWatcher.NotifyPopupOpened();
			DialogResult result = activeDialog.ShowDialog(form);
			RotaryTriggerWatcher.NotifyPopupClosed();
			if (result == DialogResult.OK && activeDialog.SelectedProfile != null)
			{
				ProfileManager.SetActiveProfile(activeDialog.SelectedProfile);
			}
			activeDialog.Dispose();
			activeDialog = null;
		});
	}

	private static void ApplyProfile(FormMain form, KeyProfile profile)
	{
		if (profile.Kind == KeyProfile.ProfileKind.FirmwareLayer)
		{
			FormMain.KeyParam.KEY_Cur_Layer = profile.LayerNumber;
			FormMain.KeyParam.PageBet_Inte_Cmd = 1;
		}
		// AppProfile entries are intentionally software-only for now: the firmware
		// exposes 3 physical layers, so an "app profile" re-labels/re-binds the
		// current on-screen layer instead of switching firmware state. Wiring an
		// app-profile key-map into the existing KEY1..KEY16 buttons is the natural
		// next step once a concrete per-app mapping table is defined.
	}
}
