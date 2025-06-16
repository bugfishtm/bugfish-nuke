using System.Data.SQLite;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Security.Policy;
using System.Media;
using System.Security.Principal;
using System.Diagnostics;
using bugfish_nuke.Library;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using System.Numerics;

namespace bugfish_nuke
{
    public partial class Interface : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private int borderWidth = 10; // Set the width of the border
        private Color borderColor = Color.FromArgb(0x00, 0x00, 0x00); // Set the color of the border
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private Sqlite sqlite;
        private Point offset;
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private Task CurrentTask;
        private bool canceled;
        SoundPlayer player;
        int overwrite_passes = 1;
        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        public Interface()
        {
            // Ensure Admin Permission
            EnsureRunAsAdmin();

            // Initialize Component
            InitializeComponent();

            // Disable Load Panel
            Panel_Load.Visible = false;
            Panel_Cred.Visible = false;


            // Set Form Title
            this.Text = "Bugfish-Nuke";

            // Initialize SQLite
            sqlite = new Sqlite("data.db");
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Shortcode TEXT NOT NULL UNIQUE, IsActive INTEGER NOT NULL);");
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Folders (Id INTEGER PRIMARY KEY AUTOINCREMENT, Path TEXT NOT NULL, IsActive INTEGER NOT NULL);");
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Cmds (Id INTEGER PRIMARY KEY AUTOINCREMENT, Path TEXT NOT NULL, IsActive INTEGER NOT NULL);");
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Settings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Value TEXT NOT NULL, Shortcode TEXT NOT NULL UNIQUE, IsActive INTEGER NOT NULL);");

            // Fix Interface
            interface_init_frame_btn();

            // Soundplayer
            player = new SoundPlayer(new MemoryStream(Properties.Resources.xx));
            var dt = sqlite.GetDataTable(
                "SELECT Value FROM Settings WHERE Shortcode = @shortcode",
                new SQLiteParameter("@shortcode", "music_path")
            );
            if (dt.Rows.Count > 0)
            {
                string dbPath = dt.Rows[0]["Value"].ToString();
                if (File.Exists(dbPath)) { string path = dbPath; player = new SoundPlayer(path); }
            }

            // Setup Window Icon
            this.Icon = new Icon(new MemoryStream(Properties.Resources.nukeicon));

            // Setup Window Fix Timer
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            // Check for Table Status
            dt = sqlite.GetDataTable("SELECT COUNT(*) as cnt FROM Items");
            int count = Convert.ToInt32(dt.Rows[0]["cnt"]);

            // Pre Configured Database Items
            var initialItems = new[]
            {
                new { Name = "[Software] Brave: Close and Delete User Data", Shortcode = "brave" },
                new { Name = "[Software] Cyberduck: Close and Delete User Data", Shortcode = "cyberduck" },
                new { Name = "[Software] Discord: Close and Delete User Data", Shortcode = "discord" },
                new { Name = "[Software] Dropbox: Close and Delete User Data", Shortcode = "dropbox" },
                new { Name = "[Software] Edge: Close and Delete User Data", Shortcode = "edge" },
                new { Name = "[Software] FileZilla: Close and Delete User Data", Shortcode = "filezilla" },
                new { Name = "[Software] Google Chrome: Close and Delete User Data", Shortcode = "chrome" },
                new { Name = "[Software] ICQ: Close and Delete User Data", Shortcode = "icq" },
                new { Name = "[Software] KeePass: Close and Delete User Data", Shortcode = "keepass" },
                new { Name = "[Software] Microsoft Outlook: Close and Delete User Data", Shortcode = "outlook" },
                new { Name = "[Software] Microsoft Teams: Close and Delete User Data", Shortcode = "teams" },
                new { Name = "[Software] Mozilla Firefox: Close and Delete User Data", Shortcode = "firefox" },
                new { Name = "[Software] Mozilla Thunderbird: Close and Delete User Data", Shortcode = "thunderbird" },
                new { Name = "[Software] Native Access: Close and Delete User Data", Shortcode = "nativeaccess" },
                new { Name = "[Software] Nextcloud: Close and Delete User Data", Shortcode = "nextcloud" },
                new { Name = "[Software] OneDrive: Close and Delete User Data", Shortcode = "onedrive" },
                new { Name = "[Software] OBS: Close and Delete User Data", Shortcode = "obs" },
                new { Name = "[Software] Opera: Close and Delete User Data", Shortcode = "opera" },
                new { Name = "[Software] Opera GX: Close and Delete User Data", Shortcode = "operagx" },
                new { Name = "[Software] Signal: Close and Delete User Data", Shortcode = "signal" },
                new { Name = "[Software] Skype: Close and Delete User Data", Shortcode = "skype" },
                new { Name = "[Software] Slack: Close and Delete User Data", Shortcode = "slack" },
                new { Name = "[Software] Steam: Close and Delete User Data", Shortcode = "steam" },
                new { Name = "[Software] Telegram: Close and Delete User Data", Shortcode = "telegram" },
                new { Name = "[Software] Tor Browser: Close and Delete User Data", Shortcode = "tor" },
                new { Name = "[Software] Unity Hub: Close and Delete User Data", Shortcode = "unityhub" },
                new { Name = "[Software] VeraCrypt: Close and Delete User Data", Shortcode = "veracrypt" },
                new { Name = "[Software] Viber: Close and Delete User Data", Shortcode = "viber" },
                new { Name = "[Software] Vivaldi: Close and Delete User Data", Shortcode = "vivaldi" },
                new { Name = "[Software] WhatsApp: Close and Delete User Data", Shortcode = "whatsapp" },
                new { Name = "[Software] WinSCP: Close and Delete User Data", Shortcode = "winscp" },
                new { Name = "[Software] Zoom: Close and Delete User Data", Shortcode = "zoom" },
                new { Name = "[Windows] Authenticator: Close and Delete Data", Shortcode = "winauth" },
                new { Name = "[Windows] BitLocker Recovery Keys: Delete", Shortcode = "securekey-bitlocker" },
                new { Name = "[Windows] Clipboard: Clear", Shortcode = "windows_clipboard" },
                new { Name = "[Windows] DirectX Shader Cache: Clear", Shortcode = "windows_directx_shader_cache" },
                new { Name = "[Windows] Credential Manager: Delete all generic Keys", Shortcode = "clear_all_creds" },
                new { Name = "[Windows] DNS Cache: Clear", Shortcode = "windows_dns_cache" },
                new { Name = "[Windows] Driver Install Logs: Clear", Shortcode = "windows_driver_install_logs" },
                new { Name = "[Windows] Explorer: Clear Most Recently Used List", Shortcode = "windows_explorer_mru" },
                new { Name = "[Windows] Explorer: Clear Thumbnail Cache", Shortcode = "windows_thumbnail_cache" },
                new { Name = "[Windows] Event Logs: Clear", Shortcode = "windows_event_logs" },
                new { Name = "[Windows] Error Reporting: Clear", Shortcode = "windows_error_reporting" },
                new { Name = "[Windows] Event Trace Logs: Clear", Shortcode = "windows_event_trace_logs" },
                new { Name = "[Windows] Machine-Level Crypto-Keys: Delete", Shortcode = "securekey-machine" },
                new { Name = "[Windows] Font Cache: Clear", Shortcode = "windows_font_cache" },
                new { Name = "[Windows] Log Files: Clear", Shortcode = "windows_log_files" },
                new { Name = "[Windows] Memory Dumps: Clear", Shortcode = "windows_memory_dumps" },
                new { Name = "[Windows] Office: Recent File History", Shortcode = "office-recent-files" },
                new { Name = "[Windows] Prefetch Data: Clear", Shortcode = "windows_prefetch" },
                new { Name = "[Windows] System Restore Points: Clear", Shortcode = "windows_system_restore_cleanup" },
                new { Name = "[Windows] Recent Documents List: Clear", Shortcode = "windows_recent_documents" },
                new { Name = "[Windows] Recent File List: Clear", Shortcode = "windows_recent" },
                new { Name = "[Windows] Trash Bin: Clear", Shortcode = "windows_trash" },
                new { Name = "[Windows] User SSH Key Folder: Delete", Shortcode = "securekey-ssh" },
                new { Name = "[Windows] User EFS Keys: Delete", Shortcode = "securekey-efs-user" },
                new { Name = "[Windows] Update Cache: Clear", Shortcode = "windows_update_cleanup" },
                new { Name = "[Windows] User Assist History: Clear", Shortcode = "windows_userassist" },
                new { Name = "[Windows] Web Cache: Clear", Shortcode = "windows_webcache" },
                new { Name = "[Windows] Visual Studio 2022: Clear Settings and History", Shortcode = "empty_vs2022_settings" },
                new { Name = "[Windows] WebDav Cache: Clear", Shortcode = "windows_webdav_cache" },
            }
            .OrderBy(x => x.Name) // Optional: ensures the list is sorted by Name
            .ToArray();

            // Restore Lost Database Items
            foreach (var item in initialItems)
            {
                var dtCheck = sqlite.GetDataTable(
                    "SELECT COUNT(*) as cnt FROM Items WHERE Shortcode = @shortcode",
                    new SQLiteParameter("@shortcode", item.Shortcode)
                );
                int exists = Convert.ToInt32(dtCheck.Rows[0]["cnt"]);
                if (exists == 0)
                {
                    sqlite.InsertData(
                        "INSERT INTO Items (Name, Shortcode, IsActive) VALUES (@name, @shortcode, @active)",
                        new SQLiteParameter("@name", item.Name),
                        new SQLiteParameter("@shortcode", item.Shortcode),
                        new SQLiteParameter("@active", "0")
                    );
                }
            }

            // Subscribe to the Paint event
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderWidth);
            this.Padding = new Padding(5);
            this.Paint += new PaintEventHandler(Interface_Paint);
            //this.Resize += Interface_Resize;

            LoadItems();
            listBoxActive.DoubleClick += listBoxActive_DoubleClick;
            listBoxInactive.DoubleClick += listBoxInactive_DoubleClick;
        }

        // Check for Admin Permissions
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // Ensure Admin Permissions
        public static void EnsureRunAsAdmin()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Administrator permissions are required to continue.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // Close the current instance
            }
        }

        // Reload List Items
        private void LoadItems()
        {
            listBoxActive.Items.Clear();
            listBoxInactive.Items.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();

            var dt = sqlite.GetDataTable("SELECT Id, Name, IsActive FROM Items");
            foreach (DataRow row in dt.Rows)
            {
                var item = new ListBoxItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()
                };

                if (Convert.ToInt32(row["IsActive"]) == 1)
                {
                    listBoxActive.Items.Add(item);
                    listBox2.Items.Add(item);
                }
                else
                {
                    listBoxInactive.Items.Add(item);
                }
            }

            dt = sqlite.GetDataTable("SELECT Id, Path FROM Folders");
            foreach (DataRow row in dt.Rows)
            {
                var item = new ListBoxItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Path"].ToString()
                };
                listBox1.Items.Add(item);
                listBox2.Items.Add("Delete: " + item);
            }

            dt = sqlite.GetDataTable("SELECT Id, Path FROM Cmds");
            foreach (DataRow row in dt.Rows)
            {
                var item = new ListBoxItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Path"].ToString()
                };
                listBox3.Items.Add(item);
                listBox2.Items.Add("Execute: " + item);
            }
        }

        // Set Active to inactive
        private void listBoxActive_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxActive.SelectedItem is ListBoxItem item)
            {
                sqlite.UpdateData("UPDATE Items SET IsActive = 0 WHERE Id = @id",
                    new SQLiteParameter("@id", item.Id));
                LoadItems();
            }
        }

        // Delete a Path
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedPath = listBox1.SelectedItem.ToString();

                // Delete from the database
                sqlite.InsertData(
                    "DELETE FROM Folders WHERE Path = @path",
                    new SQLiteParameter("@path", selectedPath)
                );

                // Remove from ListBox
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }


        // Delete a Path
        private void listBox3_DoubleClick(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
            {
                string selectedPath = listBox3.SelectedItem.ToString();

                // Delete from the database
                sqlite.InsertData(
                    "DELETE FROM Cmds WHERE Path = @path",
                    new SQLiteParameter("@path", selectedPath)
                );

                // Remove from ListBox
                listBox3.Items.Remove(listBox3.SelectedItem);
            }
        }

        // Set Entry to Active
        private void listBoxInactive_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxInactive.SelectedItem is ListBoxItem item)
            {
                sqlite.UpdateData("UPDATE Items SET IsActive = 1 WHERE Id = @id",
                    new SQLiteParameter("@id", item.Id));
                LoadItems();
            }
        }

        // List Box Item Array
        public class ListBoxItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        // Interface Paint Functionality
        private void Interface_Paint(object sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }

        // Interface Resize Functionality
        private void Interface_Resize(object sender, EventArgs e)
        {
            // Update button locations on resize
            btnMinimize.Location = new Point(this.Width - 95, 5);
            btnMaximize.Location = new Point(this.Width - 65, 5);
            btnClose.Location = new Point(this.Width - 35, 5);
        }

        // Minimize Button Click to Minimize Current Form
        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Close Window to Tray or Close Completely
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
            //this.Hide();
            //notifyIcon.Visible = true;
        }

        // Maximize Button Click Functionality
        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                Rectangle workingArea = Screen.GetWorkingArea(this);
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;
                this.Location = new Point(Math.Max(this.Location.X, workingArea.X),
                          Math.Max(this.Location.Y, workingArea.Y));
            }
        }

        // Drag Window by Holding on Header
        private void Header_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // Maximize and Minize Windows with Double Click
        private void Header_Panel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Normal)
                    this.WindowState = FormWindowState.Maximized;
                else if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;
            }
            interface_reinit_frame_btn();
        }

        // NotifyIcon DoubleClick
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show(); this.WindowState = FormWindowState.Normal; notifyIcon.Visible = true;
        }

        // Initialize Border and Buttons
        private void interface_init_frame_btn()
        {
            // Minimize Button
            btnMinimize = new System.Windows.Forms.Button
            {
                Text = "_",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 95, 5),
                BackColor = Color.FromArgb(0x24, 0x24, 0x24),
                FlatStyle = FlatStyle.Flat
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += BtnMinimize_Click;
            btnMinimize.ForeColor = Color.FromArgb(0xFE, 0xDD, 0x56);
            tooltip_frame.SetToolTip(btnMinimize, "Minimize");

            // Maximize Button
            btnMaximize = new System.Windows.Forms.Button
            {
                Text = "O",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 65, 5),
                BackColor = Color.FromArgb(0x24, 0x24, 0x24),
                FlatStyle = FlatStyle.Flat
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += BtnMaximize_Click;
            btnMaximize.ForeColor = Color.FromArgb(0xFE, 0xDD, 0x56);
            tooltip_frame.SetToolTip(btnMaximize, "Maximize");

            // Close Button
            btnClose = new System.Windows.Forms.Button
            {
                Text = "X",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 35, 5),
                BackColor = Color.FromArgb(0x24, 0x24, 0x24),
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;
            btnClose.ForeColor = Color.FromArgb(0xFE, 0xDD, 0x56);
            tooltip_frame.SetToolTip(btnClose, "Close");

            // Add buttons to the form
            this.Controls.Add(btnMinimize);
            this.Controls.Add(btnMaximize);
            this.Controls.Add(btnClose);

            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();

        }

        // ReInitialize Border and Buttons
        private void interface_reinit_frame_btn()
        {
            btnClose.Location = new Point(this.Width - 35, 5);
            btnMaximize.Location = new Point(this.Width - 65, 5);
            btnMinimize.Location = new Point(this.Width - 95, 5);
            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();

            int imageWidth = this.Width;
            int listBoxWidth = imageWidth / 3;

            panel4.Width = listBoxWidth - 7;
            panel3.Width = listBoxWidth - 7;
            panel2.Width = listBoxWidth - 7;

            panel4.Left = 0;
            panel3.Left = panel4.Right;
            panel2.Left = panel3.Right;
        }
        // Allow for resizing by overriding WndProc
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_GETMINMAXINFO = 0x24;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);

                    Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                    if (pos.X < borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPRIGHT;
                    }
                    else if (pos.X < borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                    }
                    else if (pos.X < borderWidth)
                    {
                        m.Result = (IntPtr)HTLEFT;
                    }
                    else if (pos.X > Width - borderWidth)
                    {
                        m.Result = (IntPtr)HTRIGHT;
                    }
                    else if (pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOP;
                    }
                    else if (pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOM;
                    }
                    else
                    {
                        m.Result = (IntPtr)HTCLIENT;
                    }
                    interface_reinit_frame_btn();
                    return;

                case WM_GETMINMAXINFO:
                    MINMAXINFO minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
                    minMaxInfo.ptMinTrackSize.X = 1200; // Minimum width
                    minMaxInfo.ptMinTrackSize.Y = 700; // Minimum height
                    Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
                    interface_reinit_frame_btn();
                    break;
            }
            base.WndProc(ref m);
        }

        // Tick Timer Fix Window Resize Errors
        private void timer1_Tick(object sender, EventArgs e)
        {
            interface_reinit_frame_btn();
            if (checkBox4.Checked)
            {
                pictureBox3.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                Header_Panel.BackColor = Color.Red;
            }
            else
            {
                pictureBox3.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                label14.Visible = false;
                Header_Panel.BackColor = ColorTranslator.FromHtml("#242424");
            }
        }

        // Function for Mouse Move on Title Bar Selection
        private void header_frame_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Capture the offset from the mouse cursor to the form's location
                offset = new Point(e.X, e.Y);
            }
        }

        // Additional Function for MouseMove
        private void header_frame_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Move the form with the mouse
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-offset.X, -offset.Y);

                // Ensure the form stays within the screen bounds
                Screen screen = Screen.FromPoint(newLocation);
                Rectangle screenBounds = screen.Bounds;

                // Adjust newLocation if it goes outside screen bounds
                int newX = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - this.Width, newLocation.X));
                int newY = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - this.Height, newLocation.Y));

                this.Location = new Point(newX, newY);
            }
        }

        // Show Youtube Page
        private void richTextBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.youtube.com/@BugfishTM",
                UseShellExecute = true
            });
        }

        // Show Github Page
        private void richTextBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/bugfishtm/bugfish_nuke",
                UseShellExecute = true
            });
        }

        // Add new Folder Function Reference
        private void label5_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;

                    // Check if the path already exists in the database
                    var dtCheck = sqlite.GetDataTable(
                        "SELECT COUNT(*) as cnt FROM Folders WHERE Path = @path",
                        new SQLiteParameter("@path", folderPath)
                    );
                    int exists = Convert.ToInt32(dtCheck.Rows[0]["cnt"]);

                    if (exists == 0)
                    {
                        // Insert the folder path into the Folders table, with IsActive = 1 (active)
                        sqlite.InsertData(
                            "INSERT INTO Folders (Path, IsActive) VALUES (@path, @active)",
                            new SQLiteParameter("@path", folderPath),
                            new SQLiteParameter("@active", "1")
                        );
                    }
                }
            }
            LoadItems();

        }

        // Start the Process
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            interface_reinit_frame_btn();
            Panel_Store.Visible = false;
            Panel_Load.Visible = true;
            listBox2.Visible = true;
            progressBar1.Visible = true;
            textBox1.Text = "";
            button2.Enabled = true;
            button1.Enabled = true;
            canceled = false;
            btnMaximize.Enabled = true;
            btnMinimize.Enabled = true;
            btnClose.Enabled = true;
            checkBox1.Enabled = true;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox2.Enabled = true;
            checkBox4.Checked = false;
            checkBox4.Enabled = true;
            checkBox3.Checked = false;
            checkBox3.Enabled = true;
            checkBoxTrim.Checked = false;
            checkBoxTrim.Enabled = true;
            checkBox1.AutoCheck = true;
            checkBox2.AutoCheck = true;
            checkBox3.AutoCheck = true;
            checkBox4.AutoCheck = true;
            checkBox5.AutoCheck = true;
            checkBoxTrim.AutoCheck = true;
            progressBar1.Value = 0;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            numericUpDown1.Value = overwrite_passes;
        }

        // Cancel Deletion Button
        private async void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Operation is aborting...please wait.";
            canceled = true;
            if (CurrentTask != null)
            {
                if (CurrentTask.Status == TaskStatus.Running)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    textBox1.Text = "Operation canceled.";
                }
            }
            interface_reinit_frame_btn();
            Panel_Load.Visible = false;
            listBox2.Visible = false;
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            Panel_Store.Visible = true;
            button2.Enabled = true;
            button1.Enabled = true;
            btnMaximize.Enabled = true;
            btnMinimize.Enabled = true;
            btnClose.Enabled = true;
            checkBox1.Checked = false;
            checkBox1.Enabled = true;
            checkBox2.Checked = false;
            checkBox2.Enabled = true;
            checkBox3.Checked = false;
            checkBox3.Enabled = true;
            checkBox4.Checked = false;
            checkBox4.Enabled = true;
            checkBoxTrim.Checked = false;
            checkBoxTrim.Enabled = true;
            numericUpDown1.Enabled = true;
            checkBox1.AutoCheck = true;
            checkBox2.AutoCheck = true;
            checkBox3.AutoCheck = true;
            checkBox4.AutoCheck = true;
            checkBox5.AutoCheck = true;
            checkBoxTrim.AutoCheck = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        public async Task Sleep10SecondsAsync()
        {
            await Task.Delay(10000); // waits 10 seconds asynchronously
        }

        // Confirm Deletion Button
        private async void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = false;
            btnMaximize.Enabled = false;
            btnMinimize.Enabled = false;
            btnClose.Enabled = false;
            canceled = false;
            numericUpDown1.Enabled = false;
            checkBox1.AutoCheck = false;
            checkBox2.AutoCheck = false;
            checkBox3.AutoCheck = false;
            checkBox4.AutoCheck = false;
            checkBox5.AutoCheck = false;
            checkBoxTrim.AutoCheck = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox1.Text = "Starting deletion process...";

            if (!canceled)
            {
                if (checkBox5.Checked)
                {
                    player.PlayLooping();
                }
            }

            // Delete Section Process
            var dt = sqlite.GetDataTable("SELECT Id, Name, Shortcode FROM Items WHERE IsActive = 1 ORDER BY Name ASC;");
            int rowCount = dt.Rows.Count;
            if (rowCount == 0) { rowCount = 1; } else { rowCount = rowCount + 1; }
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = rowCount;
            foreach (DataRow row in dt.Rows)
            {
                if (!canceled)
                {
                    progressBar1.Value = progressBar1.Value + 1;

                    if (row["Shortcode"].Equals("brave"))
                    {
                        textBox1.Text = "Closing and deletion files of: Brave";
                        KillProcesses("brave");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"BraveSoftware\Brave-Browser\User Data"
                        );
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("telegram"))
                    {
                        textBox1.Text = "Closing and deletion files of: Telegram";
                        KillProcesses("Telegram");
                        KillProcesses("TelegramDesktop");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Telegram Desktop");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("discord"))
                    {
                        textBox1.Text = "Closing and deletion files of: Discord";
                        KillProcesses("Discord");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "discord");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("chrome"))
                    {
                        textBox1.Text = "Closing and deletion files of: Chrome";
                        KillProcesses("Chrome");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Google\Chrome\User Data");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("signal"))
                    {
                        textBox1.Text = "Closing and deletion files of: Signal";
                        KillProcesses("Signal");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Signal");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("firefox"))
                    {
                        textBox1.Text = "Closing and deletion files of: firefox";
                        KillProcesses("firefox");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Mozilla\Firefox\Profiles");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("nextcloud"))
                    {
                        textBox1.Text = "Closing and deletion files of: Nextcloud";
                        KillProcesses("Nextcloud");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Nextcloud");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }


                    if (row["Shortcode"].Equals("steam"))
                    {
                        textBox1.Text = "Closing and deletion files of: steam";
                        KillProcesses("Steam");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Steam");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                        userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Steam");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }
                    if (row["Shortcode"].Equals("edge"))
                    {
                        textBox1.Text = "Closing and deletion files of: Edge";
                        KillProcesses("msedge");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Microsoft\Edge\User Data");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("thunderbird"))
                    {
                        textBox1.Text = "Closing and deletion files of: Thunderbird";
                        KillProcesses("thunderbird");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Thunderbird\Profiles");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("outlook"))
                    {
                        textBox1.Text = "Closing and deletion files of: Outlook";
                        KillProcesses("OUTLOOK");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Microsoft\Outlook");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("opera"))
                    {
                        textBox1.Text = "Closing and deletion files of: Opera";
                        KillProcesses("opera");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Opera Software\Opera Stable");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("vivaldi"))
                    {
                        textBox1.Text = "Closing and deletion files of: Vivaldi";
                        KillProcesses("vivaldi");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Vivaldi\User Data");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("skype"))
                    {
                        textBox1.Text = "Closing and deletion files of: Skype";
                        KillProcesses("skype");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Skype");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("slack"))
                    {
                        textBox1.Text = "Closing and deletion files of: Slack";
                        KillProcesses("slack");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Slack");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("whatsapp"))
                    {
                        textBox1.Text = "Closing and deletion files of: WhatsApp";
                        KillProcesses("WhatsApp");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"WhatsApp");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("tor"))
                    {
                        textBox1.Text = "Closing and deletion files of: Tor Browser";
                        KillProcesses("firefox"); // Tor uses a modified Firefox process
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Tor Browser");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("dropbox"))
                    {
                        textBox1.Text = "Closing and deletion files of: Dropbox";
                        KillProcesses("Dropbox");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Dropbox");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("winauth"))
                    {
                        textBox1.Text = "Closing and deletion files of: WinAuth";
                        KillProcesses("WinAuth");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"WinAuth");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("operagx"))
                    {
                        textBox1.Text = "Closing and deletion files of: Opera GX";
                        KillProcesses("opera");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Opera Software\Opera GX Stable");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("teams"))
                    {
                        textBox1.Text = "Closing and deletion files of: Teams";
                        KillProcesses("Teams");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Microsoft\Teams");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("zoom"))
                    {
                        textBox1.Text = "Closing and deletion files of: Zoom";
                        KillProcesses("Zoom");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Zoom");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("onedrive"))
                    {
                        textBox1.Text = "Closing and deletion files of: OneDrive";
                        KillProcesses("OneDrive");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Microsoft\OneDrive");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("keepass"))
                    {
                        textBox1.Text = "Closing and deletion files of: KeePass";
                        KillProcesses("KeePass");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"KeePass");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("veracrypt"))
                    {
                        textBox1.Text = "Closing and deletion files of: VeraCrypt";
                        KillProcesses("VeraCrypt");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"VeraCrypt");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("filezilla"))
                    {
                        textBox1.Text = "Closing and deletion files of: FileZilla";
                        KillProcesses("filezilla");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"FileZilla");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("winscp"))
                    {
                        textBox1.Text = "Closing and deletion files of: WinSCP";
                        KillProcesses("WinSCP");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"WinSCP");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("cyberduck"))
                    {
                        textBox1.Text = "Closing and deletion files of: Cyberduck";
                        KillProcesses("Cyberduck");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Cyberduck");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("icq"))
                    {
                        textBox1.Text = "Closing and deletion files of: ICQ";
                        KillProcesses("icq");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"ICQ");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("viber"))
                    {
                        textBox1.Text = "Closing and deletion files of: Viber";
                        KillProcesses("Viber");
                        string userData = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"ViberPC");
                        await Task.Run(() => SecureDeleteDirectoryAsync(userData, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("windows_dns_cache"))
                    {
                        textBox1.Text = "Windows: Clearing DNS Cache";
                        await Task.Run(() =>
                        {
                            var psi = new ProcessStartInfo("ipconfig", "/flushdns")
                            {
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            Process.Start(psi).WaitForExit();
                        });
                    }

                    if (row["Shortcode"].Equals("windows_explorer_mru"))
                    {
                        textBox1.Text = "Windows: Clearing Explorer MRU";
                        await Task.Run(() =>
                        {
                            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU", true))
                            {
                                key?.SetValue("", "");
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_clipboard"))
                    {
                        textBox1.Text = "Clearing: Windows Clipboard";
                        System.Windows.Forms.Clipboard.Clear();
                    }

                    if (row["Shortcode"].Equals("windows_thumbnail_cache"))
                    {
                        textBox1.Text = "Clearing: Windows Thumbnail Cache";
                        string thumbCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Explorer");
                        await Task.Run(() =>
                        {
                            foreach (var file in Directory.GetFiles(thumbCachePath, "thumbcache_*.db"))
                            {
                                try { SecureDelete.SecureDeleteFile(file, overwrite_passes); } catch { }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_event_logs"))
                    {
                        textBox1.Text = "Windows: Clearing Event Logs";
                        await Task.Run(() =>
                        {
                            var psi = new ProcessStartInfo("wevtutil", "cl Application");
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            Process.Start(psi).WaitForExit();

                            psi = new ProcessStartInfo("wevtutil", "cl Security");
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            Process.Start(psi).WaitForExit();

                            psi = new ProcessStartInfo("wevtutil", "cl System");
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            Process.Start(psi).WaitForExit();
                        });
                    }

                    if (row["Shortcode"].Equals("securekey-ssh"))
                    {
                        textBox1.Text = "Erasing all SSH keys and configs in .ssh directory...";
                        string sshDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            ".ssh"
                        );
                        if (Directory.Exists(sshDir))
                        {
                            foreach (var file in Directory.GetFiles(sshDir))
                                await SecureDeleteFileAsync(file, overwrite_passes);
                        }
                    }

                    if (row["Shortcode"].Equals("unityhub"))
                    {
                        textBox1.Text = "Erasing Unity Hub temp, cache, and log files...";
                        KillProcesses("Unity Hub");

                        // 1. Unity Hub cache and data directory
                        string unityHubDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Unity Hub"
                        );
                        if (Directory.Exists(unityHubDir))
                        {
                            foreach (var file in Directory.GetFiles(unityHubDir, "*.*", SearchOption.AllDirectories))
                            {
                                await SecureDeleteFileAsync(file, overwrite_passes);
                            }
                        }

                        // 2. Unity Hub logs directory
                        string unityHubLogs = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Unity Hub"
                        );
                        if (Directory.Exists(unityHubLogs))
                        {
                            foreach (var file in Directory.GetFiles(unityHubLogs, "*.*", SearchOption.AllDirectories))
                            {
                                await SecureDeleteFileAsync(file, overwrite_passes);
                            }
                        }

                        // 3. Temp files (optional, if you want to catch temp files with UnityHub in the name)
                        string tempDir = Path.GetTempPath();
                        foreach (var file in Directory.GetFiles(tempDir, "UnityHub*.*"))
                        {
                            await SecureDeleteFileAsync(file, overwrite_passes);
                        }
                    }


                    if (row["Shortcode"].Equals("nativeaccess"))
                    {
                        textBox1.Text = "Erasing Native Access temp files...";
                        KillProcesses("Native Access");
                        KillProcesses("NativeAccess"); // Sometimes the process name varies

                        // 1. Delete files in Native Access cache (if you know the path)
                        // Example path (may vary by version and installation):
                        string nativeAccessCache = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Native Instruments", "Native Access"
                        );
                        if (Directory.Exists(nativeAccessCache))
                        {
                            foreach (var file in Directory.GetFiles(nativeAccessCache, "*.*", SearchOption.AllDirectories))
                            {
                                await SecureDeleteFileAsync(file, overwrite_passes);
                            }
                        }

                        // 2. Delete files in the Windows temp folder that match Native Access patterns
                        string tempDir = Path.GetTempPath();
                        foreach (var file in Directory.GetFiles(tempDir, "NativeAccess*.*"))
                        {
                            await SecureDeleteFileAsync(file, overwrite_passes);
                        }

                        // 3. Optionally: Delete Native Access logs (if you want to remove all traces)
                        string nativeAccessLogs = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Native Instruments", "Native Access", "logs"
                        );
                        if (Directory.Exists(nativeAccessLogs))
                        {
                            foreach (var file in Directory.GetFiles(nativeAccessLogs))
                            {
                                await SecureDeleteFileAsync(file, overwrite_passes);
                            }
                        }
                    }

                    if (row["Shortcode"].Equals("office-recent-files"))
                    {
                        textBox1.Text = "Erasing Microsoft Office temp and recent file entries...";

                        // Delete Office temp files
                        string tempDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Microsoft", "Office", "16.0", "OfficeFileCache"
                        );
                        if (Directory.Exists(tempDir))
                        {
                            foreach (var file in Directory.GetFiles(tempDir))
                                await SecureDeleteFileAsync(file, overwrite_passes);
                        }

                        // Delete recent file shortcuts
                        string recentDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Microsoft", "Office", "Recent"
                        );
                        if (Directory.Exists(recentDir))
                        {
                            foreach (var file in Directory.GetFiles(recentDir))
                                await SecureDeleteFileAsync(file, overwrite_passes);
                        }
                    }

                    if (row["Shortcode"].Equals("obs"))
                    {
                        textBox1.Text = "Erasing OBS login and sensitive data...";
                        KillProcesses("obs");        // For OBS Studio
                        KillProcesses("obs64");      // For older OBS Studio versions
                        KillProcesses("obs32");      // Rarely needed
                        KillProcesses("obs-virtualcam"); // If you use the virtual camera

                        // Path to OBS user data
                        string obsDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "obs-studio"
                        );

                        if (Directory.Exists(obsDir))
                        {
                            // List of files that may contain login/sensitive data
                            var sensitiveFiles = new[]
                            {
                                Path.Combine(obsDir, "global.ini"),
                                Path.Combine(obsDir, "basic.ini"),
                                Path.Combine(obsDir, "service.json") // Newer versions may use this
                            };

                            foreach (var file in sensitiveFiles)
                            {
                                if (File.Exists(file))
                                    await SecureDeleteFileAsync(file, overwrite_passes);
                            }

                            // Optionally: Delete the entire 'obs-studio' directory for a full reset
                            // WARNING: This will delete all OBS settings, scenes, sources, etc.
                            await SecureDeleteDirectoryAsync(obsDir, overwrite_passes);
                        }
                    }


                    if (row["Shortcode"].Equals("securekey-efs-user"))
                    {
                        textBox1.Text = "Erasing user EFS keys (makes user EFS-encrypted files unrecoverable)...";
                        string userSid = WindowsIdentity.GetCurrent().User.Value;

                        // RSA keys
                        string userRsaDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Microsoft\Crypto\RSA", userSid);
                        if (Directory.Exists(userRsaDir))
                            foreach (var file in Directory.GetFiles(userRsaDir))
                                await SecureDeleteFileAsync(file, overwrite_passes);

                        // DSS keys (less common)
                        string userDssDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            @"Microsoft\Crypto\DSS", userSid);
                        if (Directory.Exists(userDssDir))
                            foreach (var file in Directory.GetFiles(userDssDir))
                                await SecureDeleteFileAsync(file, overwrite_passes);
                    }

                    if (row["Shortcode"].Equals("securekey-machine"))
                    {
                        textBox1.Text = "Erasing machine-level cryptographic keys...";
                        string machineRsaDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            @"Microsoft\Crypto\RSA\MachineKeys");
                        if (Directory.Exists(machineRsaDir))
                            foreach (var file in Directory.GetFiles(machineRsaDir))
                                await Task.Run(() => SecureDeleteFileAsync(file, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("securekey-bitlocker"))
                    {
                        textBox1.Text = "Erasing BitLocker recovery keys from system root...";
                        string cRoot = @"C:\";
                        foreach (var file in Directory.GetFiles(cRoot, "*.bek", SearchOption.TopDirectoryOnly))
                            await Task.Run(() => SecureDeleteFileAsync(file, overwrite_passes));
                    }

                    if (row["Shortcode"].Equals("windows_trash"))
                    {
                        textBox1.Text = "Windows: Clearing Trash";
                        SHEmptyRecycleBin(IntPtr.Zero, null, 0x7);
                    }

                    if (row["Shortcode"].Equals("windows_recent"))
                    {
                        textBox1.Text = "Windows: Clearing Recent Files List";
                        string recentPath = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                        await Task.Run(() =>
                        {
                            foreach (var file in Directory.GetFiles(recentPath))
                            {
                                try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_prefetch"))
                    {
                        textBox1.Text = "Windows: Clearing Prefetch Folder";
                        string prefetchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
                        await Task.Run(() =>
                        {
                            foreach (var file in Directory.GetFiles(prefetchPath, "*.pf"))
                            {
                                try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_log_files"))
                    {
                        textBox1.Text = "Windows: Deleting Log Files";
                        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                        string logDir = Path.Combine(windir, "Logs");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(logDir))
                            {
                                foreach (var file in Directory.GetFiles(logDir, "*.log"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                            // Also delete setup logs in Panther
                            string pantherDir = Path.Combine(windir, "Panther");
                            if (Directory.Exists(pantherDir))
                            {
                                foreach (var file in Directory.GetFiles(pantherDir, "*.log"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_event_trace_logs"))
                    {
                        textBox1.Text = "Windows: Deleting Event Trace Logs";
                        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                        string etlDir = Path.Combine(windir, "System32", "LogFiles", "WMI");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(etlDir))
                            {
                                foreach (var file in Directory.GetFiles(etlDir, "*.etl"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_font_cache"))
                    {
                        textBox1.Text = "Windows: Clearing Font Cache";
                        string fontCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"FontCache");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(fontCachePath))
                            {
                                foreach (var file in Directory.GetFiles(fontCachePath, "FontCache*.dat"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                            // Also try system-wide cache
                            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                            string sysFontCache = Path.Combine(windir, "System32", "FNTCACHE.DAT");
                            try { SecureDelete.SecureDeleteFile(sysFontCache, 0); } catch { }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_memory_dumps"))
                    {
                        textBox1.Text = "Windows: Deleting Memory Dump Files";
                        string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                        await Task.Run(() =>
                        {
                            foreach (var file in new[] { "Memory.dmp", "Minidump.dmp" })
                            {
                                string fullPath = Path.Combine(windir, file);
                                try { SecureDelete.SecureDeleteFile(fullPath, 0); } catch { }
                            }
                            // Delete all files in Minidump folder
                            string minidumpDir = Path.Combine(windir, "Minidump");
                            if (Directory.Exists(minidumpDir))
                            {
                                foreach (var file in Directory.GetFiles(minidumpDir, "*.dmp"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_error_reporting"))
                    {
                        textBox1.Text = "Windows: Deleting Windows Error Reporting Files";
                        string werPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\WER");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(werPath))
                            {
                                foreach (var dir in Directory.GetDirectories(werPath))
                                {
                                    try { SecureDelete.SecureDeleteDirectory(dir, 0); } catch { }
                                }
                                foreach (var file in Directory.GetFiles(werPath))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_userassist"))
                    {
                        textBox1.Text = "Windows: Clearing UserAssist History";
                        await Task.Run(() =>
                        {
                            try
                            {
                                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist", true))
                                {
                                    if (key != null)
                                    {
                                        foreach (var subKeyName in key.GetSubKeyNames())
                                        {
                                            try { key.DeleteSubKeyTree(subKeyName); } catch { }
                                        }
                                    }
                                }
                            }
                            catch { }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_webdav_cache"))
                    {
                        textBox1.Text = "Windows: Deleting WebDAV Cache";
                        string webDavCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\WebCache");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(webDavCache))
                            {
                                foreach (var file in Directory.GetFiles(webDavCache, "webcache*.dat"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_driver_install_logs"))
                    {
                        textBox1.Text = "Windows: Deleting Driver Installation Log Files";
                        string setupApiLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "inf", "setupapi.dev.log");
                        string setupApiAppLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "inf", "setupapi.app.log");
                        await Task.Run(() =>
                        {
                            foreach (var file in new[] { setupApiLog, setupApiAppLog })
                            {
                                try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_webcache"))
                    {
                        textBox1.Text = "Windows: Deleting WebCache Files";
                        string webCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\WebCache");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(webCachePath))
                            {
                                foreach (var file in Directory.GetFiles(webCachePath))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_update_cleanup"))
                    {
                        textBox1.Text = "Windows: Cleaning up Windows Update Files";
                        string winSxS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "WinSxS");
                        await Task.Run(() =>
                        {
                            // StartComponentCleanup via Task Scheduler
                            var psi = new ProcessStartInfo("schtasks.exe", "/Run /TN \"\\Microsoft\\Windows\\Servicing\\StartComponentCleanup\"")
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            try { Process.Start(psi).WaitForExit(); } catch { }
                            // Optionally, delete old update logs
                            string logDir = Path.Combine(winSxS, "ManifestCache");
                            if (Directory.Exists(logDir))
                            {
                                foreach (var file in Directory.GetFiles(logDir, "*.bin"))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_recent_documents"))
                    {
                        textBox1.Text = "Windows: Clearing Recent Documents History";
                        string recentPath = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                        await Task.Run(() =>
                        {
                            foreach (var file in Directory.GetFiles(recentPath))
                            {
                                try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                            }
                            // Also clear jump lists
                            string autoDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\Recent\AutomaticDestinations");
                            string customDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\Recent\CustomDestinations");
                            foreach (var dir in new[] { autoDest, customDest })
                            {
                                if (Directory.Exists(dir))
                                {
                                    foreach (var file in Directory.GetFiles(dir))
                                    {
                                        try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                    }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_system_restore_cleanup"))
                    {
                        textBox1.Text = "Windows: Cleaning up System Restore Points";
                        await Task.Run(() =>
                        {
                            var psi = new ProcessStartInfo("vssadmin", "delete shadows /for=c: /oldest /quiet")
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            try { Process.Start(psi).WaitForExit(); } catch { }
                        });
                    }

                    if (row["Shortcode"].Equals("windows_directx_shader_cache"))
                    {
                        textBox1.Text = "Windows: Deleting DirectX Shader Cache";
                        string shaderCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache");
                        await Task.Run(() =>
                        {
                            if (Directory.Exists(shaderCachePath))
                            {
                                foreach (var file in Directory.GetFiles(shaderCachePath))
                                {
                                    try { SecureDelete.SecureDeleteFile(file, 0); } catch { }
                                }
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("clear_all_creds"))
                    {
                        textBox1.Text = "Windows: Preparing to clear all credentials...";
                        await Task.Run(() =>
                        {
                            bool moduleInstalled = false;
                            bool installAttempted = false;
                            bool nugetProviderPresent = false;

                            // 1. Check if NuGet provider is available (to avoid prompts)
                            var checkNuGet = new ProcessStartInfo("powershell", "-Command \"Get-PackageProvider -Name NuGet -ListAvailable\"");
                            checkNuGet.UseShellExecute = false;
                            checkNuGet.CreateNoWindow = true;
                            checkNuGet.RedirectStandardOutput = true;
                            using (var process = Process.Start(checkNuGet))
                            {
                                process.WaitForExit();
                                nugetProviderPresent = process.StandardOutput.ReadToEnd().Contains("NuGet");
                            }

                            // 2. If NuGet provider is present, check/install CredentialManager
                            if (nugetProviderPresent)
                            {
                                // Check if CredentialManager module is available
                                var checkModule = new ProcessStartInfo("powershell", "-Command \"Get-Module -ListAvailable CredentialManager\"");
                                checkModule.UseShellExecute = false;
                                checkModule.CreateNoWindow = true;
                                checkModule.RedirectStandardOutput = true;
                                using (var process = Process.Start(checkModule))
                                {
                                    process.WaitForExit();
                                    moduleInstalled = process.StandardOutput.ReadToEnd().Contains("CredentialManager");
                                }

                                // If not installed, try to install it silently
                                if (!moduleInstalled)
                                {
                                    var installModule = new ProcessStartInfo("powershell", "-Command \"Install-Module -Name CredentialManager -Force -Scope CurrentUser -ErrorAction SilentlyContinue\"");
                                    installModule.UseShellExecute = false;
                                    installModule.CreateNoWindow = true;
                                    installModule.RedirectStandardOutput = true;
                                    installModule.RedirectStandardError = true;
                                    using (var process = Process.Start(installModule))
                                    {
                                        process.WaitForExit();
                                        // Check if install was likely successful (simple check)
                                        if (process.ExitCode == 0)
                                        {
                                            moduleInstalled = true;
                                        }
                                        installAttempted = true;
                                    }
                                }
                            }

                            // 3. Only proceed if module is available
                            if (moduleInstalled)
                            {
                                var psi = new ProcessStartInfo("powershell", "-Command \"Get-StoredCredential | ForEach-Object { Remove-StoredCredential -Target $_.TargetName }\"");
                                psi.UseShellExecute = false;
                                psi.CreateNoWindow = true;
                                Process.Start(psi).WaitForExit();
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: All credentials cleared."));
                            }
                            else if (!nugetProviderPresent)
                            {
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: NuGet provider not available. Credentials not cleared."));
                            }
                            else if (installAttempted)
                            {
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: Could not install CredentialManager module. Credentials not cleared."));
                            }
                            else
                            {
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: CredentialManager module not available. Credentials not cleared."));
                            }
                        });
                    }

                    if (row["Shortcode"].Equals("empty_vs2022_settings"))
                    {
                        textBox1.Text = "Windows: Preparing to empty VS2022 ApplicationPrivateSettings.xml files...";
                        await Task.Run(() =>
                        {
                            string vsBaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "VisualStudio");
                            string log = "";

                            // Loop through all "17.*" directories
                            foreach (string dir in Directory.GetDirectories(vsBaseDir, "17.*"))
                            {
                                string settingsFile = Path.Combine(dir, "ApplicationPrivateSettings.xml");
                                if (File.Exists(settingsFile))
                                {
                                    try
                                    {
                                        // Empty the file by overwriting with nothing
                                        File.WriteAllText(settingsFile, "");
                                        log += $"Emptied: {settingsFile}\n";
                                    }
                                    catch (Exception ex)
                                    {
                                        log += $"Failed to empty: {settingsFile} - {ex.Message}\n";
                                    }
                                }
                            }

                            // Update UI with results
                            if (string.IsNullOrEmpty(log))
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: No ApplicationPrivateSettings.xml files found to empty."));
                            else
                                textBox1.Invoke((Action)(() => textBox1.Text = "Windows: Results:\n" + log));
                        });
                    }

                }
            }

            // Delete Folder Process
            dt = sqlite.GetDataTable("SELECT Id, Path FROM Folders");
            rowCount = dt.Rows.Count;
            if (rowCount == 0) { rowCount = 1; } else { rowCount = rowCount + 1; }
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = rowCount;
            foreach (DataRow row in dt.Rows)
            {
                progressBar1.Value = progressBar1.Value + 1;
                if (!canceled)
                {
                    var currentf = $@"{row["Path"]}";
                    textBox1.Text = "Secure delting folder: " + currentf;
                    await Task.Run(() => SecureDeleteDirectoryAsync(currentf, overwrite_passes));
                }
            }

            // Execute Scripts Process
            dt = sqlite.GetDataTable("SELECT Id, Path FROM Cmds");
            rowCount = dt.Rows.Count;
            if (rowCount == 0) { rowCount = 1; } else { rowCount = rowCount + 1; }
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = rowCount;

            foreach (DataRow row in dt.Rows)
            {
                progressBar1.Value = progressBar1.Value + 1;
                if (!canceled)
                {
                    var currentf = $@"{row["Path"]}";
                    textBox1.Text = "Executing script: " + currentf;

                    // Fire and forget the execution of the script
                    Task.Run(() =>
                    {
                        try
                        {
                            var processInfo = new ProcessStartInfo
                            {
                                FileName = currentf,
                                UseShellExecute = true, // Allows running .bat, .exe, etc. in the shell
                                CreateNoWindow = true,  // No window for batch files
                                WindowStyle = ProcessWindowStyle.Hidden  // Hide window for background processes
                            };

                            // Start the process without waiting for it to finish
                            var process = Process.Start(processInfo);

                            // No need to wait for process exit; this makes the process independent
                        }
                        catch (Exception ex)
                        {
                            // Optionally log or display error
                            // MessageBox.Show($"Failed to execute {currentf}: {ex.Message}");
                        }
                    });
                }
            }

            if (checkBoxTrim.Checked)
            {
                TriggerTrimWithPowerShell();
            }

            if (!canceled)
            {
                if (checkBox4.Checked)
                {
                    if (checkBox1.Checked)
                    {
                        Process.Start("shutdown", "/r /t 30");
                    }
                    textBox1.Text = "Damaging windows...";
                    SystemFileDestroyer.DeleteCriticalSystemFiles();
                }
            }

            if (!canceled)
            {
                if (checkBox1.Checked)
                {
                    Process.Start("shutdown", "/r /t 0");
                }
            }

            if (!canceled)
            {
                if (checkBox2.Checked)
                {
                    Application.Exit();
                }
            }

            if (!canceled)
            {
                if (checkBox3.Checked)
                {
                    player.Stop();
                }
            }

            // Finish the deletion Process
            progressBar1.Value = rowCount;
            if (!canceled) { textBox1.Text = "Finished deletion process..."; }
            button2.Enabled = true;
            button1.Enabled = true;
            canceled = false;
            btnMaximize.Enabled = true;
            btnMinimize.Enabled = true;
            btnClose.Enabled = true;
            checkBox1.Checked = false;
            checkBox1.Enabled = true;
            checkBox2.Checked = false;
            checkBox2.Enabled = true;
            numericUpDown1.Enabled = true;
            checkBox3.Checked = false;
            checkBox3.Enabled = true;
            checkBox4.Checked = false;
            checkBox4.Enabled = true;
            checkBoxTrim.Checked = false;
            checkBoxTrim.Enabled = true;
            checkBox1.AutoCheck = true;
            checkBox2.AutoCheck = true;
            checkBox3.AutoCheck = true;
            checkBox4.AutoCheck = true;
            checkBox5.AutoCheck = true;
            checkBoxTrim.AutoCheck = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;

        }

        public static void TriggerTrimWithPowerShell()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Optimize-Volume -DriveLetter (Get-Volume | Where-Object { $_.DriveType -eq 'SSD' }).DriveLetter -ReTrim -Verbose",
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas"
            });
        }

        // Helper to kill all processes by name
        void KillProcesses(params string[] processNames)
        {
            foreach (var name in processNames)
            {
                foreach (var proc in Process.GetProcessesByName(name))
                {
                    try { proc.Kill(true); proc.WaitForExit(); }
                    catch { }
                }
            }
        }

        // Secure Delete Async Function
        public static Task SecureDeleteDirectoryAsync(string dirPath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteDirectory(dirPath, passes));
        }
        public static Task SecureDeleteFileAsync(string dirPath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteFile(dirPath, passes));
        }

        private void Panel_Load_Paint(object sender, PaintEventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            overwrite_passes = (int)numericUpDown1.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            player.PlayLooping();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            player.Stop();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select an executable file";
                fileDialog.Filter = "Executable Files (*.exe;*.bat)|*.exe;*.bat";
                fileDialog.Multiselect = false;
                fileDialog.CheckFileExists = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = fileDialog.FileName;

                    // Check if the path already exists in the database
                    var dtCheck = sqlite.GetDataTable(
                        "SELECT COUNT(*) as cnt FROM Cmds WHERE Path = @path",
                        new SQLiteParameter("@path", filePath)
                    );
                    int exists = Convert.ToInt32(dtCheck.Rows[0]["cnt"]);

                    if (exists == 0)
                    {
                        // Insert the file path into the Folders table, with IsActive = 1 (active)
                        sqlite.InsertData(
                            "INSERT INTO Cmds (Path, IsActive) VALUES (@path, @active)",
                            new SQLiteParameter("@path", filePath),
                            new SQLiteParameter("@active", "1")
                        );
                    }
                }
            }
            LoadItems();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio Files|*.wav;*.mp3";
                openFileDialog.Title = "Select a WAV or MP3 file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = openFileDialog.FileName;

                    // 1. Ensure the Settings table exists
                    sqlite.CreateTable(@"CREATE TABLE IF NOT EXISTS Settings (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                    Value TEXT NOT NULL, 
                                    Shortcode TEXT NOT NULL UNIQUE, 
                                    IsActive INTEGER NOT NULL
                                );");

                    // 2. Check if the shortcode exists
                    var dt = sqlite.GetDataTable(
                        "SELECT * FROM Settings WHERE Shortcode = @shortcode",
                        new SQLiteParameter("@shortcode", "music_path")
                    );

                    if (dt.Rows.Count == 0)
                    {
                        // Insert new entry
                        sqlite.InsertData(
                            "INSERT INTO Settings (Value, Shortcode, IsActive) VALUES (@value, @shortcode, 1);",
                            new SQLiteParameter("@value", selectedPath),
                            new SQLiteParameter("@shortcode", "music_path")
                        );
                    }
                    else
                    {
                        // Update existing entry
                        sqlite.UpdateData(
                            "UPDATE Settings SET Value = @value WHERE Shortcode = @shortcode;",
                            new SQLiteParameter("@value", selectedPath),
                            new SQLiteParameter("@shortcode", "music_path")
                        );
                    }

                    // 3. Set audio player file
                    if (System.IO.File.Exists(selectedPath))
                    {
                        player.Stop();
                        player.SoundLocation = selectedPath;
                    }
                    else
                    {
                        player.Stop();
                        player = new SoundPlayer(new MemoryStream(Properties.Resources.xx));
                    }
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                pictureBox3.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                Header_Panel.BackColor = Color.Red;
            }
            else
            {
                pictureBox3.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                label14.Visible = false;
                Header_Panel.BackColor = ColorTranslator.FromHtml("#242424");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Panel_Store.Visible = false;
            Panel_Cred.Visible = true;
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {
        }
        private void richTextBox4_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.youtube.com/playlist?list=PL6npOHuBGrpBMogf3Fy78H9JaiwaRWyd7",
                UseShellExecute = true
            });
        }


        // Extra Function for Minimum Resizing in Width and Height to not make the Window Disappear
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }
    }
}
