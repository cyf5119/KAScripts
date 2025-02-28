#define DEBUG

using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.MathHelpers;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;
using KodakkuAssist.Module.GameOperate;

namespace Cyf5119Script.Shadowbringers.TheEpicOfAlexander;

[ScriptType(guid: "E047803D-38D5-45B4-AF48-71C0691CDCC9", name: "亚历山大绝境战.未完工",
    territorys: [887], version: "0.0.2.1", author: "Cyf5119", note: Note)]
public class TheEpicOfAlexander
{
    private const string Note = "有问题来DC反馈。\n我也忘了我更新了什么了。\n/e KASCLEAR 清理残余画图";

    #region 用户设置

    [UserSetting("P2仅显示自己的传毒提示")] public static bool P2RotsSelfOnly { get; set; } = false;
    
    [UserSetting("P2蓝毒颜色")] public static ScriptColor P2Blue { get; set; } = new() { V4 = new Vector4(102 / 255f, 136 / 255f, 187 / 255f, 1) };
    [UserSetting("P2橙毒颜色")] public static ScriptColor P2Orange { get; set; } = new() { V4 = new Vector4(204 / 255f, 136 / 255f, 102 / 255f, 1) };
    [UserSetting("P2紫毒颜色")] public static ScriptColor P2Purple { get; set; } = new() { V4 = new Vector4(85 / 255f, 34 / 255f, 153 / 255f, 1) };
    [UserSetting("P2绿毒颜色")] public static ScriptColor P2Green { get; set; } = new() { V4 = new Vector4(51 / 255f, 85 / 255f, 17 / 255f, 1) };

    #endregion
    
    private static readonly Vector3 Center = new(100, 0, 100);

    public void Init(ScriptAccessory accessory)
    {
        P0Reset();
        P1Reset();
        P2Reset();
        P3Reset();
        P4Reset();
        accessory.Method.RemoveDraw(".*");
    }

    #region P0

    private uint _phase = 0;
    
    private uint[] _p0LimitCutList = [0, 0, 0, 0, 0, 0, 0, 0];
    private bool _p0LimitCutEnabled = false;
    private uint _p0LimitCutTimes = 0;

    private void P0Reset()
    {
        _phase = 0;
        P0LimitCutReset();
    }

    [ScriptMethod(EventTypeEnum.Chat, "clear draw", ["Type:Echo", "Message:KASCLEAR"], false)]
    public void clear_draw(Event @event, ScriptAccessory sa) => sa.Method.RemoveDraw(".*");
    
    # region 阶段控制
    
