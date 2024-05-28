using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Godot.Node;

public class PresentManager
{
    private Dictionary<Type, Type> PresentDict { get; } = Assembly.GetExecutingAssembly().GetTypes()
        .Where(x => x.BaseType != null
         && x.BaseType.IsGenericType
         && x.BaseType.GetGenericTypeDefinition() == typeof(PresentControl<,>))
        .ToDictionary(x => x.BaseType.GetGenericArguments()[0], x => x);

    public void OnViewReady(ViewControl view)
    {
        var viewType = view.GetType();

        if (!PresentDict.TryGetValue(viewType, out Type presentType))
        {
            GD.PushWarning($"Can not find present for view : {viewType}");
            return;
        }

        var present = Activator.CreateInstance(presentType) as PresentBase;
        present.Name = "Present";

        view.AddChild(present, true, InternalMode.Front);
    }
}

public class MockManager
{
    private Dictionary<Type, Type> MockDict { get; } = Assembly.GetExecutingAssembly().GetTypes()
        .Where(x => x.BaseType != null
         && x.BaseType.IsGenericType
         && x.BaseType.GetGenericTypeDefinition() == typeof(MockControl<,>))
        .ToDictionary(x => x.BaseType.GetGenericArguments()[0], x => x);

    public static object Model;

    public void OnPresentReady<TView, TModel>(PresentControl<TView, TModel> present)
        where TView : ViewControl
        where TModel : class
    {
        var viewType = typeof(TView);

        if (!MockDict.TryGetValue(viewType, out Type mockType))
        {
            GD.PushWarning($"Can not find present for view : {viewType}");
            return;
        }

        var mock = Activator.CreateInstance(mockType) as MockControl<TView, TModel>;
        mock.Name = "Mock";

        present.AddChild(mock, true, InternalMode.Front);
    }
}