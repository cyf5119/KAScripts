using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Dev.CyfHelper;

[ScriptType(guid: "FFA80AC9-E88E-437F-B303-5776423CFCD5", name: "CyfHelper", territorys: [], version: "0.0.0.1", author: "Cyf5119", note: "")]
public class CyfHelper
{
    // [UserSetting("颜色")] 
    // public ScriptColor P1AddsColor { get; set; } = new() { V4 = new(0.2f, 1, 1, 1) };
    
    public void Init(ScriptAccessory accessory)
    {
    }
    
    [ScriptMethod(name: "Test1", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(37010|133)$"])]
    public unsafe void Test1(Event @event, ScriptAccessory accessory)
    {
        // var s1 = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        // var s2 = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
        // var s3 = System.Reflection.MethodBase.GetCurrentMethod().Name;
        // accessory.Log.Debug(s1);
        // accessory.Log.Debug(s2);
        // accessory.Log.Debug(s3);
    }

    [ScriptMethod(name: "Test2", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(24286)$"])]
    public void Test2(Event @event, ScriptAccessory accessory)
    {
        // accessory.Log.Debug($"TerritoryType:{Svc.ClientState.TerritoryType}");
        // accessory.Log.Debug($"LocalContentId:{Svc.ClientState.LocalContentId}");
        // accessory.Log.Debug($"MapId:{Svc.ClientState.MapId}");
    }
    
    
}

#region Helpers

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

    public static uint Id(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["Id"]);
    }

    public static uint ActionId(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["ActionId"]);
    }

    public static uint SourceId(this Event evt)
    {
        return ParseHexId(evt["SourceId"], out var id) ? id : 0;
    }

    public static uint TargetId(this Event evt)
    {
        return ParseHexId(evt["TargetId"], out var id) ? id : 0;
    }

    public static uint IconId(this Event evt)
    {
        return ParseHexId(evt["Id"], out var id) ? id : 0;
    }

    public static Vector3 SourcePosition(this Event evt)
    {
        return JsonConvert.DeserializeObject<Vector3>(evt["SourcePosition"]);
    }

    public static Vector3 TargetPosition(this Event evt)
    {
        return JsonConvert.DeserializeObject<Vector3>(evt["TargetPosition"]);
    }

    public static Vector3 EffectPosition(this Event evt)
    {
        return JsonConvert.DeserializeObject<Vector3>(evt["EffectPosition"]);
    }

    public static float SourceRotation(this Event evt)
    {
        return JsonConvert.DeserializeObject<float>(evt["SourceRotation"]);
    }

    public static float TargetRotation(this Event evt)
    {
        return JsonConvert.DeserializeObject<float>(evt["TargetRotation"]);
    }

    public static string SourceName(this Event evt)
    {
        return evt["SourceName"];
    }

    public static string TargetName(this Event evt)
    {
        return evt["TargetName"];
    }

    public static uint DurationMilliseconds(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["DurationMilliseconds"]);
    }

    public static uint Index(this Event evt)
    {
        return ParseHexId(evt["Index"], out var id) ? id : 0;
    }

    public static uint State(this Event evt)
    {
        return ParseHexId(evt["State"], out var id) ? id : 0;
    }

    public static uint DirectorId(this Event evt)
    {
        return ParseHexId(evt["DirectorId"], out var id) ? id : 0;
    }

    public static uint StatusId(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["StatusID"]);
    }

    public static uint StackCount(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["StackCount"]);
    }

    public static uint Param(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["Param"]);
    }

    public static uint TargetIndex(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["TargetIndex"]);
    }
}

public static class ScriptAccessoryExtensions
{
    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, uint owner, uint duration, float radius, bool safe = false)
    {
        return FastDp(sa, name, owner, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, uint owner, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, Vector3 pos, uint duration, float radius, bool safe = false)
    {
        return FastDp(sa, name, pos, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, Vector3 pos, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory sa, uint target, uint duration, uint delay = 0, string name = "Waypoint")
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = sa.Data.DefaultSafeColor;
        dp.Owner = sa.Data.Me;
        dp.TargetObject = target;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory sa, Vector3 pos, uint duration, uint delay = 0, string name = "Waypoint")
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = sa.Data.DefaultSafeColor;
        dp.Owner = sa.Data.Me;
        dp.TargetPosition = pos;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
    }

    public static int MyIndex(this ScriptAccessory sa)
    {
        return sa.Data.PartyList.IndexOf(sa.Data.Me);
    }
    
    public static int PtIndex(this ScriptAccessory sa, uint pid)
    {
        return sa.Data.PartyList.IndexOf(pid);
    }

    public static IEnumerable<IPlayerCharacter?> GetParty(this ScriptAccessory sa)
    {
        return sa.Data.PartyList.Select(id => (IPlayerCharacter?)sa.Data.Objects.SearchById(id));
    }

    public static IPlayerCharacter? GetMe(this ScriptAccessory sa)
    {
        return (IPlayerCharacter?)sa.Data.Objects.SearchById(sa.Data.Me);
    }
}

public static class IbcHelper
{
    public static bool HasStatus(this IBattleChara chara, uint statusId)
    {
        return chara.StatusList.Any(x => x.StatusId == statusId);
    }

    public static bool HasStatus(this IBattleChara chara, uint[] statusIds)
    {
        return chara.StatusList.Any(x => statusIds.Contains(x.StatusId));
    }
}

public static class MathHelper
{
    public static float V3YAngle(this Vector3 v, bool toRadian = false)
    {
        return V3YAngle(v, Vector3.Zero, toRadian);
    }

    public static float V3YAngle(this Vector3 v, Vector3 origin, bool toRadian = false)
    {
        var angle = ((MathF.Atan2(v.Z - origin.Z, v.X - origin.X) - MathF.Atan2(1, 0)) / float.Pi * -180 + 360) % 360;
        return toRadian ? angle / 180 * float.Pi : angle;
    }

    public static Vector3 V3YRotate(this Vector3 v, float angle, bool isRadian = false)
    {
        return V3YRotate(v, Vector3.Zero, angle, isRadian);
    }

    public static Vector3 V3YRotate(this Vector3 v, Vector3 origin, float angle, bool isRadian = false)
    {
        var radian = isRadian ? angle : angle / 180 * float.Pi;
        return Vector3.Transform(v - origin, Matrix4x4.CreateRotationY(radian)) + origin;
    }

    public static float Rad2Deg(this float angle)
    {
        return angle / MathF.PI * 180;
    }

    public static float Deg2Rad(this float angle)
    {
        return angle * MathF.PI / 180;
    }

    public static int Angle2Dir(this float angle, int dirs, bool diagDiv = true, bool isRadian = false)
    {
        angle = isRadian ? angle.Rad2Deg() : angle;
        return (int)((angle + (diagDiv ? 180f / dirs : 0)) / (360f / dirs));
    }
    
    public static int Pos2Dir(this Vector3 pos, int dirs, bool diagDiv = true)
    {
        return Pos2Dir(pos, Vector3.Zero, dirs, diagDiv);
    }

    public static int Pos2Dir(this Vector3 pos, Vector3 origin, int dirs, bool diagDiv = true)
    {
        return Angle2Dir(pos.V3YAngle(origin), dirs, diagDiv);
    }
}

#endregion