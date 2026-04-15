namespace SmartEvaluationWinForms;

public enum FaultLevel
{
    Major,
    Important,
    Minor
}

public sealed class DeductionItem
{
    public FaultLevel Level { get; set; }
    public string Item { get; set; } = string.Empty;
    public double Points { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class SecondaryIndicatorResult
{
    public string PrimaryIndicator { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double Score { get; set; }
    public string Detail { get; set; } = string.Empty;
}

public sealed class PrimaryIndicatorResult
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double Score { get; set; }
    public List<SecondaryIndicatorResult> SecondaryIndicators { get; set; } = new();
}

public sealed class EvaluationResult
{
    public List<PrimaryIndicatorResult> PrimaryIndicators { get; set; } = new();
    public List<DeductionItem> Deductions { get; set; } = new();
    public double BaseScore { get; set; }
    public double DeductionScore { get; set; }
    public double FinalScore { get; set; }
    public string Judgment { get; set; } = string.Empty;
}



public enum DiagnosisSeverity
{
    Critical,
    Warning,
    EarlyWarning
}

public sealed class DiagnosisItem
{
    public DiagnosisSeverity Severity { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Trigger { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public bool IsPredicted { get; set; }
}

public sealed class DiagnosisReport
{
    public List<DiagnosisItem> Items { get; set; } = new();
    public string OverallStatus { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int CurrentFaultCount => Items.Count(x => !x.IsPredicted);
    public int EarlyWarningCount => Items.Count(x => x.IsPredicted);
    public bool HasAlert => Items.Count > 0;
}

public sealed class MonitoringSnapshot
{
    public string CpuStatus { get; set; } = "RUN";
    public double ScanCycleMs { get; set; } = 18;
    public double CpuLoadPercent { get; set; } = 72;
    public double MemoryUsagePercent { get; set; } = 68;
    public int SevereDiagnosticEvents { get; set; } = 1;

    public int AiTotal { get; set; } = 8;
    public int AiOnline { get; set; } = 8;
    public int DiTotal { get; set; } = 6;
    public int DiOnline { get; set; } = 5;
    public int AoDoTotal { get; set; } = 6;
    public int AoDoOnline { get; set; } = 6;
    public int IoChannelFaultCount { get; set; } = 1;
    public int IoTotalChannels { get; set; } = 20;
    public bool IoFeedbackConsistent { get; set; } = true;

    public int CommunicationDevicesTotal { get; set; } = 4;
    public int CommunicationDevicesOnline { get; set; } = 3;
    public int UpdateTimeoutCount { get; set; } = 1;
    public int CommunicationFaultEvents { get; set; } = 2;
    public int HeartbeatAliveCount { get; set; } = 3;

    public double StandpipePressure { get; set; } = 18.2;
    public double PostThrottlePressure { get; set; } = 12.4;
    public double MainChannelPressure { get; set; } = 18.0;
    public double AuxiliaryChannelPressure { get; set; } = 17.5;
    public double HydraulicPressure { get; set; } = 10.2;
    public double FlowRate { get; set; } = 0;
    public double Density { get; set; } = 1.12;
    public double HydraulicTemperature { get; set; } = 84;
    public double ValvePositionA { get; set; } = 52;
    public double ValvePositionB { get; set; } = 48;
    public string FlowMeterStatus { get; set; } = "故障";
    public string HydraulicTemperatureStatus { get; set; } = "异常";

    public double ProportionalValveCommand { get; set; } = 50;
    public double ProportionalValveFeedback { get; set; } = 56;
    public bool FlatValveCommandOpen { get; set; } = true;
    public bool FlatValveHighLimit { get; set; } = false;
    public bool FlatValveLowLimit { get; set; } = true;
    public bool HydraulicPumpCommandOn { get; set; } = true;
    public bool HydraulicPumpContactorOn { get; set; } = true;
    public bool HeaterCommandOn { get; set; } = false;
    public bool HeaterContactorOn { get; set; } = false;
}
