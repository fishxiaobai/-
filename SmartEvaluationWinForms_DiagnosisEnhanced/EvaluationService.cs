namespace SmartEvaluationWinForms;

public static class EvaluationService
{
    private const double A1Weight = 0.19;
    private const double A2Weight = 0.19;
    private const double A3Weight = 0.11;
    private const double A4Weight = 0.33;
    private const double A5Weight = 0.19;

    public static EvaluationResult Evaluate(MonitoringSnapshot s)
    {
        var result = new EvaluationResult();

        var a1 = BuildControllerHealth(s);
        var a2 = BuildIoHealth(s);
        var a3 = BuildCommunicationHealth(s);
        var a4 = BuildInstrumentHealth(s);
        var a5 = BuildActuatorHealth(s);

        result.PrimaryIndicators.AddRange(new[] { a1, a2, a3, a4, a5 });
        result.BaseScore = result.PrimaryIndicators.Sum(x => x.Score * x.Weight);

        ApplyDeductions(s, result.Deductions);
        result.DeductionScore = result.Deductions.Sum(x => x.Points);
        result.FinalScore = Math.Max(0, Math.Round(result.BaseScore - result.DeductionScore, 2));
        result.Judgment = Judge(result.FinalScore);

        return result;
    }

    private static PrimaryIndicatorResult BuildControllerHealth(MonitoringSnapshot s)
    {
        var items = new List<SecondaryIndicatorResult>
        {
            new()
            {
                PrimaryIndicator = "控制器健康度",
                Name = "CPU运行状态",
                Weight = 0.30,
                Score = MapStateScore(s.CpuStatus),
                Detail = $"CPU状态={s.CpuStatus}"
            },
            new()
            {
                PrimaryIndicator = "控制器健康度",
                Name = "扫描周期",
                Weight = 0.25,
                Score = RangeScore(s.ScanCycleMs, 0, 40, 10, 20),
                Detail = $"扫描周期={s.ScanCycleMs:F1} ms"
            },
            new()
            {
                PrimaryIndicator = "控制器健康度",
                Name = "CPU负载率",
                Weight = 0.15,
                Score = InverseRangeScore(s.CpuLoadPercent, 0, 95, 0, 70),
                Detail = $"负载率={s.CpuLoadPercent:F1}%"
            },
            new()
            {
                PrimaryIndicator = "控制器健康度",
                Name = "存储占用率",
                Weight = 0.10,
                Score = InverseRangeScore(s.MemoryUsagePercent, 0, 95, 0, 75),
                Detail = $"存储占用率={s.MemoryUsagePercent:F1}%"
            },
            new()
            {
                PrimaryIndicator = "控制器健康度",
                Name = "严重诊断事件数",
                Weight = 0.20,
                Score = CountPenaltyScore(s.SevereDiagnosticEvents, 0, 1, 3),
                Detail = $"严重诊断事件={s.SevereDiagnosticEvents}"
            }
        };

        return BuildPrimary("A1", "控制器健康度", A1Weight, items);
    }

    private static PrimaryIndicatorResult BuildIoHealth(MonitoringSnapshot s)
    {
        var aiOnlineRate = RatioScore(s.AiOnline, s.AiTotal);
        var diOnlineRate = RatioScore(s.DiOnline, s.DiTotal);
        var aoDoOnlineRate = RatioScore(s.AoDoOnline, s.AoDoTotal);
        var channelFaultRateScore = FaultRateScore(s.IoChannelFaultCount, s.IoTotalChannels);
        var consistencyScore = s.IoFeedbackConsistent ? 100 : 60;

        var items = new List<SecondaryIndicatorResult>
        {
            new()
            {
                PrimaryIndicator = "I/O模块健康度",
                Name = "AI在线率",
                Weight = 0.20,
                Score = aiOnlineRate,
                Detail = $"AI在线={s.AiOnline}/{s.AiTotal}"
            },
            new()
            {
                PrimaryIndicator = "I/O模块健康度",
                Name = "DI在线率",
                Weight = 0.20,
                Score = diOnlineRate,
                Detail = $"DI在线={s.DiOnline}/{s.DiTotal}"
            },
            new()
            {
                PrimaryIndicator = "I/O模块健康度",
                Name = "AO/DO在线率",
                Weight = 0.20,
                Score = aoDoOnlineRate,
                Detail = $"AO/DO在线={s.AoDoOnline}/{s.AoDoTotal}"
            },
            new()
            {
                PrimaryIndicator = "I/O模块健康度",
                Name = "通道故障率",
                Weight = 0.20,
                Score = channelFaultRateScore,
                Detail = $"故障通道={s.IoChannelFaultCount}/{s.IoTotalChannels}"
            },
            new()
            {
                PrimaryIndicator = "I/O模块健康度",
                Name = "命令反馈一致性",
                Weight = 0.20,
                Score = consistencyScore,
                Detail = $"I/O反馈一致={s.IoFeedbackConsistent}"
            }
        };

        return BuildPrimary("A2", "I/O模块健康度", A2Weight, items);
    }

