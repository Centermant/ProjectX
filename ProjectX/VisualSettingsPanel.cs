using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class VisualSettingsPanel : Panel
    {
        private MainForm _parentForm;
        private FontDialog _fontDialog;
        private ColorDialog _backColorDialog;
        private ColorDialog _panelColorDialog;

        private Button _changeFontButton;
        private Button _changeBackColorButton;
        private Button _changePanelColorButton;
        private Button _saveSettingsButton;

        public VisualSettingsPanel(MainForm parentForm)
        {
            _parentForm = parentForm;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            _fontDialog = new FontDialog();
            _backColorDialog = new ColorDialog();
            _panelColorDialog = new ColorDialog();

            // 1. Кнопка "Изменить шрифт"
            _changeFontButton = new Button();
            _changeFontButton.Text = "Изменить шрифт";
            _changeFontButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 75);
            _changeFontButton.Width = 200;
            _changeFontButton.Font = MainForm.DefaultFont;
            _changeFontButton.Anchor = AnchorStyles.None;
            _changeFontButton.Click += ChangeFontButton_Click;

            // 2. Кнопка "Изменить цвет фона"
            _changeBackColorButton = new Button();
            _changeBackColorButton.Text = "Изменить цвет фона";
            _changeBackColorButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 25);
            _changeBackColorButton.Width = 200;
            _changeBackColorButton.Font = MainForm.DefaultFont;
            _changeBackColorButton.Anchor = AnchorStyles.None;
            _changeBackColorButton.Click += ChangeBackColorButton_Click;

            // 3. Кнопка "Изменить цвет панели"
            _changePanelColorButton = new Button();
            _changePanelColorButton.Text = "Изменить цвет панели";
            _changePanelColorButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 + 25);
            _changePanelColorButton.Width = 200;
            _changePanelColorButton.Font = MainForm.DefaultFont;
            _changePanelColorButton.Anchor = AnchorStyles.None;
            _changePanelColorButton.Click += ChangePanelColorButton_Click;

            // 4. Кнопка "Сохранить настройки"
            _saveSettingsButton = new Button();
            _saveSettingsButton.Text = "Сохранить настройки";
            _saveSettingsButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 + 75);
            _saveSettingsButton.Width = 200;
            _saveSettingsButton.Font = MainForm.DefaultFont;
            _saveSettingsButton.Anchor = AnchorStyles.None;
            _saveSettingsButton.Click += SaveSettingsButton_Click;

            // Добавляем элементы управления на панель
            this.Controls.Add(_changeFontButton);
            this.Controls.Add(_changeBackColorButton);
            this.Controls.Add(_changePanelColorButton);
            this.Controls.Add(_saveSettingsButton);

            // Настройка панели
            this.Dock = DockStyle.Fill;
            this.BackColor = MainForm.DefaultPanelColor;
            this.Font = MainForm.DefaultFont;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            _changeFontButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 75);
            _changeBackColorButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 25);
            _changePanelColorButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 + 25);
            _saveSettingsButton.Location = new Point(this.Width / 2 - 100, this.Height / 2 + 75);
        }

        private void ChangeFontButton_Click(object sender, EventArgs e)
        {
            _fontDialog.Font = MainForm.DefaultFont;
            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                MainForm.DefaultFont = _fontDialog.Font;
                _parentForm.UpdateFontRecursive(_parentForm);
            }
        }

        private void ChangeBackColorButton_Click(object sender, EventArgs e)
        {
            _backColorDialog.Color = MainForm.DefaultBackColor;
            if (_backColorDialog.ShowDialog() == DialogResult.OK)
            {
                MainForm.DefaultBackColor = _backColorDialog.Color;
                _parentForm.UpdateBackColorRecursive(_parentForm);
            }
        }

        private void ChangePanelColorButton_Click(object sender, EventArgs e)
        {
            _panelColorDialog.Color = MainForm.DefaultPanelColor;
            if (_panelColorDialog.ShowDialog() == DialogResult.OK)
            {
                MainForm.DefaultPanelColor = _panelColorDialog.Color;
                _parentForm.UpdatePanelColorRecursive(_parentForm);
            }
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            VisualSettingsManager.SaveVisualSettings(MainForm.DefaultFont, MainForm.DefaultBackColor, MainForm.DefaultPanelColor);
            MessageBox.Show("Настройки сохранены.");
        }
    }
}