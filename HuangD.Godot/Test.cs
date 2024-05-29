using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Test : Node2D
{
    Button button => GetNode<Button>("Button");

    public override void _Ready()
    {
        base._Ready();


        GD.Print($"{Thread.CurrentThread.ManagedThreadId} _Ready0");

        //var task = GetStrLengthAsync();
        GetButtonTriggeAsync();

        GD.Print($"{Thread.CurrentThread.ManagedThreadId} _Ready1");

        //GD.Print($"{Thread.CurrentThread.ManagedThreadId} {task.Result}");
    }

    async void GetButtonTriggeAsync()
    {
        GD.Print($"{Thread.CurrentThread.ManagedThreadId} GetButtonTriggeAsync方法开始执行");
        //此处返回的<string>中的字符串类型，而不是Task<string>
        await ToSignal(button, Button.SignalName.ButtonUp);

        GD.Print($"{Thread.CurrentThread.ManagedThreadId} GetButtonTriggeAsync方法执行结束");
    }


    async Task<int> GetStrLengthAsync()
    {
        GD.Print($"{Thread.CurrentThread.ManagedThreadId} GetStrLengthAsync方法开始执行");
        //此处返回的<string>中的字符串类型，而不是Task<string>
        string str = await GetString();
        GD.Print($"{Thread.CurrentThread.ManagedThreadId} GetStrLengthAsync方法执行结束");
        return str.Length;
    }

    Task<string> GetString()
    {
        GD.Print($"{Thread.CurrentThread.ManagedThreadId} GetString方法开始执行");
        return Task<string>.Run(() =>
        {
            GD.Print($"{Thread.CurrentThread.ManagedThreadId} Task Run");

            Thread.Sleep(2000);

            GD.Print($"{Thread.CurrentThread.ManagedThreadId} Task Finish");

            return "GetString的返回值";
        });
    }
}
