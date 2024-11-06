using System;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script;

[ScriptType(guid: "243443E4-FC8E-3EA6-9A75-FD2A4B6990FF", name: "忆中金曦亚历山德里亚", territorys: [1199], version: "0.0.0.1",
    author: "Cyf5119")]
public class Alexandria
{
    private List<List<uint>> InterferonList = [];

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        InterferonList = [];
    }

    private static bool ParseObjectId(string? idStr, out uint id)
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

    #region BOSS1

    [ScriptMethod(name: "老一AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36387"])]
    public void Boss1Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 5000);
    }

    [ScriptMethod(name: "老一死刑与分摊", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:003E"])]
    public void Boss1TankbusterAndStack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["TargetId"], out var tid);
        dp.Name = "老一分摊";
        dp.Owner = tid;
        dp.Scale = new Vector2(6);
        dp.Color = new Vector4(.2f, 1f, .2f, .8f);
        dp.DestoryAt = 6000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        accessory.Method.TextInfo("坦克死刑，其余分摊", duration: 5000);
    }

    [ScriptMethod(name: "老一扇形", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(36379|36381)$"])]
    public void Boss1Fan(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
        dp.Name = "老一扇形";
        dp.Owner = sid;
        dp.Scale = new Vector2(40);
        dp.Radian = float.Pi / 180 * (aid == 36379 ? 120 : 240);
        dp.Rotation = aid == 36379 ? 0 : float.Pi;
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.DestoryAt = 6000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "老一十字与月环首次", eventType: EventTypeEnum.AddCombatant,
        eventCondition: ["DataId:regex:^(16757|16756)$"])]
    public void Boss1CrossAndDonut(Event @event, ScriptAccessory accessory)
    {
        ParseObjectId(@event["SourceId"], out var sid);
        var did = JsonConvert.DeserializeObject<uint>(@event["DataId"]);
        if (InterferonList.Count > 4) return;
        InterferonList.Add([sid, did]);
        if (InterferonList.Count < 5) return;
        DrawCrossAndDonut(accessory);
    }

    [ScriptMethod(name: "老一十字与月环后续", eventType: EventTypeEnum.ActionEffect,
        eventCondition: ["ActionId:regex:^(36383|36382)$"])]
    public void Boss1CrossAndDonutEffect(Event @event, ScriptAccessory accessory)
    {
        DrawCrossAndDonut(accessory);
    }

    private void DrawCrossAndDonut(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw("Cross or Donut");
        if(InterferonList.Count < 1) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        var sid = InterferonList[0][0];
        var did = InterferonList[0][1];
        dp.Name = "Cross or Donut";
        dp.Owner = sid;
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.DestoryAt = InterferonList.Count > 4 ? 10000 : 2500;
        if (did == 16757)
        {
            dp.Scale = new Vector2(6, 80);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
            dp.Rotation = float.Pi / 2;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
        }
        else
        {
            dp.Scale = new Vector2(40);
            dp.InnerScale = new Vector2(4.2f);
            dp.Radian = float.Pi * 2;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
        }

        InterferonList.RemoveAt(0);
    }

    #endregion

    #region BOSS2

    [ScriptMethod(name: "老二AOE", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(36338|36323)$"])]
    public void Boss2Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 5000);
    }

    [ScriptMethod(name: "老二死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36339"])]
    public void Boss2Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑", duration: 5000);
    }

    [ScriptMethod(name: "老二分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36333"])]
    public void Boss2Stack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["TargetId"], out var tid);
        dp.Name = "老二分摊";
        dp.Owner = tid;
        dp.Scale = new Vector2(6);
        dp.Color = new Vector4(.2f, 1, .2f, .8f);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        accessory.Method.TextInfo("分摊", duration: 5000);
    }

    [ScriptMethod(name: "老二辣尾", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36327"])]
    public void Boss2Rect1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老二辣尾";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(15, 90);
        dp.DestoryAt = 5000;
        dp.Owner = sid;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(name: "老二辣翅", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(3633[0|1])$"])]
    public void Boss2Rect2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        var epos = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
        var rot = JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
        dp.Name = "老二辣翅";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(25, 90);
        dp.DestoryAt = 5000;
        dp.Position = epos;
        dp.Rotation = rot;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "老二小怪", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39138"])]
    public void Boss2Rect3(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老二小怪";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(8, 55);
        dp.DestoryAt = 7800;
        dp.Owner = sid;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "老二三角", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39136"])]
    public void Boss2Triangle(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老二三角";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2((float)Math.Sqrt(2) * 40, (float)Math.Sqrt(2) * 20);
        dp.DestoryAt = 10100;
        dp.Owner = sid;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    #endregion

    #region BOSS3

    [ScriptMethod(name: "老三AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36765"])]
    public void Boss3Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 5000);
    }

    [ScriptMethod(name: "老三大AOE", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:36789"])]
    public void Boss3Aoe2(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("大AOE", duration: 9800);
    }
    
    [ScriptMethod(name: "老三多段AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36795"])]
    public void Boss3Aoe3(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("多段AOE", duration: 9000);
    }

    [ScriptMethod(name: "老三分摊", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:36778"])]
    public void Boss3Stack(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        ParseObjectId(@event["TargetId"], out var tid);
        dp.Name = "老三分摊";
        dp.Color = new Vector4(.2f, 1f, .2f, .8f);
        dp.Scale = new Vector2(6, 40);
        dp.DestoryAt = 5100;
        dp.Owner = sid;
        dp.TargetObject = tid;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        accessory.Method.TextInfo("分摊", duration: 5100);
    }

    [ScriptMethod(name: "老三刀", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:regex:^(39(007|238|249))$"])]
    public void Boss3Fan(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
        dp.Name = "老三刀";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(40);
        dp.Radian = float.Pi;
        dp.Owner = sid;
        dp.DestoryAt = aid == 39007 ? 5000 : 7000;
        dp.Rotation = aid == 39249 ? float.Pi / 2 : float.Pi / -2;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "老三爪子", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:39615"])]
    public void Boss3Rect(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
        dp.Name = "老三爪子";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(10, 40);
        dp.Owner = sid;
        dp.DestoryAt = 7000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "老三浮游炮", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:39616"])]
    public void Boss3Donut(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三浮游炮";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(40);
        dp.InnerScale = new Vector2(6);
        dp.Radian = float.Pi * 2;
        dp.Owner = sid;
        dp.DestoryAt = 7000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
    }
    
    [ScriptMethod(name: "老三击退预测", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36794"])]
    public void 老三击退预测(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三击退预测";
        dp.Scale = new(1, 15);
        dp.Color = new(0.2f, 1f, 1f, 1.6f);
        dp.Owner = accessory.Data.Me;
        dp.TargetObject = sid;
        dp.Rotation = float.Pi;
        dp.DestoryAt = 6000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Displacement, dp);
    }
    
    [ScriptMethod(name: "老三爆炸线", eventType: EventTypeEnum.StartCasting,
        eventCondition: ["ActionId:39239"])]
    public void Boss3Rect2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
        dp.Name = "老三爆炸线";
        dp.Color = new Vector4(1f, .2f, .2f, .8f);
        dp.Scale = new Vector2(8, 50);
        dp.Owner = sid;
        uint until = 2800;
        dp.Delay = 8500 - until;
        dp.DestoryAt = until;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
    }

    #endregion
}