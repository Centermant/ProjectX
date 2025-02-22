using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class AddRecordPanel : Panel
    {
        private string _tableName;
        private DataEntryPanel _parentPanel;
        private TableLayoutPanel _mainTableLayoutPanel;
        private Button _addButton;
        private List<Label> _labels = new List<Label>();
        private List<Control> _inputControls = new List<Control>();
        private List<string> _columnTypes = new List<string>();
        private List<TableLayoutPanel> _rowTableLayoutPanels = new List<TableLayoutPanel>();
        private string _databaseFilePath = "MyDatabase.db";

        public AddRecordPanel(string tableName, DataEntryPanel parentPanel)
        {
            _tableName = tableName;
            _parentPanel = parentPanel;
            InitializeComponents();
            LoadColumnsForTable(tableName);
        }

        private void InitializeComponents()
        {
            _mainTableLayoutPanel = new TableLayoutPanel();
            _mainTableLayoutPanel.Dock = DockStyle.Fill;
            _mainTableLayoutPanel.ColumnCount = 1;
            _mainTableLayoutPanel.RowCount = 2;
            _mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            _mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.Controls.Add(_mainTableLayoutPanel);
            _addButton = new Button();
            _addButton.Text = "Добавить";
            _addButton.Font = MainForm.DefaultFont;
            _addButton.AutoSize = true;
            _addButton.Dock = DockStyle.Fill;
            _addButton.Margin = new Padding(5, 5, 15, 5);
            _addButton.Click += AddButton_Click;
            _mainTableLayoutPanel.Controls.Add(_addButton, 0, 1);
            this.Dock = DockStyle.Fill;
            this.BackColor = MainForm.DefaultPanelColor;
            this.Font = MainForm.DefaultFont;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateLayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _addButton.Font = MainForm.DefaultFont;
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (_rowTableLayoutPanels != null && _rowTableLayoutPanels.Count > 0)
            {
                int oneFifthWidth = _mainTableLayoutPanel.Width / 5;
                foreach (TableLayoutPanel rowTableLayoutPanel in _rowTableLayoutPanels)
                {
                    Label label = (Label)rowTableLayoutPanel.GetControlFromPosition(0, 0);
                    Control control = rowTableLayoutPanel.GetControlFromPosition(1, 0);

                    if (label != null && control != null)
                    {
                        label.Width = oneFifthWidth;
                        rowTableLayoutPanel.ColumnStyles[0].Width = oneFifthWidth;
                        if (control is TextBox textBox)
                        {
                            textBox.Width = _mainTableLayoutPanel.Width - oneFifthWidth - 10;
                        }
                    }
                }
                _addButton.Font = MainForm.DefaultFont;
            }
        }

        private void LoadColumnsForTable(string tableName)
        {
            _labels.Clear();
            _inputControls.Clear();
            _rowTableLayoutPanels.Clear();
            _mainTableLayoutPanel.Controls.Clear();
            _columnTypes.Clear();

            TableLayoutPanel inputFieldsTableLayoutPanel = new TableLayoutPanel();
            inputFieldsTableLayoutPanel.Dock = DockStyle.Fill;
            inputFieldsTableLayoutPanel.AutoSize = true;
            inputFieldsTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
            inputFieldsTableLayoutPanel.ColumnCount = 1;
            inputFieldsTableLayoutPanel.AutoScroll = true;
            _mainTableLayoutPanel.Controls.Add(inputFieldsTableLayoutPanel, 0, 0);

            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = $"PRAGMA table_info('{tableName}')";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["name"].ToString();
                                string columnType = reader["type"].ToString().ToUpper();
                                if (columnName.ToLower() == "id") continue;
                                _columnTypes.Add(columnType);

                                Label label = new Label();
                                label.Text = columnName + ":";
                                label.AutoSize = true;
                                label.Font = MainForm.DefaultFont;
                                int oneFifthWidth = _mainTableLayoutPanel.Width / 5;
                                label.Width = oneFifthWidth;
                                _labels.Add(label);

                                Control inputControl = null;

                                switch (columnType)
                                {
                                    case "DATE":
                                        // Создаем кнопку "Выбрать дату" и лейбл с датой
                                        TableLayoutPanel datePanel = new TableLayoutPanel();
                                        datePanel.ColumnCount = 2;
                                        datePanel.AutoSize = true;
                                        datePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                                        // Распределяем ширину: 70% для кнопки, 30% для лейбла
                                        datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                                        datePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

                                        Label dateLabel = new Label();
                                        DateTime _selectedDate = DateTime.Now;
                                        dateLabel.Text = "Дата не выбрана";
                                        dateLabel.AutoSize = true;
                                        dateLabel.Font = MainForm.DefaultFont;
                                        dateLabel.TextAlign = ContentAlignment.MiddleLeft;
                                        dateLabel.Dock = DockStyle.Fill;

                                        Button dateButton = new Button();
                                        dateButton.Text = "Выбрать дату";
                                        dateButton.AutoSize = true;
                                        dateButton.Font = MainForm.DefaultFont;
                                        dateButton.Dock = DockStyle.Fill;
                                        dateButton.Click += (sender, e) =>
                                        {
                                            using (var datePickerForm = new Form { StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false, Text = "Выберите дату", ClientSize = new Size(200, 150) })
                                            {
                                                var datePicker = new DateTimePicker
                                                {
                                                    Dock = DockStyle.Fill,
                                                    Format = DateTimePickerFormat.Short
                                                };
                                                datePickerForm.Controls.Add(datePicker);
                                                Button okButton = new Button
                                                {
                                                    Text = "OK",
                                                    DialogResult = DialogResult.OK,
                                                    Location = new Point(50, 100),
                                                    Size = new Size(100, 30)
                                                };
                                                datePickerForm.Controls.Add(okButton);

                                                if (datePickerForm.ShowDialog() == DialogResult.OK)
                                                {
                                                    _selectedDate = datePicker.Value;
                                                    dateLabel.Text = _selectedDate.ToShortDateString();
                                                }
                                            }
                                        };

                                        datePanel.Controls.Add(dateButton, 0, 0);
                                        datePanel.Controls.Add(dateLabel, 1, 0);
                                        inputControl = datePanel;
                                        break;

                                    case "BOOLEAN":
                                        TableLayoutPanel boolPanel = new TableLayoutPanel();
                                        boolPanel.ColumnCount = 2;
                                        boolPanel.AutoSize = true;

                                        RadioButton yesRadioButton = new RadioButton();
                                        yesRadioButton.Text = "Да";
                                        RadioButton noRadioButton = new RadioButton();
                                        noRadioButton.Text = "Нет";
                                        yesRadioButton.CheckedChanged += (sender, e) =>
                                        {
                                            if (yesRadioButton.Checked)
                                            {
                                                noRadioButton.Checked = false;
                                            }
                                        };

                                        noRadioButton.CheckedChanged += (sender, e) =>
                                        {
                                            if (noRadioButton.Checked)
                                            {
                                                yesRadioButton.Checked = false;
                                            }
                                        };
                                        boolPanel.Controls.Add(yesRadioButton, 0, 0);
                                        boolPanel.Controls.Add(noRadioButton, 1, 0);
                                        inputControl = boolPanel;
                                        break;

                                    default:
                                        TextBox textBox = new TextBox();
                                        textBox.Font = MainForm.DefaultFont;
                                        textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                        textBox.Width = _mainTableLayoutPanel.Width - oneFifthWidth - 10;
                                        inputControl = textBox;
                                        break;
                                }

                                _inputControls.Add(inputControl);

                                TableLayoutPanel rowTableLayoutPanel = new TableLayoutPanel();
                                rowTableLayoutPanel.ColumnCount = 2;
                                rowTableLayoutPanel.RowCount = 1;
                                rowTableLayoutPanel.AutoSize = true;
                                rowTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowOnly;

                                rowTableLayoutPanel.Controls.Add(label, 0, 0);
                                rowTableLayoutPanel.Controls.Add(inputControl, 1, 0);

                                rowTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, oneFifthWidth));
                                rowTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                                inputFieldsTableLayoutPanel.Controls.Add(rowTableLayoutPanel);
                                _rowTableLayoutPanels.Add(rowTableLayoutPanel);
                            }

                            _mainTableLayoutPanel.Controls.Add(_addButton, 0, 1);
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке столбцов таблицы: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_tableName))
            {
                MessageBox.Show("Пожалуйста, выберите таблицу.");
                return;
            }

            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    List<string> columnNamesList = new List<string>();
                    List<string> valuePlaceholdersList = new List<string>();
                    List<SQLiteParameter> parametersList = new List<SQLiteParameter>();

                    for (int i = 0; i < _labels.Count; i++)
                    {
                        string columnName = _labels[i].Text.TrimEnd(':');
                        string columnType = _columnTypes[i];
                        string valuePlaceholder = $"@value{i}";
                        object value = null;

                        TableLayoutPanel rowTableLayoutPanel = _rowTableLayoutPanels[i];
                        switch (columnType)
                        {
                            case "INTEGER":
                                TextBox intTextBox = rowTableLayoutPanel.Controls[1] as TextBox;
                                if (intTextBox != null && int.TryParse(intTextBox.Text, out int intValue))
                                {
                                    value = intValue;
                                }
                                else
                                {
                                    MessageBox.Show($"Пожалуйста, введите корректное целое число для колонки '{columnName}'.");
                                    return; //Сохраняем форму открытой!
                                }
                                break;

                            case "REAL":
                                TextBox realTextBox = rowTableLayoutPanel.Controls[1] as TextBox;
                                if (realTextBox != null && double.TryParse(realTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                                {
                                    value = doubleValue;
                                }
                                else
                                {
                                    MessageBox.Show($"Пожалуйста, введите корректное число с плавающей точкой для колонки '{columnName}'. Используйте точку в качестве разделителя.");
                                    return; //Сохраняем форму открытой!
                                }
                                break;

                            case "DATE":
                                TableLayoutPanel datePanel = rowTableLayoutPanel.Controls[1] as TableLayoutPanel;
                                if (datePanel != null)
                                {
                                    Label dateLabel = datePanel.Controls[1] as Label;
                                    if (dateLabel != null)
                                    {
                                        value = DateTime.Parse(dateLabel.Text);
                                    }
                                }
                                break;

                            case "BOOLEAN":
                                TableLayoutPanel boolPanel = rowTableLayoutPanel.Controls[1] as TableLayoutPanel;
                                if (boolPanel != null)
                                {
                                    RadioButton yesRadioButton = boolPanel.Controls[0] as RadioButton;
                                    value = yesRadioButton != null && yesRadioButton.Checked;
                                }
                                break;

                            default:
                                TextBox defaultTextBox = rowTableLayoutPanel.Controls[1] as TextBox;
                                if (defaultTextBox != null)
                                {
                                    value = defaultTextBox.Text;
                                }
                                break;
                        }

                        columnNamesList.Add($"`{columnName}`");
                        valuePlaceholdersList.Add(valuePlaceholder);
                        SQLiteParameter parameter = new SQLiteParameter(valuePlaceholder, value);
                        parametersList.Add(parameter);
                    }

                    string columnNames = string.Join(", ", columnNamesList);
                    string valuePlaceholders = string.Join(", ", valuePlaceholdersList);

                    string insertQuery = $"INSERT INTO `{_tableName}` ({columnNames}) VALUES ({valuePlaceholders})";

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddRange(parametersList.ToArray());

                        command.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно добавлена.");

                        foreach (TableLayoutPanel rowTableLayoutPanel in _rowTableLayoutPanels)
                        {
                            Control control = rowTableLayoutPanel.Controls[1];
                            if (control is TextBox textBox)
                            {
                                textBox.Clear();
                            }
                            else if (control is TableLayoutPanel datePanel)
                            {
                                if (datePanel.Controls[1] is Label dateLabel)
                                {
                                    dateLabel.Text = "Дата не выбрана";
                                }
                            }
                            else if (control is TableLayoutPanel panel)
                            {
                                if (panel.Controls.Count == 2 && panel.Controls[0] is RadioButton radioButton1 && panel.Controls[1] is RadioButton radioButton2)
                                {
                                    radioButton1.Checked = false;
                                    radioButton2.Checked = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                return; //Сохраняем форму открытой!
            }
        }
    }
}