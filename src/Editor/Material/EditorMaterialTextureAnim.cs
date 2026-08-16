using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Material
{
    public class EditorMaterialTextureAnim : EditableItem
    {
        public NuTexAnimBlock Block;

        [DisplayLabel("Mode U")]
        public byte ModeU { get => Block.ModeU; set => Set(ref Block.ModeU, value); }

        [DisplayLabel("Mode V")]
        public byte ModeV { get => Block.ModeV; set => Set(ref Block.ModeV, value); }

        [DisplayLabel("Delta U")]
        public float dU { get => Block.DU; set => Set(ref Block.DU, value); }

        [DisplayLabel("Delta V")]
        public float dV { get => Block.DV; set => Set(ref Block.DV, value); }

        [DisplayLabel("Speed U")]
        public float SpeedU { get => Block.SpeedU; set => Set(ref Block.SpeedU, value); }

        [DisplayLabel("Speed V")]
        public float SpeedV { get => Block.SpeedV; set => Set(ref Block.SpeedV, value); }

        [DisplayLabel("Sprite Sheet Num. Rows")]
        public byte SpriteSheetRows { get => Block.SSNumRows; set => Set(ref Block.SSNumRows, value); }

        [DisplayLabel("Sprite Sheet Num. Columns")]
        public byte SpriteSheetCols { get => Block.SSNumColumns; set => Set(ref Block.SSNumColumns, value); }

        [DisplayLabel("Sprite Sheet Row Index")]
        public byte SpriteSheetRowIndex { get => Block.SSRowIndex; set => Set(ref Block.SSRowIndex, value); }

        [DisplayLabel("Sprite Sheet Num. Images")]
        public byte SpriteSheetNumImages { get => Block.SSNumImages; set => Set(ref Block.SSNumImages, value); }

        [DisplayLabel("Sprite Sheet Duration")]
        public float SpriteSheetDuration { get => Block.SSDuration; set => Set(ref Block.SSDuration, value); }

        [DisplayLabel("Sprite Sheet Offset")]
        public byte SpriteSheetOffset { get => Block.SSOffset; set => Set(ref Block.SSOffset, value); }

        public EditorMaterialTextureAnim(NuTexAnimBlock block)
        {
            Block = block;
        }
    }
}
