using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Dawntrail.M2n;

[ScriptType(guid: "4FD21978-B76C-4BF7-A3F5-D0490BB51915", name: "M2n", territorys: [1227], version: "0.0.0.2", author: "Cyf5119")]
public class M2n
{
    public void Init(ScriptAccessory accessory)
    {
    }

    [ScriptMethod(name: "AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(37220|37234|37243)$"])]
    public void AOE(Event @event, ScriptAccessory accessory)
    {
        var aid = @event.ActionId();
        accessory.Method.TextInfo("AOE", aid == 37234 ? 7000 : 5000);
    }

    [ScriptMethod(name: "死刑", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00E6"])]
    public void Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("双T扇形死刑", duration: 5000);
        var dp = accessory.Data.GetDefaultDrawProperties();
        var boss = accessory.Data.Objects.FirstOrDefault(x => x.DataId == 0x422A);
        dp.Name = "死刑";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = boss?.EntityId ?? 0;
        dp.TargetObject = @event.TargetId();
        dp.DestoryAt = 5000;
        dp.Radian = float.Pi / 180 * 30;
        dp.Scale = new Vector2(40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(37232|39821)$"])]
    public void Stack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 7000;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "月环", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(397(38|40))$"])]
    public void TemptingTwist(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月环";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6200;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(30);
        dp.InnerScale = new Vector2(7);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
    }

    [ScriptMethod(name: "直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3973[79])$"])]
    public void HoneyBeeline(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "直线";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6200;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(14, 60);
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(name: "扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37235"])]
    public void BlowKiss(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "扇形";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(40);
        dp.Radian = float.Pi / 180 * 120;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "毒液", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37230"])]
    public void Splinter(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "毒液";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(8);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "冲锋", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3952[56])$"])]
    public void BlindingLove(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "冲锋";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(8, 50);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }
}

public static class EventExtensions
{
    private static bool ParseHexId(string? idStr, out uint id)
    {
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;
        try
        {
            var idStr2 = idStr.Replace("0x", "");
            id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static uint ActionId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
    }

    public static uint SourceId(this Event @event)
    {
        return ParseHexId(@event["SourceId"], out var id) ? id : 0;
    }

    public static uint TargetId(this Event @event)
    {
        return ParseHexId(@event["TargetId"], out var id) ? id : 0;
    }
}
