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
using ECommons.MathHelpers;
using FFXIVClientStructs;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Shadowbringers.TheEpicOfAlexander;

[ScriptType(guid: "E047803D-38D5-45B4-AF48-71C0691CDCC9", name: "亚历山大绝境战.未完工",
    territorys: [887], version: "0.0.0.5", author: "Cyf5119", note: "半成品，缓慢制作中")]
public class TheEpicOfAlexander
{
    // [UserSetting("P1小怪连线颜色")] 
    // public ScriptColor P1AddsColor { get; set; } = new() { V4 = new(0.2f, 1, 1, 1) };

    private static readonly Vector3 Center = new(100, 0, 100);



    public void Init(ScriptAccessory accessory)
    {
        P0Reset();
        P1Reset();
        P4Reset();

        accessory.Method.RemoveDraw(".*");

        
    }

    #region P0

    private uint[] _p0LimitCutList = [0, 0, 0, 0, 0, 0, 0, 0];
    private bool _p0LimitCutEnabled = false;
    private uint _p0LimitCutTimes = 0;
    
    private void P0Reset()
    {
        P0LimitCutReset();
    }

    private void P0LimitCutReset()
    {
        _p0LimitCutList = [0, 0, 0, 0, 0, 0, 0, 0];
        _p0LimitCutEnabled = false;
        _p0LimitCutTimes = 0;
    }
    
    [ScriptMethod(EventTypeEnum.TargetIcon, "麻将记录", ["Id:regex:^(00(4F|5[0123456]))$"], false)]
    public void LimitCutRecord(Event @event, ScriptAccessory accessory)
    {
        var idx = (int)@event.IconId() - 79;
        // accessory.Method.SendChat($"/e {@event.TargetName()}:{idx}");
        if (idx < 0 || idx > 7) return;
        _p0LimitCutList[idx] = @event.TargetId();
        
    }

    [ScriptMethod(EventTypeEnum.PlayActionTimeline, "麻将执行", ["Id:7747", "SourceDataId:11342"])]
    public void LimitCutPlay(Event @event, ScriptAccessory accessory)
    {
        if (!_p0LimitCutEnabled) return;
        
        var dp = accessory.FastDp("阿尔法之剑", @event.SourceId(), 1100, 25 + 5);
        dp.TargetObject = _p0LimitCutList[_p0LimitCutTimes * 2];
        dp.Radian = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        
        dp = accessory.FastDp("超级摧毁者冲击", @event.SourceId(), 2600, new Vector2(10, 50 + 5));
        dp.TargetObject = _p0LimitCutList[_p0LimitCutTimes * 2 + 1];
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);

