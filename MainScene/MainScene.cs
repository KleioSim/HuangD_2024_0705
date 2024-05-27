using Godot;
using System;
using System.Linq;

public partial class MainScene : ViewControl
{
    public Global Global;

    public override void _Ready()
    {
        CommandConsole.IsVaild = true;
        Global = GetNode<Global>("/root/Global");
    }


    [AddCommand("testing")]
    public void testing(string provName, string countryName)
    {
        var prov = Global.Session.Provinces.Single(x => x.Name == provName);
        var country = Global.Session.Countries.Single(x => x.Name == countryName);

        Global.ChangeProvinceOwner(prov, country);
    }
}
