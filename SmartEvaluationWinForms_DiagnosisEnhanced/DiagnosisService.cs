namespace SmartEvaluationWinForms;

public static class DiagnosisService
{
    public static DiagnosisReport Analyze(MonitoringSnapshot snapshot, EvaluationResult result)
    {
        var report = new DiagnosisReport();
        var items = new List<DiagnosisItem>();

        MapDeductions(result.Deductions, items);
        AnalyzeOverall(result, items);
        AnalyzeController(snapshot, result, items);
        AnalyzeIo(snapshot, result, items);
        AnalyzeCommunication(snapshot, result, items);
        AnalyzeInstrument(snapshot, result, items);
        AnalyzeActuator(snapshot, result, items);

        report.Items = items
            .OrderByDescending(x => GetSeverityRank(x.Severity))
            .ThenBy(x => x.IsPredicted)
            .ThenBy(x => x.Category)
            .ThenBy(x => x.Title)
            .ToList();

        report.OverallStatus = BuildOverallStatus(report);
        report.Summary = BuildSummary(report, result);
        return report;
    }

    private static void MapDeductions(IEnumerable<DeductionItem> deductions, List<DiagnosisItem> items)
    {
        foreach (var deduction in deductions)
        {
            switch (deduction.Item)
            {
                case "CPU停机":
                    AddOrUpdate(items, DiagnosisSeverity.Critical, "控制器", "控制器停机故障",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议立即切换安全工况并检查 PLC/控制器电源、程序运行状态和诊断缓冲区。",
                        isPredicted: false);
                    break;

                case "关键通信设备离线":
                    AddOrUpdate(items, DiagnosisSeverity.Warning, "通信", "关键通信链路故障",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议检查交换机、网线、地址配置、从站供电和通信模块状态。",
                        isPredicted: false);
                    break;

                case "质量流量计故障":
                    AddOrUpdate(items, DiagnosisSeverity.Warning, "检测仪表", "流量检测故障",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议核查流量计供电、信号线路、传感器本体和采样工况。",
                        isPredicted: false);
                    break;

                case "关键阀门拒动或限位异常":
                    AddOrUpdate(items, DiagnosisSeverity.Warning, "执行机构", "平板阀执行异常",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议检查阀门机械卡阻、液压执行回路、限位开关和命令反馈链路。",
                        isPredicted: false);
                    break;

                case "液压站温度超限":
                    AddOrUpdate(items, DiagnosisSeverity.Warning, "液压站", "液压站过热",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议检查散热风扇、油液状态、冷却回路和持续负载情况。",
                        isPredicted: false);
                    break;

                case "单点I/O或反馈异常":
                    AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "I/O模块", "I/O 通道劣化预警",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议尽快排查异常通道、端子接触、模块供电以及反馈采集信号。",
                        isPredicted: true);
                    break;

                case "非关键通信抖动":
                    AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "通信", "通信稳定性下降预警",
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议检查网络拥塞、轮询周期设置、报文超时参数和节点响应延迟。",
                        isPredicted: true);
                    break;

                default:
                    AddOrUpdate(items, MapSeverity(deduction.Level), "系统", deduction.Item,
                        $"触发扣分：{deduction.Points:F0} 分",
                        deduction.Description,
                        "建议结合现场实际工况进一步排查。",
                        isPredicted: deduction.Level == FaultLevel.Minor);
                    break;
            }
        }
    }

    private static void AnalyzeOverall(EvaluationResult result, List<DiagnosisItem> items)
    {
        if (result.FinalScore < 60)
        {
            AddOrUpdate(items, DiagnosisSeverity.Critical, "系统", "系统综合状态危险",
                $"综合得分={result.FinalScore:F2}",
                "综合得分已低于 60 分，说明系统已存在明显故障或多项异常叠加。",
                "建议立即组织人工复核，必要时停机检查。",
                isPredicted: false);
        }
        else if (result.FinalScore < 70)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "系统", "系统综合状态较差",
                $"综合得分={result.FinalScore:F2}",
                "综合得分处于 60~70 分区间，系统运行风险较高。",
                "建议优先处理扣分项和低分一级指标。",
                isPredicted: false);
        }
        else if (result.FinalScore < 80)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "系统", "综合评分进入预警区",
                $"综合得分={result.FinalScore:F2}",
                "综合得分处于 70~80 分区间，说明系统已有劣化趋势。",
                "建议安排巡检并重点关注预警项。",
                isPredicted: true);
        }

        if (result.DeductionScore >= 80)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "系统", "累计扣分偏高",
                $"累计扣分={result.DeductionScore:F2}",
                "当前扣分合计较高，说明存在多项异常同时出现。",
                "建议按扣分项优先级逐项处置，避免故障耦合扩大。",
                isPredicted: false);
        }
    }

    private static void AnalyzeController(MonitoringSnapshot s, EvaluationResult result, List<DiagnosisItem> items)
    {
        var a1 = GetPrimaryScore(result, "A1");

        if (s.SevereDiagnosticEvents >= 2)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "控制器", "控制器诊断事件持续增加",
                $"严重诊断事件数={s.SevereDiagnosticEvents}",
                "严重诊断事件已达到 2 次及以上，控制器存在异常累积风险。",
                "建议检查程序异常、模块告警和系统日志。",
                isPredicted: false);
        }
        else if (s.SevereDiagnosticEvents == 1)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "控制器", "控制器异常事件预警",
                "严重诊断事件数=1",
                "控制器已出现一次严重诊断事件，可能是故障前兆。",
                "建议复核诊断缓冲区并持续观察。",
                isPredicted: true);
        }

        if (s.ScanCycleMs > 25 && s.ScanCycleMs <= 40)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "控制器", "扫描周期变长预警",
                $"扫描周期={s.ScanCycleMs:F1} ms",
                "扫描周期已偏离优选区间，控制器响应有变慢趋势。",
                "建议检查程序负载、通信轮询和后台任务占用。",
                isPredicted: true);
        }

        if (s.CpuLoadPercent >= 85 || s.MemoryUsagePercent >= 88)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "控制器", "控制器资源逼近上限",
                $"CPU负载={s.CpuLoadPercent:F1}% / 存储占用={s.MemoryUsagePercent:F1}%",
                "CPU 或存储占用率接近上限，可能导致扫描周期波动和响应迟滞。",
                "建议优化任务负载并排查异常数据刷新。",
                isPredicted: true);
        }

        if (a1 > 0 && a1 < 70 && string.Equals(s.CpuStatus, "RUN", StringComparison.OrdinalIgnoreCase))
        {
            AddOrUpdate(items, a1 < 60 ? DiagnosisSeverity.Warning : DiagnosisSeverity.EarlyWarning, "控制器", "控制器健康度下降",
                $"A1 得分={a1:F2}",
                "控制器一级指标得分偏低，说明资源占用、扫描周期或事件数已明显恶化。",
                "建议优先排查控制器侧参数和告警来源。",
                isPredicted: a1 >= 60);
        }
    }

    private static void AnalyzeIo(MonitoringSnapshot s, EvaluationResult result, List<DiagnosisItem> items)
    {
        var a2 = GetPrimaryScore(result, "A2");
        var aiRate = SafeRate(s.AiOnline, s.AiTotal);
        var diRate = SafeRate(s.DiOnline, s.DiTotal);
        var aoRate = SafeRate(s.AoDoOnline, s.AoDoTotal);
        var minRate = Math.Min(aiRate, Math.Min(diRate, aoRate));
        var faultRate = SafeRate(s.IoChannelFaultCount, s.IoTotalChannels);

        if (!s.IoFeedbackConsistent)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "I/O模块", "I/O 命令反馈不一致",
                "命令反馈一致性=否",
                "I/O 命令与反馈不一致，说明采集链路或现场执行反馈可能异常。",
                "建议检查点位映射、接线和反馈采集模块。",
                isPredicted: false);
        }

        if (minRate < 0.9)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "I/O模块", "I/O 在线率偏低",
                $"最低在线率={minRate:P0}",
                "I/O 模块在线率已低于 90%，存在实际离线风险。",
                "建议排查模块供电、总线连接和地址配置。",
                isPredicted: false);
        }
        else if (minRate < 1.0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "I/O模块", "I/O 在线率下降预警",
                $"最低在线率={minRate:P0}",
                "I/O 在线率未达到 100%，系统已出现轻度离线迹象。",
                "建议巡检模块状态灯、接插件和通讯总线。",
                isPredicted: true);
        }

        if (faultRate > 0.05)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "I/O模块", "I/O 通道故障扩散",
                $"故障通道占比={faultRate:P1}",
                "I/O 故障通道占比已超过 5%，异常有扩散趋势。",
                "建议对故障通道进行分组定位和批量排查。",
                isPredicted: false);
        }

        if (a2 > 0 && a2 < 70 && minRate >= 0.9 && s.IoFeedbackConsistent)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "I/O模块", "I/O 模块健康度下降",
                $"A2 得分={a2:F2}",
                "I/O 一级指标得分偏低，说明在线率或通道质量已出现下降。",
                "建议检查边缘模块和高频变化点位。",
                isPredicted: true);
        }
    }

    private static void AnalyzeCommunication(MonitoringSnapshot s, EvaluationResult result, List<DiagnosisItem> items)
    {
        var a3 = GetPrimaryScore(result, "A3");
        var onlineRate = SafeRate(s.CommunicationDevicesOnline, s.CommunicationDevicesTotal);
        var heartbeatRate = SafeRate(s.HeartbeatAliveCount, s.CommunicationDevicesTotal);

        if (s.UpdateTimeoutCount >= 2 || s.CommunicationFaultEvents >= 3)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "通信", "通信时延异常",
                $"更新时间异常={s.UpdateTimeoutCount}，通信故障事件={s.CommunicationFaultEvents}",
                "通信异常次数较多，链路稳定性已明显下降。",
                "建议检查交换机负载、轮询周期和超时重发参数。",
                isPredicted: false);
        }
        else if (s.UpdateTimeoutCount == 1 || s.CommunicationFaultEvents > 0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "通信", "通信抖动预警",
                $"更新时间异常={s.UpdateTimeoutCount}，通信故障事件={s.CommunicationFaultEvents}",
                "已出现少量超时或故障事件，存在通信抖动趋势。",
                "建议关注网络负载峰值和节点响应。",
                isPredicted: true);
        }

        if (heartbeatRate < 1.0 && onlineRate >= 1.0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "通信", "设备心跳异常预警",
                $"心跳正常率={heartbeatRate:P0}",
                "设备在线但心跳不全正常，说明通信质量开始波动。",
                "建议检查心跳周期配置和报文丢失情况。",
                isPredicted: true);
        }

        if (a3 > 0 && a3 < 70 && onlineRate >= 1.0 && heartbeatRate >= 1.0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "通信", "通信健康度下降",
                $"A3 得分={a3:F2}",
                "通信一级指标得分偏低，说明稳定性指标已经恶化。",
                "建议复核更新时间异常和历史告警记录。",
                isPredicted: true);
        }
    }

    private static void AnalyzeInstrument(MonitoringSnapshot s, EvaluationResult result, List<DiagnosisItem> items)
    {
        var a4 = GetPrimaryScore(result, "A4");
        var pressureOutOfOptimal = new List<string>();
        var pressureOutOfRange = new List<string>();

        CheckPressure("立管压力", s.StandpipePressure, 0, 25, 15, 20, pressureOutOfOptimal, pressureOutOfRange);
        CheckPressure("节流后压力", s.PostThrottlePressure, 0, 20, 10, 15, pressureOutOfOptimal, pressureOutOfRange);
        CheckPressure("主通道压力", s.MainChannelPressure, 0, 25, 15, 20, pressureOutOfOptimal, pressureOutOfRange);
        CheckPressure("辅助通道压力", s.AuxiliaryChannelPressure, 0, 25, 15, 20, pressureOutOfOptimal, pressureOutOfRange);
        CheckPressure("液压站压力", s.HydraulicPressure, 5, 16, 8, 12, pressureOutOfOptimal, pressureOutOfRange);

        if (pressureOutOfRange.Count > 0)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "检测仪表", "压力测点严重偏离量程",
                string.Join("；", pressureOutOfRange),
                "部分压力值已超出评分有效范围，存在测点异常或过程异常的可能。",
                "建议核查传感器本体、取压点堵塞和现场压力工况。",
                isPredicted: false);
        }
        else if (pressureOutOfOptimal.Count > 0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "检测仪表", "压力测点偏离优选区间",
                string.Join("；", pressureOutOfOptimal),
                "压力值虽仍在有效范围内，但已偏离优选区间，存在劣化趋势。",
                "建议结合工况变化持续观察压力曲线。",
                isPredicted: true);
        }

        if (string.Equals(s.FlowMeterStatus, "异常", StringComparison.OrdinalIgnoreCase))
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "检测仪表", "流量计测量漂移预警",
                $"流量计状态={s.FlowMeterStatus}",
                "流量计状态已从正常转为异常，可能是故障前兆。",
                "建议核查供电、接线和传感器零点漂移。",
                isPredicted: true);
        }

        if (s.FlowRate > 0.01 && s.FlowRate < 20)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "检测仪表", "流量信号偏低预警",
                $"流量值={s.FlowRate:F1}",
                "流量值仍有输出但已接近下限，存在信号衰减或工况异常趋势。",
                "建议结合工艺状态核查测量链路。",
                isPredicted: true);
        }

        if (s.HydraulicTemperature >= 75 && s.HydraulicTemperature <= 80 && !string.Equals(s.HydraulicTemperatureStatus, "异常", StringComparison.OrdinalIgnoreCase))
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "液压站", "液压站温升预警",
                $"液压温度={s.HydraulicTemperature:F1}°C",
                "液压站温度接近告警阈值，存在继续升温风险。",
                "建议检查冷却效率和油液循环状态。",
                isPredicted: true);
        }

        if ((s.Density >= 0.8 && s.Density < 1.0) || (s.Density > 1.5 && s.Density <= 1.8))
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "检测仪表", "密度测量偏离优选区间",
                $"密度={s.Density:F2}",
                "密度值已偏离优选区间，可能预示传感器偏移或工艺波动。",
                "建议校验密度仪或比对工艺设定值。",
                isPredicted: true);
        }

        if (a4 > 0 && a4 < 70 && pressureOutOfOptimal.Count == 0 && pressureOutOfRange.Count == 0)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "检测仪表", "检测仪表健康度下降",
                $"A4 得分={a4:F2}",
                "仪表一级指标得分偏低，测量链路整体稳定性已经下降。",
                "建议联合检查压力、流量、温度和阀位反馈。",
                isPredicted: true);
        }
    }

    private static void AnalyzeActuator(MonitoringSnapshot s, EvaluationResult result, List<DiagnosisItem> items)
    {
        var a5 = GetPrimaryScore(result, "A5");
        var diff = Math.Abs(s.ProportionalValveCommand - s.ProportionalValveFeedback);

        if (diff > 10)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "执行机构", "比例阀跟踪偏差过大",
                $"给定-反馈偏差={diff:F1}",
                "比例阀反馈明显滞后于给定值，执行响应已出现实质异常。",
                "建议检查阀芯卡滞、比例阀驱动和反馈采集。",
                isPredicted: false);
        }
        else if (diff > 5)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "执行机构", "比例阀响应滞后预警",
                $"给定-反馈偏差={diff:F1}",
                "比例阀偏差已超过允许误差，存在执行迟滞趋势。",
                "建议巡检比例阀和反馈元件。",
                isPredicted: true);
        }

        if (s.HydraulicPumpCommandOn != s.HydraulicPumpContactorOn)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "执行机构", "液压泵执行回路异常",
                $"命令={s.HydraulicPumpCommandOn} / 反馈={s.HydraulicPumpContactorOn}",
                "液压泵命令与接触器反馈不一致，存在执行回路故障。",
                "建议检查接触器、电气回路和联锁条件。",
                isPredicted: false);
        }

        if (s.HeaterCommandOn != s.HeaterContactorOn)
        {
            AddOrUpdate(items, DiagnosisSeverity.Warning, "执行机构", "电加热执行回路异常",
                $"命令={s.HeaterCommandOn} / 反馈={s.HeaterContactorOn}",
                "电加热命令与接触器反馈不一致，执行链路存在异常。",
                "建议检查继电器、接触器和反馈触点。",
                isPredicted: false);
        }

        if (a5 > 0 && a5 < 70 && diff <= 5 && s.HydraulicPumpCommandOn == s.HydraulicPumpContactorOn && s.HeaterCommandOn == s.HeaterContactorOn)
        {
            AddOrUpdate(items, DiagnosisSeverity.EarlyWarning, "执行机构", "执行机构健康度下降",
                $"A5 得分={a5:F2}",
                "执行机构一级指标得分偏低，说明执行链路存在潜在性能衰减。",
                "建议重点检查阀门动作时间和反馈一致性。",
                isPredicted: true);
        }
    }

    private static void CheckPressure(string name, double value, double min, double max, double optimalLow, double optimalHigh, List<string> outOfOptimal, List<string> outOfRange)
    {
        if (value < min || value > max)
        {
            outOfRange.Add($"{name}={value:F1}");
            return;
        }

        if (value < optimalLow || value > optimalHigh)
        {
            outOfOptimal.Add($"{name}={value:F1}");
        }
    }

    private static void AddOrUpdate(
        List<DiagnosisItem> items,
        DiagnosisSeverity severity,
        string category,
        string title,
        string trigger,
        string reason,
        string recommendation,
        bool isPredicted)
    {
        var existing = items.FirstOrDefault(x => x.Title == title);
        var newItem = new DiagnosisItem
        {
            Severity = severity,
            Category = category,
            Title = title,
            Trigger = trigger,
            Reason = reason,
            Recommendation = recommendation,
            IsPredicted = isPredicted
        };

        if (existing is null)
        {
            items.Add(newItem);
            return;
        }

        if (GetSeverityRank(severity) > GetSeverityRank(existing.Severity))
        {
            existing.Severity = severity;
            existing.Category = category;
            existing.Title = title;
            existing.Trigger = trigger;
            existing.Reason = reason;
            existing.Recommendation = recommendation;
            existing.IsPredicted = isPredicted;
        }
    }

    private static string BuildOverallStatus(DiagnosisReport report)
    {
        if (report.Items.Any(x => x.Severity == DiagnosisSeverity.Critical))
            return "红色告警：存在重大故障，请立即处置";
        if (report.Items.Any(x => x.Severity == DiagnosisSeverity.Warning && !x.IsPredicted))
            return "橙色告警：存在明确异常，请尽快处理";
        if (report.Items.Any())
            return "黄色预警：存在故障前兆，请安排巡检";

        return "绿色运行：当前未发现故障和预警";
    }

    private static string BuildSummary(DiagnosisReport report, EvaluationResult result)
    {
        if (!report.HasAlert)
        {
            return $"综合得分 {result.FinalScore:F2}，当前未识别到明确故障和预警趋势。";
        }

        return $"综合得分 {result.FinalScore:F2}，累计扣分 {result.DeductionScore:F2}。当前识别出 {report.CurrentFaultCount} 项故障/异常，{report.EarlyWarningCount} 项预警趋势。";
    }

    private static DiagnosisSeverity MapSeverity(FaultLevel level)
    {
        return level switch
        {
            FaultLevel.Major => DiagnosisSeverity.Critical,
            FaultLevel.Important => DiagnosisSeverity.Warning,
            _ => DiagnosisSeverity.EarlyWarning
        };
    }

    private static int GetSeverityRank(DiagnosisSeverity severity)
    {
        return severity switch
        {
            DiagnosisSeverity.Critical => 3,
            DiagnosisSeverity.Warning => 2,
            _ => 1
        };
    }

    private static double GetPrimaryScore(EvaluationResult result, string code)
    {
        return result.PrimaryIndicators.FirstOrDefault(x => x.Code == code)?.Score ?? 0;
    }

    private static double SafeRate(int current, int total)
    {
        return total <= 0 ? 0 : (double)current / total;
    }
}
