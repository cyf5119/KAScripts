using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using KodakkuAssist.Data;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Endwalker.TheGildedAraya;

[ScriptType(guid: "0F38EA83-3D3A-AF4E-8908-56655F76014E", name: "阿修罗歼灭战", territorys: [1136], version: "0.0.0.1", author: "Cyf5119")]
public class TheGildedAraya
{
    public void Init(ScriptAccessory sa)
    {
        sa.Method.RemoveDraw(".*");
    }
    
    [ScriptMethod(name: "王妃的威光", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36001"])]
    public void LowerRealm(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", 5000);
    }
    
    [ScriptMethod(name: "神出鬼没", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:35990"])]
    public void Ephemerality(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", 5000);
    }
    
    [ScriptMethod(name: "斩击", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:35990", "TargetIndex:1"])]
    public void Laceration(Event evt, ScriptAccessory sa)
    {
        var targets = sa.Data.Objects.Where(x => x.DataId == 0x40F8);
        foreach (var target in targets)
        {
            var dp = sa.FastDp("斩击", target.GameObjectId, 7300, 9);
            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }
    
    [ScriptMethod(name: "光玉", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36000"])]
    public void CuttingJewel(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("死刑", 5000);
    }
    
    [ScriptMethod(name: "圆斩", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:35970"])]
    public void PedestalPurge(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("圆斩", evt.EffectPosition, 4000, 60);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "环斩", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:35972"])]
    public void WheelOfDeincarnation(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("环斩", evt.EffectPosition, 4000, 96);
        dp.InnerScale = new Vector2(48);
        dp.Radian = float.Pi * 2;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
    }
    
    [ScriptMethod(name: "剑光波", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:35974"])]
    public void Bladewise(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("剑光波", evt.SourceId, 4000, new Vector2(28, 100));
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "三头六臂：断", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3601[123])$"])]
    public void Khadga(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("三头六臂：断", evt.SourceId, 2000, 20);
        dp.Delay = 11000;
        dp.Radian = float.Pi;
        if (evt.ActionId > 36011)
            dp.Rotation = float.Pi / (evt.ActionId > 36012 ? 2 : -2);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }
    
    [ScriptMethod(name: "脸", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(3602[23])$"])]
    public void TheFace(Event evt, ScriptAccessory sa)
    {
        var taid = evt.ActionId < 36023 ? 36015 : 36016;
        var target = sa.Data.Objects.FirstOrDefault(x => x is IBattleChara y && y.CastActionId == taid);
        if (target == null) return;
        
        var dp = sa.FastDp("脸", target.GameObjectId, 8000, 20);
        dp.Radian = float.Pi;
        sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Fan, dp);
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