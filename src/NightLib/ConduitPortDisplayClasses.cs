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
        internal PortDisplayGasInput(CellOffset offset, Color? color = null) : base(offset, true, color) { }
    }

    internal class PortDisplayGasOutput : PortDisplayGasBase, IPortDisplayOutput
    {
        internal PortDisplayGasOutput(CellOffset offset, Color? color = null) : base(offset, false, color) { }
    }

    internal class PortDisplayLiquidInput : PortDisplayLiquidBase, IPortDisplayInput
    {
        internal PortDisplayLiquidInput(CellOffset offset, Color? color = null) : base(offset, true, color) { }
    }

    internal class PortDisplayLiquidOutput : PortDisplayLiquidBase, IPortDisplayOutput
    {
        internal PortDisplayLiquidOutput(CellOffset offset, Color? color = null) : base(offset, false, color) { }
    }

    internal class PortDisplaySolidInput : PortDisplaySolidBase, IPortDisplayInput
    {
        internal PortDisplaySolidInput(CellOffset offset, Color? color = null) : base(offset, true, color) { }
    }

    internal class PortDisplaySolidOutput : PortDisplaySolidBase, IPortDisplayOutput
    {
        internal PortDisplaySolidOutput(CellOffset offset, Color? color = null) : base(offset, false, color) { }
    }



    internal abstract class PortDisplayGasBase : DisplayConduitPortInfo
    {
        protected PortDisplayGasBase(CellOffset offset, bool input, Color? color) : base(ConduitType.Gas, offset, input, color) { }
    }

    internal abstract class PortDisplayLiquidBase : DisplayConduitPortInfo
    {
        protected PortDisplayLiquidBase(CellOffset offset, bool input, Color? color) : base(ConduitType.Liquid, offset, input, color) { }
    }

    internal abstract class PortDisplaySolidBase : DisplayConduitPortInfo
    {
        protected PortDisplaySolidBase(CellOffset offset, bool input, Color? color) : base(ConduitType.Solid, offset, input, color) { }
    }


    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    internal abstract class DisplayConduitPortInfo : DisplayConduitPortInfoBase
    {
        readonly internal ConduitType type;
        readonly internal CellOffset offset;
        readonly internal bool input;
        readonly internal Color color;

        internal DisplayConduitPortInfo(ConduitType type, CellOffset offset, bool input = false, Color? color = null)
        {
            this.type = type;
            this.offset = offset;
            this.input = input;

            this.color = color ?? (input ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.5f, 0.2f));
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
