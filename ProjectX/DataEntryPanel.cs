using System;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class DataEntryPanel : Panel
    {
        private TableSelectionPanel _tableSelectionPanel;
        private AddRecordPanel _addRecordPanel;
        private ViewDataTablePanel _viewDataTablePanel;

        public DataEntryPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
        }

        public void ShowTableSelectionPanel()
        {
            // 1. Удаляем текущую панель, если она есть
            Controls.Clear();

            // 2. Создаем панель выбора таблицы
            _tableSelectionPanel = new TableSelectionPanel(this);

            // 3. Добавляем панель на DataEntryPanel
            Controls.Add(_tableSelectionPanel);
        }

        public void ShowAddRecordPanel(string tableName)
        {
            // 1. Удаляем текущую панель, если она есть
            Controls.Clear();

            // 2. Создаем панель добавления записи
            _addRecordPanel = new AddRecordPanel(tableName, this);

            // 3. Добавляем панель на DataEntryPanel
            Controls.Add(_addRecordPanel);
        }

        public void ShowViewDataTablePanel(string tableName)
        {
            // 1. Удаляем текущую панель, если она есть
            Controls.Clear();

            // 2. Создаем панель просмотра таблицы
            _viewDataTablePanel = new ViewDataTablePanel(tableName);

            // 3. Добавляем панель на DataEntryPanel
            Controls.Add(_viewDataTablePanel);
        }
    }
}