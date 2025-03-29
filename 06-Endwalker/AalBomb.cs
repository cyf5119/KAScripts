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
using ECommons.GameFunctions;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Endwalker.AalBomb;

[ScriptType(guid: "2F27EFB2-CE87-4E6A-9D83-60FE4CC26141", name: "AalBomb", territorys: [1179, 1180], version: "0.0.0.3",
    author: "Cyf5119", "已修改休眠逻辑。")]
public class AalBomb
{
    [UserSetting(note: "每轮炸弹检测间隔（毫秒）")] public int Prop1 { get; set; } = 100;

    [UserSetting(note: "炸弹检测轮数上限")] public int Prop2 { get; set; } = 300;

    public int Is2nd = 0;
    public List<IBattleChara> bombs = new List<IBattleChara>();

    public void Init(ScriptAccessory accessory)
    {
        Is2nd = 0;
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

    [ScriptMethod(EventTypeEnum.StartCasting, "Aero", ["ActionId:regex:^(35145|35174)$"], false)]
    public void Aero(Event @event, ScriptAccessory accessory)
    {
        Is2nd = 0;
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "RingARingOExplosions", ["ActionId:regex:^(35164|35193)$"], false)]
    public void RingARingOExplosions(Event @event, ScriptAccessory accessory)
    {
        Is2nd++;
    }

    [ScriptMethod(name: "生成炸弹", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:regex:^(1648[19])$"])]
    public void BombSpawn(Event @event, ScriptAccessory accessory)
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

    [ScriptMethod(name: "炸弹清除", eventType: EventTypeEnum.ActionEffect,
        eventCondition: ["ActionId:regex:^(35165|35194)$"])]
    public void BombClear(Event @event, ScriptAccessory accessory)
    {
        bombs.Clear();
        accessory.Method.RemoveDraw("老三炸弹");
    }

    private unsafe bool BombState(IBattleChara bomb)
    {
        return bomb.Struct()->Timeline.AnimationState[0] > 0;
    }

    private async void BombDetect(ScriptAccessory accessory)
    {
        for (int i = 0; i < Prop2; i++)
        {
            if (bombs.Count < 6) return;
            foreach (var bomb in bombs)
            {
                if (BombState(bomb))
                {
                    // accessory.Method.SendChat($"/e 已找到 {bomb.EntityId:X}");
                    DrawBomb(accessory, bomb);
                    return;
                }
            }

            await Task.Delay(Prop1);
        }
    }

    private unsafe void DrawBomb(ScriptAccessory accessory, IBattleChara bomb0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "老三炸弹";
        dp.Color = Is2nd == 2 ? accessory.Data.DefaultDangerColor.WithW(4) : accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 21000;
        dp.Scale = new Vector2(12);
        dp.Owner = bomb0.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        var bomb1 = (IBattleChara?)accessory.Data.Objects.SearchById(bomb0.Struct()->Vfx.Tethers[0].TargetId);
        if (bomb1 == null) return;
        dp.Owner = bomb1.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        var bomb2 = (IBattleChara?)accessory.Data.Objects.SearchById(bomb1.Struct()->Vfx.Tethers[0].TargetId);
        if (bomb2 == null) return;
        dp.Owner = bomb2.EntityId;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
}