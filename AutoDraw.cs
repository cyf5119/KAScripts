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
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

// using Lumina.Excel.GeneratedSheets2;

namespace Cyf5119Script
{
    [ScriptType(guid: "{FC02EEF5-5F68-2EF1-A41D-C8FC18346376}", name: "AutoDraw", territorys: [], version: "0.0.0.1")]
    public class AutoDraw
    {
        [UserSetting(note: "This is a test Property")]
        public int prop1 { get; set; } = 1;

        [UserSetting("Another Test Property")] public bool prop2 { get; set; } = false;

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
            var cast_type = action.CastType;
            if (cast_type == 1) return;
            float range = action.EffectRange;
            float width = action.XAxisModifier;
            // TODO USE OMEN TO HELP ADJUST THE SHAPE
            // var omen = action.Omen;
            var source = Svc.Objects.Where(x => x.GameObjectId == sid).FirstOrDefault();
            var target = Svc.Objects.Where(x => x.GameObjectId == tid).FirstOrDefault();
            if (cast_type >= 3 && cast_type <= 5) // deal fan = 3, laser = 4 , around = 5 with source radius extra
                range += source.HitboxRadius;
            
            dp.Name = $"{sid:X}:{aid}";
            dp.DestoryAt = dura;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            
            // TODO JUDGE WHO IS ENEMY
            if (cast_type == 8) // rect to target
            {
                if (source is null && target is null)
                    return;
                dp.TargetObject = tid;
                dp.ScaleMode = ScaleMode.YByDistance;
            }
            var shape = GetShapeDefault(cast_type);
            if(shape == DrawTypeEnum.Circle || shape == DrawTypeEnum.Donut)
                dp.Owner = tid;
            
            dp.Scale = new Vector2(width != 0 ? width : range, range);
            // if (width != 0)
            //     dp.Scale = new Vector2(width, range);

            accessory.Method.SendDraw(DrawModeEnum.Default, shape, dp);
            accessory.Log.Debug($"{sid:X}:{aid} has been drawn.");
        }

        [ScriptMethod(name: "CancelCast", eventType: EventTypeEnum.CancelAction)]
        public void CancelCast(Event @event, ScriptAccessory accessory)
        {
            ParseObjectId(@event["SourceId"], out var sid);
            var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);

            accessory.Method.RemoveDraw($"{sid:X}:{aid}");
            accessory.Log.Debug($"{sid:X}:{aid} has been canceled.");
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

        private DrawTypeEnum GetShapeDefault(byte cast_type)
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
            return shapeDict[cast_type];
        }

        // private byte GetBattalion(uint id, uint mode)
        // {
        //     var obj = Svc.Objects.Where(x => x.GameObjectId == id).FirstOrDefault();
        //     if (mode == 1 && obj.ObjectKind == ObjectKind.Player)
        //         return 0;
        //     // TODO FIND OUT HOW TO GET THE Battalion
        //     return (byte)obj.ObjectKind;
        // }
        //
        // private bool IsEnemy(uint? id1, uint? id2)
        // {
        //     var obj1 = GetObjById(id1);
        //     var obj2 = GetObjById(id2);
        //     if (id1 is null && id2 is null)
        //         return true;
        //     if ((byte)obj1.ObjectKind > 2 || (byte)obj2.ObjectKind > 2)
        //         return false;
        //     byte battalion_mode;
        //     try
        //     {
        //         battalion_mode = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>().GetRow(Svc.ClientState.TerritoryType).BattalionMode;
        //     }
        //     catch (KeyNotFoundException)
        //     {
        //         return false;
        //     }
        //
        //     if (battalion_mode == 0)
        //         return false;
        //     
        //     // TODO FIND OUT HOW TO GET THE Battalion
        //     var row = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Battalion>().GetRow(GetBattalion());
        //     
        //     return ;
        // }


        // private IGameObject? GetObjById(ulong? objid)
        // {
        //     if (objid is null)
        //         return null;
        //     return Svc.Objects.SearchById((ulong)objid);
        // }
    }
}