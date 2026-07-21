using Diorama.Editor;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diorama.UI.ViewModels
{
    public class EditTexturesViewModel : EditableItem
    {
        public ObservableCollection<RenderTexture> Textures { get; }

        private RenderTexture texture;
        public RenderTexture Texture { 
            get => texture; 
            set 
            {
                if (texture == value)
                    return;

                texture = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(EditorTexture));
            } 
        }

        public EditorTexture EditorTexture { get => new EditorTexture(Texture); }

        

        public EditTexturesViewModel(ObservableCollection<RenderTexture> textures)
        {
            Textures = textures;
        }
    }
}
