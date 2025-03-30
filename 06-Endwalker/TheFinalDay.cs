using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Endwalker.TheFinalDay;

[ScriptType(guid: "2BEBDA4A-DA6A-4B54-B9B8-533F34B3A2EB", name: "终结之战", territorys: [997], version: "0.0.0.2", author: "Cyf5119")]
public class TheFinalDay
{
    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "哀歌", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(26156|26242)$"])]
    public void Elegeia(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 8300);
    }

    [ScriptMethod(name: "天体撞击", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(26158|26171)$"])]
    public void StellarCollision(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "天体撞击";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = @event.ActionId() == 26158 ? 7000 : 2000;
        dp.Position = @event.EffectPosition();
        dp.Scale = new(30);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "银河", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:27754"])]
    public void Galaxias(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "银河";
        dp.Color = new(0.2f, 1, 1, 2);
        dp.DestoryAt = 5000;
        dp.Owner = accessory.Data.Me;
        dp.TargetPosition = @event.EffectPosition();
        dp.Rotation = float.Pi;
        dp.Scale = new(1, 13);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "反诘", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(26179|26180)$"])]
    public void Elenchos(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "反诘";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6000;
        dp.Owner = @event.SourceId();
        var middle = @event.ActionId() == 26180;
        dp.Scale = new(middle ? 14 : 13, 40);
        accessory.Method.SendDraw(0, middle ? DrawTypeEnum.Rect : DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(name: "药毒", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:26187"])]
    public void Pharmakon(Event @event, ScriptAccessory accessory)
    {
        // 26187 死亡拥抱   26188 药毒
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "药毒";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7700;
        dp.Position = @event.TargetPosition();
        dp.Scale = new(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "流溢", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26174"])]
    public void Aporrhoia(Event @event, ScriptAccessory accessory)
    {
        // 26173 流溢   26174 反诘
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "流溢";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.SourceId();
        dp.Scale = new(5, 40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "傲慢", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26195"])]
    public void Hubris(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "傲慢";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.TargetId();
        dp.Scale = new(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "悲惨", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26182"])]
    public void Misery(Event @event, ScriptAccessory accessory)
    {
        // 26182 后裔   26184 悲惨
        // TODO radius
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "悲惨";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Delay = 3000;
        dp.DestoryAt = 22000;
        dp.Owner = @event.SourceId();
        dp.Scale = new(12);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "冲撞", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26199"])]
    public void Crash(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "冲撞";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 9000;
        dp.Position = @event.EffectPosition();
        dp.Scale = new(15);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
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