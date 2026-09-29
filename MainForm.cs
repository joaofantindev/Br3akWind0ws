using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReconPanel;

public class MainForm : Form
{
    private Panel sidebar;
    private Panel contentPanel;
    private Label titleLabel;
    private Button currentButton;

    public MainForm()
    {
        Text = "ReconPanel — Pentest Desktop";
        Size = new Size(1200, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(30, 30, 30);
        Font = new Font("Segoe UI", 9F);

        // Sidebar
        sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 220,
            BackColor = Color.FromArgb(20, 20, 20)
        };

        titleLabel = new Label
        {
            Text = "ReconPanel",
            ForeColor = Color.FromArgb(0, 200, 255),
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20)
        };
        sidebar.Controls.Add(titleLabel);

        string[] modules = { "Discovery", "Port Scan", "Brute Force Hikvision", "IP Manager", "Wordlist" };
        int y = 70;
        foreach (var mod in modules)
        {
            var btn = CreateModuleButton(mod, y);
            sidebar.Controls.Add(btn);
            y += 50;
        }

        // Content
        contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(30, 30, 30),
            Padding = new Padding(20)
        };

        Controls.Add(contentPanel);
        Controls.Add(sidebar);

        ShowModule("Discovery");
    }

    private Button CreateModuleButton(string text, int y)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(200, 40),
            Location = new Point(10, y),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(40, 40, 40),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10F),
            Tag = text
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += ModuleButton_Click;
        return btn;
    }

    private void ModuleButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            currentButton?.BackColor = Color.FromArgb(40, 40, 40);
            currentButton = btn;
            btn.BackColor = Color.FromArgb(0, 120, 180);
            ShowModule(btn.Tag?.ToString() ?? "Discovery");
        }
    }

    private void ShowModule(string module)
    {
        contentPanel.Controls.Clear();

        Control control = module switch
        {
            "Discovery" => new DiscoveryControl(),
            "Port Scan" => new PortScanControl(),
            "Brute Force Hikvision" => new BruteForceControl(),
            "IP Manager" => new IpManagerControl(),
            "Wordlist" => new WordlistControl(),
            _ => new DiscoveryControl()
        };

        control.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(control);
    }
}