    private static PrimaryIndicatorResult BuildCommunicationHealth(MonitoringSnapshot s)
    {
        var items = new List<SecondaryIndicatorResult>
        {
            new()
            {
                PrimaryIndicator = "通信健康度",
                Name = "设备在线率",
                Weight = 0.30,
                Score = RatioScore(s.CommunicationDevicesOnline, s.CommunicationDevicesTotal),
                Detail = $"在线设备={s.CommunicationDevicesOnline}/{s.CommunicationDevicesTotal}"
            },
            new()
            {
                PrimaryIndicator = "通信健康度",
                Name = "更新时间异常",
                Weight = 0.20,
                Score = CountPenaltyScore(s.UpdateTimeoutCount, 0, 1, 4),
                Detail = $"更新时间异常={s.UpdateTimeoutCount}"
            },
            new()
            {
                PrimaryIndicator = "通信健康度",
                Name = "通信故障事件数",
                Weight = 0.20,
                Score = CountPenaltyScore(s.CommunicationFaultEvents, 0, 1, 5),
                Detail = $"通信故障事件={s.CommunicationFaultEvents}"
            },
            new()
            {
                PrimaryIndicator = "通信健康度",
                Name = "设备心跳状态",
                Weight = 0.30,
                Score = RatioScore(s.HeartbeatAliveCount, s.CommunicationDevicesTotal),
                Detail = $"心跳正常={s.HeartbeatAliveCount}/{s.CommunicationDevicesTotal}"
            }
        };

        return BuildPrimary("A3", "通信健康度", A3Weight, items);
    }

    private static PrimaryIndicatorResult BuildInstrumentHealth(MonitoringSnapshot s)
    {
        var pressureScores = new[]
        {
            RangeScore(s.StandpipePressure, 0, 25, 15, 20),
            RangeScore(s.PostThrottlePressure, 0, 20, 10, 15),
            RangeScore(s.MainChannelPressure, 0, 25, 15, 20),
            RangeScore(s.AuxiliaryChannelPressure, 0, 25, 15, 20),
            RangeScore(s.HydraulicPressure, 5, 16, 8, 12)
        };

        var valveFeedbackScore = Average(
            RangeScore(s.ValvePositionA, 0, 100, 10, 90),
            RangeScore(s.ValvePositionB, 0, 100, 10, 90));

        var items = new List<SecondaryIndicatorResult>
        {
            new()
            {
                PrimaryIndicator = "检测仪表健康度",
                Name = "压力测量状态",
                Weight = 0.30,
                Score = Average(pressureScores),
                Detail = "立管/节流后/主辅通道/液压站压力综合评分"
            },
            new()
            {
                PrimaryIndicator = "检测仪表健康度",
                Name = "流量测量状态",
                Weight = 0.25,
                Score = Math.Min(MapStateScore(s.FlowMeterStatus), RangeScore(s.FlowRate, 0, 150, 20, 100)),
                Detail = $"流量={s.FlowRate:F1}, 状态={s.FlowMeterStatus}"
            },
            new()
            {
                PrimaryIndicator = "检测仪表健康度",
                Name = "密度测量状态",
                Weight = 0.10,
                Score = RangeScore(s.Density, 0.8, 1.8, 1.0, 1.5),
                Detail = $"密度={s.Density:F2}"
            },
            new()
            {
                PrimaryIndicator = "检测仪表健康度",
                Name = "液压站状态",
                Weight = 0.15,
                Score = Math.Min(MapStateScore(s.HydraulicTemperatureStatus), RangeScore(s.HydraulicTemperature, 0, 100, 30, 75)),
                Detail = $"液压温度={s.HydraulicTemperature:F1}°C, 状态={s.HydraulicTemperatureStatus}"
            },
            new()
            {
                PrimaryIndicator = "检测仪表健康度",
                Name = "阀位反馈状态",
                Weight = 0.20,
                Score = valveFeedbackScore,
                Detail = $"A阀位={s.ValvePositionA:F1}, B阀位={s.ValvePositionB:F1}"
            }
        };

        return BuildPrimary("A4", "检测仪表健康度", A4Weight, items);
    }

