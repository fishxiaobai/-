namespace SmartEvaluationWinForms;

public sealed class DiagnosisWarningForm : Form
{
    private readonly DiagnosisReport _report;
    private readonly EvaluationResult _result;
    private Label _lblOverallStatus = null!;
    private Label _lblSummary = null!;
    private DataGridView _dgvDiagnosis = null!;

    public DiagnosisWarningForm(DiagnosisReport report, EvaluationResult result)
    {
        _report = report;
        _result = result;

        Text = "故障诊断与预警提示";
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Dpi;
        MinimumSize = new Size(980, 620);
        Size = new Size(1080, 700);
        BackColor = Color.FromArgb(244, 247, 252);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(20),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildMetricRow(), 0, 1);
        root.Controls.Add(BuildContentCard(), 0, 2);
        root.Controls.Add(BuildFooter(), 0, 3);

        Controls.Add(root);
        Load += (_, _) => BindData();
    }

    private Control BuildHeader()
    {
        var panel = CreateCardPanel();
        panel.Padding = new Padding(22, 18, 22, 16);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.White
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Dock = DockStyle.Fill,
            Text = "故障诊断与预警提示",
            Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var desc = new Label
        {
            Dock = DockStyle.Fill,
            Text = "在不改变原有评价界面与功能的前提下，系统根据指标得分、扣分项和原始输入自动给出诊断结论、预警原因与处理建议。",
            Font = new Font("Microsoft YaHei UI", 9.6F),
            ForeColor = Color.FromArgb(96, 108, 126),
            TextAlign = ContentAlignment.MiddleLeft
        };

        _lblSummary = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Microsoft YaHei UI", 10F),
            ForeColor = Color.FromArgb(72, 84, 102),
            TextAlign = ContentAlignment.MiddleLeft
        };

        left.Controls.Add(title, 0, 0);
        left.Controls.Add(desc, 0, 1);
        left.Controls.Add(_lblSummary, 0, 2);

        var right = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(24, 26, 0, 0)
        };

        _lblOverallStatus = new Label
        {
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Microsoft YaHei UI", 11.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White
        };

        var updateTime = new Label
        {
            Dock = DockStyle.Top,
            Height = 32,
            Margin = new Padding(0, 14, 0, 0),
            Text = $"生成时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Font = new Font("Microsoft YaHei UI", 9.4F),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleCenter
        };

        right.Controls.Add(updateTime);
        right.Controls.Add(_lblOverallStatus);

        layout.Controls.Add(left, 0, 0);
        layout.Controls.Add(right, 1, 0);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildMetricRow()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = BackColor,
            Margin = new Padding(0, 14, 0, 14)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

        layout.Controls.Add(CreateMetricCard("已识别故障/异常", _report.CurrentFaultCount.ToString(), "当前已触发的明确异常", Color.FromArgb(230, 81, 0), 0), 0, 0);
        layout.Controls.Add(CreateMetricCard("趋势预警", _report.EarlyWarningCount.ToString(), "即将发生故障的前兆", Color.FromArgb(245, 124, 0), 14), 1, 0);
        layout.Controls.Add(CreateMetricCard("综合得分", _result.FinalScore.ToString("F2"), "维持原评价结果不变", Color.FromArgb(0, 122, 204), 14), 2, 0);
        return layout;
    }

    private Control BuildContentCard()
    {
        _dgvDiagnosis = CreateGrid();

        var panel = CreateCardPanel();
        panel.Padding = new Padding(18, 14, 18, 18);

        var title = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            Text = "诊断与预警明细",
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100)
        };

        var subTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Text = "按严重程度排序展示触发依据、预警原因和建议处理措施。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };

        panel.Controls.Add(_dgvDiagnosis);
        panel.Controls.Add(subTitle);
        panel.Controls.Add(title);
        return panel;
    }

    private Control BuildFooter()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BackColor,
            Padding = new Padding(0, 10, 0, 0)
        };

        var note = new Label
        {
            Dock = DockStyle.Fill,
            Text = "说明：该弹窗只负责新增诊断和预警提示，不改变首页录入、监控评价界面和原有评分/扣分展示方式。",
            Font = new Font("Microsoft YaHei UI", 9.2F),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var btnClose = new Button
        {
            Text = "我知道了",
            Width = 120,
            Height = 38,
            BackColor = Color.FromArgb(25, 52, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold),
            Margin = new Padding(0)
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (_, _) => Close();

        var buttonHost = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 136,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = BackColor,
            Padding = new Padding(0)
        };
        buttonHost.Controls.Add(btnClose);

        panel.Controls.Add(buttonHost);
        panel.Controls.Add(note);
        return panel;
    }

    private void BindData()
    {
        _lblOverallStatus.Text = _report.OverallStatus;
        _lblOverallStatus.BackColor = GetOverallColor(_report);
        _lblSummary.Text = _report.Summary;

        var rows = _report.Items.Any()
            ? _report.Items.Select(x => new
            {
                级别 = GetSeverityText(x.Severity),
                类型 = x.IsPredicted ? "预警" : "诊断",
                分类 = x.Category,
                结论 = x.Title,
                触发依据 = x.Trigger,
                原因 = x.Reason,
                建议 = x.Recommendation
            }).ToList()
            : new[]
            {
                new
                {
                    级别 = "正常",
                    类型 = "--",
                    分类 = "系统",
                    结论 = "当前未识别到故障和预警",
                    触发依据 = $"综合得分={_result.FinalScore:F2}",
                    原因 = "当前输入数据未触发诊断规则。",
                    建议 = "保持现有运行状态并继续监测。"
                }
            }.ToList();

        _dgvDiagnosis.DataSource = rows;

        if (_dgvDiagnosis.Columns.Count > 0)
        {
            _dgvDiagnosis.Columns[0].FillWeight = 10;
            _dgvDiagnosis.Columns[1].FillWeight = 10;
            _dgvDiagnosis.Columns[2].FillWeight = 10;
            _dgvDiagnosis.Columns[3].FillWeight = 18;
            _dgvDiagnosis.Columns[4].FillWeight = 18;
            _dgvDiagnosis.Columns[5].FillWeight = 20;
            _dgvDiagnosis.Columns[6].FillWeight = 14;
        }

        ApplyRowStyles();
    }

    private void ApplyRowStyles()
    {
        for (var i = 0; i < _dgvDiagnosis.Rows.Count && i < _report.Items.Count; i++)
        {
            var row = _dgvDiagnosis.Rows[i];
            var severity = _report.Items[i].Severity;
            row.DefaultCellStyle.ForeColor = GetSeverityColor(severity);
            row.DefaultCellStyle.SelectionForeColor = GetSeverityColor(severity);
            row.DefaultCellStyle.SelectionBackColor = GetSelectionBackColor(severity);
        }
    }

    private static Panel CreateMetricCard(string title, string value, string subTitle, Color accent, int marginLeft)
    {
        var panel = CreateCardPanel();
        panel.Margin = new Padding(marginLeft, 0, 0, 0);
        panel.Padding = new Padding(16, 14, 16, 14);

        var titleLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Text = title,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 71, 92)
        };

        var valueLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 38,
            Text = value,
            Font = new Font("Microsoft YaHei UI", 20F, FontStyle.Bold),
            ForeColor = accent
        };

        var subTitleLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = subTitle,
            Font = new Font("Microsoft YaHei UI", 9F),
            ForeColor = Color.FromArgb(109, 120, 136)
        };

        panel.Controls.Add(subTitleLabel);
        panel.Controls.Add(valueLabel);
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
            RowTemplate = { Height = 40 },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 9.5F),
                SelectionBackColor = Color.FromArgb(220, 235, 252),
                SelectionForeColor = Color.Black,
                Padding = new Padding(3)
            },
            ColumnHeadersHeight = 38,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
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

    private static string GetSeverityText(DiagnosisSeverity severity)
    {
        return severity switch
        {
            DiagnosisSeverity.Critical => "重大故障",
            DiagnosisSeverity.Warning => "故障/异常",
            _ => "趋势预警"
        };
    }

    private static Color GetSeverityColor(DiagnosisSeverity severity)
    {
        return severity switch
        {
            DiagnosisSeverity.Critical => Color.FromArgb(198, 40, 40),
            DiagnosisSeverity.Warning => Color.FromArgb(230, 81, 0),
            _ => Color.FromArgb(245, 124, 0)
        };
    }

    private static Color GetSelectionBackColor(DiagnosisSeverity severity)
    {
        return severity switch
        {
            DiagnosisSeverity.Critical => Color.FromArgb(255, 235, 238),
            DiagnosisSeverity.Warning => Color.FromArgb(255, 243, 224),
            _ => Color.FromArgb(255, 248, 225)
        };
    }

    private static Color GetOverallColor(DiagnosisReport report)
    {
        if (report.Items.Any(x => x.Severity == DiagnosisSeverity.Critical))
            return Color.FromArgb(198, 40, 40);
        if (report.Items.Any(x => x.Severity == DiagnosisSeverity.Warning && !x.IsPredicted))
            return Color.FromArgb(230, 81, 0);
        return Color.FromArgb(245, 124, 0);
    }
}
