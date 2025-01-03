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
using FFXIVClientStructs;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Script.Shadowbringers.TheEpicOfAlexander;

[ScriptType(guid: "E047803D-38D5-45B4-AF48-71C0691CDCC9", name: "亚历山大绝境战.未完工", territorys: [887], version: "0.0.0.1", author: "Cyf5119")]
public class TheEpicOfAlexander
{
    private static readonly Vector3 Center = new Vector3(100, 0, 100);

    private Dictionary<uint, uint> _p1Tether = [];
    
    public void Init(ScriptAccessory accessory)
    {
        _p1Tether = [];
        accessory.Method.RemoveDraw(".*");
        Fluid.Reset();
        LimitCut.Reset();
        HawkBlaster.Reset();
    }

    #region P1

    private static class Fluid
    {
        private static int _state = 0;

        public static void Reset() => _state = 0;
        
        public static void Start(ScriptAccessory accessory)
        {
            // P1 水基佬 平A 比开怪时间略晚？
            if (_state != 0) return;
            _state = 1;
            Draw(accessory);
        }

        private static void Draw(ScriptAccessory accessory)
        {
            var state = _state;
            var sleepTime = new List<int> { 0, 5200, 26300, 19100 };
            if (state < 1 || state > 3) return;
            Thread.Sleep(sleepTime[state]);
            if (state != _state) return;
            DrawLiquid(accessory);
            if (state != 1)
                DrawHand(accessory);
        }

        private static void DrawHand(ScriptAccessory accessory)
        {
            // 流体强袭 18871 手 10
            var hand = IbcHelper.GetFirstByDataId(0x2C48)?.EntityId ?? 0;
            var dp = accessory.FastDp("hand", hand, 5200, 10);
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
        }
        
        private static void DrawLiquid(ScriptAccessory accessory)
        {
            //  流体摆动 18864 人 8
            var liquid = IbcHelper.GetFirstByDataId(0x2C47)?.EntityId ?? 0;
            var dp = accessory.FastDp("liquid", liquid, 5000, 8);
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);

