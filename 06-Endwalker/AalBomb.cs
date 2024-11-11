using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Endwalker.AalBomb;

[ScriptType(guid: "2F27EFB2-CE87-4E6A-9D83-60FE4CC26141", name: "AalBomb", territorys: [1179, 1180], version: "0.0.0.1", author: "Cyf5119")]
public class AalBomb
{
    [UserSetting(note:"每轮炸弹检测间隔（毫秒）")]
    public int Prop1 { get; set; } = 100;
    
    public List<IBattleChara> bombs = new List<IBattleChara>();
    
    public void Init(ScriptAccessory accessory)
    {
        bombs.Clear();
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
    
    [ScriptMethod(name: "生成炸弹", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:regex:^(1648[19])$"])]
    public unsafe void BombSpawn(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var source = (IBattleChara?)accessory.Data.Objects.SearchById(sid);
        if (source is null)
        {
            // accessory.Method.SendChat("/e source is null!");
            return;
        }
        // var timelineAnimationState0 = source.Struct()->Timeline.AnimationState[0];
        // accessory.Method.SendChat($"/e 生成 {sid:X}:{timelineAnimationState0}.");
        lock (bombs)
        {
            bombs.Add(source);
            if (bombs.Count >= 6)
                BombDetect(accessory);
        }
    }

    [ScriptMethod(name: "炸弹清除", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(35165|35194)$"])]
    public void BombClear(Event @event, ScriptAccessory accessory)
    {
        bombs.Clear();
        accessory.Method.RemoveDraw("老三炸弹");
    }

    public unsafe void BombDetect(ScriptAccessory accessory)
    {
        if (bombs.Count < 6) return;
        foreach (var bomb in bombs)
        {
            if (bomb.Struct()->Timeline.AnimationState[0] > 0)
            {
                // accessory.Method.SendChat($"/e 已找到 {bomb.EntityId:X}");
                DrawBomb(accessory, bomb);
                return;
            }
        }
        Thread.Sleep(Prop1);
        BombDetect(accessory);
    }

    public unsafe void DrawBomb(ScriptAccessory accessory, IBattleChara bomb0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "老三炸弹";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 21000;
        dp.Scale = new Vector2(12);
        dp.Owner = bomb0.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        var bomb1 = (IBattleChara)accessory.Data.Objects.SearchById(bomb0.Struct()->Vfx.Tethers[0].TargetId);
        dp.Owner = bomb1.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        var bomb2 = (IBattleChara)accessory.Data.Objects.SearchById(bomb1.Struct()->Vfx.Tethers[0].TargetId);
        dp.Owner = bomb2.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
}

