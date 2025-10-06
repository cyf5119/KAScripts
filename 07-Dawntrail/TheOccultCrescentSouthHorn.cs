using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using Lumina.Excel;
using Lumina.Excel.Sheets;
// using ECommons;
// using ECommons.DalamudServices;
using KodakkuAssist.Data;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Scripts.Dawntrail.TheOccultCrescentSouthHorn;

// [ScriptType(guid: "A9952141-34C5-459D-9CE1-A05CB6AE8D29", name: "新月自用", territorys: [1252, 1278], version: "0.0.0.1", author: "Cyf5119")]
public class TheOccultCrescentSouthHorn
{
    public void Init(ScriptAccessory sa)
    {
        sa.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "StartCast", eventType: EventTypeEnum.StartCasting, userControl: false)]
    public void OnStartCast(Event evt, ScriptAccessory sa) => AutoDraw.StartCast(evt, sa);

    [ScriptMethod(name: "CancelCast", eventType: EventTypeEnum.CancelAction, userControl: false)]
    public void OnCancelCast(Event evt, ScriptAccessory sa) => AutoDraw.CancelCast(evt, sa);
    
    // TODO
    
    // CE101 黑色彗星
    // 步进地火
    
    // CE102 神秘土偶
    // 密集的圆形
    
    // CE103 指令罐
    // 圆形与十字得提前一些
    
    // CE104 进化加鲁拉
    // 需要做冲锋组合技
    
    // CE105 死亡爪
    // 需要水平垂直90刀与分身90刀
    
    // CE106 水晶龙
    
    
    // CE107 新月狂战士
    // hoppingMad的顺序？
    
    // CE108 回廊恶魔
    
    
    // CE109 新月骑士群
    // 需要直线判定时间与步进地火
    
    // CE110 鬼火苗
    // 击退
    
    // CE111 尼姆瓣齿鲨
    // 圆形步进地火
    
    // CE112 复原狮像
    // 扇形和衰减引爆同属性的球要画
    
    // CE113 夺心魔
    // 需要强制移动相关

    // CE114 跃立狮
    // 转转转的直线和圆形
    
    // CE115 金钱龟
    // 热病冻结与强制移动提示还有击退提示
}

#region AutoDraw

public class CustomAction
{
    public uint? CastType { get; set; } = null;
    public float? Radius { get; set; } = null;
    public float? InnerRadius { get; set; } = null;
    public float? Width { get; set; } = null;
    public float? Degree { get; set; } = null;
    public uint? Delay { get; set; } = null;
    public uint? Duration { get; set; } = null;
    public Vector4? Color { get; set; } = null;
    
    public CustomAction(uint? castType=null, float? radius=null, float? innerRadius=null, float? width=null,  float? degree=null, uint? delay=null, uint? duration=null,  Vector4? color=null)
    {
        CastType = castType;
        Radius = radius;
        InnerRadius = innerRadius;
        Width = width;
        Degree = degree;
        Delay = delay;
        Duration = duration;
        Color = color;
    }
}

public static class AutoDraw
{
    private static readonly HashSet<uint> EnabledActions =
    [
        // CE101 黑色彗星
        41163, 41164, 41147, 41148,

        // CE102 神秘土偶
        41137, 41130, 41133, 41132, 41131,

        // CE103 指令罐
        41420, 39470, 41421, 39471,

        // CE104 进化加鲁拉
        41184, 43262, 41180,

        // CE105 死亡爪
        41315, 41316, 41317,

        // CE106 水晶龙
        42766, 42767, 42768, 42769,
        42728, 42729, 42730, 42731, 42732, 42733, 42734, 42735, 41758, 41759, 41760, 41761,

        // CE107 新月狂战士
        42691, 37804,

        // CE108 回廊恶魔
        41360, 41357,

        // CE110 鬼火苗
        42032, 42033, 42034, 42035,

        // CE111 尼姆瓣齿鲨
        41723, 41722, 43149,

        // CE112 复原狮像
        41297,

        // CE113 夺心魔
        41170, 41256, 41257, 41314,

        // CE114 跃立狮
        41411, 41407,
    ];

    private static readonly Dictionary<uint, CustomAction> CustomActions = new()
    {
        // CE101 黑色彗星
        { 41148, new CustomAction(innerRadius: 8) },

        // CE102 神秘土偶
        { 41137, new CustomAction(degree: 60) },

        // CE104 进化加鲁拉
        { 41184, new CustomAction(degree: 45) },
        { 43262, new CustomAction(degree: 120) },
        { 41180, new CustomAction(degree: 120) },

        // CE106 水晶龙
        { 42767, new CustomAction(innerRadius: 5) },
        { 42769, new CustomAction(innerRadius: 5) },
        { 42729, new CustomAction(innerRadius: 7) },
        { 42730, new CustomAction(innerRadius: 13) },
        { 42731, new CustomAction(innerRadius: 19) },
        { 42733, new CustomAction(innerRadius: 7) },
        { 42734, new CustomAction(innerRadius: 13) },
        { 42735, new CustomAction(innerRadius: 19) },
        { 42759, new CustomAction(innerRadius: 7) },
        { 42760, new CustomAction(innerRadius: 13) },
        { 42761, new CustomAction(innerRadius: 19) },

        // CE108 回廊恶魔
        { 41360, new CustomAction(degree: 180) },

        // CE110 鬼火苗
        { 42032, new CustomAction(innerRadius: 7) },
        { 42033, new CustomAction(innerRadius: 7) },

        // CE111 尼姆瓣齿鲨
        { 43149, new CustomAction(degree: 60) },

    };

