using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
// using ECommons;
using KodakkuAssist.Data;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Scripts.Dev.PrintTimeLine;

// [ScriptType(guid: "0558F348-30BC-4381-0578-95B86D3D1548", name: "PrintTimeLine", territorys: [], version: "0.0.0.1", author: "Cyf5119")]
public class PrintTimeLine
{
    public void Init(ScriptAccessory sa)
    {
    }

    [ScriptMethod(name: "OnTargetIcon", eventType: EventTypeEnum.TargetIcon, eventCondition: [], userControl: false)]
    public void OnTargetIcon(Event evt, ScriptAccessory sa)
    {
        
    }
    
    
}

#region Helpers

public static class ScriptAccessoryExtensions
{
    private static DrawPropertiesEdit _fastDp(ScriptAccessory sa, ulong? owner, Vector3 pos, uint duration, uint delay, Vector2 scale, bool safe)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        if (owner is null)
            dp.Position = pos;
        else
            dp.Owner = (ulong)owner;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Name = "fastDp";
        dp.Radian = float.Pi * 2;
        return dp;
    }
    
    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, ulong owner, uint duration, uint delay, float radius, bool safe = false)
    {
        return _fastDp(sa, owner, Vector3.Zero, duration, delay, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, ulong owner, uint duration, uint delay, Vector2 scale, bool safe = false)
    {
        return _fastDp(sa, owner, Vector3.Zero, duration, delay, scale, safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, Vector3 pos, uint duration, uint delay, float radius, bool safe = false)
    {
        return _fastDp(sa, null, pos, duration, delay, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, Vector3 pos, uint duration, uint delay, Vector2 scale, bool safe = false)
    {
        return _fastDp(sa, null, pos, duration, delay, scale, safe);
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory sa, uint target, uint duration, uint delay = 0)
    {
        var dp = _fastDp(sa, sa.Data.Me, Vector3.Zero, duration, delay, new Vector2(2), true);
        dp.TargetObject = target;
        dp.ScaleMode = ScaleMode.YByDistance;
        dp.Name = "waypointDp";
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory sa, Vector3 pos, uint duration, uint delay = 0)
    {
        var dp = _fastDp(sa, sa.Data.Me, Vector3.Zero, duration, delay, new Vector2(2), true);
        dp.TargetPosition = pos;
        dp.ScaleMode = ScaleMode.YByDistance;
        dp.Name = "waypointDp";
        return dp;
    }

    public static int MyIndex(this ScriptAccessory sa)
    {
        return sa.Data.PartyList.IndexOf(sa.Data.Me);
    }

    public static IEnumerable<IPlayerCharacter> GetParty(this ScriptAccessory sa)
    {
        foreach (var pid in sa.Data.PartyList)
        {
            var obj = sa.Data.Objects.SearchByEntityId(pid);
            if (obj is IPlayerCharacter character) yield return character;
        }
    }

    public static IPlayerCharacter? GetMe(this ScriptAccessory sa)
    {
        return sa.Data.MyObject;
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
}

#endregion