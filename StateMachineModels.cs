using System.Text.Json.Serialization;

namespace Cnewclinet;

public sealed class WorkTypeConfigRoot
{
    [JsonPropertyName("work_type_config")]
    public List<WorkTypeTransition> WorkTypeConfig { get; set; } = [];

    [JsonPropertyName("status_config")]
    public List<StatusConfig> StatusConfig { get; set; } = [];
}

public sealed class WorkTypeTransition
{
    [JsonPropertyName("from_id")]
    public string FromId { get; set; } = string.Empty;

    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("to_id")]
    public string ToId { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("ret_code")]
    public string? RetCode { get; set; }
}

public sealed class StatusConfig
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("hint")]
    public string Hint { get; set; } = string.Empty;

    [JsonPropertyName("variable")]
    public string Variable { get; set; } = string.Empty;

    [JsonPropertyName("input_dev")]
    public string InputDev { get; set; } = string.Empty;

    [JsonPropertyName("rule")]
    public string Rule { get; set; } = string.Empty;

    [JsonPropertyName("func_list")]
    public List<StatusFunction> FuncList { get; set; } = [];

    [JsonPropertyName("state_type")]
    public string StateType { get; set; } = string.Empty;
}

public sealed class StatusFunction
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("AppName")]
    public string AppName { get; set; } = string.Empty;

    [JsonPropertyName("AppCode")]
    public string AppCode { get; set; } = string.Empty;

    [JsonPropertyName("SrvCode")]
    public string SrvCode { get; set; } = string.Empty;
}
