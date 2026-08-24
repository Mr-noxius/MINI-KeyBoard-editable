using System.Drawing;
using System.Windows.Forms;

namespace HIDTester;

internal static class ModernTheme
{
	public static readonly Color Background = Color.FromArgb(18, 24, 38);
	public static readonly Color Surface = Color.FromArgb(29, 38, 57);
	public static readonly Color SurfaceLight = Color.FromArgb(42, 54, 77);
	public static readonly Color Accent = Color.FromArgb(67, 208, 173);
	public static readonly Color Text = Color.FromArgb(237, 242, 247);
	public static readonly Color MutedText = Color.FromArgb(166, 181, 204);

	public static void Apply(Control root)
	{
		root.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
		Style(root);
		foreach (Control control in root.Controls)
		{
			Apply(control);
		}
	}

	private static void Style(Control control)
	{
		switch (control)
		{
		case Form form:
			form.BackColor = Background;
			form.ForeColor = Text;
			break;
		case SplitContainer split:
			split.BackColor = Background;
			split.Panel1.BackColor = Surface;
			split.Panel2.BackColor = Background;
			break;
		case FlowLayoutPanel flow:
			flow.BackColor = Surface;
			break;
		case Panel panel:
			panel.BackColor = Surface;
			break;
		case PictureBox picture:
			picture.BackColor = Background;
			break;
		case UserControl userControl:
			userControl.BackColor = Background;
			userControl.ForeColor = Text;
			break;
		case Button button:
			button.FlatStyle = FlatStyle.Flat;
			button.FlatAppearance.BorderSize = 0;
			button.FlatAppearance.MouseOverBackColor = SurfaceLight;
			button.FlatAppearance.MouseDownBackColor = Accent;
			button.BackColor = SurfaceLight;
			button.ForeColor = Text;
			button.Cursor = Cursors.Hand;
			button.Margin = new Padding(6);
			break;
		case Label label:
			label.ForeColor = Text;
			break;
		case TextBox textBox:
			textBox.BackColor = SurfaceLight;
			textBox.ForeColor = Text;
			textBox.BorderStyle = BorderStyle.FixedSingle;
			break;
		case NumericUpDown numeric:
			numeric.BackColor = SurfaceLight;
			numeric.ForeColor = Text;
			break;
		}
	}
}
