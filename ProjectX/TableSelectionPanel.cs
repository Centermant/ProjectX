using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class TableSelectionPanel : Panel
    {
        private ComboBox _tableComboBox;
        private Button _selectTableButton;
        private DataEntryPanel _parentPanel;
        private string _databaseFilePath = "MyDatabase.db";

        public TableSelectionPanel(DataEntryPanel parentPanel)
        {
            _parentPanel = parentPanel;
            InitializeComponents();
            LoadTableNames();
        }

        private void InitializeComponents()
        {
            // 1. ComboBox для выбора таблицы
            _tableComboBox = new ComboBox();
            _tableComboBox.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 50);
            _tableComboBox.Width = 200;
            _tableComboBox.Font = MainForm.DefaultFont;
            _tableComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _tableComboBox.Anchor = AnchorStyles.None;

            // 2. Кнопка "Выбрать"
            _selectTableButton = new Button();
            _selectTableButton.Text = "Выбрать";
            _selectTableButton.Location = new Point(this.Width / 2 - _selectTableButton.Width / 2, this.Height / 2);
            _selectTableButton.AutoSize = true;
            _selectTableButton.Font = MainForm.DefaultFont;
            _selectTableButton.Anchor = AnchorStyles.None;
            _selectTableButton.Click += SelectTable_Click;

            // Добавляем элементы управления на панель
            this.Controls.Add(_tableComboBox);
            this.Controls.Add(_selectTableButton);

            // Настройка панели
            this.Dock = DockStyle.Fill;
            this.BackColor = MainForm.DefaultPanelColor;
            this.Font = MainForm.DefaultFont;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            _tableComboBox.Location = new Point(this.Width / 2 - 100, this.Height / 2 - 50);
            _selectTableButton.Location = new Point(this.Width / 2 - _selectTableButton.Width / 2, this.Height / 2);
        }

        private void LoadTableNames()
        {
            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Получаем список таблиц из sqlite_master, исключая "UserFonts"
                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name != 'TablesNames' AND name != 'UserFonts' ORDER BY name;";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _tableComboBox.Items.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка таблиц: {ex.Message}");
            }
        }

        private void SelectTable_Click(object sender, EventArgs e)
        {
            string selectedTable = _tableComboBox.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Пожалуйста, выберите таблицу.");
                return;
            }

            MainForm mainForm = (MainForm)Application.OpenForms[0];

            if (mainForm._currentAction == ActionType.AddRecord)
            {
                _parentPanel.ShowAddRecordPanel(selectedTable);
            }
            else if (mainForm._currentAction == ActionType.ViewTable)
            {
                _parentPanel.ShowViewDataTablePanel(selectedTable);
            }
        }
    }
}