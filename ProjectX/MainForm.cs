using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public partial class MainForm : Form
    {
        public static Font DefaultFont = new Font("Verdana", 12F);
        public static Color DefaultBackColor = SystemColors.Control;
        public static Color DefaultPanelColor = Color.LightGray;

        private MenuStrip _menuStrip;
        private ToolStripMenuItem _fileMenuItem;
        private ToolStripMenuItem _visualSettingsMenuItem;
        private ToolStripMenuItem _closePanelMenuItem;
        private ToolStripMenuItem _exitMenuItem;

        private ToolStripMenuItem _tablesMenuItem;
        private ToolStripMenuItem _createDbMenuItem;
        private ToolStripMenuItem _addToTablesMenuItem;
        private ToolStripMenuItem _viewTablesMenuItem;

        private ToolStripMenuItem _reportsMenuItem;

        public Panel _mainPanel;
        private PanelSwitcher _panelSwitcher;
        public ActionType _currentAction; // Make public

        public MainForm()
        {
            // Загружаем визуальные настройки при создании формы
            VisualSettingsManager.LoadVisualSettings(out DefaultFont, out DefaultBackColor, out DefaultPanelColor);

            InitializeComponents();
            _panelSwitcher = new PanelSwitcher(_mainPanel);
            this.Text = "Report Generator";
            this.Size = new Size(800, 600);
        }

        private void InitializeComponents()
        {
            _menuStrip = new MenuStrip();
            _menuStrip.Font = DefaultFont;

            // Файл
            _fileMenuItem = new ToolStripMenuItem("Файл");
            _visualSettingsMenuItem = new ToolStripMenuItem("Визуальные настройки");
            _closePanelMenuItem = new ToolStripMenuItem("Закрыть панель");
            _exitMenuItem = new ToolStripMenuItem("Выход");

            _fileMenuItem.DropDownItems.Add(_visualSettingsMenuItem);
            _fileMenuItem.DropDownItems.Add(_closePanelMenuItem);
            _fileMenuItem.DropDownItems.Add(_exitMenuItem);

            // Таблицы
            _tablesMenuItem = new ToolStripMenuItem("Таблицы");
            _createDbMenuItem = new ToolStripMenuItem("Создать таблицу");
            _addToTablesMenuItem = new ToolStripMenuItem("Заполнить таблицу");
            _viewTablesMenuItem = new ToolStripMenuItem("Просмотр таблицы");

            _tablesMenuItem.DropDownItems.Add(_createDbMenuItem);
            _tablesMenuItem.DropDownItems.Add(_addToTablesMenuItem);
            _tablesMenuItem.DropDownItems.Add(_viewTablesMenuItem);

            // Отчеты
            _reportsMenuItem = new ToolStripMenuItem("Отчеты");

            _menuStrip.Items.Add(_fileMenuItem);
            _menuStrip.Items.Add(_tablesMenuItem);
            _menuStrip.Items.Add(_reportsMenuItem);

            _mainPanel = new Panel();
            _mainPanel.Dock = DockStyle.Fill;

            this.Controls.Add(_mainPanel);
            this.Controls.Add(_menuStrip);

            _menuStrip.Dock = DockStyle.Top;

            // Подписываемся на события
            _createDbMenuItem.Click += CreateDbMenuItem_Click;
            _addToTablesMenuItem.Click += DataEntryMenuItem_Click;
            _viewTablesMenuItem.Click += ViewTablesMenuItem_Click;
            _visualSettingsMenuItem.Click += VisualSettingsMenuItem_Click;
            _closePanelMenuItem.Click += ClosePanelMenuItem_Click;
            _exitMenuItem.Click += ExitMenuItem_Click;

            this.MainMenuStrip = _menuStrip;

            this.Font = DefaultFont;
            this.BackColor = DefaultBackColor;
        }

        private void DataEntryMenuItem_Click(object sender, EventArgs e)
        {
            _currentAction = ActionType.AddRecord;
            DataEntryPanel dataEntryPanel = new DataEntryPanel();
            _panelSwitcher.ShowControl(dataEntryPanel);
            TableSelectionPanel tableSelectionPanel = new TableSelectionPanel(dataEntryPanel);

            dataEntryPanel.Controls.Add(tableSelectionPanel);
        }

        private void ViewTablesMenuItem_Click(object sender, EventArgs e)
        {
            _currentAction = ActionType.ViewTable;
            DataEntryPanel dataEntryPanel = new DataEntryPanel();
            _panelSwitcher.ShowControl(dataEntryPanel);
            TableSelectionPanel tableSelectionPanel = new TableSelectionPanel(dataEntryPanel);

            dataEntryPanel.Controls.Add(tableSelectionPanel);
        }

        private void CreateDbMenuItem_Click(object sender, EventArgs e)
        {
            CreateDatabasePanel createDatabasePanel = new CreateDatabasePanel();
            _panelSwitcher.ShowControl(createDatabasePanel);
        }

        private void VisualSettingsMenuItem_Click(object sender, EventArgs e)
        {
            OpenVisualSettingsPanel();
        }

        public void OpenVisualSettingsPanel()
        {
            VisualSettingsPanel visualSettingsPanel = new VisualSettingsPanel(this);
            _panelSwitcher.ShowControl(visualSettingsPanel);
        }

        private void ClosePanelMenuItem_Click(object sender, EventArgs e)
        {
            _panelSwitcher.ClearPanel();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void UpdateFontRecursive(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Font = MainForm.DefaultFont;
                control.AutoSize = true;
                if (control.Controls.Count > 0)
                {
                    UpdateFontRecursive(control);
                }
            }
        }

        public void UpdateBackColorRecursive(Control parent)
        {
            parent.BackColor = DefaultBackColor;
            foreach (Control control in parent.Controls)
            {
                UpdateBackColorRecursive(control);
            }
        }

        public void UpdatePanelColorRecursive(Control parent)
        {
            if (parent is Panel)
            {
                parent.BackColor = DefaultPanelColor;
            }

            foreach (Control control in parent.Controls)
            {
                UpdatePanelColorRecursive(control);
            }
        }
    }
}