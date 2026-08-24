using System;
using HidLibrary;

namespace HIDTester;

/// <summary>
/// Watches raw HID input reports for a "hold a key, then rotate" gesture and raises
/// PopupRequested so FormMain can show the ProfileSwitcherDialog. This keeps the
/// gesture-detection logic isolated from FormMain and from the legacy Hid.cs path.
///
/// Report layout assumption (matches the byte positions already read in
/// FormMain.myhid_DataReceived / myHidLib_ReportReceived): byte[0] = held key id
/// (0 when nothing held), byte[1] = rotary delta as a signed value (+1 clockwise,
/// -1 counter-clockwise, 0 when not turning). This mirrors how KEY_MOUSE_WHEEL_ADD /
/// SUB already encode wheel direction in MouseKey.cs, so no firmware protocol
/// changes are required beyond exposing the same delta byte while a key is held.
/// </summary>
internal static class RotaryTriggerWatcher
{
	public static event Action<byte> PopupRequested;
	public static event Action<int> RotationStep;
	public static event Action PopupDismissRequested;

	private static byte heldKeyId;
	private static bool popupActive;

	public static void Attach(HidLib hidLib)
	{
		hidLib.ReportReceived += OnReportReceived;
	}

	public static void NotifyPopupOpened()
	{
		popupActive = true;
	}

	public static void NotifyPopupClosed()
	{
		popupActive = false;
		heldKeyId = 0;
	}

	private static void OnReportReceived(HidReport report)
	{
		if (report?.Data == null || report.Data.Length < 2)
		{
			return;
		}
		byte keyId = report.Data[0];
		sbyte delta = unchecked((sbyte)report.Data[1]);

		if (!popupActive)
		{
			if (keyId != 0 && delta != 0)
			{
				heldKeyId = keyId;
				PopupRequested?.Invoke(keyId);
			}
			return;
		}

		if (keyId == 0)
		{
			PopupDismissRequested?.Invoke();
			return;
		}
		if (delta != 0)
		{
			RotationStep?.Invoke(Math.Sign((int)delta));
		}
	}
}