            _state += 1;
            Draw(accessory);
        }
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "P1死刑", ["ActionId:18808"])]
    public void P1Tankbuster(Event @event, ScriptAccessory accessory) => Fluid.Start(accessory);
    // P1BOSS平A

    [ScriptMethod(EventTypeEnum.ActionEffect, "流体强袭清除", ["ActionId:18871"], false)]
    public void HandTankbusterClear(Event @event, ScriptAccessory accessory) => accessory.Method.RemoveDraw("hand");

    [ScriptMethod(EventTypeEnum.ActionEffect, "流体摆动清除", ["ActionId:18864"], false)]
    public void LiquidTankbusterClear(Event @event, ScriptAccessory accessory) => accessory.Method.RemoveDraw("liquid");

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
    public void JagdDollClear(Event @event, ScriptAccessory accessory) => accessory.Method.RemoveDraw($"Jagd Doll {@event.SourceId()}");

    [ScriptMethod(EventTypeEnum.AddCombatant, "栓塞", ["DataId:11339"])]
    public void Embolus(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("embolus", @event.SourceId(), 20000, 1);
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(EventTypeEnum.Tether, "排水", ["Id:0003"])]
    public void Drainage(Event @event, ScriptAccessory accessory)
    {
        // 18471
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

            var dp = accessory.FastDp($"Drainage {sid} {tid}", tid, 15000, 6);
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
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

    private static class LimitCut
    {
        public static bool Enabled = false;
        private static uint _state = 0;
        private static uint[] _playerList = { 0, 0, 0, 0, 0, 0, 0, 0 };

        public static void Reset()
        {
            Enabled = false;
            _state = 0;
        }

        public static void OnTargetIcon(Event @event)
        {
            var idx = (int)@event.IconId() - 79;
            if (idx < 0 || idx > 7) return;
            _playerList[idx] = @event.TargetId();
        }

        public static void OnPlayActionTimeline(Event @event, ScriptAccessory accessory)
        {
            if (!Enabled) return;
            // 阿尔法之剑和超级摧毁者冲击都需加上目标圈大小
            var dp = accessory.FastDp("fan", @event.SourceId(), 1100, 25+5);
            dp.TargetObject = _playerList[_state * 2];
            dp.Radian = float.Pi / 2;
            accessory.Method.SendDraw(0, DrawTypeEnum.Fan, dp);
            dp = accessory.FastDp("rect", @event.SourceId(), 2600, new Vector2(10, 50+5));
            dp.TargetObject = _playerList[_state * 2 + 1];
            accessory.Method.SendDraw(0, DrawTypeEnum.Rect, dp);

            _state++;
            if (_state > 3) Reset();
        }
    }

    [ScriptMethod(EventTypeEnum.TargetIcon, "麻将记录", ["Id:regex:^(00(4F|5[0123456]))$"])]
    public void LimitCutRecord(Event @event, ScriptAccessory accessory) => LimitCut.OnTargetIcon(@event);

    [ScriptMethod(EventTypeEnum.PlayActionTimeline, "麻将执行", ["Id:7747"])]
    public void LimitCutPlay(Event @event, ScriptAccessory accessory) => LimitCut.OnPlayActionTimeline(@event, accessory);

    private static class HawkBlaster
    {
        private static uint _state = 0;
        private static object _locker = new ();

        public static void Reset() => _state = 0;
        
        public static void Start(Event @event, ScriptAccessory accessory)
        {
            //  鹰式破坏炮 18480 间隔2.2s 10m圆形
            LimitCut.Enabled = true;
            if (@event.TargetIndex() != 1) return;
            lock (_locker)
            {
                _state++;
                Calculate(@event, accessory, _state);
                if (_state > 17)
                    Reset();
            }
        }

        private static void Calculate(Event @event, ScriptAccessory accessory, uint state)
        {
            var nextPos = Vector3.Transform((@event.EffectPosition() - Center), Matrix4x4.CreateRotationY(-float.Pi / 4)) + Center;
            if (new List<uint> { 7, 8 }.Contains(state))
                Draw(accessory, nextPos, 4400);
            if (new List<uint> { 8, 17 }.Contains(state))
                Draw(accessory, Center, 2200);
            if (!new List<uint> { 7, 8, 9, 16, 17, 18 }.Contains(state))
                Draw(accessory, nextPos, 2200);
        }

        public static void Draw(ScriptAccessory accessory, Vector3 position, uint dura)
        {
            var dp = accessory.FastDp("circle", position, dura, 10);
            dp.Color = accessory.Data.DefaultDangerColor.WithW(0.1f);
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
            dp.Color = accessory.Data.DefaultDangerColor.WithW(4);
            dp.ScaleMode = ScaleMode.ByTime;
            accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(EventTypeEnum.ActionEffect, "P1.5地火", ["ActionId:18480"])]
    public void HawkBlasterStart(Event @event, ScriptAccessory accessory) => HawkBlaster.Start(@event, accessory);

    #endregion

    #region P2

    [ScriptMethod(EventTypeEnum.StartCasting, "激光战轮", ["ActionId:18517"])]
    public void EyeOfTheChakram(Event @event, ScriptAccessory accessory)
    {
        // 加上目标圈
        var dp = accessory.FastDp("Eye of the Chakram", @event.SourceId(), 6000, new Vector2(6, 70+3));
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
        var dp = accessory.FastDp("Spin Crusher", @event.SourceId(), 3000, 5+5);
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
        var dp = accessory.FastDp("Super Jump", new Vector3(0), 4300, 10);
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
        var dp = accessory.FastDp("Alpha Sword", CruiseChaser.EntityId, duration, 25+5);
        dp.Delay = delay;
        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
        for (uint i = 1; i < 4; i++)
        {
            dp.TargetOrderIndex = i;
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
        for (uint i = 1; i < times+1; i++)
        {
            dp.TargetOrderIndex = i;
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
        var dp = accessory.FastDp("Divine Spear", @event.SourceId(), 9400, 17+7.2f);
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
    public void LimitCutP3(Event @event, ScriptAccessory accessory) => LimitCut.Enabled = true;
    // 19215 限制器减档
    
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

    [ScriptMethod(EventTypeEnum.StartCasting, "株连", ["ActionId:18580"])]
    public void IrresistibleGrace(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.FastDp("Irresistible Grace", @event.TargetId(), 5000, 6);
        dp.Color = accessory.Data.DefaultSafeColor;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
    



    [ScriptMethod(EventTypeEnum.StartCasting, "地火", ["ActionId:18575"])]
    public void AlmightyJudgment(Event @event, ScriptAccessory accessory)
    {
        // 从演示咏唱到实际判定约8s 两次地火间隔是2s
        // 实际判定为 18576
        var dp = accessory.FastDp("Almighty Judgment", @event.EffectPosition(), 2000, 6);
        dp.Delay = 6000;
        accessory.Method.SendDraw(0, DrawTypeEnum.Circle, dp);
    }
        /*
        pos:vec3(      107.988,    -0.015259,      99.9924 ) 右下
        pos:vec3(      99.9924,    -0.015259,      107.988 ) 中下
        pos:vec3(      91.9966,    -0.015259,      107.988 ) 左下
        pos:vec3(      107.988,    -0.015259,      99.9924 ) 右中
        */

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
    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration,
        float radius)
    {
        return FastDp(accessory, name, owner, duration, new Vector2(radius));
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, uint owner, uint duration,
        Vector2 scale)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration, float radius)
    {
        return FastDp(accessory, name, pos, duration, new Vector2(radius));
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory accessory, string name, Vector3 pos, uint duration, Vector2 scale)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
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
}