using UnityEngine;

namespace NightLib
{
    internal interface IPortDisplayInput
    {
        ConduitType GetConduitType();
        CellOffset GetOffset();
    }

    internal interface IPortDisplayOutput
    {
        ConduitType GetConduitType();
        CellOffset GetOffset();
    }

    internal class PortDisplayGasInput : PortDisplayGasBase, IPortDisplayInput
    {
        internal PortDisplayGasInput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayGasOutput : PortDisplayGasBase, IPortDisplayOutput
    {
        internal PortDisplayGasOutput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, false, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayLiquidInput : PortDisplayLiquidBase, IPortDisplayInput
    {
        internal PortDisplayLiquidInput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayLiquidOutput : PortDisplayLiquidBase, IPortDisplayOutput
    {
        internal PortDisplayLiquidOutput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, false, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplaySolidInput : PortDisplaySolidBase, IPortDisplayInput
    {
        internal PortDisplaySolidInput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplaySolidOutput : PortDisplaySolidBase, IPortDisplayOutput
    {
        internal PortDisplaySolidOutput(CellOffset offset, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, false, colorConnected, colorDisonnected) { }
    }



    internal abstract class PortDisplayGasBase : DisplayConduitPortInfo
    {
        protected PortDisplayGasBase(CellOffset offset, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Gas, offset, input, colorConnected, colorDisonnected) { }
    }

    internal abstract class PortDisplayLiquidBase : DisplayConduitPortInfo
    {
        protected PortDisplayLiquidBase(CellOffset offset, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Liquid, offset, input, colorConnected, colorDisonnected) { }
    }

    internal abstract class PortDisplaySolidBase : DisplayConduitPortInfo
    {
        protected PortDisplaySolidBase(CellOffset offset, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Solid, offset, input, colorConnected, colorDisonnected) { }
    }


    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    internal abstract class DisplayConduitPortInfo : DisplayConduitPortInfoBase
    {
        readonly internal ConduitType type;
        readonly internal CellOffset offset;
        readonly internal bool input;
        readonly internal Color colorConnected;
        readonly internal Color colorDisconnected;

        internal DisplayConduitPortInfo(ConduitType type, CellOffset offset, bool input, Color? colorConnected, Color? colorDisonnected)
        {
            this.type = type;
            this.offset = offset;
            this.input = input;

            // assign port colors
            if (colorConnected != null)
            {
                this.colorConnected = colorConnected ?? Color.white;
                this.colorDisconnected = colorDisonnected ?? this.colorConnected;
            }
            else
            {
                // none given. Use defaults
                var resources = BuildingCellVisualizerResources.Instance();
                var ioColors = type == ConduitType.Gas ? resources.gasIOColours : resources.liquidIOColours;
                var colorSet = input ? ioColors.input : ioColors.output;

                this.colorConnected = colorSet.connected;
                this.colorDisconnected = colorSet.disconnected;
            }
        }

        public override ConduitType GetConduitType()
        {
            return this.type;
        }

        public override CellOffset GetOffset()
        {
            return this.offset;
        }
    }

    internal abstract class DisplayConduitPortInfoBase
    {
        public virtual ConduitType GetConduitType() { return ConduitType.None; }
        public virtual CellOffset GetOffset() { return new CellOffset(); }
    }
}
