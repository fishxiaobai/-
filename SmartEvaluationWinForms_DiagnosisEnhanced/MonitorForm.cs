namespace SmartEvaluationWinForms;

public sealed class MonitorForm : Form
{
    private readonly MonitoringSnapshot _snapshot;

    private Label _lblHeaderStatus = null!;
    private Label _lblLastUpdate = null!;
    private Label _lblBaseScoreValue = null!;
    private Label _lblDeductionScoreValue = null!;
    private Label _lblFinalScoreValue = null!;
    private Label _lblJudgmentValue = null!;
    private DataGridView _dgvPrimary = null!;
    private DataGridView _dgvSecondary = null!;
    private DataGridView _dgvDeductions = null!;
    private string _lastDiagnosisSignature = string.Empty;

    public MonitorForm(MonitoringSnapshot? snapshot = null)
    {
        _snapshot = snapshot ?? BuildDemoSnapshot();

        Text = "数据监控与智能评价";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1280, 820);
        BackColor = Color.FromArgb(242, 246, 251);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(20),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 148));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildSummaryRow(), 0, 1);
        root.Controls.Add(BuildContentArea(), 0, 2);

        Controls.Add(root);
        Shown += (_, _) => LoadEvaluation();
    }

    private Control BuildHeader()
    {
        var panel = CreateCardPanel();
        panel.Padding = new Padding(24, 18, 24, 18);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.White
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        var title = new Label
        {
            Dock = DockStyle.Fill,
            Text = "数据监控与智能评价",
            Font = new Font("Microsoft YaHei UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100)
        };

        var desc = new Label
        {
            Dock = DockStyle.Fill,
            Text = "基于首页输入的监测数据自动展示一级指标、二级指标、扣分项与综合结论",
            Font = new Font("Microsoft YaHei UI", 10.5F),
            ForeColor = Color.FromArgb(96, 108, 126)
        };

        _lblHeaderStatus = new Label
        {
            Dock = DockStyle.Fill,
            Text = "系统状态：等待评价",
            Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 122, 204)
        };

        left.Controls.Add(title, 0, 0);
        left.Controls.Add(desc, 0, 1);
        left.Controls.Add(_lblHeaderStatus, 0, 2);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White
        };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _lblLastUpdate = new Label
        {
            Dock = DockStyle.Fill,
            Text = "最近更新：--",
            Font = new Font("Microsoft YaHei UI", 10),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleRight
        };

        var sourceLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "数据来源：首页手工录入",
            Font = new Font("Microsoft YaHei UI", 10),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleRight
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = Color.White
        };

        var btnClose = CreateActionButton("返回首页", Color.FromArgb(25, 52, 100));
        btnClose.Click += (_, _) => Close();

        var btnRefresh = CreateActionButton("重新评价", Color.FromArgb(0, 122, 204));
        btnRefresh.Click += (_, _) => LoadEvaluation();

        buttonPanel.Controls.Add(btnClose);
        buttonPanel.Controls.Add(btnRefresh);

        right.Controls.Add(_lblLastUpdate, 0, 0);
        right.Controls.Add(sourceLabel, 0, 1);
        right.Controls.Add(buttonPanel, 0, 2);

        layout.Controls.Add(left, 0, 0);
        layout.Controls.Add(right, 1, 0);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildSummaryRow()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = BackColor,
            Margin = new Padding(0, 14, 0, 14)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        layout.Controls.Add(CreateSummaryCard("基础得分", "一级指标加权求和结果", out _lblBaseScoreValue, Color.FromArgb(0, 122, 204), out _), 0, 0);
        layout.Controls.Add(CreateSummaryCard("扣分合计", "重大/重要/一般故障累计扣分", out _lblDeductionScoreValue, Color.FromArgb(230, 81, 0), out _), 1, 0);
        layout.Controls.Add(CreateSummaryCard("综合得分", "基础得分减去扣分后的结果", out _lblFinalScoreValue, Color.FromArgb(46, 125, 50), out _), 2, 0);
        layout.Controls.Add(CreateSummaryCard("评价结论", "根据综合得分自动判断系统状态", out _lblJudgmentValue, Color.FromArgb(123, 31, 162), out _), 3, 0);
        return layout;
    }

    private Control BuildContentArea()
    {
        _dgvPrimary = CreateGrid();
        _dgvSecondary = CreateGrid();
        _dgvDeductions = CreateGrid();

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = BackColor
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));

        var upper = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = BackColor
        };
        upper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        upper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        upper.Controls.Add(CreateSectionCard("一级指标得分", "展示五个一级指标的权重与得分", _dgvPrimary), 0, 0);
        upper.Controls.Add(CreateSectionCard("扣分项", "列出重大、重要、一般故障的扣分明细", _dgvDeductions), 1, 0);

        layout.Controls.Add(upper, 0, 0);
        layout.Controls.Add(CreateSectionCard("二级指标得分明细", "按所属一级指标展开各二级指标评分", _dgvSecondary), 0, 1);
        return layout;
    }

    private void LoadEvaluation()
    {
        var result = EvaluationService.Evaluate(_snapshot);
        var diagnosisReport = DiagnosisService.Analyze(_snapshot, result);

        _lblBaseScoreValue.Text = result.BaseScore.ToString("F2");
        _lblDeductionScoreValue.Text = result.DeductionScore.ToString("F2");
        _lblFinalScoreValue.Text = result.FinalScore.ToString("F2");
        _lblJudgmentValue.Text = result.Judgment;
        _lblJudgmentValue.ForeColor = GetJudgmentColor(result.FinalScore);
        _lblHeaderStatus.Text = $"系统状态：{result.Judgment}";
        _lblHeaderStatus.ForeColor = GetJudgmentColor(result.FinalScore);
        _lblLastUpdate.Text = $"最近更新：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";

        _dgvPrimary.DataSource = result.PrimaryIndicators
            .Select(x => new
            {
                一级指标 = x.Name,
                权重 = x.Weight.ToString("F2"),
                得分 = x.Score.ToString("F2")
            }).ToList();

        _dgvSecondary.DataSource = result.PrimaryIndicators
            .SelectMany(x => x.SecondaryIndicators)
            .Select(x => new
            {
                所属一级指标 = x.PrimaryIndicator,
                二级指标 = x.Name,
                权重 = x.Weight.ToString("F2"),
                得分 = x.Score.ToString("F2"),
                说明 = x.Detail
            }).ToList();

        var deductionSource = result.Deductions.Any()
            ? result.Deductions.Select(x => new
            {
                故障等级 = GetFaultLevelText(x.Level),
                扣分项 = x.Item,
                扣分值 = x.Points.ToString("F0"),
                说明 = x.Description
            }).ToList()
            : new[]
            {
                new
                {
                    故障等级 = "--",
                    扣分项 = "无扣分项",
                    扣分值 = "0",
                    说明 = "当前输入数据未触发故障扣分"
                }
            }.ToList();

        _dgvDeductions.DataSource = deductionSource;

        ConfigurePrimaryGrid();
        ConfigureDeductionGrid();
        ConfigureSecondaryGrid();
        ShowDiagnosisAlert(result, diagnosisReport);
    }


    private void ShowDiagnosisAlert(EvaluationResult result, DiagnosisReport diagnosisReport)
    {
        if (!diagnosisReport.HasAlert)
        {
            _lastDiagnosisSignature = string.Empty;
            return;
        }

        var signature = string.Join("|", diagnosisReport.Items.Select(x => $"{x.Severity}-{x.Title}-{x.Trigger}"));
        if (signature == _lastDiagnosisSignature)
        {
            return;
        }

        _lastDiagnosisSignature = signature;
        using var form = new DiagnosisWarningForm(diagnosisReport, result);
        form.ShowDialog(this);
    }

    private void ConfigurePrimaryGrid()
    {
        if (_dgvPrimary.Columns.Count == 0)
        {
            return;
        }

        _dgvPrimary.Columns[0].FillWeight = 44;
        _dgvPrimary.Columns[1].FillWeight = 18;
        _dgvPrimary.Columns[2].FillWeight = 18;
    }

    private void ConfigureDeductionGrid()
    {
        if (_dgvDeductions.Columns.Count == 0)
        {
            return;
        }

        _dgvDeductions.Columns[0].FillWeight = 18;
        _dgvDeductions.Columns[1].FillWeight = 28;
        _dgvDeductions.Columns[2].FillWeight = 14;
        _dgvDeductions.Columns[3].FillWeight = 40;
    }

    private void ConfigureSecondaryGrid()
    {
        if (_dgvSecondary.Columns.Count == 0)
        {
            return;
        }

        _dgvSecondary.Columns[0].FillWeight = 20;
        _dgvSecondary.Columns[1].FillWeight = 24;
        _dgvSecondary.Columns[2].FillWeight = 10;
        _dgvSecondary.Columns[3].FillWeight = 10;
        _dgvSecondary.Columns[4].FillWeight = 36;
    }

    private static MonitoringSnapshot BuildDemoSnapshot()
    {
        return new MonitoringSnapshot
        {
            CpuStatus = "RUN",
            ScanCycleMs = 18,
            CpuLoadPercent = 72,
            MemoryUsagePercent = 68,
            SevereDiagnosticEvents = 1,

            AiTotal = 8,
            AiOnline = 8,
            DiTotal = 6,
            DiOnline = 5,
            AoDoTotal = 6,
            AoDoOnline = 6,
            IoChannelFaultCount = 1,
            IoTotalChannels = 20,
            IoFeedbackConsistent = true,

            CommunicationDevicesTotal = 4,
            CommunicationDevicesOnline = 3,
            UpdateTimeoutCount = 1,
            CommunicationFaultEvents = 2,
            HeartbeatAliveCount = 3,

            StandpipePressure = 18.2,
            PostThrottlePressure = 12.4,
            MainChannelPressure = 18.0,
            AuxiliaryChannelPressure = 17.5,
            HydraulicPressure = 10.2,
            FlowRate = 0,
            Density = 1.12,
            HydraulicTemperature = 84,
            ValvePositionA = 52,
            ValvePositionB = 48,
            FlowMeterStatus = "故障",
            HydraulicTemperatureStatus = "异常",

            ProportionalValveCommand = 50,
            ProportionalValveFeedback = 56,
            FlatValveCommandOpen = true,
            FlatValveHighLimit = false,
            FlatValveLowLimit = true,
            HydraulicPumpCommandOn = true,
            HydraulicPumpContactorOn = true,
            HeaterCommandOn = false,
            HeaterContactorOn = false
        };
    }

    private static Button CreateActionButton(string text, Color backColor)
    {
        var button = new Button
        {
            Text = text,
            Width = 120,
            Height = 40,
            Margin = new Padding(12, 0, 0, 0),
            BackColor = backColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static Panel CreateSummaryCard(string title, string subtitle, out Label valueLabel, Color accentColor, out Label subtitleLabel)
    {
        var panel = CreateCardPanel();
        panel.Margin = new Padding(0, 0, 14, 0);
        panel.Padding = new Padding(0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var accent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = accentColor,
            Margin = new Padding(0)
        };

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(14, 10, 14, 10)
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = title,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 71, 92),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        valueLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "--",
            Font = new Font("Microsoft YaHei UI", title == "评价结论" ? 16.5F : 18.5F, FontStyle.Bold),
            ForeColor = accentColor,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        subtitleLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = subtitle,
            Font = new Font("Microsoft YaHei UI", 8.8F),
            ForeColor = Color.FromArgb(109, 120, 136),
            TextAlign = ContentAlignment.TopLeft,
            Padding = new Padding(0, 2, 0, 0),
            Margin = new Padding(0)
        };

        content.Controls.Add(titleLabel, 0, 0);
        content.Controls.Add(valueLabel, 0, 1);
        content.Controls.Add(subtitleLabel, 0, 2);

        layout.Controls.Add(accent, 0, 0);
        layout.Controls.Add(content, 1, 0);
        panel.Controls.Add(layout);
        return panel;
    }

    private static Panel CreateSectionCard(string title, string subTitle, Control content)
    {
        var panel = CreateCardPanel();
        panel.Padding = new Padding(18, 14, 18, 18);

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 28,
            Text = title,
            Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100)
        };

        var subTitleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Text = subTitle,
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };

        content.Dock = DockStyle.Fill;
        content.Margin = new Padding(0, 10, 0, 0);

        panel.Controls.Add(content);
        panel.Controls.Add(subTitleLabel);
        panel.Controls.Add(titleLabel);
        return panel;
    }

    private static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(230, 236, 245),
            RowTemplate = { Height = 32 },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 10),
                SelectionBackColor = Color.FromArgb(220, 235, 252),
                SelectionForeColor = Color.Black,
                Padding = new Padding(3)
            },
            ColumnHeadersHeight = 36,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 240, 252),
                ForeColor = Color.FromArgb(40, 52, 70),
                SelectionBackColor = Color.FromArgb(231, 240, 252),
                SelectionForeColor = Color.FromArgb(40, 52, 70),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            },
            EnableHeadersVisualStyles = false
        };
    }

    private static Panel CreateCardPanel()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private static string GetFaultLevelText(FaultLevel level)
    {
        return level switch
        {
            FaultLevel.Major => "重大故障",
            FaultLevel.Important => "重要故障",
            FaultLevel.Minor => "一般故障",
            _ => "未知"
        };
    }

    private static Color GetJudgmentColor(double score)
    {
        if (score >= 90) return Color.FromArgb(46, 125, 50);
        if (score >= 80) return Color.FromArgb(0, 122, 204);
        if (score >= 70) return Color.FromArgb(245, 124, 0);
        if (score >= 60) return Color.FromArgb(230, 81, 0);
        return Color.FromArgb(198, 40, 40);
    }
}
