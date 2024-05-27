using Godot;
using System;
using System.Linq;

public partial class MainScene : Control
{
    public Global Global => GetNode<Global>("/root/Global");

    public override void _Ready()
    {
        CommandConsole.IsVaild = true;

        CommandConsole.AddCommand("testing", testing);
    }


    public void testing(string provName, string countryName)
    {
        var prov = Global.Session.Provinces.Single(x => x.Name == provName);
        var country = Global.Session.Countries.Single(x => x.Name == countryName);

        Global.ChangeProvinceOwner(prov, country);
    }
}
