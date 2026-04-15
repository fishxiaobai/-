namespace SmartEvaluationWinForms;

public sealed class MainForm : Form
{
    private Button _btnOpenMonitor = null!;
    private Button _btnLoadDemo = null!;

    private ComboBox _cmbCpuStatus = null!;
    private NumericUpDown _numScanCycle = null!;
    private NumericUpDown _numCpuLoad = null!;
    private NumericUpDown _numMemoryUsage = null!;
    private NumericUpDown _numDiagnosticEvents = null!;

    private NumericUpDown _numAiTotal = null!;
    private NumericUpDown _numAiOnline = null!;
    private NumericUpDown _numDiTotal = null!;
    private NumericUpDown _numDiOnline = null!;
    private NumericUpDown _numAoDoTotal = null!;
    private NumericUpDown _numAoDoOnline = null!;
    private NumericUpDown _numIoFaultCount = null!;
    private NumericUpDown _numIoTotalChannels = null!;
    private CheckBox _chkIoFeedbackConsistent = null!;

    private NumericUpDown _numCommTotal = null!;
    private NumericUpDown _numCommOnline = null!;
    private NumericUpDown _numTimeoutCount = null!;
    private NumericUpDown _numCommFaultEvents = null!;
    private NumericUpDown _numHeartbeatAlive = null!;

    private NumericUpDown _numStandpipePressure = null!;
    private NumericUpDown _numPostThrottlePressure = null!;
    private NumericUpDown _numMainChannelPressure = null!;
    private NumericUpDown _numAuxiliaryChannelPressure = null!;
    private NumericUpDown _numHydraulicPressure = null!;
    private NumericUpDown _numFlowRate = null!;
    private NumericUpDown _numDensity = null!;
    private NumericUpDown _numHydraulicTemperature = null!;
    private NumericUpDown _numValvePositionA = null!;
    private NumericUpDown _numValvePositionB = null!;
    private ComboBox _cmbFlowMeterStatus = null!;
    private ComboBox _cmbHydraulicTemperatureStatus = null!;

    private NumericUpDown _numProportionalValveCommand = null!;
    private NumericUpDown _numProportionalValveFeedback = null!;
    private CheckBox _chkFlatValveCommandOpen = null!;
    private CheckBox _chkFlatValveHighLimit = null!;
    private CheckBox _chkFlatValveLowLimit = null!;
    private CheckBox _chkHydraulicPumpCommandOn = null!;
    private CheckBox _chkHydraulicPumpContactorOn = null!;
    private CheckBox _chkHeaterCommandOn = null!;
    private CheckBox _chkHeaterContactorOn = null!;

