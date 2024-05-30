using Godot;
using HuangD.Sessions;
using System;
using System.Linq;

public partial class MainScene : ViewControl
{
    public Global Global => GetNode<Global>("/root/Global");
    public Control CurrCountry => GetNode<Control>("CanvasLayer/CurrentCountry");
    public Control eventDialog => GetNode<Control>("CanvasLayer/EventDialog");
    public Timer timer => GetNode<Timer>("Timer");

    public override void _Ready()
    {
        CommandConsole.IsVaild = true;

        CommandConsole.AddCommand("testing", testing);
    }

    public async void OnNextTurn()
    {
        timer.Stop();
        foreach (var eventObj in Global.Session.OnNextTurn())
        {
            eventDialog.Visible = true;
            await ToSignal(eventDialog, Control.SignalName.VisibilityChanged);
        }
        timer.Start();
    }

    public void testing(string provName, string countryName)
    {
        var prov = Global.Session.Provinces.Single(x => x.Name == provName);
        var country = Global.Session.Countries.Single(x => x.Name == countryName);

        Global.Session.OnMessage(new Message_ChangeProvinceOwner(prov, country));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        CurrCountry.Visible = Global.Session.currCountry != null;
        if (CurrCountry.Visible)
        {
            CurrCountry.GetNode<Label>("Label").Text = Global.Session.currCountry.Name;
        }
    }
}

public partial class EventDialog : Control
{

}