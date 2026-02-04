using System.Text;
using System.Windows.Forms;

namespace Cnewclinet;

public sealed class MainForm : Form
{
    private readonly TextBox _currentStateTextBox = new();
    private readonly TextBox _resultTypeTextBox = new();
    private readonly TextBox _retCodeTextBox = new();
    private readonly Button _simulateButton = new();
    private readonly TextBox _outputTextBox = new();

    private readonly StateMachine _stateMachine;

    public MainForm()
    {
        Text = "状态机演示";
        Width = 900;
        Height = 600;

        _stateMachine = new StateMachine(StateMachine.ParseConfig(SampleJson));

        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(12),
            AutoSize = true
        };

        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

        formLayout.Controls.Add(new Label { Text = "当前状态ID", Dock = DockStyle.Fill }, 0, 0);
        formLayout.Controls.Add(_currentStateTextBox, 1, 0);

        formLayout.Controls.Add(new Label { Text = "业务结果类型", Dock = DockStyle.Fill }, 0, 1);
        formLayout.Controls.Add(_resultTypeTextBox, 1, 1);

        formLayout.Controls.Add(new Label { Text = "业务返回码", Dock = DockStyle.Fill }, 0, 2);
        formLayout.Controls.Add(_retCodeTextBox, 1, 2);

        _simulateButton.Text = "计算下一状态";
        _simulateButton.Click += (_, _) => Simulate();
        formLayout.Controls.Add(_simulateButton, 1, 3);

        _outputTextBox.Multiline = true;
        _outputTextBox.ReadOnly = true;
        _outputTextBox.ScrollBars = ScrollBars.Vertical;
        _outputTextBox.Dock = DockStyle.Fill;

        formLayout.Controls.Add(_outputTextBox, 0, 4);
        formLayout.SetColumnSpan(_outputTextBox, 2);

        Controls.Add(formLayout);

        _currentStateTextBox.Text = "STATUS0141";
        _resultTypeTextBox.Text = "autoJump";
        _retCodeTextBox.Text = "KPS_0";

        Simulate();
    }

    private void Simulate()
    {
        var fromId = _currentStateTextBox.Text.Trim();
        var resultType = _resultTypeTextBox.Text.Trim();
        var retCode = _retCodeTextBox.Text.Trim();

        var transition = _stateMachine.GetNextTransition(fromId, resultType, retCode);
        var status = transition is null ? null : _stateMachine.GetStatus(transition.ToId);

        var builder = new StringBuilder();
        builder.AppendLine($"当前状态: {fromId}");
        builder.AppendLine($"业务结果: {resultType} / {retCode}");

        if (transition is null)
        {
            builder.AppendLine("未找到匹配的跳转规则。");
        }
        else
        {
            builder.AppendLine($"下一状态: {transition.ToId} ({transition.To})");
            if (status is not null)
            {
                builder.AppendLine($"提示: {status.Hint}");
                builder.AppendLine($"变量: {status.Variable}");
            }
        }

        _outputTextBox.Text = builder.ToString();
    }

    private const string SampleJson = """
    {
      \"work_type_config\": [
        {
          \"from_id\": \"STATUS0009\",
          \"from\": \"INPUT_SN\",
          \"to_id\": \"STATUS0141\",
          \"to\": \"INPUT_A1MSN\",
          \"type\": \"succ\"
        },
        {
          \"from_id\": \"STATUS0141\",
          \"from\": \"INPUT_A1MSN\",
          \"to_id\": \"STATUS0013\",
          \"to\": \"PASS_STATION\",
          \"type\": \"autoJump\",
          \"ret_code\": \"KPS_0\"
        },
        {
          \"from_id\": \"STATUS0141\",
          \"from\": \"INPUT_A1MSN\",
          \"to_id\": \"STATUS0141\",
          \"to\": \"INPUT_A1MSN\",
          \"type\": \"succ\"
        },
        {
          \"from_id\": \"STATUS0013\",
          \"from\": \"PASS_STATION\",
          \"to_id\": \"STATUS0009\",
          \"to\": \"INPUT_SN\",
          \"type\": \"succ\"
        }
      ],
      \"status_config\": [
        {
          \"id\": \"STATUS0009\",
          \"name\": \"INPUT_SN\",
          \"hint\": \"请输入产品SN\",
          \"variable\": \"SN\",
          \"input_dev\": \"keyboard_input\",
          \"rule\": \"keep until last status\",
          \"func_list\": [
            {
              \"index\": 0,
              \"name\": \"CheckSN\",
              \"code\": \"DefaultGroup-CheckSN\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 1,
              \"name\": \"CheckRoute\",
              \"code\": \"DefaultGroup-CheckRoute\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 2,
              \"name\": \"GetL3KitMPN\",
              \"code\": \"KittingSMT-GetL3KitMPN\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 3,
              \"name\": \"CheckL3MSNKitEnd\",
              \"code\": \"KittingSMT-CheckL3MSNKitEnd\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            }
          ],
          \"state_type\": \"begin\"
        },
        {
          \"id\": \"STATUS0141\",
          \"name\": \"INPUT_A1MSN\",
          \"hint\": \"请输入阿里一码通材料SN\",
          \"variable\": \"A1MSN\",
          \"input_dev\": \"keyboard_input\",
          \"rule\": \"keep until last status\",
          \"func_list\": [
            {
              \"index\": 0,
              \"name\": \"CheckL3MSNBirth\",
              \"code\": \"KittingSMT-CheckL3MSNBirth\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 1,
              \"name\": \"CheckL3MSNAndMPN\",
              \"code\": \"KittingSMT-CheckL3MSNAndMPN\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 2,
              \"name\": \"InsertL3MSNAndMPN\",
              \"code\": \"KittingSMT-InsertL3MSNAndMPN\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 3,
              \"name\": \"CheckL3MSNKitEnd\",
              \"code\": \"KittingSMT-CheckL3MSNKitEnd\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            }
          ],
          \"state_type\": \"\"
        },
        {
          \"id\": \"STATUS0013\",
          \"name\": \"PASS_STATION\",
          \"hint\": \"\",
          \"variable\": \"PASS_STATION\",
          \"input_dev\": \"\",
          \"rule\": \"keep until last status\",
          \"func_list\": [
            {
              \"index\": 0,
              \"name\": \"PassStation\",
              \"code\": \"DefaultGroup-PassStation\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            },
            {
              \"index\": 1,
              \"name\": \"QueryPassQty\",
              \"code\": \"DefaultGroup-QueryPassQty\",
              \"AppName\": \"生产执行中心(PEC)\",
              \"AppCode\": \"pec\",
              \"SrvCode\": \"executing\"
            }
          ],
          \"state_type\": \"pass\"
        }
      ]
    }
    """;
}
