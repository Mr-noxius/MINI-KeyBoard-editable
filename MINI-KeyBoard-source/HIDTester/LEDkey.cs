using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HIDTester;

public class LEDkey : UserControl
{
	private Button KEY_LEDMode0;

	private Button KEY_LEDMode1;

	private Button KEY_LEDMode2;

	private Button customModeButton;

	public LEDkey()
	{
		InitializeComponent();
		AddCustomModeButton();
		KEY_Colour_Init();
	}

	private void AddCustomModeButton()
	{
		customModeButton = new Button();
		customModeButton.Text = "Custom LED mode…";
		customModeButton.Size = new Size(180, 30);
		customModeButton.Location = new Point(18, 135);
		customModeButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		customModeButton.Click += CustomModeButton_Click;
		Controls.Add(customModeButton);
	}

	private void KEY_Colour_Init()
	{
		int red = 152;
		int green = 251;
		int blue = 152;
		KEY_LEDMode0.BackColor = Color.FromArgb(red, green, blue);
		KEY_LEDMode1.BackColor = Color.FromArgb(red, green, blue);
		KEY_LEDMode2.BackColor = Color.FromArgb(red, green, blue);
	}

	private void LEDGeneral_Char_Set()
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeyType_Num] |= 8;
	}

	private void KEY_LEDMode0_Click(object sender, EventArgs e)
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeySet_KeyNum] = 176;
		LEDGeneral_Char_Set();
		FormMain.KeyParam.Data_Send_Buff[2] = 0;
		FormMain.KeyParam.KeyChar[FormMain.KeyParam.KEY_Char_Num - 5] = KEY_LEDMode0.Text;
		KEY_Colour_Init();
		KEY_LEDMode0.BackColor = Color.FromArgb(255, 48, 48);
	}

	private void KEY_LEDMode1_Click(object sender, EventArgs e)
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeySet_KeyNum] = 176;
		LEDGeneral_Char_Set();
		FormMain.KeyParam.Data_Send_Buff[2] = 1;
		FormMain.KeyParam.KeyChar[FormMain.KeyParam.KEY_Char_Num - 5] = KEY_LEDMode1.Text;
		KEY_Colour_Init();
		KEY_LEDMode1.BackColor = Color.FromArgb(255, 48, 48);
	}

	private void KEY_LEDMode2_Click(object sender, EventArgs e)
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeySet_KeyNum] = 176;
		LEDGeneral_Char_Set();
		FormMain.KeyParam.Data_Send_Buff[2] = 2;
		FormMain.KeyParam.KeyChar[FormMain.KeyParam.KEY_Char_Num - 5] = KEY_LEDMode2.Text;
		KEY_Colour_Init();
		KEY_LEDMode2.BackColor = Color.FromArgb(255, 48, 48);
	}

	private void CustomModeButton_Click(object sender, EventArgs e)
	{
		using (CustomLedModeDialog dialog = new CustomLedModeDialog())
		{
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				ApplyLedMode(dialog.ModeValue, dialog.DisplayName);
			}
		}
	}

	private void ApplyLedMode(byte modeValue, string displayName)
	{
		FormMain.KeyParam.Data_Send_Buff[FormMain.KeyParam.KeySet_KeyNum] = 176;
		LEDGeneral_Char_Set();
		FormMain.KeyParam.Data_Send_Buff[2] = modeValue;
		FormMain.KeyParam.KeyChar[FormMain.KeyParam.KEY_Char_Num - 5] = displayName;
		KEY_Colour_Init();
		customModeButton.BackColor = Color.FromArgb(255, 48, 48);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(HIDTester.LEDkey));
		this.KEY_LEDMode0 = new System.Windows.Forms.Button();
		this.KEY_LEDMode1 = new System.Windows.Forms.Button();
		this.KEY_LEDMode2 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.KEY_LEDMode0, "KEY_LEDMode0");
		this.KEY_LEDMode0.Name = "KEY_LEDMode0";
		this.KEY_LEDMode0.UseVisualStyleBackColor = true;
		this.KEY_LEDMode0.Click += new System.EventHandler(KEY_LEDMode0_Click);
		componentResourceManager.ApplyResources(this.KEY_LEDMode1, "KEY_LEDMode1");
		this.KEY_LEDMode1.Name = "KEY_LEDMode1";
		this.KEY_LEDMode1.UseVisualStyleBackColor = true;
		this.KEY_LEDMode1.Click += new System.EventHandler(KEY_LEDMode1_Click);
		componentResourceManager.ApplyResources(this.KEY_LEDMode2, "KEY_LEDMode2");
		this.KEY_LEDMode2.Name = "KEY_LEDMode2";
		this.KEY_LEDMode2.UseVisualStyleBackColor = true;
		this.KEY_LEDMode2.Click += new System.EventHandler(KEY_LEDMode2_Click);
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.KEY_LEDMode2);
		base.Controls.Add(this.KEY_LEDMode1);
		base.Controls.Add(this.KEY_LEDMode0);
		base.Name = "LEDkey";
		base.ResumeLayout(false);
	}
}
