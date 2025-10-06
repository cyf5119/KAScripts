using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
// using ECommons;
// using ECommons.DalamudServices;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Endwalker.P1N;

// [ScriptType(guid: "C5E706D1-11EA-4A3D-BB87-6CEC63FCF83E", name: "P1N", territorys: [1002], version: "0.0.0.1", author: "Cyf5119")]
public class P1N
{
    // private uint _status = 0;

    public void Init(ScriptAccessory accessory)
    {
        // _status = 0;
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "死刑", ["ActionId:26099"])]
    public void HeavyHand(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("单体死刑", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "AOE", ["ActionId:regex:^(26100|26089|26090)$"])]
    public void AOE(Event @event, ScriptAccessory accessory)
    {
        var dura = @event.ActionId() == 26100 ? 5000 : 7000;
        accessory.Method.TextInfo("AOE", dura);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "左右刀", ["ActionId:regex:^(2806[67])$"])]
    public void GaolersFlail(Event @event, ScriptAccessory accessory)
    {
        var isLeft = @event.ActionId() > 28066;
        var dp = accessory.FastDp("左右刀", @event.SourceId(), 8700, 60);
        dp.Radian = float.Pi;
        dp.Rotation = isLeft ? float.Pi / 2 : -float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "直线击退", ["ActionId:26085"])]
    public void PitilessFlail(Event @event, ScriptAccessory accessory)
    {
        if (@event.TargetId() != accessory.Data.Me) return;
        // TODO fix the distance
        var dp = accessory.FastDp("直线击退", accessory.Data.Me, 5000, new Vector2(2, 11), true);
        dp.TargetObject = @event.SourceId();
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.TargetIcon, "分摊", ["Id:003E"])]
    public void TrueHoly(Event @event, ScriptAccessory accessory)
    {
        // Svc.ClientState.TerritoryType
        var dp = accessory.FastDp("分摊", @event.TargetId(), 5000, 6, true);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    // [ScriptMethod(EventTypeEnum.StatusAdd, "上冰火状态", ["StatusID:regex:^(2739|2740))$"], false)]
    // public void SpellStatus(Event @event, ScriptAccessory accessory)
    // {
    //     if (@event.TargetId() != accessory.Data.Me) return;
    //     _status = @event.StatusId();
    // }
    //
    // [ScriptMethod(EventTypeEnum.StartCasting, "魔力解放", ["ActionId:regex:^(2609[67]|2784[56])$"])]
    // public void Spell(Event @event, ScriptAccessory accessory)
    // {
    //     var aid = @event.ActionId();
    //     var dura = (uint)(aid > 26097 ? 4000 : 1500);
    //     var dp = accessory.FastDp("魔力解放", @event.EffectPosition(), dura, 19);
    //     // dp.Color = 
    //     if (aid == 26096 || aid == 27845)
    //         dp.Color = _status == 2739 ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
    //     else
    //         dp.Color = _status == 2740 ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
    //     accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    // }
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