    [ScriptMethod(name: "阶段控制P1-流体摆动", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:18864", "TargetIndex:1"], userControl: false)]
    public void PhaseControl1(Event @event, ScriptAccessory accessory) => _phase = _phase == 0 ? 100 : _phase;
    
    [ScriptMethod(name: "阶段控制P1-倾泻", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18470"], userControl: false)]
    public void PhaseControl1_1(Event @event, ScriptAccessory accessory) => _phase += 1;
    
    [ScriptMethod(name: "阶段控制P1.5-麻将点名", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:regex:^(00(4F|5[0123456]))$"], userControl: false)]
    public void PhaseControl1_5(Event @event, ScriptAccessory accessory) => _phase = _phase < 200 ? 150 : _phase;
    
    [ScriptMethod(name: "阶段控制P2-正义飞踢", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:18516", "TargetIndex:1"], userControl: false)]
    public void PhaseControl2(Event @event, ScriptAccessory accessory) => _phase = 200;
    
    [ScriptMethod(name: "阶段控制P2-制导", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18479"], userControl: false)]
    public void PhaseControl2_1(Event @event, ScriptAccessory accessory) => _phase = 210;
    
    [ScriptMethod(name: "阶段控制P2-大地导弹", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18510"], userControl: false)]
    public void PhaseControl2_2(Event @event, ScriptAccessory accessory) => _phase = 220;
    
    [ScriptMethod(name: "阶段控制P2-大火炎放射", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18501"], userControl: false)]
    public void PhaseControl2_3(Event @event, ScriptAccessory accessory) => _phase = 230;
    
    [ScriptMethod(name: "阶段控制P2-终审闭庭", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:18864", "TargetIndex:1"], userControl: false)]
    public void PhaseControl2_4(Event @event, ScriptAccessory accessory) => _phase = 340;
    
    [ScriptMethod(name: "阶段控制P3-时间停止", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18470"], userControl: false)]
    public void PhaseControl3(Event @event, ScriptAccessory accessory) => _phase = 300;
    
    [ScriptMethod(name: "阶段控制P3-时空潜行阵列", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18543"], userControl: false)]
    public void PhaseControl3_1(Event @event, ScriptAccessory accessory) => _phase = 310;
    
    [ScriptMethod(name: "阶段控制P3-次元断绝阵列", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18542"], userControl: false)]
    public void PhaseControl3_2(Event @event, ScriptAccessory accessory) => _phase = 320;
    
    [ScriptMethod(name: "阶段控制P3-召唤亚历山大", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:19029"], userControl: false)]
    public void PhaseControl3_3(Event @event, ScriptAccessory accessory) => _phase = 330;
    
    [ScriptMethod(name: "阶段控制P4-终审判决", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18557"], userControl: false)]
    public void PhaseControl4(Event @event, ScriptAccessory accessory) => _phase = 400;
    
    [ScriptMethod(name: "阶段控制P4-未来观测α", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18555"], userControl: false)]
    public void PhaseControl4_1(Event @event, ScriptAccessory accessory) => _phase = 410;
    
    [ScriptMethod(name: "阶段控制P4-未来观测β", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:19219"], userControl: false)]
    public void PhaseControl4_2(Event @event, ScriptAccessory accessory) => _phase = 420;
    
    [ScriptMethod(name: "阶段控制P4-神圣大审判", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18574"], userControl: false)]
    public void PhaseControl4_3(Event @event, ScriptAccessory accessory) => _phase = 430;
    
    [ScriptMethod(name: "阶段控制P4-时空干涉", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18582"], userControl: false)]
    public void PhaseControl4_4(Event @event, ScriptAccessory accessory) => _phase = 440;
    
    #endregion

    #region 麻将控制
    
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

    #region 超级跳与回头扫

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
    
    #endregion

    #region P1

    private uint _p1LiquidFluidTimes = 0;
    private uint _p1HandFluidTimes = 0;
    private uint _p1CascadeTimes = 0;
    private List<Vector3> _p1RangePos = [];
    private Vector3 _p1Vector = new();
    private Dictionary<uint, uint> _p1Tether = [];
    private Vector3 _p1HawkBlasterVector = new();
    private readonly object _p1HawkBlasterLocker = new();
    private uint _p1HawkBlasterTimes = 0;

    private void P1Reset()
    {
        _p1LiquidFluidTimes = 0;
        _p1HandFluidTimes = 0;
        _p1CascadeTimes = 0;
        _p1RangePos.Clear();
        _p1Vector = new Vector3();
        _p1Tether = [];
        _p1HawkBlasterVector = new Vector3();
        _p1HawkBlasterTimes = 0;
    }

    #region 流体摆动与强袭

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

    #endregion
    
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
                _p1Vector += pos;
            _p1Vector = Vector3.Normalize(_p1Vector);

            var myIdx = accessory.MyIndex();
            if (_p1CascadeTimes != 1) return;
            if (new List<int>() { 2, 4, 5 }.Contains(myIdx)) return;
            var wpos = myIdx switch
            {
                0 => new Vector3(-00.0f, 0, -19.0f),
                1 => new Vector3(-13.5f, 0, +13.5f),
                3 => new Vector3(-17.0f, 0, -07.5f),
                6 => new Vector3(+17.0f, 0, -07.5f),
                7 => new Vector3(-07.5f, 0, +17.0f),
                _ => Vector3.Zero
            };
            wpos = wpos.V3YRotate(_p1Vector.V3YAngle()) + Center;
            var dp = accessory.WaypointDp(wpos, 10000);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
    }

    #region 狩猎人偶

    [ScriptMethod(EventTypeEnum.AddCombatant, "狩猎人偶", ["DataId:11338"])]
    public void JagdDoll(Event @event, ScriptAccessory accessory)
    {
        var dollIdx = (int)((@event.SourcePosition().V3YAngle(Center) - _p1Vector.V3YAngle() + 360) % 360 / 90);
        var myIdx = accessory.MyIndex();
        var myDollIdx = myIdx switch
        {
            4 => 2,
            5 => 1,
            6 => 0,
            7 => 3,
            _ => 9
        };

        var isMyDoll = false;
        var dp = accessory.Data.GetDefaultDrawProperties();
        if (dollIdx == myDollIdx)
        {
            isMyDoll = true;
            dp = accessory.FastDp("狩猎人偶连线", accessory.Data.Me, 6000, 5, true);
            dp.TargetObject = @event.SourceId();
            dp.ScaleMode = ScaleMode.YByDistance;
            dp.Color = dp.Color.WithW(5);
            accessory.Method.SendDraw(0, DrawTypeEnum.Line, dp);
        }

        // 18462 castType=5 r=8+0.8
        // 6.024  16.693  27.289
        dp = accessory.FastDp($"狩猎人偶-{@event.SourceId()}", @event.SourceId(), 4000, 8.8f, isMyDoll);
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
        accessory.Method.RemoveDraw($"狩猎人偶-{@event.SourceId()}");

    #endregion

    #region 万变水波
    
    private void ProteanWaves(ScriptAccessory accessory, uint sid, uint delay, uint times)
    {
        var dp = accessory.FastDp("万变水波", sid, 2100, 40);
        dp.Delay = delay;
        dp.Radian = float.Pi / 180 * 30;
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        for (uint i = 1; i <= times; i++)
        {
            dp.TargetOrderIndex = i;
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        }
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "万变水波-活水之怒", ["ActionId:18869"])]
    public void ProteanWaveRange(Event @event, ScriptAccessory accessory)
    {
        ProteanWaves(accessory, @event.SourceId(), 3000, 1);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "万变水波-有生命活水", ["ActionId:18466"])]
    public void ProteanWaveBoss(Event @event, ScriptAccessory accessory)
    {
        var sid = @event.SourceId();
        var dp = accessory.FastDp("Protean Wave", sid, 2100, 40);
        dp.Radian = float.Pi / 180 * 30;
        dp.Delay = 3000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        ProteanWaves(accessory, sid, 3000, 4);
        dp.Delay = 6100;
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        ProteanWaves(accessory, sid, 6100, 4);

        var myIdx = accessory.MyIndex();
        if (_p1CascadeTimes == 1)
        {
            var wpos1 = myIdx switch
            {
                0 => new Vector3(+02.0f, 0, -02.0f),
                1 => new Vector3(+00.0f, 0, +02.8f),
                2 => new Vector3(-08.0f, 0, +12.0f),
                3 => new Vector3(-12.0f, 0, +08.0f),
                4 => new Vector3(-02.8f, 0, +00.0f),
                5 => new Vector3(+02.8f, 0, +00.0f),
                6 => new Vector3(+12.0f, 0, +08.0f),
                7 => new Vector3(+08.0f, 0, +12.0f),
                _ => Center
            };
            var wpos2 = myIdx switch
            {
                0 => new Vector3(+04.0f, 0, -04.0f),
                1 => new Vector3(+00.0f, 0, +05.6f),
                2 => new Vector3(-02.0f, 0, +02.0f),
                3 => new Vector3(-02.8f, 0, +00.0f),
                4 => new Vector3(-04.0f, 0, -04.0f),
                5 => new Vector3(+04.0f, 0, -04.0f),
                6 => new Vector3(+02.8f, 0, +00.0f),
                7 => new Vector3(+02.0f, 0, +02.0f),
                _ => Center
            };
            wpos1 = wpos1.V3YRotate(_p1Vector.V3YAngle()) + Center;
            wpos2 = wpos2.V3YRotate(_p1Vector.V3YAngle()) + Center;

            dp = accessory.WaypointDp(wpos1, 5100);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            dp = accessory.WaypointDp(wpos2, 3100, 5100);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
        else if (_p1CascadeTimes == 2)
        {
            var wpos1 = myIdx switch
            {
                0 => new Vector3(+02.0f, 0, -02.0f),
                1 => new Vector3(+00.0f, 0, +02.8f),
                2 => new Vector3(-08.0f, 0, -12.0f),
                3 => new Vector3(-12.0f, 0, -08.0f),
                4 => new Vector3(-02.8f, 0, +00.0f),
                5 => new Vector3(+02.8f, 0, +00.0f),
                6 => new Vector3(+12.0f, 0, +08.0f),
                7 => new Vector3(+08.0f, 0, +12.0f),
                _ => Center
            };
            var wpos2 = myIdx switch
            {
                0 => new Vector3(+04.0f, 0, -04.0f),
                1 => new Vector3(-09.0f, 0, +15.0f),
                2 => new Vector3(-02.8f, 0, +00.0f),
                3 => new Vector3(-02.0f, 0, -02.0f),
                4 => new Vector3(-09.0f, 0, +06.0f),
                5 => new Vector3(+12.0f, 0, -08.0f),
                6 => new Vector3(+02.8f, 0, +00.0f),
                7 => new Vector3(+02.0f, 0, +02.0f),
                _ => Center
            };
            wpos1 = wpos1.V3YRotate(_p1Vector.V3YAngle()) + Center;
            wpos2 = wpos2.V3YRotate(_p1Vector.V3YAngle()) + Center;

            dp = accessory.WaypointDp(wpos1, 5100);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            dp = accessory.WaypointDp(wpos2, 6100, 5100);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }
    }
    
    #endregion

    [ScriptMethod(EventTypeEnum.AddCombatant, "栓塞", ["DataId:11339"])]
    public void Embolus(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp($"栓塞-{@event.SourceId()}", @event.SourceId(), 30000, 1);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.RemoveCombatant, "栓塞清除", ["DataId:11339"], false)]
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

    #region 鹰式破坏炮
    
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
                var isLeft = (@event.EffectPosition().V3YAngle(Center) + 22.5f) % 360 > 180;

                _p1HawkBlasterVector =
                    isLeft ? @event.EffectPosition() : @event.EffectPosition().V3YRotate(Center, 180);

                HawkBlasterWaypoint(accessory);
            }

            if (_p1HawkBlasterTimes > 17)
                _p1HawkBlasterTimes = 0;
        }
    }

    private void HawkBlasterWaypoint(ScriptAccessory accessory)
    {
        var wpos = _p1HawkBlasterVector;
        if (!GetLimitCut(accessory.Data.Me, out var myIdx)) return;
        wpos = ((myIdx % 4) < 2) ? wpos : wpos.V3YRotate(Center, 180);

        var dp = accessory.WaypointDp(wpos, 2200);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        // myIdx /= 2;
        float rot = myIdx switch
        {
            0 => 270,
            1 => 270 + 22.5f,
            2 => 225,
            3 => 225 + 22.5f,
            4 => 135,
            5 => 135 + 22.5f,
            6 => 45,
            7 => 45 + 22.5f,
            _ => 0
        };
        uint dura = (int)(myIdx / 2) switch
        {
            0 => 7500,  // 7.45
            1 => 12100, // 12.083
            2 => 16700, // 16.726
            3 => 21400, // 21.36
            _ => 999999
        };
        wpos = wpos.V3YRotate(Center, rot);

        dp = accessory.WaypointDp(wpos, 5000, dura - 5000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    private void HawkBlasterCalculate(Event @event, ScriptAccessory accessory, uint state)
    {
        var nextPos = @event.EffectPosition().V3YRotate(Center, -45);
        if (state is 7 or 8)
            HawkBlasterDraw(accessory, nextPos, 4400);
        if (state is 17 or 8)
            HawkBlasterDraw(accessory, Center, 2200);
        if (!new List<uint> { 7, 8, 9, 16, 17, 18 }.Contains(state))
            HawkBlasterDraw(accessory, nextPos, 2200);
    }

    private void HawkBlasterDraw(ScriptAccessory accessory, Vector3 pos, uint dura)
    {
        var dp = accessory.FastDp("鹰式破坏炮", pos, dura, 10);
        dp.Color = accessory.Data.DefaultDangerColor.WithW(0.05f);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        dp.Color = accessory.Data.DefaultDangerColor.WithW(2);
        dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    
    #endregion
    
    #endregion

    #region P2
    

    private void P2Reset()
    {
    }

    #region 传毒相关

    // 蓝α 橙β 紫γ 绿δ
    private static uint[] _decreeNisis = [2222, 2223, 2137, 2138];
    private static uint[] _judgmentNisis = [2224, 2225, 2139, 2140];
    private static ScriptColor[] _nisiColors = [P2Blue, P2Orange, P2Purple, P2Green];
    
    [ScriptMethod(name: "上毒", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(222[23]|213[78])$"])]
    public void FinalDecreeNisi(Event evt, ScriptAccessory sa)
    {
        sa.Method.RemoveDraw($"P2-毒-{evt.StatusId()}");
    }

    private void P2PassRot(ScriptAccessory sa, uint state)
    {
        foreach (var player in sa.GetParty())
        {
            var nisi = _decreeNisis.FirstOrDefault(x => player.HasStatus(x));
            if (nisi is 0)
                continue;
            
            var sid = player.EntityId;
            var idx = sa.Data.PartyList.IndexOf(sid);
            var tid = sa.Data.PartyList[idx > 3 ? idx - 4 : idx + 4];
            
            var dp = sa.FastDp($"P2-毒-{nisi}", sid, 10000, 5);
            dp.Color = _nisiColors[_decreeNisis.IndexOf(nisi)].V4.WithW(5);
            dp.ScaleMode = ScaleMode.YByDistance;
            
            if (state == 1)
            {
                if (idx % 4 == 1)
                    dp.Delay = 10000;
            }
            else if (state == 2)
            {
                if (idx % 4 > 1)
                {
                    List<int> lst = idx < 4 ? [6, 7] : [2, 3];
                    var tplayer = sa.GetParty()
                        .Where(x => lst.Contains(sa.Data.PartyList.IndexOf(x.EntityId)))
                        .OrderBy(x => Vector3.Distance(player.Position, x.Position)).FirstOrDefault();
                    if (tplayer is null)
                        continue;
                    tid = tplayer.EntityId;
                }
            }
            else if (state == 3)
            {
                List<int> lst = idx < 4 ? [4, 5, 6, 7] : [0, 1, 2, 3];
                var tplayer = sa.GetParty()
                    .FirstOrDefault(x => lst.Contains(sa.Data.PartyList.IndexOf(x.EntityId)) 
                                         && x.HasStatus(_judgmentNisis[_decreeNisis.IndexOf(nisi)]));
                if (tplayer is null)
                    continue;
                tid = tplayer.EntityId;
                dp.Delay = 5000;
            }
            
            if (P2RotsSelfOnly && sa.Data.Me != sid && sa.Data.Me != tid)
                continue;
            dp.TargetObject = tid;

            sa.Method.SendDraw(0, DrawTypeEnum.Line, dp);
        }
    }

    [ScriptMethod(name: "一传提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18486"])]
    public void P2PassRot1(Event evt, ScriptAccessory sa) => P2PassRot(sa, 1);

    [ScriptMethod(name: "二传提示", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:18511", "TargetIndex:1"])]
    public void P2PassRot2(Event evt, ScriptAccessory sa) => P2PassRot(sa, 2);

    [ScriptMethod(name: "三传提示", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:18502", "TargetIndex:1"])]
    public void P2PassRot3(Event evt, ScriptAccessory sa) => P2PassRot(sa, 3);
    
    #endregion
    
    

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
        dp.Color = accessory.Data.DefaultDangerColor.WithW(0.05f);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        dp.Color = accessory.Data.DefaultDangerColor.WithW(2f);
        dp.ScaleMode = ScaleMode.ByTime;
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

    [ScriptMethod(EventTypeEnum.TargetIcon, "冰圈1", ["Id:0043"])]
    public void IceMissile1(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("冰圈1", @event.TargetId(), 5100, 9);
        dp.InnerScale = new Vector2(6);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
    }

    [ScriptMethod(EventTypeEnum.ObjectChanged, "冰圈2", ["DataId:2004365", "Operate:Add"])]
    public void IceMissile2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("冰圈2", @event.SourceId(), 4000, 9);
        dp.InnerScale = new Vector2(6);
        dp.Radian = float.Pi * 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Donut, dp);
    }

    /*
20:49:00.617 TargetIcon 0x0043
20:49:05.726|484F|寒冰导弹|
20:49:05.986|Add|400065BF|BNpcID|1E958D|ModelStatus|2304|
20:49:06.478|Change|400065BF|ModelStatus|0|
20:49:10.011|Add|400065C1|BNpcID|1E958E|ModelStatus|2304|
20:49:10.011|Change|400065C1|ModelStatus|0|
20:49:12.960|Change|400065BF|ModelStatus|256|
20:49:13.466|Change|400065BF|ModelStatus|2304|
20:49:15.970|Change|400065C1|ModelStatus|256|
20:49:16.081|Remove|400065BF|
20:49:16.381|Change|400065C1|ModelStatus|2304|
20:49:19.400|Remove|400065C1|
     */
    // TODO 冰圈大小6-9

    [ScriptMethod(EventTypeEnum.AddCombatant, "等离子护盾", ["DataId:11343"])]
    public void PlasmaShield(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("等离子护盾", @event.SourceId(), 30000, 5, true);
        accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(EventTypeEnum.RemoveCombatant, "等离子护盾清除", ["DataId:11343"], false)]
    public void PlasmaShieldClear(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw("等离子护盾");
    }

    // 18501->读条大火炎放射 18502->实际伤害大火炎放射
    [ScriptMethod(EventTypeEnum.StartCasting, "大火炎放射", ["ActionId:18501"])]
    public void FlareThrower(Event @event, ScriptAccessory accessory)
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

    #endregion

    #region P3

    private void P3Reset()
    {
    }

    private void AlphaSword(ScriptAccessory accessory, uint duration, uint delay)
    {
        // 18539 阿尔法之剑 扇形90 半径25 间隔1.07约成1.1
        var cruiseChaser = IbcHelper.GetFirstByDataId(11342);
        if (cruiseChaser == null) return;
        var dp = accessory.FastDp("Alpha Sword", cruiseChaser.EntityId, duration, 25 + 5);
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
        var bruteJustice = IbcHelper.GetFirstByDataId(11340);
        if (bruteJustice == null) return;
        var dp = accessory.FastDp("Flare Thrower", bruteJustice.EntityId, duration, 100);
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
    public void TemporalStasis(Event @event, ScriptAccessory accessory)
    {
        // 开始咏唱到上buff有9.1s
        // 咏唱到第一刀是13s
        AlphaSword(accessory, 6900, 6100);
        // 咏唱到第一喷是13.3s
        FlareThrower(accessory, 7200, 6100, 2);
        // 这里时间停止了，也不需要特别准

        var myself = IbcHelper.GetByEntityId(accessory.Data.Me);
        if (myself == null) return;
        var myIdx = accessory.MyIndex();
        var isTN = myIdx < 4;
        var cruiseChaser = IbcHelper.GetFirstByDataId(11342);
        if (cruiseChaser == null) return;
        var ccVector = Vector3.Normalize(cruiseChaser.Position - Center);

        Vector3 wpos;
        if (myself.HasStatus(1121)) // 判决确定：加重罪 电
            wpos = ccVector.V3YRotate(180) * 18;
        else if (myself.HasStatus(1123)) // 判决确定：强制接近命令 近
            wpos = new Vector3(+6, 0, isTN ? -1.5f : +1.5f);
        else if (myself.HasStatus(1124)) // 判决确定：禁止接近命令 远
            wpos = new Vector3(isTN ? (ccVector.X < 0 ? -16 : -18) : (ccVector.X < 0 ? 18 : 16), 0, 0);
        else // 闲人
            wpos = new Vector3(-6, 0, isTN ? -1.5f : +1.5f);

        wpos += Center;
        var dp = accessory.WaypointDp(wpos, 9100);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
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

    [ScriptMethod(EventTypeEnum.StartCasting, "P3一运正义喷火", ["ActionId:18523"])]
    public void PlayFlareThrower(Event @event, ScriptAccessory accessory) => FlareThrower(accessory, 4600, 15500, 3);
    // 18527 审判结晶

    [ScriptMethod(EventTypeEnum.ActionEffect, "P3一运后半飞机劈刀", ["ActionId:18527"])]
    public void PlayAlphaSword(Event @event, ScriptAccessory accessory)
    {
        // 18527 十字圣礼
        if (@event.TargetIndex() != 1) return;
        AlphaSword(accessory, 5000, 1000);
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "P3一运后半", ["ActionId:18526", "TargetIndex:1"])]
    public void Inception(Event @event, ScriptAccessory accessory)
    {
        // TODO 11422 为一运后半真心位移目标，亚历山大出现放十字圣礼之处
        var alex = IbcHelper.GetFirstByDataId(11422);
        if (alex == null) return;
        var dp = accessory.FastDp("一运后半十字", alex.EntityId, 8300, new Vector2(16, 100));
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);
        dp.Rotation = float.Pi / 2;
        accessory.Method.SendDraw(0, DrawTypeEnum.Straight, dp);

        bool isLeft;
        var myIdx = accessory.MyIndex();
        if (myIdx < 2)
            isLeft = true;
        else if (myIdx < 4)
            isLeft = false;
        else
            isLeft = ((IBattleChara)accessory.Data.Objects.SearchByEntityId(accessory.Data.Me)).HasStatus(1122);
        var wpos = new Vector3(isLeft ? -19 : +19, 0, 0).V3YRotate((Center - alex.Position).V3YAngle()) + Center;
        dp = accessory.WaypointDp(wpos, 8000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        var stack = accessory.GetParty().FirstOrDefault(x => x.StatusList.Any(s => s.StatusId == 1122));
        if (stack != null)
        {
            dp = accessory.FastDp("集团罪", stack.EntityId, 8000, 4, isLeft);
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        }

        if (myIdx == 1)
            wpos = new Vector3(119, 0, 100);
        else if (myIdx == 2)
            wpos = new Vector3(100, 0, 98.5f);
        else if (myIdx == 3)
            wpos = new Vector3(100, 0, 101.5f);
        else if (myIdx > 3 && ((IBattleChara)accessory.Data.Objects.SearchByEntityId(accessory.Data.Me)).HasStatus(1124))
            wpos = new Vector3(98.5f, 0, 100);
        else
            wpos = new Vector3(106, 0, 100);
        dp = accessory.WaypointDp(wpos, 6400, 8000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(EventTypeEnum.StartCasting, "P3二运飞机", ["ActionId:19215"])]
    public void LimitCutP3(Event @event, ScriptAccessory accessory)
    {
        // 19215 限制器减档
        P0LimitCutReset();
        _p0LimitCutEnabled = true;
    }

    [ScriptMethod(EventTypeEnum.TargetIcon, "P3麻将指路1", ["Id:regex:^(00(4F|5[0123456]))$"])]
    public void LimitCutP3Guide1(Event @event, ScriptAccessory accessory)
    {
        if (!Svc.Objects.Any(x => x is IBattleChara y && y.CastActionId == 18534)) return;
        if (@event.TargetId() != accessory.Data.Me) return;
        var myIdx = (int)@event.IconId() - 79;
        if (myIdx < 0 || myIdx > 7) return;
        var bruteJustice = IbcHelper.GetFirstByDataId(11340);
        if (bruteJustice == null) return;
        // 以A为北 方便计算
        var bjRight = bruteJustice.Position.X - 100 > 0;

        var dp = myIdx switch
        {
            0 => accessory.WaypointDp(new Vector3(bjRight ? +13 : -13, 0, -13) + Center, 9500),
            1 => accessory.WaypointDp(new Vector3(bjRight ? -13 : +13, 0, -13) + Center, 9500),
            2 => accessory.WaypointDp(new Vector3(bjRight ? +13 : -13, 0, +13) + Center, 9500),
            3 => accessory.WaypointDp(new Vector3(bjRight ? -13 : +13, 0, +13) + Center, 9500),
            4 => accessory.WaypointDp(new Vector3(bjRight ? +19 : -19, 0, +00) + Center, 3300),
            5 => accessory.WaypointDp(new Vector3(bjRight ? -19 : +19, 0, +00) + Center, 3300),
            6 => accessory.WaypointDp(new Vector3(bjRight ? +19 : -19, 0, +00) + Center, 9500),
            7 => accessory.WaypointDp(new Vector3(bjRight ? -19 : +19, 0, +00) + Center, 9500),
            // _ => accessory.WaypointDp(Center, 9500),
        };
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        // 9.5 到飞机第一刀
        // 忏悔区大小依次为 8 6 3？
    }

    [ScriptMethod(EventTypeEnum.ObjectChanged, "P3麻将指路2", ["DataId:2007519", "Operate:Add"])]
    public void LimitCutP3Guide2(Event @event, ScriptAccessory accessory)
    {
        // 6.226 到飞机第一刀
        if (!GetLimitCut(accessory.Data.Me, out var myIdx)) return;
        var objRight = @event.SourcePosition().X - 100 > 0;
        var bruteJustice = IbcHelper.GetFirstByDataId(11340);
        if (bruteJustice == null) return;
        // 以A为北 方便计算
        var bjRight = bruteJustice.Position.X - 100 > 0;

        if ((myIdx % 2 == 0) != (bjRight == objRight)) return;
        // 玩家是否同侧 与 正义灵泉是否同侧  不同则停止
        DrawPropertiesEdit dp1, dp2;
        switch (myIdx)
        {
            case 0:
                dp1 = accessory.WaypointDp(new Vector3(bjRight ? +19 : -19, 0, +00) + Center, 8600, 6300);
                dp2 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 13 + 100, 0, @event.SourcePosition().Z),
                    4200, 14900);
                break;
            case 1:
                dp1 = accessory.WaypointDp(new Vector3(bjRight ? -19 : +19, 0, +00) + Center, 8600, 6300);
                dp2 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 13 + 100, 0, @event.SourcePosition().Z),
                    4200, 14900);
                break;
            case 2:
                dp1 = accessory.WaypointDp(new Vector3(bjRight ? +19 : -19, 0, +00) + Center, 4200, 10700);
                dp2 = dp1;
                dp2.DestoryAt = 1;
                break;
            case 3:
                dp1 = accessory.WaypointDp(new Vector3(bjRight ? -19 : +19, 0, +00) + Center, 4200, 10700);
                dp2 = dp1;
                dp2.DestoryAt = 1;
                break;
            case 4:
                dp1 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 18 + 100, 0, @event.SourcePosition().Z),
                    10700);
                dp2 = accessory.WaypointDp(new Vector3(bjRight ? +13 : -13, 0, -13) + Center, 4200, 10700);
                break;
            case 5:
                dp1 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 18 + 100, 0, @event.SourcePosition().Z),
                    10700);
                dp2 = accessory.WaypointDp(new Vector3(bjRight ? -13 : +13, 0, -13) + Center, 4200, 10700);
                break;
            case 6:
                dp1 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 16 + 100, 0, @event.SourcePosition().Z),
                    4200, 10700);
                dp2 = accessory.WaypointDp(new Vector3(bjRight ? +13 : -13, 0, +13) + Center, 4200, 14900);
                break;
            case 7:
                dp1 = accessory.WaypointDp(
                    new Vector3(MathF.Sign(@event.SourcePosition().X - 100) * 16 + 100, 0, @event.SourcePosition().Z),
                    4200, 10700);
                dp2 = accessory.WaypointDp(new Vector3(bjRight ? -13 : +13, 0, +13) + Center, 4200, 14900);
                break;
            default:
                return;
        }

        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp1);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp2);
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

    private bool P4GetIndex(uint id, out int index)
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
        if (!P4GetIndex(accessory.Data.Me, out var myidx)) return;
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
        if (!P4GetIndex(accessory.Data.Me, out var myidx)) return;
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
        if (!P4GetIndex(accessory.Data.Me, out var myIdx)) return;
        var isLight = myIdx % 2 > 0;
        // 0为大暗 1为大光 2小暗 3小光 4近线小暗 5近小光 6远线小暗 7远小光
        // 怀疑分摊在3
        var dp = accessory.Data.GetDefaultDrawProperties();

        if (myIdx > 1)
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

        var wpos = myIdx switch
        {
            0 => new Vector3(119.0f, 0, 100.0f),
            1 => new Vector3(093.8f, 0, 083.1f),
            2 => new Vector3(116.0f, 0, 100.0f),
            6 => new Vector3(116.0f, 0, 101.7f),
            _ => new Vector3(116.0f, 0, 098.3f),
        };
        dp = accessory.WaypointDp(wpos, 40000);
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

        dp = accessory.FastDp("连带神判", _p4ShadowPlayers[3], 7200, 4, true);
        dp.Delay = 40000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        switch (myIdx)
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
        if (!P4GetIndex(accessory.Data.Me, out var myIdx)) return;
        var isLight = myIdx % 2 > 0;

        var dp = accessory.FastDp("2测分摊", _p4ShadowPlayers[0], 6100, 6, !isLight);
        dp.Delay = 27500;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        dp = accessory.FastDp("2测分摊", _p4ShadowPlayers[1], 6100, 6, isLight);
        dp.Delay = 27500;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);

        if (myIdx < 2) return;
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
            {
                _p4AlmightyJudgments.Add(fire);
                if (_p4AlmightyJudgments.Count == 2)
                {
                    AlmightyJudgmentGuide(accessory);
                }
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

        dp = accessory.FastDp("地火预指路", wpos1, 6000, 2);
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
}

public static class AccessoryExtensions
{
    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration, float radius, bool safe = false)
    {
        return FastDp(accessory, name, owner, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration, float radius, bool safe = false)
    {
        return FastDp(accessory, name, pos, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, uint target, uint duration, uint delay = 0, string name = "Waypoint")
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

    public static DrawPropertiesEdit WaypointDp(this ScriptAccessory accessory, Vector3 pos, uint duration, uint delay = 0, string name = "Waypoint")
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

    public static IEnumerable<IPlayerCharacter> GetParty(this ScriptAccessory sa)
    {
        foreach (var pid in sa.Data.PartyList)
        {
            yield return (IPlayerCharacter)sa.Data.Objects.SearchByEntityId(pid);
        }
    }

    public static IPlayerCharacter GetMe(this ScriptAccessory sa)
    {
        return (IPlayerCharacter)sa.Data.Objects.SearchByEntityId(sa.Data.Me);
    }
}

public static class IbcHelper
{
    public static IBattleChara? GetByEntityId(uint id)
    {
        return (IBattleChara?)Svc.Objects.SearchByEntityId(id);
    }

    public static IGameObject? GetFirstByDataId(uint dataId)
    {
        return Svc.Objects.FirstOrDefault(x => x.DataId == dataId);
    }

    public static IEnumerable<IGameObject?> GetByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId);
    }

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