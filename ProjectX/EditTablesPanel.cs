using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public class EditTablesPanel : Panel
    {
        private Label _label;
        private TextBox _textBox;
        private Button _button;

        public EditTablesPanel()
        {
            _label = new Label();
            _label.Text = "База данных:";
            _label.Location = new Point(10, 10);
            _label.Font = MainForm.DefaultFont; // Применяем шрифт по умолчанию

            _textBox = new TextBox();
            _textBox.Location = new Point(10, 40);
            _textBox.Width = 200;
            _textBox.Font = MainForm.DefaultFont; // Применяем шрифт по умолчанию

            _button = new Button();
            _button.Text = "Изменить базу данных";
            _button.Location = new Point(10, 70);
            _button.Font = MainForm.DefaultFont; // Применяем шрифт по умолчанию
            _button.Click += Button_Click;

            //  Добавление элементов управления на панель
            this.Controls.Add(_label);
            this.Controls.Add(_textBox);
            this.Controls.Add(_button);

            // Настройка панели
            this.Dock = DockStyle.Fill; // Важно для правильного отображения в PanelSwitcher
            this.BackColor = Color.LightGray; // Для визуального различия
            this.Font = MainForm.DefaultFont; // Применяем шрифт ко всей панели
        }

        private void Button_Click(object sender, EventArgs e)
        {
            //  Обработчик нажатия на кнопку
            string databaseName = _textBox.Text;
            //  Здесь вызывайте методы DatabaseCreator для создания базы данных.
            MessageBox.Show($"Будет создана база данных: {databaseName}");
        }
    }
}