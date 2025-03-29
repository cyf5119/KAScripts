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

namespace Cyf5119Script.Endwalker.P1To4N;

[ScriptType(guid: "AD2A9D42-E040-442E-A8F2-86F7D13768BD", name: "P1N-P4N捆绑包", territorys: [1002, 1004, 1006, 1008],
    version: "0.0.0.1", author: "Cyf5119", note: "包含P1N,P2N,P3N,P4N")]
public class P1To4N
{
    // private uint _status = 0;

    public void Init(ScriptAccessory accessory)
    {
        // _status = 0;
        accessory.Method.RemoveDraw(".*");
    }

    #region P1N

    [ScriptMethod(EventTypeEnum.StartCasting, "P1N-死刑", ["ActionId:26099"])]
    public void HeavyHand(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("单体死刑", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P1N-AOE", ["ActionId:regex:^(26100|26089|26090)$"])]
    public void P1NAOE(Event @event, ScriptAccessory accessory)
    {
        var dura = @event.ActionId() == 26100 ? 5000 : 7000;
        accessory.Method.TextInfo("AOE", dura);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P1N-左右刀", ["ActionId:regex:^(2806[67])$"])]
    public void GaolersFlail(Event @event, ScriptAccessory accessory)
    {
        var isLeft = @event.ActionId() > 28066;
        var dp = accessory.FastDp("左右刀", @event.SourceId(), 8700, 60);
        dp.Radian = float.Pi;
        dp.Rotation = isLeft ? float.Pi / 2 : -float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P1N-直线击退", ["ActionId:26085"])]
    public void PitilessFlail(Event @event, ScriptAccessory accessory)
    {
        if (@event.TargetId() != accessory.Data.Me) return;
        // TODO fix the distance
        var dp = accessory.FastDp("直线击退", accessory.Data.Me, 5000, new Vector2(2, 11), true);
        dp.TargetObject = @event.SourceId();
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.TargetIcon, "P1N-分摊", ["Id:003E"])]
    public void TrueHoly(Event @event, ScriptAccessory accessory)
    {
        if (Svc.ClientState.TerritoryType != 1002) return;
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

    #endregion

    #region P2N

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-分摊死刑", ["ActionId:26638"])]
    public void DoubledImpact(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("分摊死刑", @event.TargetId(), 5000, 6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-AOE", ["ActionId:regex:^(26639|26614)$"])]
    public void P2NAOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-吐息飞瀑1", ["ActionId:regex:^(2661[567])$"])]
    public void SpokenCataract1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("吐息飞瀑", @event.SourceId(), 7000, 60);
        dp.Radian = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-吐息飞瀑2", ["ActionId:regex:^(2662[12])$"])]
    public void SpokenCataract2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("吐息飞瀑", @event.SourceId(), 7000, new Vector2(15, 100));
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-震荡波", ["ActionId:regex:26631"])]
    public void Shockwave(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("震荡波", accessory.Data.Me, 6000, new Vector2(2, 13), true);
        dp.TargetPosition = @event.EffectPosition();
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "P2N-连贯攻击", ["ActionId:regex:27924", "TargetIndex:1"])]
    public void Coherence(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("连贯攻击", @event.SourceId(), 8400, new Vector2(6, 60), true);
        dp.TargetObject = @event.TargetId();
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P2N-分离", ["ActionId:regex:26630"])]
    public void Dissociation(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("分离", @event.SourceId(), 8000, new Vector2(20, 50));
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    #endregion

    #region P3N

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-狱炎炎击", ["ActionId:26291"])]
    public void HeatOfCondemnation(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("狱炎炎击", @event.TargetId(), 6000, 6);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-AOE", ["ActionId:regex:^(26296)$"])]
    public void P3NAOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-暗黑不死鸟", ["ActionId:26281"])]
    public void DeadRebirth(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("大AOE", 10000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-场中大圈", ["ActionId:26263"])]
    public void Fireplume(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("场中大圈", @event.EffectPosition(), 6000, 15);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-半场刀", ["ActionId:regex:^(2629[23])$"])]
    public void Cinderwing(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("半场刀", @event.SourceId(), 5000, 60);
        dp.Radian = float.Pi;
        dp.Rotation = @event.ActionId() > 26292 ? float.Pi / 2 : -float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3N-狱炎之焰", ["ActionId:26287"])]
    public void TrailOfCondemnation(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("狱炎之焰", @event.SourceId(), 4500, new Vector2(15, 40));
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    #endregion

    #region P4N

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-优雅除脏", ["ActionId:27216"])]
    public void ElegantEvisceration(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("优雅除脏", @event.TargetId(), 5000, 5);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-AOE", ["ActionId:regex:^(27217|27200)$"])]
    public void P4NAOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-地狱穿刺", ["ActionId:27215"])]
    public void HellSkewer(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("优雅除脏", @event.SourceId(), 5000, new Vector2(6, 60));
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-喷水板画", ["ActionId:27198"])]
    public void WellPinax(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("喷水板画", accessory.Data.Me, 9000, new Vector2(2, 15), true);
        dp.TargetPosition = new Vector3(100, 0, 100);
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-换位强袭扇形", ["ActionId:27214"])]
    public void ShiftingStrikeFan(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("换位强袭扇形", @event.SourceId(), 8500, 60);
        dp.Radian = float.Pi / 180 * 120;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4N-换位强袭击退", ["ActionId:28082"])]
    public void ShiftingStrikeKnockback(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("换位强袭击退", accessory.Data.Me, 8700, new Vector2(2, 25), true);
        dp.TargetObject = @event.SourceId();
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    #endregion
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

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }

    public static uint StatusId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StatusID"]);
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
}