using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace ProjectX
{
    public partial class ReportEditorPanel : Panel
    {
        private DataGridView _reportGrid;
        private TreeView _databaseStructureTree;
        private TextBox _formulaTextBox;
        private Button _previewButton;
        private Label _tablesLabel;
        private Label _formulaLabel;
        private string _databaseFilePath = "MyDatabase.db";

        private TextBox _rowsTextBox;
        private TextBox _columnsTextBox;
        private Button _resizeButton;
        private Button _saveButton;

        private TableLayoutPanel _topPanel; // Use TableLayoutPanel
        private SplitContainer _splitContainer;
        private TableLayoutPanel _leftPanel;
        private TableLayoutPanel _rightPanel;
        private TextBox _reportNameTextBox; //Add
        private Label _reportNameLabel; //Add

        public ReportEditorPanel()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 1. DataGridView для отображения макета отчета
            _reportGrid = new DataGridView();
            _reportGrid.Dock = DockStyle.Fill;
            _reportGrid.AllowDrop = true;
            _reportGrid.DragEnter += ReportGrid_DragEnter;
            _reportGrid.DragDrop += ReportGrid_DragDrop;
            _reportGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _reportGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _reportGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // 2. TreeView для отображения структуры базы данных
            _databaseStructureTree = new TreeView();
            _databaseStructureTree.Dock = DockStyle.Fill;
            _databaseStructureTree.ItemDrag += DatabaseStructureTree_ItemDrag;
            _databaseStructureTree.AllowDrop = true;
            _databaseStructureTree.ShowNodeToolTips = true;
            this._databaseStructureTree.ImageList = new ImageList();
            _databaseStructureTree.ImageList.Images.Add(new Icon(SystemIcons.WinLogo, new Size(16, 16)));
            LoadDatabaseStructure();

            // 3. TextBox для ввода формул
            _formulaTextBox = new TextBox();
            _formulaTextBox.Dock = DockStyle.Fill;
            _formulaTextBox.Multiline = true;

            // 4. Кнопка "Предварительный просмотр"
            _previewButton = new Button();
            _previewButton.Text = "Предварительный просмотр";
            _previewButton.Dock = DockStyle.Fill;
            _previewButton.Click += PreviewButton_Click;
            _previewButton.AutoSize = true;

            // 4.a Кнопка "Сохранить"
            _saveButton = new Button();
            _saveButton.Text = "Сохранить";
            _saveButton.Dock = DockStyle.Fill;
            _saveButton.Click += SaveButton_Click;
            _saveButton.AutoSize = true;

            // Labels
            _tablesLabel = new Label();
            _tablesLabel.Text = "Таблицы";
            _tablesLabel.Dock = DockStyle.Fill;
            _tablesLabel.TextAlign = ContentAlignment.MiddleCenter;
            _tablesLabel.Font = MainForm.DefaultFont;

            _formulaLabel = new Label();
            _formulaLabel.Text = "Формула";
            _formulaLabel.Dock = DockStyle.Fill;
            _formulaLabel.TextAlign = ContentAlignment.MiddleCenter;
            _formulaLabel.Font = MainForm.DefaultFont;

            // 6a  "Наименование отчета"
            _reportNameLabel = new Label();
            _reportNameLabel.Text = "Наименование отчета";
            _reportNameLabel.Dock = DockStyle.Fill;
            _reportNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            _reportNameLabel.Font = MainForm.DefaultFont;
            _reportNameLabel.AutoSize = true; // Set AutoSize to true
            _reportNameLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            // 6b
            _reportNameTextBox = new TextBox();
            _reportNameTextBox.Width = 100; // Double the width
            _reportNameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            //Rows and Columns
            _rowsTextBox = new TextBox();
            _rowsTextBox.Width = 50;
            _rowsTextBox.Text = "10"; // Default value
            _rowsTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            //Columns TextBox
            _columnsTextBox = new TextBox();
            _columnsTextBox.Width = 50;
            _columnsTextBox.Text = "10"; // Default value
            _columnsTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            //Resize Button
            _resizeButton = new Button();
            _resizeButton.Text = "Изменить";
            _resizeButton.Click += ResizeButton_Click;
            _resizeButton.AutoSize = true;
            _resizeButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            // 6. Top Panel with TableLayoutPanel
            _topPanel = new TableLayoutPanel();
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 50;
            _topPanel.ColumnCount = 7;
            _topPanel.RowCount = 1;

            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _topPanel.Controls.Add(_reportNameLabel, 0, 0);
            _topPanel.Controls.Add(_reportNameTextBox, 1, 0);
            _topPanel.Controls.Add(new Label { Text = "Строки:" }, 2, 0);
            _topPanel.Controls.Add(_rowsTextBox, 3, 0);
            _topPanel.Controls.Add(new Label { Text = "Столбцы:" }, 4, 0);
            _topPanel.Controls.Add(_columnsTextBox, 5, 0);
            _topPanel.Controls.Add(_resizeButton, 6, 0);

            // 5. SplitContainer для разделения DataGridView и TreeView
            _splitContainer = new SplitContainer();
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Orientation = Orientation.Horizontal;

            //Left and Right Panels
            _leftPanel = new TableLayoutPanel();
            _leftPanel.Dock = DockStyle.Fill;
            _leftPanel.RowCount = 2;
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _leftPanel.Controls.Add(_tablesLabel, 0, 0);
            _leftPanel.Controls.Add(_databaseStructureTree, 0, 1);

            _rightPanel = new TableLayoutPanel();
            _rightPanel.Dock = DockStyle.Fill;
            _rightPanel.RowCount = 4; //add SaveButton
            _rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); //add SaveButton
            _rightPanel.Controls.Add(_formulaLabel, 0, 0);
            _rightPanel.Controls.Add(_formulaTextBox, 0, 1);
            _rightPanel.Controls.Add(_previewButton, 0, 2);
            _rightPanel.Controls.Add(_saveButton, 0, 3); //add SaveButton

            // Set up Panel2 of SplitContainer
            TableLayoutPanel panel2Layout = new TableLayoutPanel { Dock = DockStyle.Fill };
            panel2Layout.ColumnCount = 2;
            panel2Layout.RowCount = 1;
            panel2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panel2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panel2Layout.Controls.Add(_leftPanel, 0, 0);
            panel2Layout.Controls.Add(_rightPanel, 1, 0);

            // Main Layout to put everything together.
            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(_topPanel, 0, 0);
            mainLayout.Controls.Add(_splitContainer, 0, 1);

            // Set up Panel1 of SplitContainer
            _splitContainer.Panel1.Controls.Add(_reportGrid);

            _splitContainer.Panel2.Controls.Add(panel2Layout);

            // Set SplitterDistance (25% from the bottom)
            _splitContainer.SplitterDistance = this.Height - (int)(this.Height * 0.25);

            // Add MainLayout to the panel
            Controls.Add(mainLayout);

            this.Font = MainForm.DefaultFont;
            this.Dock = DockStyle.Fill;

            // Load Tables And Columns
            LoadDatabaseStructure();

            CheckOrdersListTableExists();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            // Устанавливаем SplitterDistance после того, как панель получит размеры
            _splitContainer.SplitterDistance = this.Height - (int)(this.Height * 0.25);
        }

        private void LoadDatabaseStructure()
        {
            // Очищаем TreeView перед загрузкой новой структуры
            _databaseStructureTree.Nodes.Clear();

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
                                _databaseStructureTree.Nodes.Add(tableNode);

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

        private void DatabaseStructureTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Обработка начала перетаскивания
            TreeNode node = (TreeNode)e.Item;
            if (node.Tag != null)
            {
                DoDragDrop(node.Tag.ToString(), DragDropEffects.Copy); // Передаем имя таблицы.имя столбца
            }
        }

        private void ReportGrid_DragEnter(object sender, DragEventArgs e)
        {
            // Разрешаем перетаскивание
            e.Effect = DragDropEffects.Copy;
        }

        private void ReportGrid_DragDrop(object sender, DragEventArgs e)
        {
            // Обработка сбрасывания
            Point clientPoint = _reportGrid.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = _reportGrid.HitTest(clientPoint.X, clientPoint.Y);

            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                string data = e.Data.GetData(DataFormats.StringFormat) as string;
                _reportGrid.Rows[hit.RowIndex].Cells[hit.ColumnIndex].Value = data; // Устанавливаем значение в ячейку
            }
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            // TODO: Реализуем предварительный просмотр отчета
            MessageBox.Show("Предварительный просмотр отчета");
        }

        private void ResizeButton_Click(object sender, EventArgs e)
        {
            // 1. Get the number of rows and columns
            if (int.TryParse(_rowsTextBox.Text, out int rows) && int.TryParse(_columnsTextBox.Text, out int columns))
            {
                // 2. Resize the DataGridView
                ResizeDataGridView(rows, columns);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для строк и столбцов.");
            }
        }

        private void ResizeDataGridView(int rows, int columns)
        {
            // 1. Clear existing columns and rows
            _reportGrid.Columns.Clear();
            _reportGrid.Rows.Clear();

            // 2. Add new columns
            for (int i = 0; i < columns; i++)
            {
                _reportGrid.Columns.Add(new DataGridViewTextBoxColumn());
            }

            // 3. Add new rows
            for (int i = 0; i < rows; i++)
            {
                _reportGrid.Rows.Add();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveReportTemplate();
        }

        private void SaveReportTemplate()
        {
            // 1. Get Report Name
            string reportName = _reportNameTextBox.Text;
            if (string.IsNullOrEmpty(reportName))
            {
                MessageBox.Show("Пожалуйста, введите наименование отчета.");
                return;
            }

            // 2. Get Table Name
            string tableName = "Orders_List";
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Пожалуйста, введите наименование таблицы.");
                return;
            }

            // 2. Создаем XML-документ
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("ReportTemplate");
            doc.AppendChild(root);

            // 3. Сохраняем размеры DataGridView
            XmlElement gridSettings = doc.CreateElement("GridSettings");
            gridSettings.SetAttribute("Rows", _rowsTextBox.Text);
            gridSettings.SetAttribute("Columns", _columnsTextBox.Text);
            root.AppendChild(gridSettings);

            // 4. Сохраняем содержимое ячеек DataGridView
            XmlElement cells = doc.CreateElement("Cells");
            root.AppendChild(cells);
            for (int row = 0; row < _reportGrid.Rows.Count; row++)
            {
                for (int col = 0; col < _reportGrid.Columns.Count; col++)
                {
                    DataGridViewCell cell = _reportGrid.Rows[row].Cells[col];
                    if (cell.Value != null)
                    {
                        XmlElement cellElement = doc.CreateElement("Cell");
                        cellElement.SetAttribute("Row", row.ToString());
                        cellElement.SetAttribute("Column", col.ToString());
                        cellElement.InnerText = cell.Value.ToString();
                        cells.AppendChild(cellElement);
                    }
                }
            }

            // 5. Сохраняем формулу
            XmlElement formulaElement = doc.CreateElement("Formula");
            formulaElement.InnerText = _formulaTextBox.Text;
            root.AppendChild(formulaElement);

            // 6. Сохраняем имя таблицы
            XmlElement tableNameElement = doc.CreateElement("TableName");
            tableNameElement.InnerText = tableName; // Сохраняем имя таблицы
            root.AppendChild(tableNameElement);

            // 7. Сохраняем XML-файл в папку Orders
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Orders");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{reportName.Replace(" ", "_")}.xml"; // Replace spaces for file name
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                doc.Save(filePath);
                // Сохраняем информацию о шаблоне в базе данных
                SaveReportInfoToDatabase(reportName, filePath);
                MessageBox.Show("Шаблон отчета сохранен успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении шаблона: {ex.Message}");
            }
        }

        private void SaveReportInfoToDatabase(string reportName, string filePath)
        {
            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Создаем запрос на вставку данных
                    string insertQuery = "INSERT INTO Orders_List (name, filepath) VALUES (@name, @filepath)";
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@name", reportName);
                        insertCommand.Parameters.AddWithValue("@filepath", filePath);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при сохранении информации о шаблоне в базе данных: {ex.Message}");
            }
        }

        private void CheckOrdersListTableExists()
        {
            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Проверяем, существует ли таблица Orders_List
                    string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Orders_List';";
                    using (SQLiteCommand checkTableCommand = new SQLiteCommand(checkTableQuery, connection))
                    {
                        object tableExists = checkTableCommand.ExecuteScalar();

                        // 3. Если таблица не существует, создаем ее
                        if (tableExists == null)
                        {
                            string createTableQuery = @"CREATE TABLE Orders_List (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT,
                                Filepath TEXT
                            );";
                            using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                            {
                                createTableCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при проверке/создании таблицы Orders_List: {ex.Message}");
            }
        }
    }
}