        _p0LimitCutTimes++;
    }
    
    private bool GetLimitCut(uint id, out int index)
    {
        if (!_p0LimitCutList.Contains(id))
        {
            index = 0;
            return false;
        }

        index = _p0LimitCutList.IndexOf(id);
        return true;
    }

    #endregion

    #region P1

    private uint _p1LiquidFluidTimes = 0;
    private uint _p1HandFluidTimes = 0;
    private uint _p1CascadeTimes = 0;
    private List<Vector3> _p1RangePos = [];
    private Vector3 _p1Vector = new();
    private Dictionary<uint, uint> _p1Tether = [];

    private void P1Reset()
    {
        _p1LiquidFluidTimes = 0;
        _p1HandFluidTimes = 0;
        _p1CascadeTimes = 0;
        _p1RangePos.Clear();
        _p1Vector = new Vector3();
        _p1Tether = [];
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "流体摆动0", ["ActionId:18808"])]
    public void FluidSwing0(Event @event, ScriptAccessory accessory)
    {
        // P1BOSS平A 应该是10.2秒左右 但用触发清除就好了 摆了
        if (_p1LiquidFluidTimes > 0) return;
        _p1LiquidFluidTimes = 1;
        var dp = accessory.FastDp($"流体摆动_{_p1LiquidFluidTimes}", @event.SourceId(), 7000, 11.5f);
        dp.Delay = 5000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "流体摆动1", ["ActionId:18864", "TargetIndex:1"])]
    public void FluidSwing(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw($"流体摆动_{_p1LiquidFluidTimes}");
        _p1LiquidFluidTimes += 1;
        if (_p1LiquidFluidTimes < 2 || _p1LiquidFluidTimes > 3) return;
        // 18864->流体摆动 r=8+3.5
        var dp = accessory.FastDp($"流体摆动_{_p1LiquidFluidTimes}", @event.SourceId(), 5000, 11.5f);
        dp.Delay = _p1LiquidFluidTimes switch
        {
            2 => 26300 - 5000,
            3 => 19100 - 5000,
            // 4 => 77400,  一般不会打到这里，不管了
            _ => 99999
        };
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.AddCombatant, "流体强袭1", ["DataId:11336"])]
    public void FluidStrike1(Event @event, ScriptAccessory accessory)
    {
        _p1HandFluidTimes = 2;
        var dp = accessory.FastDp($"流体强袭_{_p1HandFluidTimes}", @event.SourceId(), 5000, 11.6f);
        dp.Delay = 17400 - 5000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "流体强袭2", ["ActionId:18871", "TargetIndex:1"])]
    public void FluidStrike2(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw($"流体强袭_{_p1HandFluidTimes}");
        _p1HandFluidTimes += 1;
        if (_p1HandFluidTimes < 3 || _p1HandFluidTimes > 3) return;
        // 18871->流体强袭 r=10+1.6
        var dp = accessory.FastDp($"流体强袭_{_p1HandFluidTimes}", @event.SourceId(), 5000, 11.6f);
        dp.Delay = _p1HandFluidTimes switch
        {
            3 => 19300 - 5000,
            // 4 => 77400,  一般不会打到这里，不管了
            _ => 99999
        };
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "倾泻计数", ["ActionId:18470"], false)]
    public void Cascade(Event @event, ScriptAccessory accessory)
    {
        _p1CascadeTimes += 1;
        _p1RangePos.Clear();
        _p1Vector = new Vector3(0);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "进1.5禁用倾泻相关", ["ActionId:18480", "TargetIndex:1"], false)]
    public void CascadeLock(Event @event, ScriptAccessory accessory) => _p1CascadeTimes = 10;

    [ScriptMethod(EventTypeEnum.AddCombatant, "生成活水之怒", ["DataId:11337"])]
    public void AddLiquidRage(Event @event, ScriptAccessory accessory)
    {
        lock (_p1RangePos)
        {
            if (_p1CascadeTimes > 5) return; // 防止在P2触发
            _p1RangePos.Add(@event.SourcePosition() - Center);
            if (_p1RangePos.Count != 3) return;

            foreach (var pos in _p1RangePos)
            {
                _p1Vector += pos;
            }

            _p1Vector = Vector3.Normalize(_p1Vector);
            var myIdx = accessory.MyIndex();

            if (_p1CascadeTimes == 1) // 仅允许第一次触发
            {
                if (new List<int>() { 2, 4, 5 }.Contains(myIdx)) return;
                var wpos = myIdx switch
                {
                    0 => _p1Vector.V3YRotate(180) * 19,
                    1 => _p1Vector.V3YRotate(-45) * 19,
                    3 => _p1Vector.V3YRotate(-117) * 17,
                    6 => _p1Vector.V3YRotate(117) * 17,
                    7 => _p1Vector.V3YRotate(-27) * 17,
                    _ => Vector3.Zero
                };
                wpos += Center;
                var dp = accessory.WaypointDp(wpos, 10000);
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
            else
            {
            }
        }
    }

    [ScriptMethod(EventTypeEnum.AddCombatant, "狩猎人偶", ["DataId:11338"])]
    public void JagdDoll(Event @event, ScriptAccessory accessory)
    {
        // 18462 castType=5 r=8+0.8
        // 6.024  16.693  27.289
        var dp = accessory.FastDp($"JagdDoll {@event.SourceId()}", @event.SourceId(), 4000, 8.8f);
        dp.Delay = 2000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        dp.Delay = 12600; // 16.693
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        dp.Delay = 23300; // 27.289
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    // jagddoll tether 0011->player 0029->boss
    [ScriptMethod(EventTypeEnum.Tether, "狩猎人偶清除", ["Id:0029"], false)]
    public void JagdDollClear(Event @event, ScriptAccessory accessory) =>
        accessory.Method.RemoveDraw($"JagdDoll {@event.SourceId()}");


    private void ProteanWaves(uint sid, uint times, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Protean Wave", sid, 2100, 40);
        dp.Radian = float.Pi / 180 * 30;
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        for (uint i = 1; i <= times; i++)
        {
            dp.TargetOrderIndex = i;
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        }
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "万变水波-活水之怒", ["ActionId:18869"])]
    public async void ProteanWaveRange(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(3000);
        ProteanWaves(@event.SourceId(), 1, accessory);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "万变水波-有生命活水", ["ActionId:18466"])]
    public async void ProteanWaveBoss(Event @event, ScriptAccessory accessory)
    {
        var sid = @event.SourceId();
        var dp = accessory.FastDp("Protean Wave", sid, 2100, 40);
        dp.Radian = float.Pi / 180 * 30;
        await Task.Delay(3000);
        ProteanWaves(sid, 4, accessory);
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        await Task.Delay(3100);
        ProteanWaves(sid, 4, accessory);
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }


    [ScriptMethod(EventTypeEnum.AddCombatant, "栓塞", ["DataId:11339"])]
    public void Embolus(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp($"栓塞-{@event.SourceId()}", @event.SourceId(), 30000, 1);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(EventTypeEnum.RemoveCombatant, "栓塞清除",["DataId:11339"])]
    public void EmbolusClear(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw($"栓塞-{@event.SourceId()}");
    }

    [ScriptMethod(EventTypeEnum.Tether, "排水", ["Id:0003"])]
    public void Drainage(Event @event, ScriptAccessory accessory)
    {
        // 18471 7.3s
        var sid = @event.SourceId();
        var tid = @event.TargetId();
        lock (_p1Tether)
        {
            if (_p1Tether.ContainsKey(sid))
            {
                accessory.Method.RemoveDraw($"Drainage {sid} {_p1Tether[sid]}");
                _p1Tether[sid] = tid;
            }
            else
                _p1Tether.Add(sid, tid);

            var dp = accessory.FastDp($"Drainage {sid} {tid}", tid, 10000, 6);
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
            // dp.CentreResolvePattern = PositionResolvePatternEnum.TetherTarget;
        }
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "排水清除", ["ActionId:18471"], false)]
    public void DrainageClear(Event @event, ScriptAccessory accessory)
    {
        foreach (var item in _p1Tether)
        {
            accessory.Method.RemoveDraw($"Drainage {item.Key} {item.Value}");
        }
    }



    private Vector3 _p1HawkBlasterVector = new();
    private readonly object _p1HawkBlasterLocker = new();
    private uint _p1HawkBlasterTimes = 0;
    
    [ScriptMethod(EventTypeEnum.ActionEffect, "P1.5地火", ["ActionId:18480", "TargetIndex:1"])]
    public void HawkBlaster(Event @event, ScriptAccessory accessory)
    {
        // 鹰式破坏炮 间隔2.2s 10m圆形
        _p0LimitCutEnabled = true;
        lock (_p1HawkBlasterLocker)
        {
            _p1HawkBlasterTimes++;
            HawkBlasterCalculate(@event, accessory, _p1HawkBlasterTimes);

            if (_p1HawkBlasterTimes == 1)
            {
                bool isLeft = (@event.EffectPosition().V3YAngle(Center) + 22.5f) % 360 > 180;
                _p1HawkBlasterVector = isLeft? @event.EffectPosition(): @event.EffectPosition().V3YRotate(Center, 180);
                HawkBlasterWaypoint(accessory);
            }
            
            if (_p1HawkBlasterTimes > 17)
            {
                _p1HawkBlasterTimes = 0;
            }
        }
    }

    private void HawkBlasterWaypoint(ScriptAccessory accessory)
    {
        var wpos = _p1HawkBlasterVector;
        if (!GetLimitCut(accessory.Data.Me, out var myIdx)) return;
        wpos = ((myIdx % 4) < 2) ? wpos : wpos.V3YRotate(Center, 180);
        
        var dp = accessory.WaypointDp(wpos, 2200);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        myIdx /= 2;
        float rot = myIdx switch
        {
            0 => 270,
            1 => 225,
            2 => 135,
            3 => 90,
            _ => 0
        };
        uint dura = myIdx switch
        {
            0 => 7500, // 7.45
            1 => 12100, // 12.083
            2 => 16700, // 16.726
            3 => 21400,  // 21.36
            _ => 999999
        };
        wpos = wpos.V3YRotate(Center, rot);

        dp = accessory.WaypointDp(wpos, 5000, dura - 5000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }
    
    private void HawkBlasterCalculate(Event @event, ScriptAccessory accessory, uint state)
    {
        var nextPos = @event.EffectPosition().V3YRotate(Center, -45);
        if (state == 7 || state == 8)
            HawkBlasterDraw(accessory, nextPos, 4400);
        if (state == 17 || state == 8)
            HawkBlasterDraw(accessory, Center, 2200);
        if (!new List<uint> { 7, 8, 9, 16, 17, 18 }.Contains(state))
            HawkBlasterDraw(accessory, nextPos, 2200);
    }

    private void HawkBlasterDraw(ScriptAccessory accessory, Vector3 pos, uint dura)
    {
        var dp = accessory.FastDp("鹰式破坏炮", pos, dura, 10);
        dp.Color = accessory.Data.DefaultDangerColor.WithW(0.1f);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        
        dp.Color = accessory.Data.DefaultDangerColor.WithW(2);
        dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    #endregion

    #region P2

    [ScriptMethod(EventTypeEnum.StartCasting, "激光战轮", ["ActionId:18517"])]
    public void EyeOfTheChakram(Event @event, ScriptAccessory accessory)
    {
        // 加上目标圈
        var dp = accessory.FastDp("Eye of the Chakram", @event.SourceId(), 6000, new Vector2(6, 70 + 3));
        accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "鹰式破坏炮", ["ActionId:18481"])]
    public void HawkBlasterP2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Hawk Blaster", @event.EffectPosition(), 5000, 10);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "回旋碎踢", ["ActionId:19058"])]
    public void SpinCrusher(Event @event, ScriptAccessory accessory)
    {
        // 加上目标圈
        var dp = accessory.FastDp("Spin Crusher", @event.SourceId(), 3000, 5 + 5);
        dp.Radian = float.Pi / 180 * 120;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StatusAdd, "水属性压缩", ["StatusID:2142"])]
    public void CompressedWater(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("water", @event.TargetId(), 5000, 8);
        dp.Delay = 24000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StatusAdd, "雷属性压缩", ["StatusID:2143"])]
    public void CompressedLightning(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("lightning", @event.TargetId(), 5000, 8);
        dp.Delay = 24000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "水雷清除", ["ActionId:18492"], false)]
    public void CompressedClear(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw("water");
        accessory.Method.RemoveDraw("lightning");
    }

    // 18501->读条大火炎放射 18502->实际伤害大火炎放射
    [ScriptMethod(EventTypeEnum.StartCasting, "大火炎放射", ["ActionId:18501"])]
    public void Flarethrower(Event @event, ScriptAccessory accessory)
    {
        // 第一个读条是3.9秒
        var dp = accessory.FastDp("Flarethrower", @event.SourceId(), 4200, 100);
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        dp.TargetOrderIndex = 1;
        dp.Radian = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "螺旋桨强风", ["ActionId:18482"])]
    public void PropellerWind(Event @event, ScriptAccessory accessory)
    {
        var ice = IbcHelper.GetFirstByDataId(0x2C81);
        if (ice == null) return;
        var dp = accessory.FastDp("Propeller Wind", @event.SourceId(), 6000, new Vector2(1, 40));
        dp.TargetObject = ice.EntityId;
        dp.Color = accessory.Data.DefaultSafeColor;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "双重火箭飞拳", ["ActionId:18503"])]
    public void DoubleRocketPunch(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Double Rocket Punch", @event.TargetId(), 4000, 3);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    // 18505->读条超级跳越 18506->实际伤害超级跳越
    [ScriptMethod(EventTypeEnum.StartCasting, "超级跳跃", ["ActionId:18505"])]
    public void SuperJump(Event @event, ScriptAccessory accessory)
    {
        // 第一个读条是3.9秒
        var dp = accessory.FastDp("Super Jump", new Vector3(0), 4200, 10);
        dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
        dp.CentreOrderIndex = 1;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    // 18507->末世宣言 对自身读条 18508->末世宣言 回头扇形多次 角度90 半径25？
    [ScriptMethod(EventTypeEnum.ActionEffect, "末世宣言", ["ActionId:18507"])]
    public void ApocalypticRay(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Apocalyptic Ray", @event.SourceId(), 5000, 25);
        dp.Radian = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    #endregion

    #region P3

    private void AlphaSword(ScriptAccessory accessory, uint duration, uint delay)
    {
        // 18539 阿尔法之剑 扇形90 半径25 间隔1.07约成1.1
        var CruiseChaser = IbcHelper.GetFirstByDataId(11342);
        if (CruiseChaser == null) return;
        var dp = accessory.FastDp("Alpha Sword", CruiseChaser.EntityId, duration, 25 + 5);
        dp.Delay = delay;
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        for (uint i = 0; i < 3; i++)
        {
            dp.TargetOrderIndex = i + 1;
            dp.DestoryAt = duration + i * 1100;
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        }
    }

    private void FlareThrower(ScriptAccessory accessory, uint duration, uint delay, uint times)
    {
        // 18540 大火炎放射 扇形90 半径100 间隔[2.141, 2.314]暂取2.3
        var BruteJustice = IbcHelper.GetFirstByDataId(11340);
        if (BruteJustice == null) return;
        var dp = accessory.FastDp("Flare Thrower", BruteJustice.EntityId, duration, 100);
        dp.Delay = delay;
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        for (uint i = 0; i < times; i++)
        {
            dp.TargetOrderIndex = i + 1;
            dp.DestoryAt = duration + i * 2300;
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        }
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "过场时停", ["ActionId:18522"])]
    public async void TemporalStasis(Event @event, ScriptAccessory accessory)
    {
        // 开始咏唱到上buff有9.1s
        await Task.Delay(6100);
        // 咏唱到第一刀是13s
        AlphaSword(accessory, 6900, 0);
        // 咏唱到第一喷是13.3s
        FlareThrower(accessory, 7200, 0, 2);
        // 这里时间停止了，也不需要特别准
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "神罚射线", ["ActionId:19072"])]
    public void ChasteningHeat(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Chastening Heat", @event.TargetId(), 5000, 5);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "圣炎", ["ActionId:19072"])]
    public void DivineSpear(Event @event, ScriptAccessory accessory)
    {
        // 19072 神罚射线 圆形死刑 半径5
        // 19074 圣炎 扇形90 半径17+7.2
        var dp = accessory.FastDp("Divine Spear", @event.SourceId(), 9400, 17 + 7.2f);
        dp.Delay = 5000;
        dp.Radian = float.Pi / 2;
        dp.TargetResolvePattern = PositionResolvePatternEnum.OwnerEnmityOrder;
        dp.TargetOrderIndex = 1;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "P3一运后半飞机劈刀", ["ActionId:18527"])]
    public void PlayAlphaSword(Event @event, ScriptAccessory accessory)
    {
        // 18527 十字圣礼
        if (@event.TargetIndex() != 1) return;
        AlphaSword(accessory, 5000, 1000);
    }


    [ScriptMethod(EventTypeEnum.StartCasting, "P3一运正义喷火", ["ActionId:18523"])]
    public void PlayFlareThrower(Event @event, ScriptAccessory accessory) => FlareThrower(accessory, 4600, 15500, 3);
    // 18527 审判结晶

    [ScriptMethod(EventTypeEnum.StartCasting, "P3二运飞机", ["ActionId:19215"])]
    public void LimitCutP3(Event @event, ScriptAccessory accessory)
    {
        // 19215 限制器减档
        P0LimitCutReset();
        _p0LimitCutEnabled = true;
    }
    

    [ScriptMethod(EventTypeEnum.StartCasting, "十字圣礼", ["ActionId:18519"])]
    public void Sacrament(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Sacrament", @event.SourceId(), 6000, new Vector2(16, 200));
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        dp.Rotation = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "净化射线", ["ActionId:19025"])]
    public void IncineratingHeat(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Incinerating Heat", @event.TargetId(), 5000, 5);
        dp.Color = accessory.Data.DefaultSafeColor;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    #endregion

    #region P4

    private uint _p4FinalWordLightPlayer;
    private uint _p4FinalWordDarkPlayer;
    private Dictionary<uint, uint> _p4ShadowDict = new();
    private List<uint> _p4ShadowPlayers = new();
    private bool[] _p4OrdainList = [false, false];
    private List<Vector3> _p4AlmightyJudgments = new();

    private void P4Reset()
    {
        _p4FinalWordLightPlayer = 0;
        _p4FinalWordDarkPlayer = 0;
        _p4ShadowDict.Clear();
        _p4ShadowPlayers.Clear();
        _p4OrdainList = [false, false];
        _p4AlmightyJudgments.Clear();
    }

    // FinalWord

    [ScriptMethod(EventTypeEnum.StatusAdd, "0.5测大光", ["StatusID:2153"])]
    public void FinalWordContactRegulation(Event @event, ScriptAccessory accessory)
    {
        var actor = _p4FinalWordLightPlayer = @event.TargetId();
        if (actor != accessory.Data.Me) return;

        var dp = accessory.WaypointDp(new Vector3(100, 0, 81), 10000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.StatusAdd, "0.5测大暗", ["StatusID:2155"])]
    public void FinalWordEscapeDetection(Event @event, ScriptAccessory accessory)
    {
        var actor = _p4FinalWordDarkPlayer = @event.TargetId();
        if (actor != accessory.Data.Me) return;

        var dp = accessory.WaypointDp(new Vector3(100, 0, 114), 10000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.StatusAdd, "0.5测小光", ["StatusID:2152"])]
    public async void FinalWordContactProhibition(Event @event, ScriptAccessory accessory)
    {
        var actor = @event.TargetId();
        if (actor != accessory.Data.Me) return;

        var dp = accessory.WaypointDp(new Vector3(100, 0, 112), 10000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        await Task.Delay(100);
        if (_p4FinalWordLightPlayer == 0) return;

        dp = accessory.FastDp("小光", actor, 9900, new Vector2(2, 20), true);
        dp.TargetObject = _p4FinalWordLightPlayer;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("大光", _p4FinalWordLightPlayer, 9900, 22);
        dp.InnerScale = new Vector2(21.8f);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);
    }

    [ScriptMethod(EventTypeEnum.StatusAdd, "0.5测小暗", ["StatusID:2154"])]
    public async void FinalWordEscapeProhibition(Event @event, ScriptAccessory accessory)
    {
        var actor = @event.TargetId();
        if (actor != accessory.Data.Me) return;

        var dp = accessory.WaypointDp(new Vector3(100, 0, 112), 10000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        await Task.Delay(100);
        if (_p4FinalWordDarkPlayer == 0) return;

        dp = accessory.FastDp("小暗", actor, 9900, new Vector2(2, 20), true);
        dp.TargetObject = _p4FinalWordDarkPlayer;
        dp.Rotation = float.Pi;
        accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("大暗", _p4FinalWordDarkPlayer, 9900, 5, true);
        dp.InnerScale = new Vector2(4.8f);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "0.5测动", ["ActionId:18558"])]
    public void OrdainedMotion(Event @event, ScriptAccessory accessory) => accessory.Method.TextInfo("动", 4000, true);

    [ScriptMethod(EventTypeEnum.StartCasting, "0.5测静", ["ActionId:18559"])]
    public void OrdainedStillness(Event @event, ScriptAccessory accessory) =>
        accessory.Method.TextInfo("静", 4000, true);

    // FateProjection

    [ScriptMethod(EventTypeEnum.Tether, "P4幻影连线记录", ["Id:0062"], false)]
    public void ShadowsRecord(Event @event, ScriptAccessory accessory)
    {
        lock (_p4ShadowDict)
        {
            _p4ShadowDict.Add(@event.SourceId(), @event.TargetId());
            if (_p4ShadowDict.Count > 7)
            {
                _p4ShadowDict = _p4ShadowDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                _p4ShadowPlayers = _p4ShadowDict.Keys.ToList();

                // DEBUG
                // string str = "";
                // foreach (var eid in _p4ShadowPlayers)
                // {
                //     var actor = accessory.Data.Objects.SearchByEntityId(eid);
                //     str += actor.Name + "  ";
                // }
                // accessory.Method.SendChat("/e "+str);
            }
        }
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P4幻影连线记录清除", ["ActionId:18578"], false)]
    public void ShadowsReset(Event @event, ScriptAccessory accessory)
    {
        _p4ShadowDict.Clear();
        _p4ShadowPlayers.Clear();
    }

    private bool GetIndex(uint id, out int index)
    {
        if (!_p4ShadowPlayers.Contains(id))
        {
            index = 0;
            return false;
        }

        index = _p4ShadowPlayers.IndexOf(id);
        return true;
    }

    // Alpha

    [ScriptMethod(EventTypeEnum.StartCasting, "1测1", ["ActionId:18556"])]
    public void FateCalibrationAlpha(Event @event, ScriptAccessory accessory)
    {
        // 未来确定α
        // 0为分摊 1为大圈 234为电 567无
        if (!GetIndex(accessory.Data.Me, out var myidx)) return;
        var wpos = myidx == 1 ? new Vector3(100, 0, 81) : new Vector3(100, 0, 119);

        var dp = accessory.WaypointDp(wpos, 10000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("一测大圈", _p4ShadowPlayers[1], 32200, 30);
        dp.InnerScale = new Vector2(29.8f);
        dp.Rotation = float.Pi * 2;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);

        var isStack = myidx < 1 || myidx > 4;
        dp = accessory.FastDp("一测分摊", _p4ShadowPlayers[0], 32200, 4, isStack);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "1测2", ["ActionId:18591", "TargetIndex:1"])]
    public void AlphaSacrament(Event @event, ScriptAccessory accessory)
    {
        // 18591->幻影十字圣礼 18569->十字圣礼 间隔18.1
        var dp = accessory.FastDp("1测十字圣礼", @event.SourceId(), 18100, new Vector2(16, 100));
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        dp.Rotation = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        // (78.29|91.01|0.00) (108.99|78.29|0.00) (121.71|91.01|0.00)
        var pos = @event.SourcePosition() - Center;
        if (pos.Z > -15) return;
        pos = Vector3.Normalize(pos.WithX(-pos.X)) * 18;
        Vector3 wpos = pos + Center;
        if (!GetIndex(accessory.Data.Me, out var myidx)) return;
        switch (myidx)
        {
            case 0 or 5 or 6 or 7:
                wpos = Vector3.Transform(pos, Matrix4x4.CreateRotationY(-float.Pi / 180 * 165)) + Center;
                break;
            case 2 or 3 or 4:
                wpos = Vector3.Transform(pos, Matrix4x4.CreateRotationY(float.Pi / 180 * 165)) + Center;
                break;
            case 1:
                break;
        }

        dp = accessory.WaypointDp(wpos, 18100);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "1测动静", ["ActionId:regex:^(1921[34]|1858[56])$", "TargetIndex:1"])]
    public void FateCalibrationAlphaOrdain(Event @event, ScriptAccessory accessory)
    {
        string str;
        switch (@event.ActionId())
        {
            case 19213:
                _p4OrdainList[0] = true;
                break;
            case 19214:
                _p4OrdainList[0] = false;
                break;
            case 18585:
                _p4OrdainList[1] = true;
                str = $"一{(_p4OrdainList[0] ? "动" : "静")}，二{(_p4OrdainList[1] ? "动" : "静")}";
                accessory.Method.TextInfo(str, 5000, true);
                accessory.Method.SendChat("/e " + str);
                break;
            case 18586:
                _p4OrdainList[1] = false;
                str = $"一{(_p4OrdainList[0] ? "动" : "静")}，二{(_p4OrdainList[1] ? "动" : "静")}";
                accessory.Method.TextInfo(str, 5000, true);
                accessory.Method.SendChat("/e " + str);
                break;
        }
    }

    // Beta

    [ScriptMethod(EventTypeEnum.StartCasting, "2测", ["ActionId:19220"])]
    public void FateCalibrationBeta(Event @event, ScriptAccessory accessory)
    {
        if (!GetIndex(accessory.Data.Me, out var myidx)) return;
        var isLight = myidx % 2 > 0;
        // 0为大暗 1为大光 2小暗 3小光 4近线小暗 5近小光 6远线小暗 7远小光
        // 怀疑分摊在3
        var dp = accessory.Data.GetDefaultDrawProperties();

        if (myidx > 1)
        {
            if (isLight)
            {
                dp = accessory.FastDp("2测小光", accessory.Data.Me, 40000, new Vector2(2, 20), true);
                dp.TargetObject = _p4ShadowPlayers[1];
                accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);

                dp = accessory.FastDp("2测大光", _p4ShadowPlayers[1], 40000, 22);
                dp.InnerScale = new Vector2(21.8f);
                dp.Radian = float.Pi * 2;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);
            }
            else
            {
                dp = accessory.FastDp("2测小暗", accessory.Data.Me, 40000, new Vector2(2, 20), true);
                dp.TargetObject = _p4ShadowPlayers[0];
                dp.Rotation = float.Pi;
                accessory.Method.SendDraw(0, DrawTypeEnum.Displacement, dp);

                dp = accessory.FastDp("2测大暗", _p4ShadowPlayers[0], 40000, 5, true);
                dp.InnerScale = new Vector2(4.8f);
                dp.Radian = float.Pi * 2;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);
            }
        }

        var wpos = myidx switch
        {
            0 => new Vector3(119, 0, 100),
            1 => new Vector3(93.84f, 0, 83.08f),
            2 => new Vector3(116, 0, 100),
            6 => new Vector3(116, 0, 101.7f),
            _ => new Vector3(116, 0, 98.3f),
        };
        dp = accessory.WaypointDp(wpos, 40000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("连带神判", _p4ShadowPlayers[3], 7200, 4, true);
        dp.Delay = 40000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        switch (myidx)
        {
            case 0:
                dp = accessory.WaypointDp(new Vector3(119, 0, 100), 8200);
                dp.Delay = 40000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                break;
            case 2:
                dp = accessory.WaypointDp(new Vector3(81, 0, 100), 8200);
                dp.Delay = 40000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                break;
            case 6:
                dp = accessory.WaypointDp(new Vector3(100, 0, 119), 8200);
                dp.Delay = 40000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                break;
        }
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "2测超级跳", ["ActionId:18589", "TargetIndex:1"])]
    public void BetaJJump(Event @event, ScriptAccessory accessory)
    {
        // 18589->幻影正义之跃 18565->正义之跃 间隔33.027
        var dp = accessory.FastDp("正义之跃", @event.SourceId(), 5000, 10);
        dp.Delay = 28000;
        dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
        dp.CentreOrderIndex = 1;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "2测分散", ["ActionId:18592", "TargetIndex:1"])]
    public void BetaSpread(Event @event, ScriptAccessory accessory)
    {
        // 18592->幻影制导动画技能分散 18861->制导动画技能 间隔33.613
        var dp = accessory.FastDp("2测分散", 0, 6100, 6);
        dp.Delay = 27500;
        foreach (var aid in accessory.Data.PartyList)
        {
            dp.Owner = aid;
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "2测分摊", ["ActionId:18593", "TargetIndex:1"])]
    public void BetaStack(Event @event, ScriptAccessory accessory)
    {
        // 18593->幻影制导动画技能分摊 18862->制导动画技能
        if (!GetIndex(accessory.Data.Me, out var myidx)) return;
        var isLight = myidx % 2 > 0;

        var dp = accessory.FastDp("2测分摊", _p4ShadowPlayers[0], 6100, 6, !isLight);
        dp.Delay = 27500;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        dp = accessory.FastDp("2测分摊", _p4ShadowPlayers[1], 6100, 6, isLight);
        dp.Delay = 27500;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        if (myidx < 2) return;
        if (isLight)
        {
            dp = accessory.WaypointDp(_p4ShadowPlayers[1], 6100);
            dp.Delay = 27500;
        }
        else
        {
            dp = accessory.WaypointDp(_p4ShadowPlayers[0], 6100);
            dp.Delay = 27500;
        }

        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "2测月环", ["ActionId:18590", "TargetIndex:1"])]
    public void BetaDonut(Event @event, ScriptAccessory accessory)
    {
        // 18590->幻影拜火圣礼 18566->拜火圣礼 月环
        var dp = accessory.FastDp("2测月环", @event.SourceId(), 5000, 60);
        dp.Delay = 28000;
        dp.InnerScale = new Vector2(8);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);

        dp = accessory.WaypointDp(@event.SourceId(), 5000);
        dp.Delay = 28000;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "加重诛罚", ["ActionId:18578"])]
    public void OrdainedCapitalPunishment(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("加重诛罚", @event.SourceId(), 8300, 4);
        dp.CentreResolvePattern = PositionResolvePatternEnum.OwnerEnmityOrder;
        dp.CentreOrderIndex = 1;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "诛罚", ["ActionId:18577"])]
    public void OrdainedPunishment(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("加重诛罚", @event.TargetId(), 5000, 5);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    // AlmightyJudgment

    [ScriptMethod(EventTypeEnum.StartCasting, "株连", ["ActionId:18580"])]
    public void IrresistibleGrace(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Irresistible Grace", @event.TargetId(), 5000, 6, true);
        dp.InnerScale = new Vector2(5.8f);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Donut, dp);
    }

    private static bool AlmostEqual(Vector3 v1, Vector3 v2)
    {
        if (Vector3.Distance(v1, v2) < 1)
            return true;
        return false;
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "地火清除", ["ActionId:18574"], false)]
    public void AlmightyJudgmentClear(Event @event, ScriptAccessory accessory)
    {
        _p4AlmightyJudgments.Clear();
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "地火", ["ActionId:18575"])]
    public void AlmightyJudgment(Event @event, ScriptAccessory accessory)
    {
        // 从演示咏唱到实际判定约8s 两次地火间隔是2s
        // 实际判定为 18576
        var dp = accessory.FastDp("Almighty Judgment", @event.EffectPosition(), 2000, 6);
        dp.Delay = 6000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        var pos = @event.EffectPosition();
        foreach (var fire in fires)
        {
            if (AlmostEqual(pos, fire))
                _p4AlmightyJudgments.Add(fire);
            if (_p4AlmightyJudgments.Count == 2)
            {
                AlmightyJudgmentGuide(accessory);
            }
        }
    }

    private static readonly List<Vector3> fires = [new(92, 0, 108), new(100, 0, 108), new(108, 0, 108)];
    private static readonly Vector3 anotherfire = new(108, 0, 100);

    private void AlmightyJudgmentGuide(ScriptAccessory accessory)
    {
        Vector3 wpos1, wpos2;
        if (AlmostEqual(_p4AlmightyJudgments[1], fires[1]))
        {
            if (AlmostEqual(_p4AlmightyJudgments[0], fires[0]))
            {
                wpos1 = fires[2];
                wpos2 = anotherfire;
            }
            else
            {
                wpos1 = anotherfire;
                wpos2 = fires[2];
            }
        }
        else if (AlmostEqual(_p4AlmightyJudgments[0], fires[1]))
        {
            wpos1 = AlmostEqual(_p4AlmightyJudgments[1], fires[0]) ? fires[2] : fires[0];
            wpos2 = fires[1];
        }
        else
        {
            wpos1 = fires[1];
            wpos2 = _p4AlmightyJudgments[0];
        }

        var dp = accessory.WaypointDp(wpos1, 6000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        dp = accessory.WaypointDp(wpos2, 4000, 6000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("地火预指路", wpos1, 6000, 1);
        dp.TargetPosition = wpos2;
        dp.ScaleMode = ScaleMode.YByDistance;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    #endregion
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

    public static uint Id(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Id"]);
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

    public static uint IconId(this Event @event)
    {
        return ParseHexId(@event["Id"], out var id) ? id : 0;
    }

    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }

    public static Vector3 TargetPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
    }

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }

    public static float SourceRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
    }

    public static float TargetRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
    }

    public static string SourceName(this Event @event)
    {
        return @event["SourceName"];
    }

    public static string TargetName(this Event @event)
    {
        return @event["TargetName"];
    }

    public static uint DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["DurationMilliseconds"]);
    }

    public static uint Index(this Event @event)
    {
        return ParseHexId(@event["Index"], out var id) ? id : 0;
    }

    public static uint State(this Event @event)
    {
        return ParseHexId(@event["State"], out var id) ? id : 0;
    }

    public static uint DirectorId(this Event @event)
    {
        return ParseHexId(@event["DirectorId"], out var id) ? id : 0;
    }

    public static uint StatusId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StatusID"]);
    }

    public static uint StackCount(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StackCount"]);
    }

    public static uint Param(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Param"]);
    }

    public static uint TargetIndex(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["TargetIndex"]);
    }

    public static uint SourceDataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]);
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

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, uint target, uint duration,
        uint delay = 0, string name = "Waypoint")
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = accessory.Data.Me;
        dp.TargetObject = target;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, Vector3 pos, uint duration,
        uint delay = 0, string name = "Waypoint")
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = accessory.Data.Me;
        dp.TargetPosition = pos;
        dp.DestoryAt = duration;
        dp.Delay = delay;
        dp.Scale = new Vector2(2);
        dp.ScaleMode = ScaleMode.YByDistance;
        return dp;
    }

    public static int MyIndex(this ScriptAccessory accessory)
    {
        return accessory.Data.PartyList.IndexOf(accessory.Data.Me);
    }
}

