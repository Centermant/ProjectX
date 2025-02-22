using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectX
{
    public static class VisualSettingsManager
    {
        private static string _databaseFilePath = "MyDatabase.db";

        public static void SaveVisualSettings(Font font, Color backColor, Color panelColor)
        {
            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Проверяем, существует ли таблица UserFonts
                    string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='UserFonts';";
                    using (SQLiteCommand checkTableCommand = new SQLiteCommand(checkTableQuery, connection))
                    {
                        object tableExists = checkTableCommand.ExecuteScalar();

                        // 3. Если таблица не существует, создаем ее
                        if (tableExists == null)
                        {
                            string createTableQuery = @"CREATE TABLE UserFonts (
                                FontName TEXT,
                                FontSize REAL,
                                FontStyle INTEGER,
                                BackColor TEXT,
                                PanelColor TEXT
                            );";
                            using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                            {
                                createTableCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // 4. Удаляем старые настройки (если они есть)
                    string deleteQuery = "DELETE FROM UserFonts;";
                    using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }

                    // 5. Сохраняем новые настройки
                    string insertQuery = @"INSERT INTO UserFonts (FontName, FontSize, FontStyle, BackColor, PanelColor)
                                    VALUES (@FontName, @FontSize, @FontStyle, @BackColor, @PanelColor);";

                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@FontName", font.Name);
                        insertCommand.Parameters.AddWithValue("@FontSize", font.Size);
                        insertCommand.Parameters.AddWithValue("@FontStyle", (int)font.Style);
                        insertCommand.Parameters.AddWithValue("@BackColor", ColorTranslator.ToHtml(backColor));
                        insertCommand.Parameters.AddWithValue("@PanelColor", ColorTranslator.ToHtml(panelColor));

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при сохранении визуальных настроек: {ex.Message}");
            }
        }

        public static void LoadVisualSettings(out Font font, out Color backColor, out Color panelColor)
        {
            font = new Font("Verdana", 12F);
            backColor = SystemColors.Control;
            panelColor = Color.LightGray;

            // 1. Создаем строку подключения к базе данных
            string connectionString = $"Data Source={_databaseFilePath};Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 2. Получаем настройки из таблицы UserFonts
                    string selectQuery = "SELECT * FROM UserFonts;";
                    using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fontName = reader["FontName"].ToString();
                                float fontSize = Convert.ToSingle(reader["FontSize"]);
                                FontStyle fontStyle = (FontStyle)Convert.ToInt32(reader["FontStyle"]);
                                string backColorHtml = reader["BackColor"].ToString();
                                string panelColorHtml = reader["PanelColor"].ToString();

                                font = new Font(fontName, fontSize, fontStyle);
                                backColor = ColorTranslator.FromHtml(backColorHtml);
                                panelColor = ColorTranslator.FromHtml(panelColorHtml);
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Ошибка при загрузке визуальных настроек: {ex.Message}");
            }
        }
    }
}