using System;
using System.Windows.Forms;

namespace ProjectX
{
    public class ReportEditorPanel : Panel
    {
        private Label _label;
        private TextBox _textBox;
        private Button _button;

        public ReportEditorPanel()
        {
            //  Инициализация элементов управления
            _label = new Label();
            _label.Text = "Введите имя базы данных:";
            _label.Location = new System.Drawing.Point(10, 10);

            _textBox = new TextBox();
            _textBox.Location = new System.Drawing.Point(10, 40);
            _textBox.Width = 200;

            _button = new Button();
            _button.Text = "Создать базу данных";
            _button.Location = new System.Drawing.Point(10, 70);
            _button.Click += Button_Click;

            //  Добавление элементов управления на панель
            this.Controls.Add(_label);
            this.Controls.Add(_textBox);
            this.Controls.Add(_button);

            // Настройка панели
            this.Dock = DockStyle.Fill; // Важно для правильного отображения в PanelSwitcher
            this.BackColor = System.Drawing.Color.LightGray; // Для визуального различия
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