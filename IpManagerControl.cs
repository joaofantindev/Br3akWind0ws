using System;
using System.Drawing;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace ReconPanel;

public class IpManagerControl : UserControl
{
    private TextBox txtLog;

    public IpManagerControl()
    {
        BackColor = Color.FromArgb(30, 30, 30);
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(10) };

        var btnList = new Button { Text = "Listar Interfaces", BackColor = Color.FromArgb(0,120,180), ForeColor = Color.White, Size = new Size(150,30), FlatStyle = FlatStyle.Flat };
        btnList.Click += (s, e) => ListInterfaces();
        layout.Controls.Add(btnList);

        txtLog = new TextBox
        {
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(20,20,20), ForeColor = Color.LightGreen,
            Font = new Font("Consolas", 9F), Dock = DockStyle.Fill
        };

        Controls.Add(txtLog);
        Controls.Add(layout);
    }

    void ListInterfaces()
    {
        txtLog.Clear();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up)
            {
                txtLog.AppendText($"{nic.Name} ({nic.NetworkInterfaceType}){Environment.NewLine}");
                foreach (var addr in nic.GetIPProperties().UnicastAddresses)
                    txtLog.AppendText($"  {addr.Address}{Environment.NewLine}");
            }
        }
    }
}
