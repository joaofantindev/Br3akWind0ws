using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReconPanel;

public class DiscoveryControl : UserControl
{
    private TextBox txtTarget;
    private Button btnScan;
    private TextBox txtLog;
    private ProgressBar progress;

    public DiscoveryControl()
    {
        BackColor = Color.FromArgb(30, 30, 30);
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(10) };

        layout.Controls.Add(new Label { Text = "Rede (ex: 192.168.1.0/24):", ForeColor = Color.White, AutoSize = true }, 0, 0);
        txtTarget = new TextBox { Text = "192.168.1.0/24", BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White, Width = 200 };
        layout.Controls.Add(txtTarget, 1, 0);

        btnScan = new Button { Text = "Escanear", BackColor = Color.FromArgb(0,120,180), ForeColor = Color.White, Size = new Size(100,30), FlatStyle = FlatStyle.Flat };
        btnScan.Click += BtnScan_Click;
        layout.Controls.Add(btnScan, 1, 1);

        progress = new ProgressBar { Dock = DockStyle.Top, Height = 15 };

        txtLog = new TextBox
        {
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(20,20,20), ForeColor = Color.LightGreen,
            Font = new Font("Consolas", 9F), Dock = DockStyle.Fill
        };

        Controls.Add(txtLog);
        Controls.Add(progress);
        Controls.Add(layout);
    }

    private async void BtnScan_Click(object? sender, EventArgs e)
    {
        btnScan.Enabled = false;
        txtLog.Clear();
        var ips = Expand(txtTarget.Text.Trim());
        progress.Maximum = ips.Count;
        progress.Value = 0;

        foreach (var ip in ips)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ip, 800);
                if (reply.Status == IPStatus.Success)
                    txtLog.AppendText($"[UP] {ip} — {reply.RoundtripTime}ms{Environment.NewLine}");
            }
            catch { }
            progress.Value++;
        }

        txtLog.AppendText($"{Environment.NewLine}Scan concluído.{Environment.NewLine}");
        btnScan.Enabled = true;
    }

    static System.Collections.Generic.List<string> Expand(string target)
    {
        var result = new System.Collections.Generic.List<string>();
        if (!target.Contains('/'))
        {
            result.Add(target);
            return result;
        }

        var parts = target.Split('/');
        var baseIp = parts[0].Split('.').Select(int.Parse).ToArray();
        int prefix = int.Parse(parts[1]);
        int count = 1 << (32 - prefix);
        uint start = (uint)((baseIp[0] << 24) | (baseIp[1] << 16) | (baseIp[2] << 8) | baseIp[3]);

        for (int i = 0; i < count; i++)
        {
            uint addr = start + (uint)i;
            result.Add($"{addr >> 24 & 0xFF}.{addr >> 16 & 0xFF}.{addr >> 8 & 0xFF}.{addr & 0xFF}");
        }
        return result;
    }
}
