using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script;

[ScriptType(guid: "32BC9D47-D623-507F-CDBF-E17EFEA73FA4", name: "噩梦乐园迷途鬼区", territorys: [1204], version: "0.0.0.2", author:"Cyf5119")]
public class TheStrayboroughDeadwalk
{
    private List<Vector3> tethered = [];
    private uint stackRecord = 0;


    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        tethered = [];
        stackRecord = 0;
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

    [ScriptMethod(name: "老一AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36529"])]
    public void Boss1Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 5000);
    }

    [ScriptMethod(name: "老一地火", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39683"])]
    public void Boss1Exaflare(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var spos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
        var srot = JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
        dp.Name = "Boss1Exaflare";
        dp.Scale = new Vector2(4);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Delay = 4400;
        dp.DestoryAt = 3200;
        for (int i = 0; i < 4; i++)
        {
            spos = new Vector3(spos.X + (float)Math.Sin(srot) * 4, spos.Y, spos.Z + (float)Math.Cos(srot) * 4);
            dp.Position = spos;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            dp.Delay += 1600;
        }
    }

    [ScriptMethod(name: "老一追踪AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39686"])]
    public void Boss1ChasingAoe(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var pos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
        dp.Name = "Boss1ChasingAoe";
        dp.Scale = new Vector2(4);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 1600;

        // var nearTarget = FakeParty.Get().OrderBy(x => Vector3.Distance(pos, x.Position)).FirstOrDefault();
        var target = FakeParty.Get().MinBy(x => Vector3.Distance(pos, x.Position));
        if (target is null) return;
        Thread.Sleep(2000);
        for (int i = 0; i < 4; i++)
        {
            var tpos = target.Position;
            pos += Vector3.Normalize(tpos - pos) * 3;
            dp.Position = pos;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            Thread.Sleep(1600);
        }
    }

    [ScriptMethod(name: "好脑袋的朋友们", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:36533"])]
    public void Boss1Friends(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);

        dp.DestoryAt = 30000;
        dp.Owner = sid;

        dp.Name = $"好脑袋的朋友 {sid} 一";
        dp.Scale = new Vector2(1f, 2f);
        dp.Color = new Vector4(1f, .2f, .2f, 1.5f);
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        dp.Name = $"好脑袋的朋友 {sid} 二";
        dp.Scale = new Vector2(1f, 40f);
        dp.Color = new Vector4(1f, 1f, .2f, .5f);
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "好脑袋的朋友们清除", eventType: EventTypeEnum.PlayActionTimeline, eventCondition: ["Id:7740"],
        userControl: false)]
    public void Boss1FriendsClear(Event @event, ScriptAccessory accessory)
    {
        if (JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]) != 16827) return;
        ParseObjectId(@event["SourceId"], out var sid);
        accessory.Method.RemoveDraw($"好脑袋的朋友 {sid} 一");
        accessory.Method.RemoveDraw($"好脑袋的朋友 {sid} 二");
    }

    [ScriptMethod(name: "好脑袋", eventType: EventTypeEnum.PlayActionTimeline, eventCondition: ["Id:4561"])]
    public void Boss1Heads(Event @event, ScriptAccessory accessory)
    {
        if (JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]) != 16901) return;
        ParseObjectId(@event["SourceId"], out var sid);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Owner = sid;
        dp.Name = $"好脑袋 {sid}";
        dp.Scale = new Vector2(2);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "好脑袋清除", eventType: EventTypeEnum.ActionEffect,
        eventCondition: ["ActionId:regex:^(3653[26])$"])]
    public void Boss1HeadsClear(Event @event, ScriptAccessory accessory)
    {
        ParseObjectId(@event["SourceId"], out var sid);
        accessory.Method.RemoveDraw($"好脑袋 {sid}");
    }

    #endregion

    #region BOSS2

    [ScriptMethod(name: "老二AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36725"])]
    public void Boss2Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 5000);
    }

    [ScriptMethod(name: "老二死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36726"])]
    public void Boss2Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑", duration: 5000);
    }

    [ScriptMethod(name: "连线记录", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0114"], userControl: false)]
    public void Boss2TetheredRecord(Event @event, ScriptAccessory accessory)
    {
        var spos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
        lock (tethered)
        {
            if (tethered.Count > 1)
                tethered.Clear();
            tethered.Add(spos);
        }
    }

    [ScriptMethod(name: "连线清除", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:36720"])]
    public void Boss2TetheredClear(Event @event, ScriptAccessory accessory)
    {
        tethered.Clear();
    }

    [ScriptMethod(name: "老二茶杯", eventType: EventTypeEnum.EnvControl,
        eventCondition: ["Index:regex:^(00000023|00000001)$"])]
    public void Boss2Teacups(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        var id = @event["Id"];
        if (!TeacupsHelper(id, out uint dura, out List<Vector3> pos)) return;
        dp.Name = "老二茶杯";
        dp.Scale = new(19);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = dura;
        foreach (var p in pos)
        {
            dp.Position = p;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    private bool TeacupsHelper(string Id, out uint dura, out List<Vector3> pos)
    {
        pos = [];
        dura = 0;
        if (tethered.Count > 2 || tethered.Count < 1) return false;
        List<(Vector3, Vector3?, List<Vector3>)> _pos;
        switch (Id)
        {
            case "02000100":
                dura = 11500;
                _pos = new List<(Vector3, Vector3?, List<Vector3>)>
                {
                    (new(17, -38, -163), new(17, -38, -177), [new(3.5f, -38, -161.5f), new(30.5f, -38, -178.5f)]),
                    (new(17, -38, -153), new(10, -38, -170), [new(25.5f, -38, -156.5f), new(20.5f, -38, -178.5f)]),
                    (new(17, -38, -153), new(17, -38, -177), [new(20.5f, -38, -178.5f), new(3.5f, -38, -161.5f)]),
                    (new(34, -38, -170), null, [new(8.5f, -38, -173.5f)]),
                    (new(0, -38, -170), null, [new(25.5f, -38, -166.5f)])
                };
                break;
            case "10000800":
                dura = 14500;
                _pos = new List<(Vector3, Vector3?, List<Vector3>)>
                {
                    (new(0, -38, -170), new(34, -38, -170), [new(8.5f, -38, -156.5f), new(25.5f, -38, -183.5f)]),
                    (new(0, -38, -170), new(17, -38, -187), [new(3.5f, -38, -178.5f), new(8.5f, -38, -156.5f)]),
                    (new(17, -38, -187), new(17, -38, -153), [new(30.5f, -38, -161.5f), new(3.5f, -38, -178.5f)])
                };
                break;
            case "00100001":
                dura = 16000;
                pos = tethered;
                return true;
            case "00400020":
                dura = 19000;
                _pos = new List<(Vector3, Vector3?, List<Vector3>)>
                {
                    (new(0, -38, -170), new(17, -38, -163), [new(5, -38, -165), new(22, -38, -182)]),
                    (new(17, -38, -177), new(17, -38, -153), [new(5, -38, -175), new(29, -38, -175)])
                };
                break;
            default:
                return false;
        }

        foreach (var (pos1, pos2, positions) in _pos)
        {
            if (CheckPositions(pos1, pos2))
            {
                pos = positions;
                return true;
            }
        }

        return false;
    }

    private bool CheckPositions(Vector3 pos1, Vector3? pos2)
    {
        if (pos2 is null)
            return Vector3.Distance(pos1, tethered[0]) < 1;
        return Vector3.Distance(pos1, tethered[0]) < 1 && Vector3.Distance((Vector3)pos2, tethered[1]) < 1;
    }
    
    #endregion

    #region BOSS3

    [ScriptMethod(name: "老三AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37168"])]
    public void Boss3Aoe(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("流血AOE", duration: 5000);
    }

    [ScriptMethod(name: "老三分摊记录", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:37144"],
        userControl: false)]
    public void Boss3ShareRecord(Event @event, ScriptAccessory accessory)
    {
        ParseObjectId(@event["TargetId"], out var tid);
        stackRecord = tid;
    }

    [ScriptMethod(name: "老三分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37142"])]
    public void Boss3Share(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三分摊";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Scale = new Vector2(8, 80);
        dp.DestoryAt = 5000;
        dp.Owner = sid;
        
        Thread.Sleep(100);
        dp.TargetObject = stackRecord;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "老三辣尾", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37139"])]
    public void Boss3Rect1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三辣尾";
        dp.Color = new Vector4(.2f, 1f, 1f, .8f);
        dp.Scale = new Vector2(16, 80);
        dp.DestoryAt = 6700;
        dp.Owner = sid;
        
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "老三辣翅", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37147"])]
    public void Boss3Rect2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三辣翅";
        dp.Color = new Vector4(.2f, 1f, 1f, .8f);
        dp.Scale = new Vector2(12, 50);
        dp.DestoryAt = 6700;
        dp.Owner = sid;
        
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    
    [ScriptMethod(name: "老三小怪", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37340"])]
    public void Boss3Rect3(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        ParseObjectId(@event["SourceId"], out var sid);
        dp.Name = "老三小怪";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Scale = new Vector2(4, 40);
        dp.DestoryAt = 6000;
        dp.Owner = sid;
        
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    #endregion
}
