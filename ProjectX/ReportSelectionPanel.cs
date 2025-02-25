using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ProjectX
{
    public partial class ReportSelectionPanel : Panel
    {
        private ComboBox _reportTemplateComboBox;
        private ComboBox _filterTypeComboBox;
        private TableLayoutPanel _filterPanel;
        private Button _printButton;

        private TreeView _columnTreeView1;
        private TreeView _columnTreeView2;
        private TextBox _filterTextBox;
        private DateTimePicker _datePicker1;
        private DateTimePicker _datePicker2;
        private Label _dateLabel1;
        private Label _dateLabel2;

        private string _databaseFilePath = "MyDatabase.db";

        public ReportSelectionPanel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Non commercial
            InitializeComponents();
            LoadReportTemplates();
        }

        private void InitializeComponents()
        {
            // 1. Report Template ComboBox
            _reportTemplateComboBox = new ComboBox();
            _reportTemplateComboBox.Dock = DockStyle.Top;
            _reportTemplateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // 2. Filter Type ComboBox
            _filterTypeComboBox = new ComboBox();
            _filterTypeComboBox.Dock = DockStyle.Top;
            _filterTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _filterTypeComboBox.Items.AddRange(new object[] {
                "Без сортировки",
                "Сортировка по наименованию",
                "Сортировка по периоду",
                "Сортировка по наименованию и периоду"
            });
            _filterTypeComboBox.SelectedIndexChanged += FilterTypeComboBox_SelectedIndexChanged;

            // 3. Filter Panel
            _filterPanel = new TableLayoutPanel();
            _filterPanel.Dock = DockStyle.Top;
            _filterPanel.AutoSize = true;

            // 4. Print Button
            _printButton = new Button();
            _printButton.Text = "Печать";
            _printButton.Dock = DockStyle.Bottom;
            _printButton.Click += PrintButton_Click;

            //Add Elements
            this.Controls.Add(_printButton);
            this.Controls.Add(_filterPanel);
            this.Controls.Add(_filterTypeComboBox);
            this.Controls.Add(_reportTemplateComboBox);

            this.Font = MainForm.DefaultFont;
            this.Dock = DockStyle.Fill;
        }

        private void FilterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Очищаем _filterPanel перед добавлением новых элементов управления
            _filterPanel.Controls.Clear();
            _filterPanel.RowStyles.Clear(); // Очищаем RowStyles, чтобы TableLayoutPanel пересчитал их заново

            switch (_filterTypeComboBox.SelectedIndex)
            {
                case 1: // Сортировка по наименованию
                    AddFilterByNameControls();
                    break;
                case 2: // Сортировка по периоду
                    AddFilterByPeriodControls();
                    break;
                case 3: // Сортировка по наименованию и периоду
                    AddFilterByNameAndPeriodControls();
                    break;
            }

            // Автоматически изменяем высоту _filterPanel в зависимости от добавленных элементов
            _filterPanel.AutoSize = true;
        }

        private void AddFilterByNameControls()
        {
            // Создание элементов управления для фильтрации по наименованию
            Label columnLabel = new Label { Text = "Выберите столбец:", AutoSize = true };
            _columnTreeView1 = new TreeView { Dock = DockStyle.Fill, Width = 150 };
            LoadDatabaseStructure(_columnTreeView1);
            Label filterLabel = new Label { Text = "Введите значение:", AutoSize = true };
            _filterTextBox = new TextBox { Width = 100 };

            // Добавление элементов управления в _filterPanel
            _filterPanel.Controls.Add(columnLabel);
            _filterPanel.Controls.Add(_columnTreeView1);
            _filterPanel.Controls.Add(filterLabel);
            _filterPanel.Controls.Add(_filterTextBox);

            // Настройка TableLayoutPanel
            _filterPanel.ColumnCount = 2;
            _filterPanel.RowCount = 2;
            _filterPanel.AutoSize = true;

            _filterPanel.Controls.Add(columnLabel, 0, 0);
            _filterPanel.Controls.Add(_columnTreeView1, 1, 0);
            _filterPanel.Controls.Add(filterLabel, 0, 1);
            _filterPanel.Controls.Add(_filterTextBox, 1, 1);
        }

        private void AddFilterByPeriodControls()
        {
            // Создание элементов управления для фильтрации по периоду
            Label columnLabel = new Label { Text = "Выберите столбец:", AutoSize = true };
            _columnTreeView1 = new TreeView { Dock = DockStyle.Fill, Width = 150 };
            LoadDatabaseStructure(_columnTreeView1);
            _dateLabel1 = new Label { Text = "Дата начала:", AutoSize = true };
            _datePicker1 = new DateTimePicker { Width = 150 };
            _dateLabel2 = new Label { Text = "Дата окончания:", AutoSize = true };
            _datePicker2 = new DateTimePicker { Width = 150 };

            // Добавление элементов управления в _filterPanel
            _filterPanel.Controls.Add(columnLabel);
            _filterPanel.Controls.Add(_columnTreeView1);
            _filterPanel.Controls.Add(_dateLabel1);
            _filterPanel.Controls.Add(_datePicker1);
            _filterPanel.Controls.Add(_dateLabel2);
            _filterPanel.Controls.Add(_datePicker2);

            // Настройка TableLayoutPanel
            _filterPanel.ColumnCount = 2;
            _filterPanel.RowCount = 3;
            _filterPanel.AutoSize = true;

            _filterPanel.Controls.Add(columnLabel, 0, 0);
            _filterPanel.Controls.Add(_columnTreeView1, 1, 0);
            _filterPanel.Controls.Add(_dateLabel1, 0, 1);
            _filterPanel.Controls.Add(_datePicker1, 1, 1);
            _filterPanel.Controls.Add(_dateLabel2, 0, 2);
            _filterPanel.Controls.Add(_datePicker2, 1, 2);
        }

        private void AddFilterByNameAndPeriodControls()
        {
            // Создание элементов управления для фильтрации по наименованию и периоду
            Label columnLabel1 = new Label { Text = "Выберите столбец (наименование):", AutoSize = true };
            _columnTreeView1 = new TreeView { Dock = DockStyle.Fill, Width = 150 };
            LoadDatabaseStructure(_columnTreeView1);

            Label filterLabel = new Label { Text = "Введите значение (наименование):", AutoSize = true };
            _filterTextBox = new TextBox { Width = 100 };
            Label columnLabel2 = new Label { Text = "Выберите столбец (дата):", AutoSize = true };
            _columnTreeView2 = new TreeView { Dock = DockStyle.Fill, Width = 150 };
            LoadDatabaseStructure(_columnTreeView2);
            _dateLabel1 = new Label { Text = "Дата начала:", AutoSize = true };
            _datePicker1 = new DateTimePicker { Width = 150 };
            _dateLabel2 = new Label { Text = "Дата окончания:", AutoSize = true };
            _datePicker2 = new DateTimePicker { Width = 150 };

            // Добавление элементов управления в _filterPanel
            _filterPanel.Controls.Add(columnLabel1);
            _filterPanel.Controls.Add(_columnTreeView1);
            _filterPanel.Controls.Add(filterLabel);
            _filterPanel.Controls.Add(_filterTextBox);
            _filterPanel.Controls.Add(columnLabel2);
            _filterPanel.Controls.Add(_columnTreeView2);
            _filterPanel.Controls.Add(_dateLabel1);
            _filterPanel.Controls.Add(_datePicker1);
            _filterPanel.Controls.Add(_dateLabel2);
            _filterPanel.Controls.Add(_datePicker2);

            // Настройка TableLayoutPanel
            _filterPanel.ColumnCount = 2;
            _filterPanel.RowCount = 5;
            _filterPanel.AutoSize = true;

            _filterPanel.Controls.Add(columnLabel1, 0, 0);
            _filterPanel.Controls.Add(_columnTreeView1, 1, 0);
            _filterPanel.Controls.Add(filterLabel, 0, 1);
            _filterPanel.Controls.Add(_filterTextBox, 1, 1);
            _filterPanel.Controls.Add(columnLabel2, 0, 2);
            _filterPanel.Controls.Add(_columnTreeView2, 1, 2);
            _filterPanel.Controls.Add(_dateLabel1, 0, 3);
            _filterPanel.Controls.Add(_datePicker1, 1, 3);
            _filterPanel.Controls.Add(_dateLabel2, 0, 4);
            _filterPanel.Controls.Add(_datePicker2, 1, 4);
        }

        private void LoadDatabaseStructure(TreeView treeView)
        {
            // Очищаем TreeView перед загрузкой новой структуры
            treeView.Nodes.Clear();

            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Получаем список таблиц
                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name != 'UserFonts'";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tableName = reader["name"].ToString();
                                TreeNode tableNode = new TreeNode(tableName);
                                tableNode.Tag = tableName;
                                treeView.Nodes.Add(tableNode);

                                // Получаем список столбцов для каждой таблицы
                                List<string> columns = GetTableColumns(connection, tableName);
                                foreach (string columnName in columns)
                                {
                                    TreeNode columnNode = new TreeNode(columnName);
                                    columnNode.Tag = $"{tableName}.{columnName}"; // Сохраняем имя таблицы и столбца
                                    tableNode.Nodes.Add(columnNode);
                                }
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке структуры базы данных: {ex.Message}");
            }
        }

        private List<string> GetTableColumns(SQLiteConnection connection, string tableName)
        {
            List<string> columns = new List<string>();
            string columnsQuery = $"PRAGMA table_info('{tableName}')";

            using (SQLiteCommand columnsCommand = new SQLiteCommand(columnsQuery, connection))
            {
                using (SQLiteDataReader columnsReader = columnsCommand.ExecuteReader())
                {
                    while (columnsReader.Read())
                    {
                        string columnName = columnsReader["name"].ToString();
                        columns.Add(columnName);
                    }
                }
            }
            return columns;
        }

        private void LoadReportTemplates()
        {
            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Получаем список шаблонов отчетов из Orders_List
                    string query = "SELECT Name, Filepath FROM Orders_List";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _reportTemplateComboBox.Items.Add(new TemplateItem
                                {
                                    Name = reader["Name"].ToString(),
                                    FilePath = reader["Filepath"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка шаблонов: {ex.Message}");
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            // 1. Get selected report template
            TemplateItem selectedTemplate = _reportTemplateComboBox.SelectedItem as TemplateItem;

            if (selectedTemplate == null)
            {
                MessageBox.Show("Пожалуйста, выберите шаблон отчета.");
                return;
            }

            // 2. Load report template from XML
            ReportTemplate reportTemplate = LoadReportTemplateFromXml(selectedTemplate.FilePath);

            // 3. Build SQL query based on selected filter type
            string sqlQuery = BuildSqlQuery(reportTemplate);

            // 4. Generate Excel report
            GenerateExcelReport(reportTemplate, sqlQuery);
        }

        private ReportTemplate LoadReportTemplateFromXml(string filePath)
        {
            ReportTemplate template = new ReportTemplate();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            // Load grid settings
            XmlNode gridSettingsNode = doc.SelectSingleNode("//GridSettings");
            template.Rows = int.Parse(gridSettingsNode.Attributes["Rows"].Value);
            template.Columns = int.Parse(gridSettingsNode.Attributes["Columns"].Value);

            // Load cell values
            template.CellValues = new Dictionary<string, string>();
            XmlNodeList cellNodes = doc.SelectNodes("//Cells/Cell");
            foreach (XmlNode cellNode in cellNodes)
            {
                string row = cellNode.Attributes["Row"].Value;
                string column = cellNode.Attributes["Column"].Value;
                string cellKey = $"{row},{column}";
                template.CellValues[cellKey] = cellNode.InnerText;
            }

            // Load formula
            XmlNode formulaNode = doc.SelectSingleNode("//Formula");
            template.Formula = formulaNode?.InnerText;

            // Load TableName
            XmlNode tableNameNode = doc.SelectSingleNode("//TableName");
            template.TableName = tableNameNode?.InnerText;

            return template;
        }

        private string BuildSqlQuery(ReportTemplate reportTemplate)
        {
            string baseQuery = "SELECT * FROM ";
            string tableName = reportTemplate.TableName; // Take the name from xml

            string sqlQuery = baseQuery + tableName;

            // TODO: Add filter conditions based on selected filter type
            switch (_filterTypeComboBox.SelectedIndex)
            {
                case 1: // Filter by name
                    if (_columnTreeView1.SelectedNode != null)
                    {
                        string columnName = _columnTreeView1.SelectedNode.Tag.ToString();
                        string filterValue = _filterTextBox.Text;
                        sqlQuery += $" WHERE {columnName} LIKE '%{filterValue}%'";
                    }
                    break;

                case 2: // Filter by period
                    if (_columnTreeView1.SelectedNode != null)
                    {
                        string columnName = _columnTreeView1.SelectedNode.Tag.ToString();
                        DateTime startDate = _datePicker1.Value;
                        DateTime endDate = _datePicker2.Value;
                        sqlQuery += $" WHERE {columnName} BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}'";
                    }
                    break;

                case 3: // Filter by name and period
                        // TODO: implement
                    break;
            }

            return sqlQuery;
        }

        private void GenerateExcelReport(ReportTemplate reportTemplate, string sqlQuery)
        {
            // 1. Create excel package
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Report");

            // 2. Get Data From Template
            Dictionary<string, string> displayColumns = GetDisplayColumns(reportTemplate);

            // 3. Fill data from database
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_databaseFilePath};Version=3;"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // 4. Add header row
                            int colIndex = 1;
                            foreach (KeyValuePair<string, string> displayColumn in displayColumns)
                            {
                                string dbColumnName = displayColumn.Value;
                                worksheet.Cells[1, colIndex].Value = dbColumnName;
                                worksheet.Cells[1, colIndex].Style.Font.Bold = true;
                                colIndex++;
                            }

                            int row = 2; // Start from row 2 (after header)
                            while (reader.Read())
                            {
                                colIndex = 1;
                                foreach (KeyValuePair<string, string> displayColumn in displayColumns)
                                {
                                    string dbColumnName = displayColumn.Value;
                                    try
                                    {
                                        object columnValue = reader[dbColumnName];
                                        if (columnValue != null && columnValue != DBNull.Value)
                                        {
                                            worksheet.Cells[row, colIndex].Value = columnValue.ToString();
                                        }
                                        else
                                        {
                                            worksheet.Cells[row, colIndex].Value = ""; // Set empty value if DBNull
                                        }
                                    }
                                    catch (IndexOutOfRangeException ex)
                                    {
                                        worksheet.Cells[row, colIndex].Value = "Column Not Found"; //If colunm is out of range - add mesage
                                    }
                                    colIndex++;
                                }
                                row++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                return;
            }

            // 4. Save the excel file
            try
            {
                // 1. Get the path
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 2. Set the report name
                TemplateItem selectedTemplate = _reportTemplateComboBox.SelectedItem as TemplateItem;
                string reportName = selectedTemplate.Name;
                string filePath = Path.Combine(folderPath, $"{reportName}.xlsx");

                // 3. Try to save
                FileInfo excelFile = new FileInfo(filePath);
                excelPackage.SaveAs(excelFile);

                MessageBox.Show($"Отчет успешно создан: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении Excel файла: {ex.Message}");
            }
        }

        private Dictionary<string, string> GetDisplayColumns(ReportTemplate reportTemplate)
        {
            Dictionary<string, string> displayColumns = new Dictionary<string, string>();
            foreach (var cell in reportTemplate.CellValues)
            {
                // Check if cell.Value contains "TableName.ColumnName" format
                string cellValue = cell.Value;
                if (cellValue.Contains("."))
                {
                    string columnName = cellValue.Split('.')[1];
                    displayColumns[cell.Key] = columnName;
                }
                else
                {
                    displayColumns[cell.Key] = cellValue; // Use the whole value if no "."
                }
            }
            return displayColumns;
        }
    }

    //Helper Classes
    public class TemplateItem
    {
        public string Name { get; set; }
        public string FilePath { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ReportTemplate
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Dictionary<string, string> CellValues { get; set; }
        public string Formula { get; set; }

        public string TableName { get; set; }
    }

}
