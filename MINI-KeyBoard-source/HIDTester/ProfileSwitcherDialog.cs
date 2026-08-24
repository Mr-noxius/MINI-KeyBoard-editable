using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIDTester;

/// <summary>
/// Popup shown when the user presses-and-turns a rotary-capable key. Lets the user
/// pick which firmware layer or software app-profile should become active. Applying
/// the choice updates ProfileManager.ActiveProfile, which FormMain listens to in order
/// to re-label the on-screen keys and (for firmware layers) send the layer-switch HID
/// report via Send_SwLayer().
/// </summary>
public sealed class ProfileSwitcherDialog : Form
{
	private readonly ListBox list = new ListBox();
	private readonly Label header = new Label();
	private readonly Label keyContext = new Label();
	private readonly Button cancelButton = new Button();
	private readonly Button applyButton = new Button();

	public KeyProfile SelectedProfile { get; private set; }

	public ProfileSwitcherDialog(string triggeringKeyName)
	{
		Text = "Key Profile Switcher";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		StartPosition = FormStartPosition.CenterScreen;
		MaximizeBox = false;
		MinimizeBox = false;
		ShowInTaskbar = false;
		ClientSize = new Size(420, 360);
		TopMost = true;

		header.Text = "Choose Profile";
		header.Font = new Font("Segoe UI Semibold", 13f);
		header.AutoSize = true;
		header.Location = new Point(20, 16);

		keyContext.Text = triggeringKeyName + " — turn to preview, click to select";
		keyContext.AutoSize = true;
		keyContext.Location = new Point(20, 44);
		keyContext.ForeColor = ModernTheme.MutedText;

		list.SetBounds(20, 76, 380, 210);
		list.BorderStyle = BorderStyle.None;
		list.DrawMode = DrawMode.OwnerDrawFixed;
		list.ItemHeight = 52;
		list.IntegralHeight = false;
		foreach (KeyProfile profile in ProfileManager.Profiles)
		{
			list.Items.Add(profile);
		}
		list.SelectedItem = ProfileManager.ActiveProfile;
		list.DrawItem += List_DrawItem;
		list.KeyDown += List_KeyDown;
		list.DoubleClick += delegate { ApplyAndClose(); };

		cancelButton.Text = "Cancel";
		cancelButton.Size = new Size(90, 32);
		cancelButton.Location = new Point(210, 306);
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };

		applyButton.Text = "Apply";
		applyButton.Size = new Size(110, 32);
		applyButton.Location = new Point(310, 306);
		applyButton.BackColor = ModernTheme.Accent;
		applyButton.ForeColor = ModernTheme.Background;
		applyButton.Click += delegate { ApplyAndClose(); };

		Controls.AddRange(new Control[] { header, keyContext, list, cancelButton, applyButton });
		ModernTheme.Apply(this);
		list.BackColor = ModernTheme.Background;
	}

	/// <summary>
	/// Called repeatedly while the user keeps turning the rotary key, so the
	/// highlighted row advances without closing the popup. Wraps around at the ends.
	/// </summary>
	public void PreviewStep(int direction)
	{
		if (list.Items.Count == 0)
		{
			return;
		}
		int next = (list.SelectedIndex < 0 ? 0 : list.SelectedIndex) + Math.Sign(direction);
		if (next < 0)
		{
			next = list.Items.Count - 1;
		}
		if (next >= list.Items.Count)
		{
			next = 0;
		}
		list.SelectedIndex = next;
	}

	private void List_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			ApplyAndClose();
		}
		else if (e.KeyCode == Keys.Escape)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}

	private void ApplyAndClose()
	{
		SelectedProfile = list.SelectedItem as KeyProfile;
		DialogResult = DialogResult.OK;
		Close();
	}

	private void List_DrawItem(object sender, DrawItemEventArgs e)
	{
		if (e.Index < 0)
		{
			return;
		}
		KeyProfile profile = (KeyProfile)list.Items[e.Index];
		bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
		Color rowBack = selected ? ModernTheme.Accent : ModernTheme.Surface;
		Color rowFore = selected ? ModernTheme.Background : ModernTheme.Text;
		Color rowMuted = selected ? ModernTheme.Background : ModernTheme.MutedText;

		using (SolidBrush backBrush = new SolidBrush(rowBack))
		{
			e.Graphics.FillRectangle(backBrush, e.Bounds);
		}
		using (SolidBrush dotBrush = new SolidBrush(profile.Indicator))
		{
			e.Graphics.FillEllipse(dotBrush, e.Bounds.Left + 12, e.Bounds.Top + 18, 12, 12);
		}
		using (SolidBrush nameBrush = new SolidBrush(rowFore))
		using (Font nameFont = new Font("Segoe UI Semibold", 10f))
		{
			e.Graphics.DrawString(profile.Name, nameFont, nameBrush, e.Bounds.Left + 34, e.Bounds.Top + 6);
		}
		using (SolidBrush descBrush = new SolidBrush(rowMuted))
		using (Font descFont = new Font("Segoe UI", 8.5f))
		{
			e.Graphics.DrawString(profile.Description, descFont, descBrush, e.Bounds.Left + 34, e.Bounds.Top + 26);
		}
		if (selected)
		{
			using (Font checkFont = new Font("Segoe UI", 12f))
			using (SolidBrush checkBrush = new SolidBrush(rowFore))
			{
				e.Graphics.DrawString("\u2713", checkFont, checkBrush, e.Bounds.Right - 28, e.Bounds.Top + 14);
			}
		}
	}
}
