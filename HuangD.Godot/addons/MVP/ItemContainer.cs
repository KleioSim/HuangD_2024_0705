using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IItemView
{
    object Id { get; set; }
}

public class ItemContainer<T> where T : Control, IItemView
{
    private Func<InstancePlaceholder> itemPlaceHolder;

    public Action<T> OnAddedItem { get; set; }

    public ItemContainer(Func<InstancePlaceholder> placeholder)
    {
        this.itemPlaceHolder = placeholder;
    }

    public IEnumerable<T> GetCurrentItems()
    {
        return itemPlaceHolder().GetParent().GetChildren().Select(x => x.GetNodeOrNull<T>("."))
            .Where(x => x != null);
    }

    public T AddItem(object taskId)
    {
        var item = itemPlaceHolder().CreateInstance() as T;
        item.Id = taskId;
        OnAddedItem?.Invoke(item);

        return item;
    }

    public void RemoveItem(T item)
    {
        itemPlaceHolder().GetParent().RemoveChild(item);
    }

    public IEnumerable<T> Refresh(HashSet<object> keys)
    {
        var taskViewDict = GetCurrentItems().ToDictionary(x => x.Id, x => x);

        var needRemoves = new Queue<object>(taskViewDict.Keys.Except(keys));
        var needAdds = new Queue<object>(keys.Except(taskViewDict.Keys));

        var addedItems = new List<T>();

        while (needAdds.TryDequeue(out object key))
        {
            if (needRemoves.TryDequeue(out object replaceKey))
            {
                var Item = taskViewDict[replaceKey];
                Item.Id = key;
            }
            else
            {
                var newItem = AddItem(key);
                addedItems.Add(newItem);
            }
        }

        while (needRemoves.TryDequeue(out object key))
        {
            var item = taskViewDict[key];
            RemoveItem(item);
        }

        return addedItems;
    }
}