using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReconPanel;

public class PortScanControl : UserControl
{
    private TextBox txtIp;
    private TextBox txtPorts;
    private Button btnScan;
    private TextBox txtLog;

    public PortScanControl()
    {
        BackColor = Color.FromArgb(30, 30, 30);
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(10) };

        layout.Controls.Add(new Label { Text = "IP:", ForeColor = Color.White, AutoSize = true }, 0, 0);
        txtIp = new TextBox { Text = "192.168.1.64", BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White, Width = 200 };
        layout.Controls.Add(txtIp, 1, 0);

        layout.Controls.Add(new Label { Text = "Portas (ex: 80,443,8000):", ForeColor = Color.White, AutoSize = true }, 0, 1);
        txtPorts = new TextBox { Text = "80,443,8000,23,21", BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White, Width = 200 };
        layout.Controls.Add(txtPorts, 1, 1);

        btnScan = new Button { Text = "Escanear", BackColor = Color.FromArgb(0,120,180), ForeColor = Color.White, Size = new Size(100,30), FlatStyle = FlatStyle.Flat };
        btnScan.Click += BtnScan_Click;
        layout.Controls.Add(btnScan, 1, 2);

        txtLog = new TextBox
        {
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(20,20,20), ForeColor = Color.LightGreen,
            Font = new Font("Consolas", 9F), Dock = DockStyle.Fill
        };

        Controls.Add(txtLog);
        Controls.Add(layout);
    }

    private async void BtnScan_Click(object? sender, EventArgs e)
    {
        btnScan.Enabled = false;
        txtLog.Clear();

        string ip = txtIp.Text.Trim();
        var ports = txtPorts.Text.Split(',').Select(p => int.Parse(p.Trim())).ToArray();

        foreach (var port in ports)
        {
            try
            {
                using var client = new TcpClient();
                var connect = client.ConnectAsync(ip, port);
                if (await Task.WhenAny(connect, Task.Delay(2000)) == connect && client.Connected)
                    txtLog.AppendText($"[ABERTA] {port}{Environment.NewLine}");
            }
            catch { }
        }

        txtLog.AppendText($"{Environment.NewLine}Scan concluído.{Environment.NewLine}");
        btnScan.Enabled = true;
    }
}
