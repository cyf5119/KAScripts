﻿using System;
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

namespace KAScript.Cyf5119Script.Dawntrail.M3n;

[ScriptType(guid: "AA6A8D8A-4462-41A2-B500-90515DE6534A", name: "M3n", territorys: [1229], version: "0.0.0.1", author: "Cyf5119")]
public class M3n
{
    public void Init(ScriptAccessory accessory)
    {
    }
    
    [ScriptMethod(name: "AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37846"])]
    public void BrutalImpact(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("多段AOE", 5000);
    }

    [ScriptMethod(name: "分摊死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37845"])]
    public void KnuckleSandwich(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("分摊死刑", duration: 5000);
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊死刑";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.TargetId();
        dp.DestoryAt = 5000;
        dp.Scale = new Vector2(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37929"])]
    public void Stack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "碎颈臂", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(396(3[89]|5[2345]))$"])]
    public void BrutalLariat(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var aid = @event.ActionId();
        dp.Name = "碎颈臂";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = aid > 39653 ? 3100 : 6100;
        // dp.Owner = @event.SourceId();
        dp.Position = @event.EffectPosition();
        dp.Rotation = IbcHelper.GetById(@event.SourceId()).Rotation; 
        dp.Scale = new Vector2(34, 70);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "碎颈臂连击提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3964[4567])$"])]
    public void LariatComboTip(Event @event, ScriptAccessory accessory)
    {
        var aid = @event.ActionId();
        accessory.Method.TextInfo(aid % 2 == 0 ? "等下对穿" : "等下不动", 6100);
    }
    
    [ScriptMethod(name: "致命毒雾", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37813"])]
    public void MurderousMist(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "致命毒雾";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(40);
        dp.Radian = float.Pi / 180 * 270;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }
    
    [ScriptMethod(name: "自爆", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3781[67])$"])]
    public void SelfDestruct(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var aid = @event.ActionId();
        dp.Name = "自爆";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Delay = aid == 37816 ? 0 : 5000;
        dp.DestoryAt = aid == 37816 ? 5000 : 3000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(8);
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

    public static float SourceRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
    }

    public static float TargetRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
    }

    public static string SourceName(this Event @event)
    {
        return @event["SourceName"];
    }

    public static string TargetName(this Event @event)
    {
        return @event["TargetName"];
    }

    public static uint DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["DurationMilliseconds"]);
    }

    public static uint Index(this Event @event)
    {
        return ParseHexId(@event["Index"], out var id) ? id : 0;
    }

    public static uint State(this Event @event)
    {
        return ParseHexId(@event["State"], out var id) ? id : 0;
    }

    public static uint DirectorId(this Event @event)
    {
        return ParseHexId(@event["DirectorId"], out var id) ? id : 0;
    }

    public static uint StatusId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StatusId"]);
    }

    public static uint StackCount(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StackCount"]);
    }

    public static uint Param(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Param"]);
    }
}

public static class IbcHelper
{
    public static IBattleChara? GetById(uint id)
    {
        return (IBattleChara?)Svc.Objects.SearchByEntityId(id);
    }

    public static IBattleChara? GetMe()
    {
        return Svc.ClientState.LocalPlayer;
    }

    public static IEnumerable<IBattleChara> GetByDataId(uint dataId)
    {
        return (IEnumerable<IBattleChara>)Svc.Objects.Where(x => x.DataId == dataId);
    }
    
}