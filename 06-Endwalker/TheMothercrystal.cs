using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Utility.Numerics;
using Newtonsoft.Json;
using KodakkuAssist.Data;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Scripts.Endwalker.TheMothercrystal;

[ScriptType(guid: "EB83FE40-4DDC-42C8-B248-CBFDF9D0E2C1", name: "海德林歼灭战", territorys: [995], version: "0.0.0.1", author: "Cyf5119")]
public class TheMothercrystal
{
    private static List<Vector3> _beaconList = [];
    private static readonly Vector3 Center = new(100f, 0f, 100f);

    public void Init(ScriptAccessory sa)
    {
        _beaconList = [];
        sa.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2607[12]|26064)$"])]
    public void Aoe(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", 5000);
    }

    [ScriptMethod(name: "众生离绝", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:26043"])]
    public void Exodus(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("众生离绝", 14582, true);
    }

    [ScriptMethod(name: "分摊死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26070"])]
    public void MousasScorn(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("分摊死刑", evt.TargetId, 5000, 4);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        sa.Method.TextInfo("分摊死刑", 5000);
    }

    [ScriptMethod(name: "扇形死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26069"])]
    public void HerossSundering(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("扇形死刑", evt.SourceId, 5000, 40);
        dp.TargetObject = evt.TargetId;
        dp.Radian = float.Pi / 2;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        sa.Method.TextInfo("扇形死刑", 5000);
    }

    [ScriptMethod(name: "钢铁", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2273", "Param:436"])]
    public void HighestHoly(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("钢铁", evt.SourceId, 6000, 10);
        dp.Color = dp.Color.WithW(0.5f);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Color = dp.Color.WithW(3);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "月环", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2273", "Param:437"])]
    public void Anthelion(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("月环", evt.SourceId, 6000, 40);
        dp.InnerScale = new Vector2(5);
        dp.Radian = float.Pi * 2;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
    }

    [ScriptMethod(name: "十字", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:2273"])]
    public void Equinox(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("十字", evt.SourceId, 6000, new Vector2(10, 80));
        dp.Color = dp.Color.WithW(0.5f);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
        dp.Rotation = float.Pi / 2;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);

        dp.ScaleMode = ScaleMode.XByTime;
        dp.Color = dp.Color.WithW(3);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
        dp.Rotation = 0;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(name: "分摊垒石", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:27737"])]
    public void CrystallineStoneIII(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("分摊垒石", evt.TargetId, 5000, 6, true);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "光芒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:26062"])]
    public void Beacon(Event evt, ScriptAccessory sa)
    {
        lock (_beaconList)
        {
            _beaconList.Add(evt.EffectPosition);
            if (_beaconList.Count < 15) return;
            Vector3 wpos1 = new Vector3(0), wpos2 = new Vector3(0);
            for (var i = 0; i < 5; i++)
            {
                wpos1 += _beaconList[i + 10] - Center;
                wpos2 += _beaconList[i] - Center;
            }

            wpos1 = Vector3.Normalize(wpos1) * -8 + Center;
            wpos2 = Vector3.Normalize(wpos2) * 8 + Center;

            var dp = sa.FastDp("光芒", sa.Data.Me, 1, 2, true);
            dp.ScaleMode = ScaleMode.YByDistance;

            dp.TargetPosition = wpos1;
            dp.Delay = 5200;
            dp.DestoryAt = 12700;
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            dp.TargetPosition = wpos2;
            dp.Delay = 17900;
            dp.DestoryAt = 3200;
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
    }

    [ScriptMethod(name: "幻日环", eventType: EventTypeEnum.ObjectChanged, eventCondition: ["DataId:regex:^(201172[45])$", "Operate:Add"])]
    public void ParhelicCircle(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("幻想光", Center, 9300, 6);
        if (evt["DataId"] == "2011724")
        {
            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            for (var i = 0; i < 6; i++)
            {
                dp.Position = Vector3.Transform(new Vector3(0, 0, 17), Matrix4x4.CreateRotationY(float.Pi / 3 * i + evt.SourceRotation)) + Center;
                sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
        }
        else
        {
            for (var i = 0; i < 3; i++)
            {
                dp.Position = Vector3.Transform(new Vector3(0, 0, 8), Matrix4x4.CreateRotationY(float.Pi / 3 * (2 * i + 1) + evt.SourceRotation)) + Center;
                sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
        }
    }

    [ScriptMethod(name: "光波", eventType: EventTypeEnum.ObjectChanged, eventCondition: ["DataId:2011723", "Operate:Add"], suppress: 5000)]
    public void LightWave(Event evt, ScriptAccessory sa)
    {
        foreach (var actor in sa.Data.Objects.Where(x => x.DataId == 9020 && Vector3.Distance(x.Position, new Vector3(100, 0, 100)) > 20))
        {
            var dp = sa.FastDp("光波", actor.EntityId, 15000, new Vector2(16, 60));
            dp.Offset = new Vector3(0, 0, 7.5f);
            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
    }

    [ScriptMethod(name: "回声", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0131"])]
    public void Echoes(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("回声", evt.TargetId, 10000, 6, true);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
}

public static class ScriptAccessoryExtensions
{
    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, ulong owner, uint duration, float radius, bool safe = false)
    {
        return FastDp(sa, name, owner, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, ulong owner, uint duration, Vector2 scale, bool safe = false)
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
}