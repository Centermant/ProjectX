using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class CreateDatabasePanel : Panel
    {
        private Label _tableNameLabel;
        private TextBox _tableNameTextBox;
        private Label _columnNameLabel;
        private TextBox _columnNameTextBox;
        private Label _columnTypeLabel;
        private ComboBox _columnTypeComboBox;
        private Button _addColumnButton;
        private ListBox _columnsListBox;
        private Button _createTableButton;

        private string _databaseFilePath = "MyDatabase.db"; // Путь к файлу БД по умолчанию

        // Словарь для преобразования понятных типов данных в типы SQLite
        private Dictionary<string, string> _typeMapping = new Dictionary<string, string>()
        {
            { "Целое число", "INTEGER" },
            { "Число со знаками после запятой", "REAL" },
            { "Да/Нет", "BOOLEAN" },
            { "Текст", "TEXT" },
            { "Дата", "DATE" }
        };

        public CreateDatabasePanel()
        {
            InitializeComponents();
            // Заполняем ComboBox понятными типами данных
            foreach (string key in _typeMapping.Keys)
            {
                _columnTypeComboBox.Items.Add(key);
            }
            _columnTypeComboBox.SelectedIndex = 0; // Выбираем первый элемент по умолчанию
        }

        private void InitializeComponents()
        {
            // Определяем половину ширины панели (будет использоваться для размещения элементов)
            int halfWidth = this.Width / 2;

            // 6. Кнопка "Создать таблицу" (СОЗДАЕМ РАНЬШЕ)
            _createTableButton = new Button();
            _createTableButton.Text = "Создать таблицу";
            _createTableButton.AutoSize = true;
            _createTableButton.Font = MainForm.DefaultFont;
            _createTableButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Прижимаем к низу и левому краю
            _createTableButton.Click += CreateTableButton_Click; // Подписываемся на событие Click

            // 1. Название таблицы
            _tableNameLabel = new Label();
            _tableNameLabel.Text = "Название таблицы:";
            _tableNameLabel.Location = new Point(10, 10);
            _tableNameLabel.AutoSize = true;
            _tableNameLabel.Font = MainForm.DefaultFont;
            _tableNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            _tableNameTextBox = new TextBox();
            _tableNameTextBox.Location = new Point(10, 35);
            _tableNameTextBox.Width = halfWidth - 20; // Ограничиваем ширину
            _tableNameTextBox.Font = MainForm.DefaultFont;
            _tableNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Растягиваем по ширине

            // 2. Название колонки
            _columnNameLabel = new Label();
            _columnNameLabel.Text = "Название колонки:";
            _columnNameLabel.Location = new Point(10, 70);
            _columnNameLabel.AutoSize = true;
            _columnNameLabel.Font = MainForm.DefaultFont;
            _columnNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            _columnNameTextBox = new TextBox();
            _columnNameTextBox.Location = new Point(10, 95);
            _columnNameTextBox.Width = halfWidth - 20; // Ограничиваем ширину
            _columnNameTextBox.Font = MainForm.DefaultFont;
            _columnNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Растягиваем по ширине

            // 3. Тип данных
            _columnTypeLabel = new Label();
            _columnTypeLabel.Text = "Тип данных:";
            _columnTypeLabel.Location = new Point(10, 130);
            _columnTypeLabel.AutoSize = true;
            _columnTypeLabel.Font = MainForm.DefaultFont;
            _columnTypeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            _columnTypeComboBox = new ComboBox();
            _columnTypeComboBox.Location = new Point(10, 155);
            _columnTypeComboBox.Width = halfWidth - 20; // Ограничиваем ширину
            _columnTypeComboBox.Font = MainForm.DefaultFont;
            _columnTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Запрещаем ввод текста
            _columnTypeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Растягиваем по ширине

            // 4. Кнопка "Добавить столбец"
            _addColumnButton = new Button();
            _addColumnButton.Text = "Добавить столбец";
            _addColumnButton.Location = new Point(10, 190);
            _addColumnButton.AutoSize = true;
            _addColumnButton.Font = MainForm.DefaultFont;
            _addColumnButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _addColumnButton.Click += AddColumnButton_Click; // Подписываемся на событие Click

            // 5. ListBox для отображения добавленных столбцов
            _columnsListBox = new ListBox();
            _columnsListBox.Location = new Point(halfWidth, 10);
            _columnsListBox.Width = this.Width - halfWidth - 10; // Оставшаяся часть ширины
            _columnsListBox.Height = this.Height - 10 - _createTableButton.Height - 15;  // Вычисляем высоту с учетом кнопки и отступов
            _columnsListBox.Font = MainForm.DefaultFont;
            _columnsListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; // Растягиваем по ширине и высоте!


            // Размещаем кнопку "Создать таблицу" внизу (ПОСЛЕ ListBox)
            _createTableButton.Location = new Point(10, this.Height - _createTableButton.Height - 5); // Размещаем кнопку внизу

            // Добавляем элементы управления на панель (ВАЖНО: В ПРАВИЛЬНОМ ПОРЯДКЕ!)
            this.Controls.Add(_tableNameLabel);
            this.Controls.Add(_tableNameTextBox);
            this.Controls.Add(_columnNameLabel);
            this.Controls.Add(_columnNameTextBox);
            this.Controls.Add(_columnTypeLabel);
            this.Controls.Add(_columnTypeComboBox);
            this.Controls.Add(_addColumnButton);
            this.Controls.Add(_columnsListBox);
            this.Controls.Add(_createTableButton);

            // Настройка панели
            this.Dock = DockStyle.Fill;
            this.BackColor = MainForm.DefaultPanelColor;
            this.Font = MainForm.DefaultFont;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            // Вычисляем новую половину ширины при изменении размера панели
            int halfWidth = this.Width / 2;

            // Обновляем ширину TextBox'ов и ComboBox'a
            _tableNameTextBox.Width = halfWidth - 20;
            _columnNameTextBox.Width = halfWidth - 20;
            _columnTypeComboBox.Width = halfWidth - 20;

            // Обновляем положение и ширину ListBox'a
            _columnsListBox.Location = new Point(halfWidth, 10);
            _columnsListBox.Width = this.Width - halfWidth - 10;
            _columnsListBox.Height = this.Height - 10 - _createTableButton.Height - 15; // Пересчитываем высоту!

            // Обновляем положение кнопки "Создать таблицу"
            _createTableButton.Location = new Point(10, this.Height - _createTableButton.Height - 5);
        }

        private void AddColumnButton_Click(object sender, EventArgs e)
        {
            string columnName = _columnNameTextBox.Text.Trim();
            string columnTypeFriendly = _columnTypeComboBox.SelectedItem.ToString(); // Тип данных "для пользователя"
            string columnType = _typeMapping[columnTypeFriendly]; // Преобразуем в тип SQLite

            if (string.IsNullOrEmpty(columnName))
            {
                MessageBox.Show("Введите название колонки.");
                return;
            }

            // Отображаем в ListBox понятное название типа данных
            _columnsListBox.Items.Add($"{columnName}, {columnTypeFriendly}");

            // Очищаем поля ввода
            _columnNameTextBox.Clear();
        }

        private void CreateTableButton_Click(object sender, EventArgs e)
        {
            string tableName = _tableNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Введите название таблицы.");
                return;
            }

            if (_columnsListBox.Items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну колонку.");
                return;
            }

            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            // 2. Создаем базу данных, если она не существует
            if (!System.IO.File.Exists(_databaseFilePath))
            {
                SQLiteConnection.CreateFile(_databaseFilePath);
            }

            // 3. Проверяем, существует ли таблица
            bool tableExists = false;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string checkTableQuery = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
                using (SQLiteCommand command = new SQLiteCommand(checkTableQuery, connection))
                {
                    object result = command.ExecuteScalar();
                    tableExists = result != null;
                }
            }

            // 4. Если таблица существует, предлагаем варианты действий
            if (tableExists)
            {
                DialogResult dialogResult = MessageBox.Show($"Таблица '{tableName}' уже существует. Хотите удалить ее и создать новую?", "Таблица существует", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // 5. Удаляем существующую таблицу
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string dropTableQuery = $"DROP TABLE `{tableName}`";
                        using (SQLiteCommand command = new SQLiteCommand(dropTableQuery, connection))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                                MessageBox.Show($"Таблица '{tableName}' успешно удалена.");
                            }
                            catch (SQLiteException ex)
                            {
                                MessageBox.Show($"Ошибка при удалении таблицы: {ex.Message}");
                                return; // Прерываем создание таблицы
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Создание таблицы отменено.");
                    return; // Отменяем создание таблицы
                }
            }

            // 6. Формируем SQL-запрос для создания таблицы
            string createTableQuery = $"CREATE TABLE IF NOT EXISTS `{tableName}` (";
            List<string> columnDefinitions = new List<string>();

            foreach (string item in _columnsListBox.Items)
            {
                string[] parts = item.Split(new string[] { ", " }, StringSplitOptions.None);
                string columnName = parts[0];
                string columnTypeFriendly = parts[1];
                string columnType = _typeMapping[columnTypeFriendly];

                columnDefinitions.Add($"`{columnName}` {columnType}");
            }

            createTableQuery += string.Join(", ", columnDefinitions.ToArray()) + ")";

            // 7. Выполняем SQL-запрос
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show($"Таблица '{tableName}' успешно создана.");
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Ошибка при создании таблицы: {ex.Message}");
                    }
                }
            }

            // Очищаем форму
            _tableNameTextBox.Clear();
            _columnsListBox.Items.Clear();
        }
    }
}