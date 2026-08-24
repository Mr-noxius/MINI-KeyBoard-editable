using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIDTester;

/// <summary>
/// Lets the user assign an LED mode value that is supported by the keyboard firmware.
/// Values 0, 1 and 2 are the modes discovered in the original application.
/// </summary>
public sealed class CustomLedModeDialog : Form
{
	private readonly NumericUpDown modeValue = new NumericUpDown();
	private readonly TextBox displayName = new TextBox();

	public byte ModeValue => (byte)modeValue.Value;

	public string DisplayName => string.IsNullOrWhiteSpace(displayName.Text)
		? "Custom LED " + ModeValue
		: displayName.Text.Trim();

	public CustomLedModeDialog()
	{
		Text = "Custom LED mode";
		StartPosition = FormStartPosition.CenterParent;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		ClientSize = new Size(400, 190);

		Label nameLabel = new Label { Text = "Name", AutoSize = true, Location = new Point(18, 20) };
		displayName.SetBounds(18, 42, 364, 24);
		Label valueLabel = new Label { Text = "Firmware LED mode value (0–255)", AutoSize = true, Location = new Point(18, 78) };
		modeValue.SetBounds(18, 100, 110, 24);
		modeValue.Minimum = 0;
		modeValue.Maximum = 255;

		Label warning = new Label
		{
			Text = "Known values: 0, 1 and 2. Other values work only when your keyboard firmware supports them.",
			AutoSize = false,
			Size = new Size(364, 32),
			Location = new Point(18, 128)
		};
		Button cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(226, 160), Size = new Size(75, 25) };
		Button apply = new Button { Text = "Use mode", DialogResult = DialogResult.OK, Location = new Point(307, 160), Size = new Size(75, 25) };
		AcceptButton = apply;
		CancelButton = cancel;
		Controls.AddRange(new Control[] { nameLabel, displayName, valueLabel, modeValue, warning, cancel, apply });
	}
}
