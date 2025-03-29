using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Endwalker.StormsCrown;

[ScriptType(guid: "776A7DFB-F8C3-4ECC-BFB7-3631D083A117", name: "巴尔巴莉希娅歼灭战", territorys: [1071], version: "0.0.0.1",
    author: "Cyf5119")]
public class StormsCrown
{
    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "虚空飙风", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30134"])]
    public void VoidAeroIV(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(name: "虚空暴风", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30135"])]
    public void VoidAeroIII(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "虚空暴风";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.TargetId();
        dp.Scale = new(5);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        accessory.Method.TextInfo("分摊死刑", 5000);
    }

    [ScriptMethod(name: "野蛮剃-一", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(30138|30144)$"])]
    public void SavageBarbery1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "野蛮剃";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 8000;
        dp.Owner = @event.SourceId();
        if (@event.ActionId() == 30138)
        {
            dp.Scale = new Vector2(20);
            dp.InnerScale = new Vector2(6);
            dp.Radian = float.Pi * 2;
            accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
        }
        else
        {
            dp.Scale = new(12, 40);
            accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        }
    }

    [ScriptMethod(name: "野蛮剃-二", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(30139|30145)$"])]
    public void SavageBarbery2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "野蛮剃";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 10100;
        dp.Position = @event.EffectPosition();
        dp.Scale = new(20);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "咒发突袭", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30147"])]
    public void HairRaid(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "咒发突袭";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 8000;
        dp.Owner = @event.SourceId();
        dp.Scale = new(40);
        dp.Radian = float.Pi / 180 * 120;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "咒发刺", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30172"])]
    public void DeadlyTwist(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "咒发刺";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.TargetId();
        dp.Scale = new(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "咒发武装", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30130"])]
    public void CurlingIron(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("出目标圈，过场动画预备。", 13200);
    }

    [ScriptMethod(name: "落狱煞", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:30140"])]
    public void Catabasis(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("大AOE", 8000);
    }

    [ScriptMethod(name: "怒拳连震", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:30103"])]
    public void KnuckleDrum(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("连续AOE", 10000);
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

    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }

    public static Vector3 TargetPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
    }

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }
}