    private static PrimaryIndicatorResult BuildActuatorHealth(MonitoringSnapshot s)
    {
        var flatValveConsistent = s.FlatValveCommandOpen
            ? s.FlatValveHighLimit && !s.FlatValveLowLimit
            : !s.FlatValveHighLimit && s.FlatValveLowLimit;

        var items = new List<SecondaryIndicatorResult>
        {
            new()
            {
                PrimaryIndicator = "执行机构健康度",
                Name = "比例阀给定-反馈偏差",
                Weight = 0.30,
                Score = ConsistencyScore(s.ProportionalValveCommand, s.ProportionalValveFeedback, 5),
                Detail = $"给定={s.ProportionalValveCommand:F1}, 反馈={s.ProportionalValveFeedback:F1}"
            },
            new()
            {
                PrimaryIndicator = "执行机构健康度",
                Name = "平板阀开关一致性",
                Weight = 0.30,
                Score = flatValveConsistent ? 100 : 0,
                Detail = $"开阀命令={s.FlatValveCommandOpen}, 高位={s.FlatValveHighLimit}, 低位={s.FlatValveLowLimit}"
            },
            new()
            {
                PrimaryIndicator = "执行机构健康度",
                Name = "液压泵状态",
                Weight = 0.20,
                Score = s.HydraulicPumpCommandOn == s.HydraulicPumpContactorOn ? 100 : 0,
                Detail = $"命令={s.HydraulicPumpCommandOn}, 接触器={s.HydraulicPumpContactorOn}"
            },
            new()
            {
                PrimaryIndicator = "执行机构健康度",
                Name = "电加热状态",
                Weight = 0.20,
                Score = s.HeaterCommandOn == s.HeaterContactorOn ? 100 : 0,
                Detail = $"命令={s.HeaterCommandOn}, 接触器={s.HeaterContactorOn}"
            }
        };

        return BuildPrimary("A5", "执行机构健康度", A5Weight, items);
    }

