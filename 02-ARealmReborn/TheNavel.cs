using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.ARealmReborn.TheNavel;

[ScriptType(guid: "BC3B91DA-224A-4356-B7B3-75A8366A2C1C", name: "泰坦歼灭战", territorys: [1046, 293], version: "0.0.0.2",
    author: "Cyf5119")]
// 假泰坦、真泰坦
public class TheNavel
{
    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "地裂", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(650|1364)$"])]
    public void Landslide(Event @event, ScriptAccessory accessory)
    {
        var r = accessory.Data.Objects.SearchByEntityId(@event.SourceId())?.HitboxRadius ?? 0;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "地裂";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = @event.ActionId() == 650 ? 3000 : 2200;
        dp.Owner = @event.SourceId();
        dp.Scale = new(6, 35 + r);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "大地之重", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(973|1363)$"])]
    public void WeightOfTheLand(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "大地之重";
        dp.Color = accessory.Data.DefaultDangerColor.WithW(4);
        dp.DestoryAt = @event.ActionId() == 650 ? 3500 : 2500;
        dp.Position = @event.EffectPosition();
        dp.Scale = new(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "爆炸", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:1052"])]
    public void Burst(Event @event, ScriptAccessory accessory)
    {
        var r = accessory.Data.Objects.SearchByEntityId(@event.SourceId())?.HitboxRadius ?? 0;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "爆炸";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.SourceId();
        dp.Scale = new(5 + r);
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