public static class IbcHelper
{
    public static IBattleChara? GetById(uint id)
    {
        return (IBattleChara?)Svc.Objects.SearchByEntityId(id);
    }

    public static IBattleChara? GetMe()
    {
        return Svc.ClientState.LocalPlayer;
    }

    public static IGameObject? GetFirstByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId).FirstOrDefault();
    }

    public static IEnumerable<IGameObject?> GetByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId);
    }
}

public static class MathHelper
{
    public static float V3YAngle(this Vector3 v)
    {
        return V3YAngle(v, Vector3.Zero);
    }
    
    public static float V3YAngle(this Vector3 v, Vector3 origin, bool toRadian = false)
    {
        var angle = (MathF.Atan2(v.Z - origin.Z, v.X - origin.X).RadToDeg() + 360) % 360;
        return toRadian ? angle.DegToRad() : angle;
    }

    public static Vector3 V3YRotate(this Vector3 v, float angle, bool isRadian = false)
    {
        return V3YRotate(v, Vector3.Zero, angle, isRadian);
    }

    public static Vector3 V3YRotate(this Vector3 v, Vector3 origin, float angle, bool isRadian = false)
    {
        var radian = isRadian ? angle : angle.DegToRad();
        return Vector3.Transform(v - origin, Matrix4x4.CreateRotationY(radian)) + origin;
    }
}

#endregion