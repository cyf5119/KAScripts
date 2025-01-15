using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Endwalker.P2N;

// [ScriptType(guid: "59E0935E-EAFE-445E-B55E-532782EFBDEE", name: "P2N", territorys: [1004], version: "0.0.0.1", author: "Cyf5119")]
public class P2N
{
    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "分摊死刑", ["ActionId:26638"])]
    public void DoubledImpact(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("分摊死刑", @event.TargetId(), 5000, 6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "AOE", ["ActionId:regex:^(26639|26614)$"])]
    public void AOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "吐息飞瀑1", ["ActionId:regex:^(2661[567])$"])]
    public void SpokenCataract1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("吐息飞瀑", @event.SourceId(), 7000, 60);
        dp.Radian = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "吐息飞瀑2", ["ActionId:regex:^(2662[12])$"])]
    public void SpokenCataract2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("吐息飞瀑", @event.SourceId(), 7000, new Vector2(15, 100));
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "震荡波", ["ActionId:regex:26631"])]
    public void Shockwave(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("震荡波", accessory.Data.Me, 6000, new Vector2(2, 13), true);
        dp.TargetPosition = @event.EffectPosition();
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "连贯攻击", ["ActionId:regex:27924", "TargetIndex:1"])]
    public void Coherence(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("连贯攻击", @event.SourceId(), 8400, new Vector2(6, 60), true);
        dp.TargetObject = @event.TargetId();
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "分离", ["ActionId:regex:26630"])]
    public void Dissociation(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("分离", @event.SourceId(), 8000, new Vector2(20, 50));
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
        return JsonConvert.DeserializeObject<uint>(@event["StatusID"]);
    }

    public static uint StackCount(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StackCount"]);
    }

    public static uint Param(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Param"]);
    }

    public static uint TargetIndex(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["TargetIndex"]);
    }
}

public static class AccessoryExtensions
{
    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration,
        float radius, bool safe = false)
    {
        return FastDp(accessory, name, owner, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration,
        Vector2 scale, bool safe = false)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration,
        float radius, bool safe = false)
    {
        return FastDp(accessory, name, pos, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration,
        Vector2 scale, bool safe = false)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, uint target, uint duration,
        uint delay = 0, string name = "Waypoint")
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = accessory.Data.Me;
        dp.TargetObject = target;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, Vector3 pos, uint duration,
        uint delay = 0, string name = "Waypoint")
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = accessory.Data.Me;
        dp.TargetPosition = pos;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
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

    public static IGameObject? GetFirstByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId).FirstOrDefault();
    }

    public static IEnumerable<IGameObject?> GetByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId);
    }
}