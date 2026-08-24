using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIDTester;

/// <summary>
/// Modern LED editor. It deliberately transmits only the LED mode byte exposed
/// by the original software; colour, speed and brightness are app-side profiles
/// until corresponding firmware commands are documented.
/// </summary>
public sealed class LedStudioControl : UserControl
{
	private readonly Panel preview = new Panel();
	private readonly NumericUpDown modeValue = new NumericUpDown();
	private readonly TrackBar brightness = new TrackBar();
	private readonly TrackBar speed = new TrackBar();
	private readonly Label values = new Label();
	private Color selectedColor = Color.FromArgb(67, 208, 173);

	public LedStudioControl()
	{
		Dock = DockStyle.Fill;
		AutoScroll = true;
		AutoScrollMinSize = new Size(620, 600);
		BuildLayout();
		UpdatePreview();
	}

	private void BuildLayout()
	{
		Label title = new Label { Text = "LED Studio", Font = new Font("Segoe UI Semibold", 20f), AutoSize = true, Location = new Point(32, 26) };
		Label subtitle = new Label { Text = "Maak een profiel, kies een LED-modus en wijs hem toe aan de geselecteerde toets.", AutoSize = true, Location = new Point(35, 65), ForeColor = ModernTheme.MutedText };
		preview.SetBounds(34, 105, 500, 78);
		preview.BorderStyle = BorderStyle.FixedSingle;
		preview.Paint += Preview_Paint;

		FlowLayoutPanel presets = new FlowLayoutPanel { Location = new Point(30, 205), Size = new Size(520, 45), WrapContents = false };
		presets.Controls.Add(CreatePreset("Static", 0));
		presets.Controls.Add(CreatePreset("Breathing", 1));
		presets.Controls.Add(CreatePreset("Cycle", 2));

		Label modeLabel = new Label { Text = "Firmware mode", AutoSize = true, Location = new Point(35, 280), ForeColor = ModernTheme.MutedText };
		modeValue.SetBounds(35, 302, 110, 28);
		modeValue.Minimum = 0;
		modeValue.Maximum = 255;
		modeValue.ValueChanged += delegate { UpdatePreview(); };

		Button colorButton = new Button { Text = "Choose colour", Location = new Point(170, 300), Size = new Size(135, 32) };
		colorButton.Click += ChooseColour_Click;

		Label brightnessLabel = new Label { Text = "Brightness", AutoSize = true, Location = new Point(35, 355), ForeColor = ModernTheme.MutedText };
		brightness.SetBounds(35, 378, 250, 42);
		brightness.Minimum = 0;
		brightness.Maximum = 100;
		brightness.Value = 80;
		brightness.TickFrequency = 10;
		brightness.ValueChanged += delegate { UpdatePreview(); };

		Label speedLabel = new Label { Text = "Animation speed", AutoSize = true, Location = new Point(315, 355), ForeColor = ModernTheme.MutedText };
		speed.SetBounds(315, 378, 220, 42);
		speed.Minimum = 0;
		speed.Maximum = 100;
		speed.Value = 50;
		speed.TickFrequency = 10;
		speed.ValueChanged += delegate { UpdatePreview(); };

		values.SetBounds(35, 430, 500, 25);
		values.ForeColor = ModernTheme.MutedText;
		Button apply = new Button { Text = "Assign LED profile to selected key", Location = new Point(35, 475), Size = new Size(280, 38), BackColor = ModernTheme.Accent, ForeColor = ModernTheme.Background };
		apply.Click += Apply_Click;
		Label note = new Label { Text = "Known device modes are 0–2. Values 3–255 are sent only if your firmware supports them; colour, brightness and speed remain a visual app profile until the protocol is known.", Location = new Point(35, 530), Size = new Size(550, 46), ForeColor = ModernTheme.MutedText };

		Controls.AddRange(new Control[] { title, subtitle, preview, presets, modeLabel, modeValue, colorButton, brightnessLabel, brightness, speedLabel, speed, values, apply, note });
		ModernTheme.Apply(this);
	}

	private Button CreatePreset(string name, byte value)
	{
		Button button = new Button { Text = name, Tag = value, Size = new Size(120, 32) };
		button.Click += delegate { modeValue.Value = (byte)button.Tag; };
		return button;
	}

	private void ChooseColour_Click(object sender, EventArgs e)
	{
		using (ColorDialog dialog = new ColorDialog { Color = selectedColor, FullOpen = true })
		{
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				selectedColor = dialog.Color;
				UpdatePreview();
			}
		}
	}

	private void Apply_Click(object sender, EventArgs e)
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeySet_KeyNum] = 176;
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeyType_Num] |= 8;
		FormMain.KeyParam.Data_Send_Buff[2] = (byte)modeValue.Value;
		FormMain.KeyParam.KeyChar[FormMain.KeyParam.KEY_Char_Num - 5] = "LED " + modeValue.Value;
		MessageBox.Show(this, "LED profile assigned. Click Download in the main window to write the supported mode value to the keyboard.", "LED Studio", MessageBoxButtons.OK, MessageBoxIcon.Information);
	}

	private void UpdatePreview()
	{
		values.Text = "Mode " + modeValue.Value + "  •  Brightness " + brightness.Value + "%  •  Speed " + speed.Value + "%";
		preview.Invalidate();
	}

	private void Preview_Paint(object sender, PaintEventArgs e)
	{
		float amount = brightness.Value / 100f;
		Color light = Color.FromArgb((int)(selectedColor.R * amount), (int)(selectedColor.G * amount), (int)(selectedColor.B * amount));
		using (SolidBrush brush = new SolidBrush(light))
		{
			e.Graphics.FillRectangle(brush, new Rectangle(1, 1, preview.Width - 2, preview.Height - 2));
		}
		TextRenderer.DrawText(e.Graphics, "LED preview", Font, new Point(16, 28), Color.White);
	}
}
