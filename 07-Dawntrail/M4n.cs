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

namespace KAScript.Cyf5119Script.Dawntrail.M4n;

[ScriptType(guid: "F4A95B34-AE13-40E4-9106-78D607BCFD57", name: "M4n", territorys: [1231], version: "0.0.0.1",
    author: "Cyf5119")]
public class M4n
{
    // private uint BossId = 0;
    private List<bool> IsFront = [];
    private int MaxCannonTimes = 0;


    public void Init(ScriptAccessory accessory)
    {
        // BossId = 0;
        IsFront = [];
        MaxCannonTimes = 0;
    }

    [ScriptMethod(name: "AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37575"])]
    public void WrathOfZeus(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(name: "死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37576"])]
    public void WickedJolt(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑", duration: 5000);
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "死刑";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.TargetObject = @event.TargetId();
        dp.DestoryAt = 5000;
        dp.Scale = new Vector2(5, 60);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "分摊", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:013C"])]
    public void Stack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 9000;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(5);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "分摊二", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00A1"])]
    public void Stack2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊二";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 5400;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "侧方电火花", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(3756[4567])$"])]
    public void SidewiseSpark(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "侧方电火花";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7000;
        dp.Owner = @event.SourceId();
        dp.Rotation = @event.ActionId() % 2 == 0 ? float.Pi / -2 : float.Pi / 2;
        dp.Scale = new Vector2(60);
        dp.Radian = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "分身侧方电火花", eventType: EventTypeEnum.PlayActionTimeline,
        eventCondition: ["Id:regex:^(456[68])$", "SourceDataId:16996"])]
    public void SidewiseSparkAdds(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "侧方电火花";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 8000;
        dp.Owner = @event.SourceId();
        dp.Rotation = @event.Id() == 4566 ? float.Pi / -2 : float.Pi / 2;
        dp.Scale = new Vector2(60);
        dp.Radian = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "奔雷炮", eventType: EventTypeEnum.ActionEffect,
        eventCondition: ["ActionId:regex:^(3754[78])$"])]
    public void StampedingThunder(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "奔雷炮";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 9300;
        dp.Position = @event.ActionId() == 37547 ? new Vector3(95, 0, 80) : new Vector3(105, 0, 80);
        dp.Scale = new Vector2(30, 40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "加农炮读条", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(375(49|52)|39759|3976[567])$"])]
    public void CannonStartCasting(Event @event, ScriptAccessory accessory)
    {
        var aid = @event.ActionId();
        // BossId = @event.SourceId();
        IsFront = [];
        switch (aid)
        {
            case 37549 or 37552:
                MaxCannonTimes = 3;
                break;
            case 39759 or 39765:
                MaxCannonTimes = 4;
                break;
            case 39766 or 39767:
                MaxCannonTimes = 5;
                break;
        }
    }

    [ScriptMethod(name: "加农炮", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2970"])]
    public void CannonRecord(Event @event, ScriptAccessory accessory)
    {
        // if(BossId != @event.TargetId()) return;
        IsFront.Add(@event.Param() == 723);
        // 723 为北  724 为南

        var count = IsFront.Count;
        var max = MaxCannonTimes;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"加农炮 {count}";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.TargetId();
        dp.Rotation = @event.Param() == 723 ? 0 : float.Pi;
        dp.Scale = new Vector2(10, 40);

        // (100*x-67*n+254*m+210)/100
        // AddStatus到判定时间，特别的，第一次需要+0.57
        // x 为Add时间 n 为第几次add m 为上限
        var destime = -670 * count + 2540 * max + 2100;
        if (count < 2)
        {
            dp.DestoryAt = destime + 570;
        }
        else
        {
            dp.Delay = destime - 1870;
            dp.DestoryAt = 1870;
        }

        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }


    [ScriptMethod(name: "魔女回翔", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37560"])]
    public void BewitchingFlight(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "魔女回翔";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(5, 40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "雷炸", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37561"])]
    public void Burst(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "雷炸";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 7000;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(16, 40);
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "猎杀女巫", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37557"])]
    public void WitchHunt(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "猎杀女巫";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6200;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(6);
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

    public static uint IconId(this Event @event)
    {
        return ParseHexId(@event["Id"], out var id) ? id : 0;
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