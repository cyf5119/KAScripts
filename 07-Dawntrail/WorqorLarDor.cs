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

namespace Cyf5119Script.Dawntrail.WorqorLarDor;

[ScriptType(guid: "E9ABB865-498F-4C4F-8071-8A2E9F0589F6", name: "艳翼蛇鸟歼灭战", territorys: [1195], version: "0.0.0.1", author:"Cyf5119")]
public class WorqorLarDor
{
    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }
    
    [ScriptMethod(name: "蛇行吐息", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36156"])]
    public void SusurrantBreath(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "蛇行吐息";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7300;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(50);
        dp.Radian = float.Pi / 180 * 80;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }
    
    [ScriptMethod(name: "蛇行强袭", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36158"])]
    public void SlitheringStrike(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "蛇行强袭";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7300;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(24);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "蛇行盘绕", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36160"])]
    public void StranglingCoil(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "蛇行盘绕";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7300;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(30);
        dp.InnerScale = new Vector2(8);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
    }
    
    [ScriptMethod(name: "灾祸冲", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36189"])]
    public void Ruinfall(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "灾祸冲";
        dp.Color = new Vector4(0.2f, 1, 1, 2);
        dp.DestoryAt = 8000;
        dp.Owner = accessory.Data.Me;
        dp.Scale = new Vector2(1, 21);
        dp.FixRotation = true;
        dp.Rotation = 0;
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "灾厄落雷", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36174"])]
    public void BlightedBolt(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "灾厄落雷";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7800;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(7);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "灾厄之鸣", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(34722|26708)$"])]
    public void CalamitousCry(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "灾厄之鸣";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = @event.ActionId() == 34722 ? 6100 : 5000;
        dp.Owner = @event.SourceId();
        dp.TargetObject = @event.TargetId();
        dp.Scale = new Vector2(6, 40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "雷法阵", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:16769"])]
    public void ArcaneLightning(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "雷法阵";
        dp.Color = accessory.Data.DefaultDangerColor;
        // 8.897  9.084  9.322 同一次副本里出现了三种间隔 暂取最短时间
        dp.DestoryAt = 8900;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(5, 50);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "冰法阵", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:17555"])]
    public void ChillingCataclysm(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "冰法阵";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7700;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(5, 80);
        dp.FixRotation = true;
        for (int i = 0; i < 4; i++)
        {
            dp.Rotation = float.Pi / 4 * i;
            accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        }
    }

    [ScriptMethod(name: "北十字星", eventType: EventTypeEnum.EnvControl, eventCondition: ["Index:00000002"])]
    public void NorthernCross(Event @event, ScriptAccessory accessory)
    {
        if(!(@event["Id"] == "00020001" || @event["Id"] == "00200010")) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "冰法阵";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 9100;
        dp.Scale = new Vector2(25, 60);
        dp.Rotation = -2.21f;
        if (@event["Id"] == "00020001")
            dp.Position = new Vector3(116.47f, -0.02f, 127.98f);
        else
            dp.Position = new Vector3(131.49f, -0.02f, 107.99f);
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

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }
}