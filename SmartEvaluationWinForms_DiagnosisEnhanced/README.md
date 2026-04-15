# 智能评价系统 WinForms 示例

## 功能
- 首页按钮进入“数据监控与智能评价”界面
- 自动执行一级指标、二级指标评分
- 展示扣分项、综合得分和运行结论
- 评分逻辑基于你提供的工控系统评价模型

## 工程结构
- `Program.cs`：程序入口
- `MainForm.cs`：首页
- `MonitorForm.cs`：数据监控与评价界面
- `Models.cs`：数据模型
- `EvaluationService.cs`：评价算法与扣分规则

## 运行方式
1. 使用 Visual Studio 2022 或更高版本打开 `SmartEvaluationWinForms.csproj`
2. 选择 .NET 8 Windows 桌面环境
3. 编译运行

## 你需要重点修改的地方
- `EvaluationService.cs` 中的各二级指标权重
- 各模拟量允许范围、优选范围
- 扣分规则和状态分级阈值
- `MonitorForm.cs` 中的 `BuildDemoSnapshot()`，替换为 PLC/HMI/数据库实时数据来源

## 实时数据接入建议
你后续可以把 `BuildDemoSnapshot()` 替换成以下任一方式：
- OPC UA 采集 PLC 数据
- 读取 SQL Server / SQLite 监控库
- 订阅上位机采集服务
- 从 Web API 获取监测数据


## 新增功能（本次增强）
- 基于一级指标得分、二级指标状态和扣分项自动诊断故障
- 对即将发生故障的趋势项弹出新的“故障诊断与预警提示”窗口
- 弹窗中展示预警原因、触发依据和建议处理措施
- 不改变原有首页、监控评价界面和原有功能

## 新增文件
- `DiagnosisService.cs`：诊断与预警规则
- `DiagnosisWarningForm.cs`：故障诊断与预警弹窗
