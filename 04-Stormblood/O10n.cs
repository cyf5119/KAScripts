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
using ECommons.Hooks;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Stormblood.O10n;

[ScriptType(guid: "9248BFE3-D4AE-46AA-9111-AEC2DACDE893", name: "O10n", territorys: [799], version: "0.0.0.1", author: "Cyf5119")]
public class O10n
{
    private bool IsCross = false;
    private uint BossId = 0;
    private bool TimeLock = false;
    
    public void Init(ScriptAccessory accessory)
    {
        IsCross = false;
        BossId = 0;
        TimeLock = false;
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "CrossJudge", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:12744"], userControl: false)]
    public void CrossJudge(Event @event, ScriptAccessory accessory)
    {
        IsCross = true;
        BossId = @event.SourceId();
    }
    
    [ScriptMethod(name: "十字", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:12744"])]
    public void Cross(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "十字";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 9600;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(18, 60);
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        dp.Rotation = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(name: "CircleOrDonut", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:12743"], userControl: false)]
    public void CircleOrDonut(Event @event, ScriptAccessory accessory)
    {
        IsCross = false;
        BossId = @event.SourceId();
    }
    
    [ScriptMethod(name: "钢铁", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:12745"])]
    public void Circle(Event @event, ScriptAccessory accessory)
    {
        if (IsCross) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "钢铁";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 3500;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(14);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "月环", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:12747"])]
    public void Donut(Event @event, ScriptAccessory accessory)
    {
        if (IsCross) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月环";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 3500;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(40);
        dp.InnerScale = new Vector2(10);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
    }
    
    [ScriptMethod(name: "大地摇动", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0028"])]
    public void EarthShaker(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "大地摇动";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4100;
        dp.Owner = BossId;
        dp.TargetObject = @event.TargetId();
        dp.Scale = new Vector2(60);
        dp.Radian = float.Pi / 180 * 30;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }
    
    [ScriptMethod(name: "分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:12742"])]
    public void AkhMorn(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 9300;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "俯冲", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:12865"])]
    public void Cauterize(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "俯冲";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4500;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(20, 60);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "真红射线", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:9290"])]
    public void ScarletThread(Event @event, ScriptAccessory accessory)
    {
        if (TimeLock) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "真红射线";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6900;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(4, 80);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
        Thread.Sleep(1000);
        TimeLock = true;
        Thread.Sleep(2000);
        TimeLock = false;
    }
    
    [ScriptMethod(name: "百京核爆", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:13871"])]
    public void Exaflare(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var spos = @event.SourcePosition();
        var srot = @event.SourceRotation();
        dp.Name = "Exaflare";
        dp.Scale = new Vector2(6);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Delay = 2600;
        dp.DestoryAt = 3000;
        for (int i = 0; i < 4; i++)
        {
            spos = new Vector3(spos.X + (float)Math.Sin(srot) * 8, spos.Y, spos.Z + (float)Math.Cos(srot) * 8);
            dp.Position = spos;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp.Delay += 1500;
        }
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

    public static uint Id(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Id"]);
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

    public static float SourceRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
    }

    public static float TargetRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
    }
}