using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReconPanel;

public class WordlistControl : UserControl
{
    private TextBox txtLog;

    public WordlistControl()
    {
        BackColor = Color.FromArgb(30, 30, 30);
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(10) };

        var btnGen = new Button { Text = "Gerar Wordlist zZeElLaA@000-999", BackColor = Color.FromArgb(0,120,180), ForeColor = Color.White, Size = new Size(250,30), FlatStyle = FlatStyle.Flat };
        btnGen.Click += (s, e) => GenWordlist();
        layout.Controls.Add(btnGen);

        txtLog = new TextBox
        {
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(20,20,20), ForeColor = Color.LightGreen,
            Font = new Font("Consolas", 9F), Dock = DockStyle.Fill
        };

        Controls.Add(txtLog);
        Controls.Add(layout);
    }

    void GenWordlist()
    {
        txtLog.Clear();
        for (int i = 0; i <= 999; i++)
            txtLog.AppendText($"zZeElLaA@{i:D3}{Environment.NewLine}");
    }
}