    public static void StartCast(Event evt, ScriptAccessory sa)
    {
        if (!EnabledActions.Contains(evt.ActionId()))
            return;
        
        var aid = evt.ActionId();
        var sid = evt.SourceId();
        var tid = evt.TargetId();
        var dura = evt.DurationMilliseconds() + 300;
        var epos = evt.EffectPosition();

        // var action = Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Action>().GetRow(aid);
        var action = new Lumina.Excel.Sheets.Action();
        
        uint castType = action.CastType;
        float radius = action.EffectRange;
        float innerRadius = radius / 2;
        float width = action.XAxisModifier;
        float degree = 90f;
        uint delay = 0;
        uint duration = dura;
        Vector4 color = sa.Data.DefaultDangerColor;
        if (CustomActions.ContainsKey(aid))
        {
            castType = CustomActions[aid].CastType ?? castType;
            radius = CustomActions[aid].Radius ?? radius;
            innerRadius = CustomActions[aid].InnerRadius ?? innerRadius;
            width = CustomActions[aid].Width ?? width;
            degree = CustomActions[aid].Degree ?? degree;
            delay = CustomActions[aid].Delay ?? delay;
            duration = CustomActions[aid].Duration ?? duration;
            color = CustomActions[aid].Color ?? color;
        }
        
        var sobj = sa.Data.Objects.SearchById(sid);
        var tobj = sa.Data.Objects.SearchById(tid);
        if (sobj is null)
            return;

        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = $"AutoDraw {sid:X}:{aid}";

        if (castType >= 3 && castType <= 5) // 三到五的加上目标圈
            radius += sobj.HitboxRadius;
        
        var shape = DrawTypeEnum.Circle;
        switch (castType)
        {
            case 2 or 5 or 6 or 7: // 圆形 6为扇形或者圆形 7放置型地面圆形
                shape = DrawTypeEnum.Circle;
                break;
            case 10: // 环形
                shape = DrawTypeEnum.Donut;
                dp.InnerScale = new Vector2(innerRadius);
                dp.Radian = 360f.Deg2Rad();
                break;
            case 3 or 13: // 扇形
                shape = DrawTypeEnum.Fan;
                dp.Radian = degree.Deg2Rad();
                break;
            case 4 or 8 or 11 or 12: // 矩形 8从源头到目标或地点 11为十字
                shape = DrawTypeEnum.Rect;
                break;
            default: // 1为单体 9不存在 14为三角形
                return;
        }
        if (shape is DrawTypeEnum.Circle or DrawTypeEnum.Donut)
        {
            if (tid != sid)
                dp.Owner = tid;
            else
                dp.Position = epos;
        }
        else
        {
            dp.Owner = sid;
        }
        if (castType == 8) // 向目标或地点的矩形
        {
            if (tobj is null)
                return;
            dp.TargetObject = tid;
            dp.ScaleMode = ScaleMode.YByDistance;
        }
        
        dp.Scale = new Vector2(width != 0 ? width : radius, radius);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.Color = color;
        
        sa.Method.SendDraw(DrawModeEnum.Default, shape, dp);
        if (castType == 11)
        {
            dp.Rotation += 90f.Deg2Rad();
            sa.Method.SendDraw(DrawModeEnum.Default, shape, dp);
        }
        sa.Log.Debug($"AutoDraw {sid:X}:{aid}");
    }

    public static void CancelCast(Event evt, ScriptAccessory sa)
    {
        var sid = evt.SourceId();
        var aid = evt.ActionId();
        sa.Method.RemoveDraw($"AutoDraw {sid:X}:{aid}");
        sa.Log.Debug($"CancelDraw {sid:X}:{aid}");
    }
}

#endregion


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
    private static DrawPropertiesEdit _fastDp(ScriptAccessory sa, uint? owner, Vector3 pos, uint duration, uint delay, Vector2 scale, bool safe)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        if (owner is null)
            dp.Position = pos;
        else
            dp.Owner = (ulong)owner;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Scale = scale;
        dp.Name = "fastDp";
        dp.Radian = float.Pi * 2;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, uint owner, uint duration, uint delay, float radius, bool safe = false)
    {
        return _fastDp(sa, owner, Vector3.Zero, duration, delay, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, uint owner, uint duration, uint delay, Vector2 scale, bool safe = false)
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

    public static int PartyIndex(this ScriptAccessory sa, uint eid)
    {
        return sa.Data.PartyList.IndexOf(eid);
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

    public static float Rad2Deg(this float radians)
    {
        return (radians * 180 / float.Pi + 360) % 360;
    }

    public static float Deg2Rad(this float degrees)
    {
        return degrees % 360 == 0 ? float.Pi * 2 : (degrees + 360) % 360 / 180 * float.Pi;
    }
}

#endregion