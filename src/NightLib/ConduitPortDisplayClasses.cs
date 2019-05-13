using UnityEngine;

namespace NightLib
{
    internal interface IPortDisplayInput
    {
        ConduitType GetConduitType();
        CellOffset GetOffset();
        CellOffset GetOffsetFlipped();
    }

    internal interface IPortDisplayOutput
    {
        ConduitType GetConduitType();
        CellOffset GetOffset();
        CellOffset GetOffsetFlipped();
    }

    internal class PortDisplayGasInput : PortDisplayGasBase, IPortDisplayInput
    {
        internal PortDisplayGasInput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayGasOutput : PortDisplayGasBase, IPortDisplayOutput
    {
        internal PortDisplayGasOutput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, false, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayLiquidInput : PortDisplayLiquidBase, IPortDisplayInput
    {
        internal PortDisplayLiquidInput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplayLiquidOutput : PortDisplayLiquidBase, IPortDisplayOutput
    {
        internal PortDisplayLiquidOutput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, false, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplaySolidInput : PortDisplaySolidBase, IPortDisplayInput
    {
        internal PortDisplaySolidInput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, true, colorConnected, colorDisonnected) { }
    }

    internal class PortDisplaySolidOutput : PortDisplaySolidBase, IPortDisplayOutput
    {
        internal PortDisplaySolidOutput(CellOffset offset, CellOffset? offsetFlipped = null, Color? colorConnected = null, Color? colorDisonnected = null) : base(offset, offsetFlipped, false, colorConnected, colorDisonnected) { }
    }



    internal abstract class PortDisplayGasBase : DisplayConduitPortInfo
    {
        protected PortDisplayGasBase(CellOffset offset, CellOffset? offsetFlipped, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Gas, offset, offsetFlipped, input, colorConnected, colorDisonnected) { }
    }

    internal abstract class PortDisplayLiquidBase : DisplayConduitPortInfo
    {
        protected PortDisplayLiquidBase(CellOffset offset, CellOffset? offsetFlipped, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Liquid, offset, offsetFlipped, input, colorConnected, colorDisonnected) { }
    }

    internal abstract class PortDisplaySolidBase : DisplayConduitPortInfo
    {
        protected PortDisplaySolidBase(CellOffset offset, CellOffset? offsetFlipped, bool input, Color? colorConnected, Color? colorDisonnected) : base(ConduitType.Solid, offset, offsetFlipped, input, colorConnected, colorDisonnected) { }
    }


    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    internal abstract class DisplayConduitPortInfo
    {
        readonly internal ConduitType type;
        readonly internal CellOffset offset;
        readonly internal CellOffset offsetFlipped;
        readonly internal bool input;
        readonly internal Color colorConnected;
        readonly internal Color colorDisconnected;

        internal DisplayConduitPortInfo(ConduitType type, CellOffset offset, CellOffset? offsetFlipped, bool input, Color? colorConnected, Color? colorDisonnected)
        {
            this.type = type;
            this.offset = offset;
            this.input = input;

            this.offsetFlipped = offsetFlipped ?? offset;

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

        public virtual ConduitType GetConduitType()
        {
            return this.type;
        }

        public virtual CellOffset GetOffset()
        {
            return this.offset;
        }

        public virtual CellOffset GetOffsetFlipped()
        {
            return this.offsetFlipped;
        }
    }
}