    private static void ApplyDeductions(MonitoringSnapshot s, List<DeductionItem> deductions)
    {
        if (!string.Equals(s.CpuStatus, "RUN", StringComparison.OrdinalIgnoreCase))
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Major,
                Item = "CPU停机",
                Points = 100,
                Description = "CPU处于STOP或不可用状态"
            });
        }

        if (s.CommunicationDevicesOnline < s.CommunicationDevicesTotal)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Important,
                Item = "关键通信设备离线",
                Points = 40,
                Description = $"在线设备 {s.CommunicationDevicesOnline}/{s.CommunicationDevicesTotal}"
            });
        }

        if (string.Equals(s.FlowMeterStatus, "故障", StringComparison.OrdinalIgnoreCase) || s.FlowRate <= 0.01)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Important,
                Item = "质量流量计故障",
                Points = 40,
                Description = "流量信号异常或无有效输出"
            });
        }

        var flatValveConsistent = s.FlatValveCommandOpen
            ? s.FlatValveHighLimit && !s.FlatValveLowLimit
            : !s.FlatValveHighLimit && s.FlatValveLowLimit;

        if (!flatValveConsistent)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Important,
                Item = "关键阀门拒动或限位异常",
                Points = 40,
                Description = "平板阀命令与高低位反馈不一致"
            });
        }

        if (string.Equals(s.HydraulicTemperatureStatus, "异常", StringComparison.OrdinalIgnoreCase) || s.HydraulicTemperature > 80)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Important,
                Item = "液压站温度超限",
                Points = 40,
                Description = $"液压温度={s.HydraulicTemperature:F1}°C"
            });
        }

        if (s.IoChannelFaultCount > 0)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Minor,
                Item = "单点I/O或反馈异常",
                Points = 15,
                Description = $"故障通道数量={s.IoChannelFaultCount}"
            });
        }

        if (s.UpdateTimeoutCount > 0 || s.CommunicationFaultEvents > 0)
        {
            deductions.Add(new DeductionItem
            {
                Level = FaultLevel.Minor,
                Item = "非关键通信抖动",
                Points = 15,
                Description = $"更新时间异常={s.UpdateTimeoutCount}, 通信故障事件={s.CommunicationFaultEvents}"
            });
        }
    }

    private static PrimaryIndicatorResult BuildPrimary(string code, string name, double weight, List<SecondaryIndicatorResult> items)
    {
        var score = items.Sum(x => x.Weight * x.Score);
        return new PrimaryIndicatorResult
        {
            Code = code,
            Name = name,
            Weight = weight,
            Score = Math.Round(score, 2),
            SecondaryIndicators = items
        };
    }

    private static double MapStateScore(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return 40;

        var text = state.Trim().ToUpperInvariant();
        return text switch
        {
            "RUN" => 100,
            "正常" => 100,
            "OK" => 100,
            "GOOD" => 100,
            "异常" => 60,
            "WARNING" => 60,
            "WARN" => 60,
            "故障" => 0,
            "STOP" => 0,
            "OFFLINE" => 0,
            _ => 40
        };
    }

    private static double RangeScore(double value, double min, double max, double optimalLow, double optimalHigh)
    {
        if (value < min || value > max)
            return 0;

        if (value >= optimalLow && value <= optimalHigh)
            return 100;

        if (value < optimalLow)
            return Math.Round((value - min) / Math.Max(0.0001, optimalLow - min) * 100, 2);

        return Math.Round((max - value) / Math.Max(0.0001, max - optimalHigh) * 100, 2);
    }

    private static double InverseRangeScore(double value, double min, double max, double optimalLow, double optimalHigh)
    {
        if (value < min || value > max)
            return 0;

        if (value >= optimalLow && value <= optimalHigh)
            return 100;

        return Math.Round((max - value) / Math.Max(0.0001, max - optimalHigh) * 100, 2);
    }

    private static double RatioScore(int online, int total)
    {
        if (total <= 0)
            return 0;

        return Math.Round((double)online / total * 100, 2);
    }

    private static double FaultRateScore(int faultCount, int total)
    {
        if (total <= 0)
            return 0;

        return Math.Round(Math.Max(0, 100 - (double)faultCount / total * 100), 2);
    }

    private static double CountPenaltyScore(int count, int fullScoreThreshold, int warningThreshold, int zeroThreshold)
    {
        if (count <= fullScoreThreshold)
            return 100;
        if (count == warningThreshold)
            return 70;
        if (count >= zeroThreshold)
            return 0;

        return 40;
    }

    private static double ConsistencyScore(double commandValue, double feedbackValue, double tolerance)
    {
        var diff = Math.Abs(commandValue - feedbackValue);
        if (diff <= tolerance)
            return 100;
        if (diff >= tolerance * 3)
            return 0;

        return Math.Round((1 - (diff - tolerance) / (tolerance * 2)) * 100, 2);
    }

    private static double Average(params double[] values)
    {
        return values.Length == 0 ? 0 : Math.Round(values.Average(), 2);
    }

    private static double Average(IEnumerable<double> values)
    {
        var arr = values.ToArray();
        return arr.Length == 0 ? 0 : Math.Round(arr.Average(), 2);
    }

    private static string Judge(double score)
    {
        if (score >= 90) return "优秀：系统运行健康";
        if (score >= 80) return "良好：存在轻微异常";
        if (score >= 70) return "一般：建议巡检";
        if (score >= 60) return "较差：需要尽快处理";
        return "危险：建议停机检查";
    }
}
