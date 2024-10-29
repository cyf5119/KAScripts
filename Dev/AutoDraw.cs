using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Network.Structures.InfoProxy;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.GeneratedSheets2;

// using Lumina.Excel.GeneratedSheets2;

namespace Cyf5119Script
{
    [ScriptType(guid: "{FC02EEF5-5F68-2EF1-A41D-C8FC18346376}", name: "AutoDraw", territorys: [], version: "0.0.0.2")]
    public class AutoDraw
    {
        [UserSetting(note: "Show friend (true/false)")]
        public bool ShowFriend { get; set; } = true;

        public void Init(ScriptAccessory accessory)
        {
        }

        [ScriptMethod(name: "StartCast", eventType: EventTypeEnum.StartCasting)]
        public void StartCast(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            ParseObjectId(@event["SourceId"], out var sid);
            ParseObjectId(@event["TargetId"], out var tid);
            var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
            var dura = JsonConvert.DeserializeObject<uint>(@event["DurationMilliseconds"]);
            var spos = JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            var tpos = JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);

            var action = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(aid);
            var castType = action.CastType;
            float range = action.EffectRange;
            float width = action.XAxisModifier;
            // TODO USE OMEN TO HELP ADJUST THE SHAPE
            // var omen = action.Omen;
            
            dp.Name = $"AutoDraw {sid:X}:{aid}";
            
            if (castType == 1) return; // 1 is single target cast
            
            var source = GetObjById(sid);
            var target = GetObjById(tid);
            // var source = Svc.Objects.Where(x => x.GameObjectId == sid).FirstOrDefault();
            // var target = Svc.Objects.Where(x => x.GameObjectId == tid).FirstOrDefault();
            
            if (castType >= 3 && castType <= 5) // deal fan = 3, laser = 4 , around = 5 with source radius extra
                range += source.HitboxRadius;
            
            dp.Color = new(0f, 0f, 0f, 0f);
            // TODO here should add something for special actions
            if (IsEnemy(Svc.ClientState.LocalPlayer, (ICharacter?)source))
                dp.Color = accessory.Data.DefaultDangerColor;
            else if (ShowFriend)
                dp.Color = accessory.Data.DefaultSafeColor;
            else
                return;
            
            // TODO here should add something for special actions
            dp.Delay = 0;
            dp.DestoryAt = dura;
            
            if (castType == 8) // rect to target
            {
                if (source is null || target is null)
                    return;
                dp.TargetObject = tid;
                dp.ScaleMode = ScaleMode.YByDistance;
            }
            
            // TODO here should add something for special actions
            var shape = GetShapeDefault(castType);
            if (shape is null) return;
            if (shape == DrawTypeEnum.Donut)
            {
                dp.InnerScale = new (range / 2);
                dp.Radian = float.Pi * 2;
            }
            
            dp.Scale = new Vector2(width != 0 ? width : range, range);

            if (shape == DrawTypeEnum.Circle || shape == DrawTypeEnum.Donut)
            {
                if (tid != sid)
                    dp.Owner = tid;
                else
                    dp.Position = tpos;
            }
            else
            {
                dp.Owner = sid;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, (DrawTypeEnum)shape, dp);
            accessory.Log.Debug($"AutoDraw {sid:X}:{aid}");
        }

        [ScriptMethod(name: "CancelCast", eventType: EventTypeEnum.CancelAction)]
        public void CancelCast(Event @event, ScriptAccessory accessory)
        {
            ParseObjectId(@event["SourceId"], out var sid);
            var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);

            accessory.Method.RemoveDraw($"AutoDraw {sid:X}:{aid}");
            accessory.Log.Debug($"CancelAuto {sid:X}:{aid}");
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

        private static DrawTypeEnum? GetShapeDefault(byte castType)
        {
            var shapeDict = new Dictionary<byte, DrawTypeEnum>
            {
                // 1 is single
                { 2, DrawTypeEnum.Circle }, // circle
                { 3, DrawTypeEnum.Fan }, // fan
                { 4, DrawTypeEnum.Rect }, // rect
                { 5, DrawTypeEnum.Circle }, // circle
                { 6, DrawTypeEnum.Circle }, // circle ?
                { 7, DrawTypeEnum.Circle }, // circle
                { 8, DrawTypeEnum.Rect }, // rect to target
                { 10, DrawTypeEnum.Donut }, // donut
                { 11, DrawTypeEnum.Straight }, // TODO cross
                { 12, DrawTypeEnum.Rect }, // rect 
                { 13, DrawTypeEnum.Fan } // fan
            };
            if (shapeDict.ContainsKey(castType))
                return shapeDict[castType];
            return null;
        }
        
        private static bool ReadUnknown(Battalion? b, int i)
        {
            if (b is null) return false;
            switch (i)
            {
                case 0:
                    return b.Unknown0;
                case 1:
                    return b.Unknown1;
                case 2:
                    return b.Unknown2;
                case 3:
                    return b.Unknown3;
                case 4:
                    return b.Unknown4;
                case 5:
                    return b.Unknown5;
                case 6:
                    return b.Unknown6;
                case 7:
                    return b.Unknown7;
                case 8:
                    return b.Unknown8;
                case 9:
                    return b.Unknown9;
                case 10:
                    return b.Unknown10;
                case 11:
                    return b.Unknown11;
                case 12:
                    return b.Unknown12;
                case 13:
                    return b.Unknown13;
                case 14:
                    return b.Unknown14;
                default:
                    return false;
            }
        }
        
        private static unsafe byte GetBattalionKey(ICharacter character, uint mode)
        {
            if (mode == 1 && character.ObjectKind == ObjectKind.Player)
                return 0;
            return character.Struct()->Battalion;
        }
        
        private static bool IsEnemy(ICharacter? a1, ICharacter? a2)
        {
            if (a1 is null || a2 is null) return true;
            byte battalionMode;
            try
            {
                battalionMode = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>().GetRow(Svc.ClientState.TerritoryType).BattalionMode;
            }
            catch(KeyNotFoundException)
            {
                return false;
            }

            if (battalionMode == 0)
                return false;
            var row = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Battalion>().GetRow(GetBattalionKey(a1, battalionMode));
            return ReadUnknown(row, GetBattalionKey(a2, battalionMode));
        }
        
        private IGameObject? GetObjById(ulong? objid)
        {
            if (objid is null)
                return null;
            return Svc.Objects.SearchById((ulong)objid);
        }
    }
}