    public MainForm()
    {
        Text = "工控系统智能评价系统";
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;
        MinimumSize = new Size(1440, 900);
        Size = new Size(1480, 940);
        BackColor = Color.FromArgb(243, 247, 252);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(24),
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 124));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildContent(), 0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);

        Controls.Add(root);
        LoadDemoValues();
    }

    private Control BuildHeader()
    {
        var panel = CreateCardPanel();
        panel.Padding = new Padding(28, 18, 28, 18);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "工控系统智能评价系统",
            Font = new Font("Microsoft YaHei UI", 18.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var subTitle = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "首页支持手工录入监测数据，并按评价模型自动计算一级指标、二级指标、扣分项与综合结论。",
            Font = new Font("Microsoft YaHei UI", 10.2F),
            ForeColor = Color.FromArgb(95, 109, 128),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(0, 2, 0, 0)
        };

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(subTitle, 0, 1);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildContent()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = BackColor,
            Margin = new Padding(0, 18, 0, 18)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(BuildIntroPanel(), 0, 0);
        layout.Controls.Add(BuildInputPanel(), 1, 0);
        return layout;
    }

    private Control BuildIntroPanel()
    {
        var panel = CreateCardPanel();
        panel.Margin = new Padding(0, 0, 16, 0);
        panel.Padding = new Padding(0);

        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(24),
            BackColor = Color.White
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 7,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 252));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));   // 评分规则标题行
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));  // 评分规则内容行

        var badge = new Label
        {
            AutoSize = false,
            Width = 246,
            Height = 30,
            Text = "手工录入 / 智能评价 / 模型可视化",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 9.2F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 95, 184),
            BackColor = Color.FromArgb(231, 241, 255),
            Anchor = AnchorStyles.Left
        };

        var title = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "首页直接输入监测数据，再跳转到评价结果界面",
            Font = new Font("Microsoft YaHei UI", 16.8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 48, 74),
            TextAlign = ContentAlignment.TopLeft
        };

        var desc = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "模型按控制器、I/O 模块、通信、检测仪表、执行机构五类一级指标建立，并通过状态映射、分段函数和一致性评价完成量化。一级指标最终按 A1=0.19、A2=0.19、A3=0.11、A4=0.33、A5=0.19 进行综合计算。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(92, 102, 118),
            TextAlign = ContentAlignment.TopLeft
        };

        var weightTitle = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = "一级指标权重",
            Font = new Font("Microsoft YaHei UI", 11.4F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var weights = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        for (var i = 0; i < 5; i++)
        {
            weights.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        }
        weights.Controls.Add(CreateInfoLine("A1 控制器健康度", "0.19"), 0, 0);
        weights.Controls.Add(CreateInfoLine("A2 I/O 模块健康度", "0.19"), 0, 1);
        weights.Controls.Add(CreateInfoLine("A3 通信健康度", "0.11"), 0, 2);
        weights.Controls.Add(CreateInfoLine("A4 检测仪表健康度", "0.33"), 0, 3);
        weights.Controls.Add(CreateInfoLine("A5 执行机构健康度", "0.19"), 0, 4);

        var ruleTitle = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Padding = new Padding(0, 2, 0, 0),
            Text = "评分规则提示",
            Font = new Font("Microsoft YaHei UI", 11.0F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var rulePanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(247, 250, 254),
            Padding = new Padding(16, 14, 16, 14),
            AutoScroll = true
        };

        var ruleLabel = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            MaximumSize = new Size(380, 0),
            Font = new Font("Microsoft YaHei UI", 9.2F),
            ForeColor = Color.FromArgb(88, 99, 116),
            Text = string.Join(Environment.NewLine,
                "1. 状态量：正常/Run 记高分，异常下降，故障接近或等于 0 分。",
                "2. 模拟量：输入行末已提示“评分有效范围 / 优选区间”，处于优选区间记满分，进入预警区线性下降。",
                "3. 一致性：给定值与反馈值偏差越大，得分越低；开关命令与限位反馈不一致会触发扣分。",
                "4. 扣分项：重大故障扣 100，重要故障扣 40，一般故障扣 15。",
                "",
                "说明：文档明确给出了状态映射、分段函数和故障扣分机制；部分具体数值区间在代码中做成了可调默认值，因此现在首页显示的是当前程序采用的输入/评分范围。"
            )
        };

        rulePanel.Controls.Add(ruleLabel);

        layout.Controls.Add(badge, 0, 0);
        layout.Controls.Add(title, 0, 1);
        layout.Controls.Add(desc, 0, 2);
        layout.Controls.Add(weightTitle, 0, 3);
        layout.Controls.Add(weights, 0, 4);
        layout.Controls.Add(ruleTitle, 0, 5);
        layout.Controls.Add(rulePanel, 0, 6);

        scrollPanel.Controls.Add(layout);
        panel.Controls.Add(scrollPanel);
        return panel;
    }

    private Control BuildInputPanel()
    {
        var outer = CreateCardPanel();
        outer.Padding = new Padding(0);

        var host = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White
        };
        host.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(25, 52, 100),
            Padding = new Padding(22, 16, 22, 18)
        };
        var headerTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            AutoSize = false,
            Text = "首页手工录入监测数据",
            Font = new Font("Microsoft YaHei UI", 15.2F, FontStyle.Bold),
            ForeColor = Color.White
        };
        var headerSubTitle = new Label
        {
            Dock = DockStyle.Fill,
            Text = "请按右侧提示范围输入数据。点击按钮后会直接携带这些数据跳转到“数据监控与智能评价”界面。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(220, 231, 247)
        };
        header.Controls.Add(headerSubTitle);
        header.Controls.Add(headerTitle);

        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(18)
        };

        var content = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 7,
            BackColor = Color.White,
            Padding = new Padding(0)
        };
        for (var i = 0; i < 7; i++)
        {
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        content.Controls.Add(CreateGuideCard(), 0, 0);
        content.Controls.Add(BuildControllerSection(), 0, 1);
        content.Controls.Add(BuildIoSection(), 0, 2);
        content.Controls.Add(BuildCommunicationSection(), 0, 3);
        content.Controls.Add(BuildInstrumentSection(), 0, 4);
        content.Controls.Add(BuildActuatorSection(), 0, 5);
        content.Controls.Add(BuildActionArea(), 0, 6);

        scrollPanel.Controls.Add(content);
        host.Controls.Add(header, 0, 0);
        host.Controls.Add(scrollPanel, 0, 1);
        outer.Controls.Add(host);
        return outer;
    }

    private Control CreateGuideCard()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 84,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 14),
            BackColor = Color.FromArgb(244, 248, 255)
        };

        var title = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            Text = "输入说明",
            Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 52, 100)
        };
        var text = new Label
        {
            Dock = DockStyle.Fill,
            Text = "每一行右侧都会显示“评分有效范围 / 优选区间 / 说明”。如果你的输入超出评分有效范围，监控界面仍可显示，但该项评分会被压低或记为 0。在线数量类数据会自动检查“在线数不能大于总数”。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(88, 99, 116)
        };

        panel.Controls.Add(text);
        panel.Controls.Add(title);
        return panel;
    }

    private Control BuildControllerSection()
    {
        _cmbCpuStatus = CreateComboBox("RUN", "正常", "异常", "STOP", "故障");
        _numScanCycle = CreateNumericInput(0, 200, 1, 1, 18);
        _numCpuLoad = CreateNumericInput(0, 100, 1, 1, 72);
        _numMemoryUsage = CreateNumericInput(0, 100, 1, 1, 68);
        _numDiagnosticEvents = CreateNumericInput(0, 20, 0, 1, 1);

        var section = CreateSectionTable();
        AddInputRow(section, 0, "CPU运行状态", _cmbCpuStatus, "状态量：RUN/正常=高分，异常=降分，STOP/故障=0分附近");
        AddInputRow(section, 1, "扫描周期(ms)", _numScanCycle, "评分有效范围 0~40；优选区间 10~20");
        AddInputRow(section, 2, "CPU负载率(%)", _numCpuLoad, "评分有效范围 0~95；优选区间 0~70");
        AddInputRow(section, 3, "存储占用率(%)", _numMemoryUsage, "评分有效范围 0~95；优选区间 0~75");
        AddInputRow(section, 4, "严重诊断事件数", _numDiagnosticEvents, "0 个满分；1 个预警；≥3 个记低分/0分");

        return WrapSection("A1 控制器健康度输入", "输入 CPU 状态和资源占用数据", section);
    }

    private Control BuildIoSection()
    {
        _numAiTotal = CreateNumericInput(1, 100, 0, 1, 8);
        _numAiOnline = CreateNumericInput(0, 100, 0, 1, 8);
        _numDiTotal = CreateNumericInput(1, 100, 0, 1, 6);
        _numDiOnline = CreateNumericInput(0, 100, 0, 1, 5);
        _numAoDoTotal = CreateNumericInput(1, 100, 0, 1, 6);
        _numAoDoOnline = CreateNumericInput(0, 100, 0, 1, 6);
        _numIoFaultCount = CreateNumericInput(0, 500, 0, 1, 1);
        _numIoTotalChannels = CreateNumericInput(1, 500, 0, 1, 20);
        _chkIoFeedbackConsistent = CreateCheckBox("命令与反馈一致", true);

        var section = CreateSectionTable();
        AddInputRow(section, 0, "AI 总数 / 在线数", CreateDualNumericPanel(_numAiTotal, _numAiOnline, "总", "在线"), "在线数范围 0~总数；在线率越高得分越高");
        AddInputRow(section, 1, "DI 总数 / 在线数", CreateDualNumericPanel(_numDiTotal, _numDiOnline, "总", "在线"), "在线数范围 0~总数；在线率越高得分越高");
        AddInputRow(section, 2, "AO/DO 总数 / 在线数", CreateDualNumericPanel(_numAoDoTotal, _numAoDoOnline, "总", "在线"), "在线数范围 0~总数；在线率越高得分越高");
        AddInputRow(section, 3, "I/O 通道故障数 / 通道总数", CreateDualNumericPanel(_numIoFaultCount, _numIoTotalChannels, "故障", "总通道"), "故障数范围 0~总通道；故障率越低得分越高");
        AddInputRow(section, 4, "I/O 反馈一致性", _chkIoFeedbackConsistent, "勾选表示一致；未勾选则降分");

        return WrapSection("A2 I/O 模块健康度输入", "输入在线率、通道故障和反馈一致性数据", section);
    }

    private Control BuildCommunicationSection()
    {
        _numCommTotal = CreateNumericInput(1, 100, 0, 1, 4);
        _numCommOnline = CreateNumericInput(0, 100, 0, 1, 3);
        _numTimeoutCount = CreateNumericInput(0, 50, 0, 1, 1);
        _numCommFaultEvents = CreateNumericInput(0, 50, 0, 1, 2);
        _numHeartbeatAlive = CreateNumericInput(0, 100, 0, 1, 3);

        var section = CreateSectionTable();
        AddInputRow(section, 0, "通信设备总数 / 在线数", CreateDualNumericPanel(_numCommTotal, _numCommOnline, "总", "在线"), "在线数范围 0~总数；设备在线率越高越好");
        AddInputRow(section, 1, "更新时间异常次数", _numTimeoutCount, "0 次满分；1 次预警；≥4 次记低分/0分");
        AddInputRow(section, 2, "通信故障事件数", _numCommFaultEvents, "0 次满分；1 次预警；≥5 次记低分/0分");
        AddInputRow(section, 3, "心跳正常设备数", _numHeartbeatAlive, "范围 0~通信总数；心跳正常率越高越好");

        return WrapSection("A3 通信健康度输入", "输入通信在线率、心跳状态和异常次数", section);
    }

    private Control BuildInstrumentSection()
    {
        _numStandpipePressure = CreateNumericInput(-50, 200, 1, 0.1M, 18.2M);
        _numPostThrottlePressure = CreateNumericInput(-50, 200, 1, 0.1M, 12.4M);
        _numMainChannelPressure = CreateNumericInput(-50, 200, 1, 0.1M, 18.0M);
        _numAuxiliaryChannelPressure = CreateNumericInput(-50, 200, 1, 0.1M, 17.5M);
        _numHydraulicPressure = CreateNumericInput(-50, 200, 1, 0.1M, 10.2M);
        _numFlowRate = CreateNumericInput(-10, 300, 1, 0.1M, 0);
        _numDensity = CreateNumericInput(0, 5, 2, 0.01M, 1.12M);
        _numHydraulicTemperature = CreateNumericInput(-20, 180, 1, 0.1M, 84M);
        _numValvePositionA = CreateNumericInput(0, 100, 1, 0.1M, 52M);
        _numValvePositionB = CreateNumericInput(0, 100, 1, 0.1M, 48M);
        _cmbFlowMeterStatus = CreateComboBox("正常", "异常", "故障");
        _cmbHydraulicTemperatureStatus = CreateComboBox("正常", "异常", "故障");

        var section = CreateSectionTable();
        AddInputRow(section, 0, "立管压力", _numStandpipePressure, "评分有效范围 0~25；优选区间 15~20");
        AddInputRow(section, 1, "节流后压力", _numPostThrottlePressure, "评分有效范围 0~20；优选区间 10~15");
        AddInputRow(section, 2, "主通道压力", _numMainChannelPressure, "评分有效范围 0~25；优选区间 15~20");
        AddInputRow(section, 3, "辅助通道压力", _numAuxiliaryChannelPressure, "评分有效范围 0~25；优选区间 15~20");
        AddInputRow(section, 4, "液压站压力", _numHydraulicPressure, "评分有效范围 5~16；优选区间 8~12");
        AddInputRow(section, 5, "流量值", _numFlowRate, "评分有效范围 0~150；优选区间 20~100");
        AddInputRow(section, 6, "流量计状态", _cmbFlowMeterStatus, "正常=高分；异常=降分；故障会触发重要扣分");
        AddInputRow(section, 7, "密度值", _numDensity, "评分有效范围 0.8~1.8；优选区间 1.0~1.5");
        AddInputRow(section, 8, "液压站温度", _numHydraulicTemperature, "评分有效范围 0~100；优选区间 30~75；>80 可能触发扣分");
        AddInputRow(section, 9, "液压站温度状态", _cmbHydraulicTemperatureStatus, "正常=高分；异常会触发重要扣分");
        AddInputRow(section, 10, "A/B 节流阀阀位", CreateDualNumericPanel(_numValvePositionA, _numValvePositionB, "A阀位", "B阀位"), "评分有效范围 0~100；优选区间 10~90");

        return WrapSection("A4 检测仪表健康度输入", "输入压力、流量、密度、液压站和阀位反馈数据", section);
    }

    private Control BuildActuatorSection()
    {
        _numProportionalValveCommand = CreateNumericInput(0, 100, 1, 0.1M, 50M);
        _numProportionalValveFeedback = CreateNumericInput(0, 100, 1, 0.1M, 56M);
        _chkFlatValveCommandOpen = CreateCheckBox("平板阀开阀命令", true);
        _chkFlatValveHighLimit = CreateCheckBox("高位到位", false);
        _chkFlatValveLowLimit = CreateCheckBox("低位到位", true);
        _chkHydraulicPumpCommandOn = CreateCheckBox("液压泵启停命令", true);
        _chkHydraulicPumpContactorOn = CreateCheckBox("液压泵接触器反馈", true);
        _chkHeaterCommandOn = CreateCheckBox("电加热命令", false);
        _chkHeaterContactorOn = CreateCheckBox("电加热接触器反馈", false);

        var section = CreateSectionTable();
        AddInputRow(section, 0, "比例阀给定 / 反馈", CreateDualNumericPanel(_numProportionalValveCommand, _numProportionalValveFeedback, "给定", "反馈"), "范围 0~100；允许误差 5，偏差越大得分越低");
        AddInputRow(section, 1, "平板阀命令状态", _chkFlatValveCommandOpen, "勾选表示开阀命令；未勾选表示关阀命令");
        AddInputRow(section, 2, "平板阀高/低位反馈", CreateCheckPanel(_chkFlatValveHighLimit, _chkFlatValveLowLimit), "开阀时应“高位=是、低位=否”；关阀时相反");
        AddInputRow(section, 3, "液压泵命令 / 反馈", CreateCheckPanel(_chkHydraulicPumpCommandOn, _chkHydraulicPumpContactorOn), "两者一致得高分，不一致记 0 分");
        AddInputRow(section, 4, "电加热命令 / 反馈", CreateCheckPanel(_chkHeaterCommandOn, _chkHeaterContactorOn), "两者一致得高分，不一致记 0 分");

        return WrapSection("A5 执行机构健康度输入", "输入比例阀、平板阀、液压泵和电加热状态", section);
    }

    private Control BuildActionArea()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 86,
            Padding = new Padding(0, 8, 0, 0),
            Margin = new Padding(0, 0, 0, 6)
        };

        _btnOpenMonitor = new Button
        {
            Text = "使用输入数据开始评价",
            Width = 220,
            Height = 46,
            Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 174, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnOpenMonitor.FlatAppearance.BorderSize = 0;
        _btnOpenMonitor.Click += BtnOpenMonitor_Click;

        _btnLoadDemo = new Button
        {
            Text = "恢复示例值",
            Width = 140,
            Height = 46,
            Margin = new Padding(12, 0, 0, 0),
            Font = new Font("Microsoft YaHei UI", 11, FontStyle.Bold),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25, 52, 100),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnLoadDemo.FlatAppearance.BorderColor = Color.FromArgb(25, 52, 100);
        _btnLoadDemo.Click += (_, _) => LoadDemoValues();

        var tip = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 26,
            Text = "点击后会先校验输入，再跳转到监控评价界面。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        flow.Controls.Add(_btnOpenMonitor);
        flow.Controls.Add(_btnLoadDemo);

        panel.Controls.Add(tip);
        panel.Controls.Add(flow);
        return panel;
    }

    private void BtnOpenMonitor_Click(object? sender, EventArgs e)
    {
        if (!TryBuildSnapshot(out var snapshot))
        {
            return;
        }

        var monitorForm = new MonitorForm(snapshot);
        monitorForm.FormClosed += (_, _) => Show();
        Hide();
        monitorForm.Show();
    }

    private bool TryBuildSnapshot(out MonitoringSnapshot snapshot)
    {
        snapshot = new MonitoringSnapshot();

        if ((int)_numAiOnline.Value > (int)_numAiTotal.Value)
        {
            ShowValidationError("AI 在线数不能大于 AI 总数。");
            return false;
        }

        if ((int)_numDiOnline.Value > (int)_numDiTotal.Value)
        {
            ShowValidationError("DI 在线数不能大于 DI 总数。");
            return false;
        }

        if ((int)_numAoDoOnline.Value > (int)_numAoDoTotal.Value)
        {
            ShowValidationError("AO/DO 在线数不能大于 AO/DO 总数。");
            return false;
        }

        if ((int)_numIoFaultCount.Value > (int)_numIoTotalChannels.Value)
        {
            ShowValidationError("I/O 故障通道数不能大于通道总数。");
            return false;
        }

        if ((int)_numCommOnline.Value > (int)_numCommTotal.Value)
        {
            ShowValidationError("通信在线设备数不能大于通信设备总数。");
            return false;
        }

        if ((int)_numHeartbeatAlive.Value > (int)_numCommTotal.Value)
        {
            ShowValidationError("心跳正常设备数不能大于通信设备总数。");
            return false;
        }

        snapshot = new MonitoringSnapshot
        {
            CpuStatus = _cmbCpuStatus.Text,
            ScanCycleMs = (double)_numScanCycle.Value,
            CpuLoadPercent = (double)_numCpuLoad.Value,
            MemoryUsagePercent = (double)_numMemoryUsage.Value,
            SevereDiagnosticEvents = (int)_numDiagnosticEvents.Value,

            AiTotal = (int)_numAiTotal.Value,
            AiOnline = (int)_numAiOnline.Value,
            DiTotal = (int)_numDiTotal.Value,
            DiOnline = (int)_numDiOnline.Value,
            AoDoTotal = (int)_numAoDoTotal.Value,
            AoDoOnline = (int)_numAoDoOnline.Value,
            IoChannelFaultCount = (int)_numIoFaultCount.Value,
            IoTotalChannels = (int)_numIoTotalChannels.Value,
            IoFeedbackConsistent = _chkIoFeedbackConsistent.Checked,

            CommunicationDevicesTotal = (int)_numCommTotal.Value,
            CommunicationDevicesOnline = (int)_numCommOnline.Value,
            UpdateTimeoutCount = (int)_numTimeoutCount.Value,
            CommunicationFaultEvents = (int)_numCommFaultEvents.Value,
            HeartbeatAliveCount = (int)_numHeartbeatAlive.Value,

            StandpipePressure = (double)_numStandpipePressure.Value,
            PostThrottlePressure = (double)_numPostThrottlePressure.Value,
            MainChannelPressure = (double)_numMainChannelPressure.Value,
            AuxiliaryChannelPressure = (double)_numAuxiliaryChannelPressure.Value,
            HydraulicPressure = (double)_numHydraulicPressure.Value,
            FlowRate = (double)_numFlowRate.Value,
            Density = (double)_numDensity.Value,
            HydraulicTemperature = (double)_numHydraulicTemperature.Value,
            ValvePositionA = (double)_numValvePositionA.Value,
            ValvePositionB = (double)_numValvePositionB.Value,
            FlowMeterStatus = _cmbFlowMeterStatus.Text,
            HydraulicTemperatureStatus = _cmbHydraulicTemperatureStatus.Text,

            ProportionalValveCommand = (double)_numProportionalValveCommand.Value,
            ProportionalValveFeedback = (double)_numProportionalValveFeedback.Value,
            FlatValveCommandOpen = _chkFlatValveCommandOpen.Checked,
            FlatValveHighLimit = _chkFlatValveHighLimit.Checked,
            FlatValveLowLimit = _chkFlatValveLowLimit.Checked,
            HydraulicPumpCommandOn = _chkHydraulicPumpCommandOn.Checked,
            HydraulicPumpContactorOn = _chkHydraulicPumpContactorOn.Checked,
            HeaterCommandOn = _chkHeaterCommandOn.Checked,
            HeaterContactorOn = _chkHeaterContactorOn.Checked
        };

        return true;
    }

    private void LoadDemoValues()
    {
        _cmbCpuStatus.SelectedItem = "RUN";
        _numScanCycle.Value = 18;
        _numCpuLoad.Value = 72;
        _numMemoryUsage.Value = 68;
        _numDiagnosticEvents.Value = 1;

        _numAiTotal.Value = 8;
        _numAiOnline.Value = 8;
        _numDiTotal.Value = 6;
        _numDiOnline.Value = 5;
        _numAoDoTotal.Value = 6;
        _numAoDoOnline.Value = 6;
        _numIoFaultCount.Value = 1;
        _numIoTotalChannels.Value = 20;
        _chkIoFeedbackConsistent.Checked = true;

        _numCommTotal.Value = 4;
        _numCommOnline.Value = 3;
        _numTimeoutCount.Value = 1;
        _numCommFaultEvents.Value = 2;
        _numHeartbeatAlive.Value = 3;

        _numStandpipePressure.Value = 18.2M;
        _numPostThrottlePressure.Value = 12.4M;
        _numMainChannelPressure.Value = 18.0M;
        _numAuxiliaryChannelPressure.Value = 17.5M;
        _numHydraulicPressure.Value = 10.2M;
        _numFlowRate.Value = 0M;
        _numDensity.Value = 1.12M;
        _numHydraulicTemperature.Value = 84M;
        _numValvePositionA.Value = 52M;
        _numValvePositionB.Value = 48M;
        _cmbFlowMeterStatus.SelectedItem = "故障";
        _cmbHydraulicTemperatureStatus.SelectedItem = "异常";

        _numProportionalValveCommand.Value = 50M;
        _numProportionalValveFeedback.Value = 56M;
        _chkFlatValveCommandOpen.Checked = true;
        _chkFlatValveHighLimit.Checked = false;
        _chkFlatValveLowLimit.Checked = true;
        _chkHydraulicPumpCommandOn.Checked = true;
        _chkHydraulicPumpContactorOn.Checked = true;
        _chkHeaterCommandOn.Checked = false;
        _chkHeaterContactorOn.Checked = false;
    }

    private static TableLayoutPanel CreateSectionTable()
    {
        return new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Color.White
        };
    }

    private static Panel WrapSection(string title, string subTitle, TableLayoutPanel section)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(18, 16, 18, 18),
            Margin = new Padding(0, 0, 0, 14),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

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
            Height = 22,
            Text = subTitle,
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };

        section.Dock = DockStyle.Top;
        section.Margin = new Padding(0, 12, 0, 0);
        section.BringToFront();

        panel.Controls.Add(section);
        panel.Controls.Add(subTitleLabel);
        panel.Controls.Add(titleLabel);
        return panel;
    }

    private static void AddInputRow(TableLayoutPanel table, int rowIndex, string name, Control editor, string hint)
    {
        while (table.RowCount <= rowIndex)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowCount++;
        }

        if (table.ColumnStyles.Count == 0)
        {
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        }

        var label = new Label
        {
            Dock = DockStyle.Fill,
            Text = name,
            Font = new Font("Microsoft YaHei UI", 10),
            ForeColor = Color.FromArgb(55, 71, 92),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 0, 8, 10),
            Height = 36
        };

        editor.Dock = DockStyle.Fill;
        editor.Margin = new Padding(0, 0, 8, 10);
        editor.Height = 36;

        var hintLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = hint,
            Font = new Font("Microsoft YaHei UI", 9.3F),
            ForeColor = Color.FromArgb(104, 115, 132),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 0, 0, 10),
            AutoEllipsis = true
        };

        table.Controls.Add(label, 0, rowIndex);
        table.Controls.Add(editor, 1, rowIndex);
        table.Controls.Add(hintLabel, 2, rowIndex);
    }

    private static ComboBox CreateComboBox(params string[] items)
    {
        var combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Microsoft YaHei UI", 10),
            BackColor = Color.White
        };
        combo.Items.AddRange(items);
        if (combo.Items.Count > 0)
        {
            combo.SelectedIndex = 0;
        }
        return combo;
    }

    private static NumericUpDown CreateNumericInput(decimal minimum, decimal maximum, int decimalPlaces, decimal increment, decimal defaultValue)
    {
        return new NumericUpDown
        {
            Minimum = minimum,
            Maximum = maximum,
            DecimalPlaces = decimalPlaces,
            Increment = increment,
            Value = Math.Min(maximum, Math.Max(minimum, defaultValue)),
            Font = new Font("Microsoft YaHei UI", 10),
            ThousandsSeparator = decimalPlaces == 0,
            TextAlign = HorizontalAlignment.Right
        };
    }

    private static CheckBox CreateCheckBox(string text, bool isChecked)
    {
        return new CheckBox
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            Font = new Font("Microsoft YaHei UI", 10),
            ForeColor = Color.FromArgb(55, 71, 92),
            Margin = new Padding(0, 6, 16, 0)
        };
    }

    private static Control CreateDualNumericPanel(NumericUpDown left, NumericUpDown right, string leftLabel, string rightLabel)
    {
        var panel = new TableLayoutPanel
        {
            ColumnCount = 4,
            RowCount = 1,
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var leftText = new Label
        {
            Dock = DockStyle.Fill,
            Text = leftLabel,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 9.3F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };
        var rightText = new Label
        {
            Dock = DockStyle.Fill,
            Text = rightLabel,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Microsoft YaHei UI", 9.3F),
            ForeColor = Color.FromArgb(104, 115, 132)
        };

        left.Dock = DockStyle.Fill;
        right.Dock = DockStyle.Fill;

        panel.Controls.Add(leftText, 0, 0);
        panel.Controls.Add(left, 1, 0);
        panel.Controls.Add(rightText, 2, 0);
        panel.Controls.Add(right, 3, 0);
        return panel;
    }

    private static Control CreateCheckPanel(params CheckBox[] checkBoxes)
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White
        };

        foreach (var checkBox in checkBoxes)
        {
            panel.Controls.Add(checkBox);
        }

        return panel;
    }

    private static Control CreateInfoLine(string labelText, string valueText)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(247, 250, 254),
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(0, 0, 0, 10)
        };

        var inner = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(247, 250, 254),
            Margin = new Padding(0)
        };
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inner.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88));

        var label = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = labelText,
            Font = new Font("Microsoft YaHei UI", 9.8F),
            ForeColor = Color.FromArgb(55, 71, 92),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var value = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Text = valueText,
            Font = new Font("Microsoft YaHei UI", 10.8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 123, 255),
            TextAlign = ContentAlignment.MiddleRight
        };

        inner.Controls.Add(label, 0, 0);
        inner.Controls.Add(value, 1, 0);
        panel.Controls.Add(inner);
        return panel;
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

    private void ShowValidationError(string message)
    {
        MessageBox.Show(this, message, "输入校验失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private Control BuildFooter()
    {
        return new Label
        {
            Dock = DockStyle.Fill,
            Text = "当前首页已支持手工录入评价数据；后续可继续接入 PLC、数据库或 OPC UA 实时数据源。",
            Font = new Font("Microsoft YaHei UI", 9.5F),
            ForeColor = Color.FromArgb(105, 117, 132),
            TextAlign = ContentAlignment.MiddleRight
        };
    }
}