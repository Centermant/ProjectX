using System.Windows.Forms;

namespace ProjectX
{
    public class PanelSwitcher
    {
        private Panel _mainPanel; // Панель, на которой будут отображаться элементы управления.
        public Control _currentControl; // Текущий отображаемый элемент управления.

        public PanelSwitcher(Panel mainPanel)
        {
            _mainPanel = mainPanel;
            _mainPanel.Dock = DockStyle.Fill; //  Важно, чтобы панель занимала все доступное пространство.
            _currentControl = null;
        }

        public void ShowControl(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (_currentControl != null)
            {
                _mainPanel.Controls.Remove(_currentControl);
                _currentControl.Dispose();
            }

            control.Dock = DockStyle.Fill;
            _mainPanel.Controls.Add(control);
            _currentControl = control;
            _mainPanel.Refresh();
        }

        // Вспомогательный метод для очистки панели
        public Control CurrentControl
        {
            get { return _currentControl; }
        }

        // Вспомогательный метод для очистки панели
        public void ClearPanel()
        {
            if (_currentControl != null)
            {
                _mainPanel.Controls.Remove(_currentControl);
                _currentControl.Dispose();
                _currentControl = null;
                _mainPanel.Refresh();
            }
        }
    }
}