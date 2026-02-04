using System.Text.Json;

namespace Cnewclinet;

public sealed class StateMachine
{
    private readonly Dictionary<string, List<WorkTypeTransition>> _transitions;
    private readonly Dictionary<string, StatusConfig> _statusLookup;

    public StateMachine(WorkTypeConfigRoot config)
    {
        _transitions = config.WorkTypeConfig
            .GroupBy(item => item.FromId)
            .ToDictionary(group => group.Key, group => group.ToList());

        _statusLookup = config.StatusConfig
            .ToDictionary(item => item.Id, item => item);
    }

    public StatusConfig? GetStatus(string statusId)
    {
        return _statusLookup.GetValueOrDefault(statusId);
    }

    public WorkTypeTransition? GetNextTransition(string fromId, string resultType, string? retCode)
    {
        if (!_transitions.TryGetValue(fromId, out var options))
        {
            return null;
        }

        if (string.Equals(resultType, "autoJump", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(retCode))
        {
            return options.FirstOrDefault(option =>
                string.Equals(option.Type, resultType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(option.RetCode, retCode, StringComparison.OrdinalIgnoreCase));
        }

        return options.FirstOrDefault(option =>
            string.Equals(option.Type, resultType, StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(option.RetCode));
    }

    public static WorkTypeConfigRoot ParseConfig(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<WorkTypeConfigRoot>(json, options);
        if (config is null)
        {
            throw new InvalidOperationException("无法解析配置。");
        }

        return config;
    }
}
