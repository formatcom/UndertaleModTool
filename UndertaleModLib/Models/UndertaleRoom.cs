﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModLib.Models
{
    public class UndertaleRoom : UndertaleNamedResource, INotifyPropertyChanged
    {
        [Flags]
        public enum RoomEntryFlags : uint
        {
            EnableViews = 1,
            ShowColor = 2,
            ClearDisplayBuffer = 4
        }

        private UndertaleString _Name;
        private UndertaleString _Caption;
        private uint _Width = 320;
        private uint _Height = 240;
        private uint _Speed = 30;
        private bool _Persistent = false;
        private uint _BackgroundColor = 0x00000000;
        private bool _DrawBackgroundColor = true;
        private UndertaleResourceById<UndertaleCode> _CreationCodeId { get; } = new UndertaleResourceById<UndertaleCode>("CODE");
        private RoomEntryFlags _Flags = RoomEntryFlags.EnableViews;
        private uint _World = 0;
        private uint _Top = 0;
        private uint _Left = 0;
        private uint _Right = 1024;
        private uint _Bottom = 768;
        private float _GravityX = 0;
        private float _GravityY = 10;
        private float _MetersPerPixel = 0.1f;

        public UndertaleString Name { get => _Name; set { _Name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name")); } }
        public UndertaleString Caption { get => _Caption; set { _Caption = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Caption")); } }
        public uint Width { get => _Width; set { _Width = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width")); } }
        public uint Height { get => _Height; set { _Height = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Height")); } }
        public uint Speed { get => _Speed; set { _Speed = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Speed")); } }
        public bool Persistent { get => _Persistent; set { _Persistent = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Persistent")); } }
        public uint BackgroundColor { get => _BackgroundColor; set { _BackgroundColor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundColor")); } }
        public bool DrawBackgroundColor { get => _DrawBackgroundColor; set { _DrawBackgroundColor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DrawBackgroundColor")); } }
        public UndertaleCode CreationCodeId { get => _CreationCodeId.Resource; set { _CreationCodeId.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CreationCodeId")); } }
        public RoomEntryFlags Flags { get => _Flags; set { _Flags = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags")); } }
        public uint World { get => _World; set { _World = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("World")); } }
        public uint Top { get => _Top; set { _Top = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Top")); } }
        public uint Left { get => _Left; set { _Left = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Left")); } }
        public uint Right { get => _Right; set { _Right = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Right")); } }
        public uint Bottom { get => _Bottom; set { _Bottom = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bottom")); } }
        public float GravityX { get => _GravityX; set { _GravityX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GravityX")); } }
        public float GravityY { get => _GravityY; set { _GravityY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GravityY")); } }
        public float MetersPerPixel { get => _MetersPerPixel; set { _MetersPerPixel = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetersPerPixel")); } }
        public UndertalePointerList<Background> Backgrounds { get; private set; } = new UndertalePointerList<Background>();
        public UndertalePointerList<View> Views { get; private set; } = new UndertalePointerList<View>();
        public UndertalePointerList<GameObject> GameObjects { get; private set; } = new UndertalePointerList<GameObject>();
        public UndertalePointerList<Tile> Tiles { get; private set; } = new UndertalePointerList<Tile>();
        public UndertalePointerList<Layer> Layers { get; private set; } = new UndertalePointerList<Layer>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UndertaleRoom()
        {
            for (int i = 0; i < 8; i++)
                Backgrounds.Add(new Background());
            for (int i = 0; i < 8; i++)
                Views.Add(new View());
            if (Flags.HasFlag(RoomEntryFlags.EnableViews))
                Views[0].Enabled = true;
        }

        public void Serialize(UndertaleWriter writer)
        {
            writer.WriteUndertaleString(Name);
            writer.WriteUndertaleString(Caption);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Speed);
            writer.Write(Persistent);
            writer.Write(BackgroundColor);
            writer.Write(DrawBackgroundColor);
            writer.Write(_CreationCodeId.Serialize(writer));
            writer.Write((uint)Flags);
            writer.WriteUndertaleObjectPointer(Backgrounds);
            writer.WriteUndertaleObjectPointer(Views);
            writer.WriteUndertaleObjectPointer(GameObjects);
            writer.WriteUndertaleObjectPointer(Tiles);
            writer.Write(World);
            writer.Write(Top);
            writer.Write(Left);
            writer.Write(Right);
            writer.Write(Bottom);
            writer.Write(GravityX);
            writer.Write(GravityY);
            writer.Write(MetersPerPixel);
            if (writer.undertaleData.GeneralInfo.Major >= 2)
                writer.WriteUndertaleObjectPointer(Layers);
            writer.WriteUndertaleObject(Backgrounds);
            writer.WriteUndertaleObject(Views);
            writer.WriteUndertaleObject(GameObjects);
            writer.WriteUndertaleObject(Tiles);
            if (writer.undertaleData.GeneralInfo.Major >= 2)
                writer.WriteUndertaleObject(Layers);
        }

        public void Unserialize(UndertaleReader reader)
        {
            Name = reader.ReadUndertaleString();
            Caption = reader.ReadUndertaleString();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            Speed = reader.ReadUInt32();
            Persistent = reader.ReadBoolean();
            BackgroundColor = reader.ReadUInt32();
            DrawBackgroundColor = reader.ReadBoolean();
            _CreationCodeId.Unserialize(reader, reader.ReadInt32());
            Flags = (RoomEntryFlags)reader.ReadUInt32();
            Backgrounds = reader.ReadUndertaleObjectPointer<UndertalePointerList<Background>>();
            Views = reader.ReadUndertaleObjectPointer<UndertalePointerList<View>>();
            GameObjects = reader.ReadUndertaleObjectPointer<UndertalePointerList<GameObject>>();
            Tiles = reader.ReadUndertaleObjectPointer<UndertalePointerList<Tile>>();
            World = reader.ReadUInt32();
            Top = reader.ReadUInt32();
            Left = reader.ReadUInt32();
            Right = reader.ReadUInt32();
            Bottom = reader.ReadUInt32();
            GravityX = reader.ReadSingle();
            GravityY = reader.ReadSingle();
            MetersPerPixel = reader.ReadSingle();
            if (reader.undertaleData.GeneralInfo.Major >= 2)
                Layers = reader.ReadUndertaleObjectPointer<UndertalePointerList<Layer>>();
            if (reader.ReadUndertaleObject<UndertalePointerList<Background>>() != Backgrounds)
                throw new IOException();
            if (reader.ReadUndertaleObject<UndertalePointerList<View>>() != Views)
                throw new IOException();
            if (reader.ReadUndertaleObject<UndertalePointerList<GameObject>>() != GameObjects)
                throw new IOException();
            if (reader.ReadUndertaleObject<UndertalePointerList<Tile>>() != Tiles)
                throw new IOException();
            if (reader.undertaleData.GeneralInfo.Major >= 2)
                if (reader.ReadUndertaleObject<UndertalePointerList<Layer>>() != Layers)
                    throw new IOException();
        }

        public override string ToString()
        {
            return Name.Content + " (" + GetType().Name + ")";
        }

        public interface RoomObject
        {
            int X { get; }
            int Y { get; }
            uint InstanceID { get; }
        }

        public class Background : UndertaleObject, INotifyPropertyChanged
        {
            private bool _Enabled = false;
            private bool _Foreground = false;
            private UndertaleResourceById<UndertaleBackground> _BackgroundDefinition { get; } = new UndertaleResourceById<UndertaleBackground>("BGND");
            private uint _X = 0;
            private uint _Y = 0;
            private uint _TileX = 1;
            private uint _TileY = 1;
            private int _SpeedX = 0;
            private int _SpeedY = 0;
            private UndertaleResourceById<UndertaleGameObject> _ObjectId { get; } = new UndertaleResourceById<UndertaleGameObject>("OBJT");

            public bool Enabled { get => _Enabled; set { _Enabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled")); } }
            public bool Foreground { get => _Foreground; set { _Foreground = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Foreground")); } }
            public UndertaleBackground BackgroundDefinition { get => _BackgroundDefinition.Resource; set { _BackgroundDefinition.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundDefinition")); } }
            public uint X { get => _X; set { _X = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X")); } }
            public uint Y { get => _Y; set { _Y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y")); } }
            public uint TileX { get => _TileX; set { _TileX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileX")); } }
            public uint TileY { get => _TileY; set { _TileY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileY")); } }
            public int SpeedX { get => _SpeedX; set { _SpeedX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedX")); } }
            public int SpeedY { get => _SpeedY; set { _SpeedY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedY")); } }
            public UndertaleGameObject ObjectId { get => _ObjectId.Resource; set { _ObjectId.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObjectId")); } }

            public event PropertyChangedEventHandler PropertyChanged;

            public void Serialize(UndertaleWriter writer)
            {
                writer.Write(Enabled);
                writer.Write(Foreground);
                writer.Write(_BackgroundDefinition.Serialize(writer));
                writer.Write(X);
                writer.Write(Y);
                writer.Write(TileX);
                writer.Write(TileY);
                writer.Write(SpeedX);
                writer.Write(SpeedY);
                writer.Write(_ObjectId.Serialize(writer));
            }

            public void Unserialize(UndertaleReader reader)
            {
                Enabled = reader.ReadBoolean();
                Foreground = reader.ReadBoolean();
                _BackgroundDefinition.Unserialize(reader, reader.ReadInt32());
                X = reader.ReadUInt32();
                Y = reader.ReadUInt32();
                TileX = reader.ReadUInt32();
                TileY = reader.ReadUInt32();
                SpeedX = reader.ReadInt32();
                SpeedY = reader.ReadInt32();
                _ObjectId.Unserialize(reader, reader.ReadInt32());
            }
        }

        public class View : UndertaleObject, INotifyPropertyChanged
        {
            private bool _Enabled = false;
            private int _ViewX = 0;
            private int _ViewY = 0;
            private int _ViewWidth = 640;
            private int _ViewHeight = 480;
            private int _PortX = 0;
            private int _PortY = 0;
            private int _PortWidth = 640;
            private int _PortHeight = 480;
            private uint _BorderX = 32;
            private uint _BorderY = 32;
            private int _SpeedX = -1;
            private int _SpeedY = -1;
            private UndertaleResourceById<UndertaleGameObject> _ObjectId { get; } = new UndertaleResourceById<UndertaleGameObject>("OBJT");

            public bool Enabled { get => _Enabled; set { _Enabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled")); } }
            public int ViewX { get => _ViewX; set { _ViewX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewX")); } }
            public int ViewY { get => _ViewY; set { _ViewY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewY")); } }
            public int ViewWidth { get => _ViewWidth; set { _ViewWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewWidth")); } }
            public int ViewHeight { get => _ViewHeight; set { _ViewHeight = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewHeight")); } }
            public int PortX { get => _PortX; set { _PortX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortX")); } }
            public int PortY { get => _PortY; set { _PortY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortY")); } }
            public int PortWidth { get => _PortWidth; set { _PortWidth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortWidth")); } }
            public int PortHeight { get => _PortHeight; set { _PortHeight = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PortHeight")); } }
            public uint BorderX { get => _BorderX; set { _BorderX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderX")); } }
            public uint BorderY { get => _BorderY; set { _BorderY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderY")); } }
            public int SpeedX { get => _SpeedX; set { _SpeedX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedX")); } }
            public int SpeedY { get => _SpeedY; set { _SpeedY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedY")); } }
            public UndertaleGameObject ObjectId { get => _ObjectId.Resource; set { _ObjectId.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObjectId")); } }

            public event PropertyChangedEventHandler PropertyChanged;

            public void Serialize(UndertaleWriter writer)
            {
                writer.Write(Enabled);
                writer.Write(ViewX);
                writer.Write(ViewY);
                writer.Write(ViewWidth);
                writer.Write(ViewHeight);
                writer.Write(PortX);
                writer.Write(PortY);
                writer.Write(PortWidth);
                writer.Write(PortHeight);
                writer.Write(BorderX);
                writer.Write(BorderY);
                writer.Write(SpeedX);
                writer.Write(SpeedY);
                writer.Write(_ObjectId.Serialize(writer));
            }

            public void Unserialize(UndertaleReader reader)
            {
                Enabled = reader.ReadBoolean();
                ViewX = reader.ReadInt32();
                ViewY = reader.ReadInt32();
                ViewWidth = reader.ReadInt32();
                ViewHeight = reader.ReadInt32();
                PortX = reader.ReadInt32();
                PortY = reader.ReadInt32();
                PortWidth = reader.ReadInt32();
                PortHeight = reader.ReadInt32();
                BorderX = reader.ReadUInt32();
                BorderY = reader.ReadUInt32();
                SpeedX = reader.ReadInt32();
                SpeedY = reader.ReadInt32();
                _ObjectId.Unserialize(reader, reader.ReadInt32());
            }
        }

        public class GameObject : UndertaleObject, RoomObject, INotifyPropertyChanged
        {
            private int _X;
            private int _Y;
            private UndertaleResourceById<UndertaleGameObject> _ObjectDefinition { get; } = new UndertaleResourceById<UndertaleGameObject>("OBJT");
            private uint _InstanceID;
            private UndertaleResourceById<UndertaleCode> _CreationCode { get; } = new UndertaleResourceById<UndertaleCode>("CODE");
            private float _ScaleX = 1;
            private float _ScaleY = 1;
            private uint _Color = 0xFFFFFFFF;
            private float _Rotation = 0;
            private UndertaleResourceById<UndertaleCode> _PreCreateCode { get; } = new UndertaleResourceById<UndertaleCode>("CODE");

            public int X { get => _X; set { _X = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X")); } }
            public int Y { get => _Y; set { _Y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y")); } }
            public UndertaleGameObject ObjectDefinition { get => _ObjectDefinition.Resource; set { _ObjectDefinition.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObjectDefinition")); } }
            public uint InstanceID { get => _InstanceID; set { _InstanceID = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstanceID")); } }
            public UndertaleCode CreationCode { get => _CreationCode.Resource; set { _CreationCode.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CreationCode")); } }
            public float ScaleX { get => _ScaleX; set { _ScaleX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleX")); } }
            public float ScaleY { get => _ScaleY; set { _ScaleY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleY")); } }
            public uint Color { get => _Color; set { _Color = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color")); } }
            public float Rotation { get => _Rotation; set { _Rotation = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rotation")); } }
            public UndertaleCode PreCreateCode { get => _PreCreateCode.Resource; set { _PreCreateCode.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreCreateCode")); } }

            public event PropertyChangedEventHandler PropertyChanged;

            public void Serialize(UndertaleWriter writer)
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(_ObjectDefinition.Serialize(writer));
                writer.Write(InstanceID);
                writer.Write(_CreationCode.Serialize(writer));
                writer.Write(ScaleX);
                writer.Write(ScaleY);
                writer.Write(Color);
                writer.Write(Rotation);
                if (writer.undertaleData.GeneralInfo.BytecodeVersion >= 16) // TODO: is that dependent on bytecode or something else?
                    writer.Write(_PreCreateCode.Serialize(writer));         // Note: Appears in GM:S 1.4.9999 as well, so that's probably the closest it gets
            }

            public void Unserialize(UndertaleReader reader)
            {
                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                _ObjectDefinition.Unserialize(reader, reader.ReadInt32());
                InstanceID = reader.ReadUInt32();
                _CreationCode.Unserialize(reader, reader.ReadInt32());
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                Color = reader.ReadUInt32();
                Rotation = reader.ReadSingle();
                if (reader.undertaleData.GeneralInfo.BytecodeVersion >= 16) // TODO: is that dependent on bytecode or something else?
                    _PreCreateCode.Unserialize(reader, reader.ReadInt32()); // Note: Appears in GM:S 1.4.9999 as well, so that's probably the closest it gets
            }
        }

        public class Tile : UndertaleObject, RoomObject, INotifyPropertyChanged
        {
            private int _X;
            private int _Y;
            private UndertaleResourceById<UndertaleBackground> _BackgroundDefinition { get; } = new UndertaleResourceById<UndertaleBackground>("BGND");
            private uint _SourceX;
            private uint _SourceY;
            private uint _Width;
            private uint _Height;
            private int _TileDepth = 0;
            private uint _InstanceID;
            private float _ScaleX = 1;
            private float _ScaleY = 1;
            private uint _Color = 0xFFFFFFFF;

            public int X { get => _X; set { _X = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X")); } }
            public int Y { get => _Y; set { _Y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y")); } }
            public UndertaleBackground BackgroundDefinition { get => _BackgroundDefinition.Resource; set { _BackgroundDefinition.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundDefinition")); } }
            public uint SourceX { get => _SourceX; set { _SourceX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceX")); } }
            public uint SourceY { get => _SourceY; set { _SourceY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceY")); } }
            public uint Width { get => _Width; set { _Width = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width")); } }
            public uint Height { get => _Height; set { _Height = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Height")); } }
            public int TileDepth { get => _TileDepth; set { _TileDepth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileDepth")); } }
            public uint InstanceID { get => _InstanceID; set { _InstanceID = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstanceID")); } }
            public float ScaleX { get => _ScaleX; set { _ScaleX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleX")); } }
            public float ScaleY { get => _ScaleY; set { _ScaleY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleY")); } }
            public uint Color { get => _Color; set { _Color = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color")); } }

            public event PropertyChangedEventHandler PropertyChanged;

            public void Serialize(UndertaleWriter writer)
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(_BackgroundDefinition.Serialize(writer));
                writer.Write(SourceX);
                writer.Write(SourceY);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(TileDepth);
                writer.Write(InstanceID);
                writer.Write(ScaleX);
                writer.Write(ScaleY);
                writer.Write(Color);
            }

            public void Unserialize(UndertaleReader reader)
            {
                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                _BackgroundDefinition.Unserialize(reader, reader.ReadInt32());
                SourceX = reader.ReadUInt32();
                SourceY = reader.ReadUInt32();
                Width = reader.ReadUInt32();
                Height = reader.ReadUInt32();
                TileDepth = reader.ReadInt32();
                InstanceID = reader.ReadUInt32();
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                Color = reader.ReadUInt32();
            }
        }

        // For GMS2, Backgrounds and Tiles are empty and this is used instead
        public enum LayerType
        {
            Instances = 2,
            Tiles = 4,
            Background = 1,
            Assets = 3
        }

        public class Layer : UndertaleObject
        {
            public UndertaleString LayerName; // "Instances" // "Tiles_1" // "Background"
            public uint LayerId; // 0 // 1 // 2
            public LayerType LayerType; // 2 // 4 // 1
            public uint LayerDepth; // 0 // 100 // 200
            public float XOffset; // 0 // 0 // 0
            public float YOffset; // 0 // 0 // 0
            public float HSpeed; // 0 // 0 // 0
            public float VSpeed; // 0 // 0 // 0
            public bool IsVisible; // 1 // 1 // 1

            public LayerInstancesData InstancesData;
            public LayerTilesData TilesData;
            public LayerBackgroundData BackgroundData;
            public LayerAssetsData AssetsData;

            public void Serialize(UndertaleWriter writer)
            {
                writer.WriteUndertaleString(LayerName);
                writer.Write(LayerId);
                writer.Write((uint)LayerType);
                writer.Write(LayerDepth);
                writer.Write(XOffset);
                writer.Write(YOffset);
                writer.Write(HSpeed);
                writer.Write(VSpeed);
                writer.Write(IsVisible);
                if (LayerType == LayerType.Instances)
                {
                    writer.WriteUndertaleObject(InstancesData);
                }
                else if (LayerType == LayerType.Tiles)
                {
                    writer.WriteUndertaleObject(TilesData);
                }
                else if (LayerType == LayerType.Background)
                {
                    writer.WriteUndertaleObject(BackgroundData);
                }
                else if (LayerType == LayerType.Assets)
                {
                    writer.WriteUndertaleObject(AssetsData);
                }
                else
                {
                    throw new Exception("Unsupported layer type " + LayerType);
                }
            }

            public void Unserialize(UndertaleReader reader)
            {
                LayerName = reader.ReadUndertaleString();
                LayerId = reader.ReadUInt32();
                LayerType = (LayerType)reader.ReadUInt32();
                LayerDepth = reader.ReadUInt32();
                XOffset = reader.ReadSingle();
                YOffset = reader.ReadSingle();
                HSpeed = reader.ReadSingle();
                VSpeed = reader.ReadSingle();
                IsVisible = reader.ReadBoolean();
                if (LayerType == LayerType.Instances)
                {
                    InstancesData = reader.ReadUndertaleObject<LayerInstancesData>();
                }
                else if (LayerType == LayerType.Tiles)
                {
                    TilesData = reader.ReadUndertaleObject<LayerTilesData>();
                }
                else if (LayerType == LayerType.Background)
                {
                    BackgroundData = reader.ReadUndertaleObject<LayerBackgroundData>();
                }
                else if (LayerType == LayerType.Assets)
                {
                    AssetsData = reader.ReadUndertaleObject<LayerAssetsData>();
                }
                else
                {
                    throw new Exception("Unsupported layer type " + LayerType);
                }
            }
        }
    }

    public class LayerInstancesData : UndertaleObject
    {
        public uint[] InstanceIds; // 100000, 100001, 100002, 100003 - probably instance ids from GameObjects list in the room
                                   // confirmed

        public void Serialize(UndertaleWriter writer)
        {
            writer.Write((uint)InstanceIds.Length);
            foreach (var id in InstanceIds)
                writer.Write(id);
        }

        public void Unserialize(UndertaleReader reader)
        {
            uint InstanceCount = reader.ReadUInt32();
            InstanceIds = new uint[InstanceCount];
            for (uint i = 0; i < InstanceCount; i++)
                InstanceIds[i] = reader.ReadUInt32();
        }
    }

    public class LayerTilesData : UndertaleObject
    {
        public UndertaleResourceById<UndertaleBackground> Background = new UndertaleResourceById<UndertaleBackground>("BGND"); // In GMS2 backgrounds are just tilesets
        public uint TilesX;
        public uint TilesY;
        public uint[] TileData; // Each is simply an ID from the tileset/background/sprite

        public void Serialize(UndertaleWriter writer)
        {
            writer.Write(Background.Serialize(writer));
            writer.Write(TilesX);
            writer.Write(TilesY);
            if (TileData.Length != TilesX * TilesY)
                throw new Exception("Invalid TileData length");
            foreach (var tile in TileData)
                writer.Write(tile);
        }

        public void Unserialize(UndertaleReader reader)
        {
            Background.Unserialize(reader, reader.ReadInt32());
            TilesX = reader.ReadUInt32();
            TilesY = reader.ReadUInt32();
            TileData = new uint[TilesX * TilesY];
            for (uint i = 0; i < TilesX * TilesY; i++)
                TileData[i] = reader.ReadUInt32();
        }
    }

    public class LayerBackgroundData : UndertaleObject
    {
        public bool Visible;
        public bool Foreground;
        public UndertaleResourceById<UndertaleSprite> Sprite = new UndertaleResourceById<UndertaleSprite>("SPRT"); // Apparently there's a mode where it's a background reference, but probably not necessary
        public bool TiledHorizontally;
        public bool TiledVertically;
        public bool Stretch;
        public int Color; // includes alpha channel
        public float FirstFrame;
        public float AnimationSpeed;
        public int AnimationSpeedType; // 0 means it's in FPS, 1 means it's in "frames per game frame", I believe

        public void Serialize(UndertaleWriter writer)
        {
            writer.Write(Visible);
            writer.Write(Foreground);
            writer.Write(Sprite.Serialize(writer));
            writer.Write(TiledHorizontally);
            writer.Write(TiledVertically);
            writer.Write(Stretch);
            writer.Write(Color);
            writer.Write(FirstFrame);
            writer.Write(AnimationSpeed);
            writer.Write(AnimationSpeedType);
        }

        public void Unserialize(UndertaleReader reader)
        {
            Visible = reader.ReadBoolean();
            Foreground = reader.ReadBoolean();
            Sprite.Unserialize(reader, reader.ReadInt32());
            TiledHorizontally = reader.ReadBoolean();
            TiledVertically = reader.ReadBoolean();
            Stretch = reader.ReadBoolean();
            Color = reader.ReadInt32();
            FirstFrame = reader.ReadSingle();
            AnimationSpeed = reader.ReadSingle();
            AnimationSpeedType = reader.ReadInt32();
        }
    }

    public class LayerAssetsData : UndertaleObject
    {
        public UndertalePointerList<AssetLegacyTileItem> LegacyTiles;
        public UndertalePointerList<AssetSpriteItem> Sprites;

        public void Serialize(UndertaleWriter writer)
        {
            writer.WriteUndertaleObjectPointer(LegacyTiles);
            writer.WriteUndertaleObjectPointer(Sprites);
            writer.WriteUndertaleObject(LegacyTiles);
            writer.WriteUndertaleObject(Sprites);
        }

        public void Unserialize(UndertaleReader reader)
        {
            LegacyTiles = reader.ReadUndertaleObjectPointer<UndertalePointerList<AssetLegacyTileItem>>();
            Sprites = reader.ReadUndertaleObjectPointer<UndertalePointerList<AssetSpriteItem>>();
            if (reader.ReadUndertaleObject<UndertalePointerList<AssetLegacyTileItem>>() != LegacyTiles)
                throw new IOException("LegacyTiles misaligned");
            if (reader.ReadUndertaleObject<UndertalePointerList<AssetSpriteItem>>() != Sprites)
                throw new IOException("Sprites misaligned");
        }
    }

    // TODO: Ugly copy-paste for now
    public class AssetLegacyTileItem : UndertaleObject, INotifyPropertyChanged
    {
        private int _X;
        private int _Y;
        private UndertaleResourceById<UndertaleSprite> _BackgroundDefinition { get; } = new UndertaleResourceById<UndertaleSprite>("SPRT");
        private uint _SourceX;
        private uint _SourceY;
        private uint _Width;
        private uint _Height;
        private int _TileDepth;
        private uint _InstanceID;
        private float _ScaleX;
        private float _ScaleY;
        private uint _Color;

        public int X { get => _X; set { _X = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X")); } }
        public int Y { get => _Y; set { _Y = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y")); } }
        public UndertaleSprite BackgroundDefinition { get => _BackgroundDefinition.Resource; set { _BackgroundDefinition.Resource = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundDefinition")); } }
        public uint SourceX { get => _SourceX; set { _SourceX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceX")); } }
        public uint SourceY { get => _SourceY; set { _SourceY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceY")); } }
        public uint Width { get => _Width; set { _Width = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width")); } }
        public uint Height { get => _Height; set { _Height = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Height")); } }
        public int TileDepth { get => _TileDepth; set { _TileDepth = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileDepth")); } }
        public uint InstanceID { get => _InstanceID; set { _InstanceID = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstanceID")); } }
        public float ScaleX { get => _ScaleX; set { _ScaleX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleX")); } }
        public float ScaleY { get => _ScaleY; set { _ScaleY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaleY")); } }
        public uint Color { get => _Color; set { _Color = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color")); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Serialize(UndertaleWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(_BackgroundDefinition.Serialize(writer));
            writer.Write(SourceX);
            writer.Write(SourceY);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(TileDepth);
            writer.Write(InstanceID);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(Color);
        }

        public void Unserialize(UndertaleReader reader)
        {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            _BackgroundDefinition.Unserialize(reader, reader.ReadInt32());
            SourceX = reader.ReadUInt32();
            SourceY = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            TileDepth = reader.ReadInt32();
            InstanceID = reader.ReadUInt32();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            Color = reader.ReadUInt32();
        }
    }

    public class AssetSpriteItem : UndertaleObject
    {
        public UndertaleString Name;
        public UndertaleResourceById<UndertaleSprite> Sprite = new UndertaleResourceById<UndertaleSprite>("SPRT");
        public int X;
        public int Y;
        public float ScaleX;
        public float ScaleY;
        public int Color;
        public float AnimationSpeed;
        public int AnimationSpeedType; // 0 is FPS, 1 is "frames per game frame" I believe
        public float FrameIndex;
        public float Rotation; 

        public void Serialize(UndertaleWriter writer)
        {
            writer.WriteUndertaleString(Name);
            writer.Write(Sprite.Serialize(writer));
            writer.Write(X);
            writer.Write(Y);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(Color);
            writer.Write(AnimationSpeed);
            writer.Write(AnimationSpeedType);
            writer.Write(FrameIndex);
            writer.Write(Rotation);
        }

        public void Unserialize(UndertaleReader reader)
        {
            Name = reader.ReadUndertaleString();
            Sprite.Unserialize(reader, reader.ReadInt32());
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            Color = reader.ReadInt32();
            AnimationSpeed = reader.ReadSingle();
            AnimationSpeedType = reader.ReadInt32();
            FrameIndex = reader.ReadSingle();
            Rotation = reader.ReadSingle();
        }
    }

    public static class UndertaleRoomExtensions
    {
        public static T ByInstanceID<T>(this IList<T> list, uint instance) where T : UndertaleRoom.RoomObject
        {
            return list.Where((x) => x.InstanceID == instance).FirstOrDefault();
        }
    }
}
