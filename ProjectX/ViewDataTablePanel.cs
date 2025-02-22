using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class ViewDataTablePanel : Panel
    {
        private DataGridView _dataGridView;
        private string _databaseFilePath = "MyDatabase.db";
        private string _tableName;

        public ViewDataTablePanel(string tableName)
        {
            _tableName = tableName;
            InitializeComponents();
            LoadTableData(tableName);
        }

        private void InitializeComponents()
        {
            _dataGridView = new DataGridView();
            _dataGridView.Dock = DockStyle.Fill;
            _dataGridView.AllowUserToAddRows = false;
            _dataGridView.AllowUserToDeleteRows = false;
            _dataGridView.AllowUserToOrderColumns = true;
            _dataGridView.ReadOnly = true;
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Настройка стиля заголовка столбцов
            _dataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            this.Controls.Add(_dataGridView);
            this.Dock = DockStyle.Fill;
        }

        private void LoadTableData(string tableName)
        {
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM `{tableName}`";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            _dataGridView.DataSource = dataTable;
                            _dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из таблицы '{tableName}': {ex.Message}");
            }
        }
    }
}