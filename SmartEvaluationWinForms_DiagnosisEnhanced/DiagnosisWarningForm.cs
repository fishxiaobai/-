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
        MinimumSize = new Size(1220, 760);
        Size = new Size(1420, 860);
        BackColor = Color.FromArgb(244, 247, 252);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(18),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

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
        panel.Padding = new Padding(20, 16, 20, 14);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "故障诊断与预警提示",
            Font = new Font("Microsoft YaHei UI", 17.6F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        var desc = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "系统根据指标得分、扣分项和原始输入自动给出诊断结论、预警原因与处理建议。",
            Font = new Font("Microsoft YaHei UI", 9.6F),
            ForeColor = Color.FromArgb(96, 108, 126),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0),
            Padding = new Padding(0, 2, 0, 0)
        };

        _lblSummary = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font("Microsoft YaHei UI", 10F),
            ForeColor = Color.FromArgb(72, 84, 102),
            TextAlign = ContentAlignment.TopLeft,
            Margin = new Padding(0),
            Padding = new Padding(0, 6, 0, 0)
        };

        left.Controls.Add(title, 0, 0);
        left.Controls.Add(desc, 0, 1);
        left.Controls.Add(_lblSummary, 0, 2);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(26, 12, 0, 0)
        };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        _lblOverallStatus = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(10, 0, 10, 0)
        };

        var updateTime = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = $"生成时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Font = new Font("Microsoft YaHei UI", 9.3F),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new Padding(0, 8, 0, 0)
        };

        right.Controls.Add(_lblOverallStatus, 0, 0);
        right.Controls.Add(updateTime, 0, 1);

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
            Margin = new Padding(0, 12, 0, 12)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

        layout.Controls.Add(CreateMetricCard("已识别故障/异常", _report.CurrentFaultCount.ToString(), "当前已触发的明确异常", Color.FromArgb(230, 81, 0), 0), 0, 0);
        layout.Controls.Add(CreateMetricCard("趋势预警", _report.EarlyWarningCount.ToString(), "即将发生故障的前兆", Color.FromArgb(245, 124, 0), 12), 1, 0);
        layout.Controls.Add(CreateMetricCard("综合得分", _result.FinalScore.ToString("F2"), "维持原评价结果不变", Color.FromArgb(0, 122, 204), 12), 2, 0);
        return layout;
    }

    private Control BuildContentCard()
    {
        _dgvDiagnosis = CreateGrid();

        var panel = CreateCardPanel();
        panel.Padding = new Padding(14);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "诊断与预警明细",
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        var subTitle = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "按严重程度排序展示触发依据、预警原因和建议处理措施。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(subTitle, 0, 1);
        layout.Controls.Add(_dgvDiagnosis, 0, 2);

        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildFooter()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BackColor,
            Padding = new Padding(0, 6, 0, 0)
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
            Width = 132,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = BackColor,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };
        buttonHost.Controls.Add(btnClose);

        panel.Controls.Add(buttonHost);
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
            _dgvDiagnosis.Columns[0].FillWeight = 11;
            _dgvDiagnosis.Columns[1].FillWeight = 9;
            _dgvDiagnosis.Columns[2].FillWeight = 10;
            _dgvDiagnosis.Columns[3].FillWeight = 16;
            _dgvDiagnosis.Columns[4].FillWeight = 18;
            _dgvDiagnosis.Columns[5].FillWeight = 18;
            _dgvDiagnosis.Columns[6].FillWeight = 18;
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
            row.DefaultCellStyle.BackColor = GetRowBackColor(severity);
        }
    }

    private static Panel CreateMetricCard(string title, string value, string subTitle, Color accent, int marginLeft)
    {
        var panel = CreateCardPanel();
        panel.Margin = new Padding(marginLeft, 0, 0, 0);
        panel.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

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

        var valueLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = value,
            Font = new Font("Microsoft YaHei UI", 18.8F, FontStyle.Bold),
            ForeColor = accent,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };

        var subTitleLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = subTitle,
            Font = new Font("Microsoft YaHei UI", 9.2F),
            ForeColor = Color.FromArgb(109, 120, 136),
            TextAlign = ContentAlignment.TopLeft,
            Margin = new Padding(0),
            Padding = new Padding(0, 2, 0, 0)
        };

        layout.Controls.Add(titleLabel, 0, 0);
        layout.Controls.Add(valueLabel, 0, 1);
        layout.Controls.Add(subTitleLabel, 0, 2);
        panel.Controls.Add(layout);
        return panel;
    }

    private static DataGridView CreateGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(230, 236, 245),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 9.5F),
                SelectionBackColor = Color.FromArgb(220, 235, 252),
                SelectionForeColor = Color.Black,
                Padding = new Padding(5, 4, 5, 4),
                WrapMode = DataGridViewTriState.True,
                Alignment = DataGridViewContentAlignment.TopLeft
            },
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(250, 252, 255),
                WrapMode = DataGridViewTriState.True,
                Padding = new Padding(5, 4, 5, 4)
            },
            RowTemplate = { Height = 56 },
            ColumnHeadersHeight = 42,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 240, 252),
                ForeColor = Color.FromArgb(40, 52, 70),
                SelectionBackColor = Color.FromArgb(231, 240, 252),
                SelectionForeColor = Color.FromArgb(40, 52, 70),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.True
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

    private static Color GetRowBackColor(DiagnosisSeverity severity)
    {
        return severity switch
        {
            DiagnosisSeverity.Critical => Color.FromArgb(255, 249, 249),
            DiagnosisSeverity.Warning => Color.FromArgb(255, 252, 247),
            _ => Color.FromArgb(255, 253, 245)
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
